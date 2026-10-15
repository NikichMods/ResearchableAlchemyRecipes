// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using BepInEx.Logging;
using HarmonyLib;

internal static class FailureContainmentTests
{
    private const BindingFlags Private = BindingFlags.NonPublic | BindingFlags.Static;
    private const BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Instance;
    private static Assembly assembly;
    private static Type plugin, source;
    private static int tabsAttempts, guidanceAttempts, noticeAttempts, priceAttempts;
    private static bool SkipBag() { return false; }
    private static bool ScienceCapacity(string item_id, ref int __result)
    { if (item_id != "science") return true; __result = 1000; return false; }
    private static bool Definition(Item __instance, ref ItemDefinition __result)
    { __result = new ItemDefinition { id = __instance.id, stack_count = 1000 }; return false; }
    private static void TabsFault() { tabsAttempts++; throw new InvalidOperationException("fixture_tabs_fault"); }
    private static void GuidanceFault() { guidanceAttempts++; throw new InvalidOperationException("fixture_guidance_fault"); }
    // Verify dispatch/continuation; actual native window disposal requires the engine.
    private static bool NoticeAttempt() { noticeAttempts++; return false; }
    private static void PriceFault() { priceAttempts++; throw new InvalidOperationException("fixture_price_fault"); }
    private static object Call(Type type, string method, params object[] args) { return type.GetMethod(method, Private).Invoke(null, args); }
    private static object Field(Type type, string name) { return type.GetField(name, Private).GetValue(null); }
    private static void Set(Type type, string name, object value) { type.GetField(name, Private).SetValue(null, value); }
    private static Type Own(string name) { return assembly.GetType("NikichMods.ResearchableAlchemyRecipes." + name, true); }
    private static MethodInfo Advice(string owner, string nested, string method)
    { return Own(owner).GetNestedType(nested, BindingFlags.NonPublic).GetMethod(method, Private); }
    private static void Assert(bool value, string message) { if (!value) throw new Exception(message); }
    private static GameSave Save()
    {
        var save = (GameSave)FormatterServices.GetUninitializedObject(typeof(GameSave));
        save.completed_one_time_crafts = new List<string> { "unrelated" };
        return save;
    }
    private static void State(bool enabled, bool registered)
    { Set(plugin, "Enabled", enabled); Set(plugin, "registered", registered); Set(plugin, "failed", false); }

    public static int Main(string[] args)
    {
        AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e) {
            foreach (string dir in args) { string path = Path.Combine(dir, new AssemblyName(e.Name).Name + ".dll"); if (File.Exists(path)) return Assembly.LoadFrom(path); }
            return null;
        };
        try { Run(args); Console.WriteLine("PASS containment: disabled payment/queue guards, verified paid completion/deduplication, startup gate, native Item refund, cleanup fault isolation/reentry/log count and cleared UI ownership"); return 0; }
        catch (Exception ex) { Console.WriteLine(ex); return 1; }
    }

    private static void Run(string[] args)
    {
        assembly = Assembly.LoadFrom(Path.Combine(args[0], "compile-check.dll"));
        plugin = Own("Plugin"); source = Own("SourceResearch");
        var fixture = new Harmony("rar.offline.failure-fixtures");
        try
        {
            fixture.Patch(AccessTools.Method(typeof(Item), "CheckAndInitBagItem"), prefix: new HarmonyMethod(typeof(FailureContainmentTests), "SkipBag"));
            fixture.Patch(AccessTools.Method(typeof(Item), "CanAddCount", new[] { typeof(string), typeof(bool), typeof(bool) }), prefix: new HarmonyMethod(typeof(FailureContainmentTests), "ScienceCapacity"));
            fixture.Patch(AccessTools.PropertyGetter(typeof(Item), "definition"), prefix: new HarmonyMethod(typeof(FailureContainmentTests), "Definition"));
            // Install these before the first Fail JIT: tiny cleanup methods can otherwise
            // already be inlined into its caller and bypass later test-process detours.
            fixture.Patch(Own("RecipeGuidance").GetMethod("Restore", Private), prefix: new HarmonyMethod(typeof(FailureContainmentTests), "GuidanceFault"));
            var job = new CraftDefinition { id = "rar_research_fixture", needs_from_wgo = new List<Item> { new Item("science", 8) } };
            var registry = (IDictionary)Field(source, "topics");
            var topicType = source.GetNestedType("Topic", BindingFlags.NonPublic);
            var topic = Activator.CreateInstance(topicType, true);
            topicType.GetField("job", Instance).SetValue(topic, job);
            topicType.GetField("product", Instance).SetValue(topic, "fixture_product");
            var formulas = (List<CraftDefinition>)topicType.GetField("formulas", Instance).GetValue(topic);
            formulas.Add(new CraftDefinition { id = "fixture_formula_a" });
            formulas.Add(new CraftDefinition { id = "fixture_formula_b" });
            registry.Add(job.id, topic);
            State(false, true);
            var cc = (CraftComponent)FormatterServices.GetUninitializedObject(typeof(CraftComponent));
            object[] start = { cc, job, 3, true };
            Assert(!(bool)Advice("ResearchRepetitionGuard", "Start", "Prefix").Invoke(null, start) && !(bool)start[3] && (int)start[2] == 3,
                "Disabled research falls through to payment or changes prepaid amount");
            object[] enqueue = { cc, job, 4 };
            Assert(!(bool)Advice("ResearchRepetitionGuard", "Enqueue", "Prefix").Invoke(null, enqueue), "Disabled research can enqueue");
            var ordinary = new CraftDefinition { id = "unrelated" };
            object[] outside = { cc, ordinary, 7, true };
            Assert((bool)Advice("ResearchRepetitionGuard", "Start", "Prefix").Invoke(null, outside) && (int)outside[2] == 7 && (bool)outside[3], "Disabled containment reaches foreign crafts");
            object[] multiple = { job, true };
            Advice("ResearchRepetitionGuard", "MultipleResult", "Postfix").Invoke(null, multiple);
            Assert(!(bool)multiple[1], "Disabled registry lost single-operation restriction");
            cc.craft_queue = new List<CraftComponent.CraftQueueItem> {
                new CraftComponent.CraftQueueItem { id = job.id, n = 2 }, new CraftComponent.CraftQueueItem { id = ordinary.id, n = 9 }
            };
            Advice("ResearchRepetitionGuard", "QueueEntry", "Prefix").Invoke(null, new object[] { cc });
            Assert(cc.craft_queue.Count == 1 && cc.craft_queue[0].n == 9 && cc.craft_queue[0].id == ordinary.id, "Disabled unpaid queue cleanup affects unrelated work");
            cc.is_crafting = true; cc.current_craft = job;
            var ledger = new Item("fixture_table", 1); ledger.inventory.Add(new Item("science", 12));
            var reserve = Own("ResearchScienceReserve"); Call(reserve, "Record", ledger, 8);
            var logs = new List<string>();
            var logger = new ManualLogSource("failure-fixture");
            logger.LogEvent += delegate(object sender, LogEventArgs e) { logs.Add(e.Data.ToString()); };
            Set(plugin, "log", logger);
            Call(plugin, "Fail", new InvalidOperationException("fixture_runtime_failure"));
            Assert(cc.is_crafting && ReferenceEquals(cc.current_craft, job) && ledger.GetItemsCount("science", true) == 12 && (int)Call(reserve, "Read", ledger) == 8,
                "Presentation failure cancels or refunds work outside a native interruption");
            var save = Save(); save.completed_one_time_crafts.Add(formulas[0].id);
            var complete = Advice("Plugin", "Completion", "Postfix");
            complete.Invoke(null, new object[] { save, job });
            complete.Invoke(null, new object[] { save, job });
            Assert(save.completed_one_time_crafts.Count == 3 && save.completed_one_time_crafts.Contains(formulas[1].id), "Disabled paid completion loses/duplicates remaining reward");
            var other = Save(); State(false, false); complete.Invoke(null, new object[] { other, job });
            Assert(other.completed_one_time_crafts.Count == 1, "Unverified startup registry grants knowledge");
            State(false, true);
            Advice("Plugin", "Registration", "Prefix").Invoke(null, new object[] { FormatterServices.GetUninitializedObject(typeof(GameBalance)) });
            Assert(!(bool)Field(plugin, "registered"), "Native cache rebuild retains stale settlement validation");
            var cancelStateType = reserve.GetNestedType("CancelState", BindingFlags.NonPublic);
            var cancelState = Activator.CreateInstance(cancelStateType, true);
            cancelStateType.GetField("table", Instance).SetValue(cancelState, ledger);
            cancelStateType.GetField("fee", Instance).SetValue(cancelState, 8);
            cc.is_crafting = false; cc.current_craft = null; // Model native Cancel/End boundary, not engine execution.
            Advice("ResearchScienceReserve", "Cancel", "Postfix").Invoke(null, new[] { (object)cc, cancelState });
            Assert(ledger.GetItemsCount("science", true) == 20 && (int)Call(reserve, "Read", ledger) == 0, "Disabled native cancellation loses exact fee");

            // Exceptions are injected only into the test process, without any shipped fault seam.
            var cleanup = new Harmony("rar.offline.cleanup-faults");
            try
            {
                cleanup.Patch(Own("StudyTabs").GetMethod("Restore", Private), prefix: new HarmonyMethod(typeof(FailureContainmentTests), "TabsFault"));
                cleanup.Patch(Own("AvailabilityNotifications").GetMethod("Dispose", Private), prefix: new HarmonyMethod(typeof(FailureContainmentTests), "NoticeAttempt"));
                logs.Clear(); tabsAttempts = guidanceAttempts = noticeAttempts = 0; State(true, true);
                Call(plugin, "Fail", new InvalidOperationException("fixture_primary_failure"));
                Call(plugin, "Fail", new InvalidOperationException("fixture_repeat"));
                Assert(!(bool)Field(plugin, "Enabled") && tabsAttempts == 1 && guidanceAttempts == 1 && noticeAttempts == 1,
                    "Failure cleanup counts: tabs=" + tabsAttempts + " guidance=" + guidanceAttempts + " notices=" + noticeAttempts);
                Assert(logs.FindAll(x => x.StartsWith("RAR_DISABLED ")).Count == 1 && logs.FindAll(x => x.StartsWith("RAR_CLEANUP_FAILED ")).Count == 2 &&
                    logs.Count > 0 && logs[0].Contains("settlement=retained") && logs[0].Contains("fixture_primary_failure"),
                    "Failure support log mismatch: " + string.Join(" | ", logs.ToArray()));
            }
            finally { cleanup.UnpatchSelf(); }

            var tabs = Own("StudyTabs");
            var snapshotType = tabs.GetNestedType("Snapshot", BindingFlags.NonPublic);
            Set(tabs, "snapshot", FormatterServices.GetUninitializedObject(snapshotType));
            Set(tabs, "table", FormatterServices.GetUninitializedObject(typeof(WorldGameObject)));
            var priceFault = new Harmony("rar.offline.price-fault");
            try
            {
                // Inject at the managed owner; the cell Dispose directly contains Unity
                // ECalls and cannot be restored by a standalone-CLR detour.
                priceFault.Patch(Own("ResearchCostDisplay").GetMethod("Restore", Private), prefix: new HarmonyMethod(typeof(FailureContainmentTests), "PriceFault"));
                logs.Clear(); State(true, true); Call(plugin, "Fail", new InvalidOperationException("fixture_failure_with_open_tabs"));
                Assert(Field(tabs, "snapshot") == null && Field(tabs, "table") == null && priceAttempts == 1,
                    "Cleanup exception retains owned tab state or repeats recursively");
                Assert(logs.FindAll(x => x.StartsWith("RAR_DISABLED ")).Count == 1 && logs.Exists(x => x.StartsWith("RAR_PRICE_RESTORE_ERROR ")),
                    "Nested price cleanup failure is hidden or repeats disable logging");
            }
            finally { priceFault.UnpatchSelf(); }
        }
        finally { fixture.UnpatchSelf(); }
    }
}
