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
    private static int pageAttempts;
    private static bool failPage, failNotice;
    private static TechUnlockDialogGUI closedPage;
    private static bool SkipBag() { return false; }
    private static bool ScienceCapacity(string item_id, ref int __result)
    { if (item_id != "science") return true; __result = 1000; return false; }
    private static bool Definition(Item __instance, ref ItemDefinition __result)
    { __result = new ItemDefinition { id = __instance.id, stack_count = 1000 }; return false; }
    private static void TabsFault() { tabsAttempts++; throw new InvalidOperationException("fixture_tabs_fault"); }
    private static void GuidanceFault() { guidanceAttempts++; throw new InvalidOperationException("fixture_guidance_fault"); }
    // Verify dispatch/continuation; actual native window disposal requires the engine.
    private static bool NoticeAttempt() { noticeAttempts++; if (failNotice) throw new InvalidOperationException("fixture_notice_fault"); return false; }
    private static void PriceFault() { priceAttempts++; throw new InvalidOperationException("fixture_price_fault"); }
    private static bool PageAttempt(TechUnlockDialogGUI page)
    {
        pageAttempts++; closedPage = page;
        var notices = Own("AvailabilityNotifications");
        Assert(Field(notices, "owner") == null && !(bool)Field(notices, "pageOpen") && !(bool)Field(notices, "nextPage") &&
            Field(notices, "displayedPage") == null, "Native close reached before relinquishing page ownership");
        if (failPage) throw new InvalidOperationException("fixture_page_fault");
        return false;
    }
    private static void Marker() { }
    private static bool MarkerPrefix() { return false; }
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
        var notices = Own("AvailabilityNotifications");
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

                var bubble = (TooltipBubbleGUI)FormatterServices.GetUninitializedObject(typeof(TooltipBubbleGUI));
                Set(Own("GuidanceTooltip"), "bubble", bubble);
                logs.Clear(); tabsAttempts = guidanceAttempts = noticeAttempts = 0; State(true, true);
                // The real owned-bubble handler encounters its missing widget; its
                // cleanup also throws. No engine Update/Input reconstruction is needed.
                try { Advice("GuidanceTooltip", "Position", "Postfix").Invoke(null, new object[] { bubble }); }
                finally { Set(Own("GuidanceTooltip"), "bubble", null); }
                Assert(!(bool)Field(plugin, "Enabled") && (bool)Field(plugin, "failed") &&
                    tabsAttempts == 1 && guidanceAttempts == 1 && noticeAttempts == 1 &&
                    logs.Exists(x => x.StartsWith("RAR_DISABLED ") && x.Contains("NullReferenceException")),
                    "Actual guidance handler loses primary failure or bypasses containment when cleanup also throws");

                var instance = FormatterServices.GetUninitializedObject(plugin);
                var teardownOwner = new Harmony("rar.offline.teardown-owned");
                var marker = typeof(FailureContainmentTests).GetMethod("Marker", Private);
                teardownOwner.Patch(marker, prefix: new HarmonyMethod(typeof(FailureContainmentTests), "MarkerPrefix"));
                plugin.GetField("harmony", Instance).SetValue(instance, teardownOwner);
                logs.Clear(); tabsAttempts = guidanceAttempts = noticeAttempts = 0;
                failNotice = true; State(true, true);
                bool removedOwnPatch;
                try
                {
                    plugin.GetMethod("OnDestroy", Instance).Invoke(instance, null);
                    removedOwnPatch = !Harmony.GetPatchInfo(marker).Owners.Contains(teardownOwner.Id);
                }
                finally { failNotice = false; teardownOwner.UnpatchSelf(); }
                Assert(!(bool)Field(plugin, "Enabled") && tabsAttempts == 1 && guidanceAttempts == 1 && noticeAttempts == 1 && removedOwnPatch,
                    "Teardown skips later cleanup after a prior cleanup exception");
                Assert(logs.Exists(x => x.StartsWith("RAR_CLEANUP_FAILED feature=notices ")) &&
                    logs.Exists(x => x.StartsWith("RAR_CLEANUP_FAILED feature=guidance ")) &&
                    logs.Exists(x => x.StartsWith("RAR_CLEANUP_FAILED feature=tabs ")),
                    "Teardown does not report independent failed cleanup categories");
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

            // Optional failure must release a paid-ineligible presentation only,
            // retaining gameplay and the unseen-topic ledger even if native cleanup fails.
            var view = (TechUnlockDialogGUI)FormatterServices.GetUninitializedObject(typeof(TechUnlockDialogGUI));
            var unseen = new Item("fixture_unseen_notice", 1);
            Set(notices, "disabled", false); Set(notices, "loaded", true); Set(notices, "dirty", true);
            Set(notices, "pageOpen", true); Set(notices, "nextPage", true); Set(notices, "owner", view);
            Set(notices, "displayedData", unseen); Set(notices, "displayedPage", new List<string> { "fixture_unseen" });
            int generation = (int)Field(notices, "generation");
            var optionalOwner = new Harmony("rar.offline.notice-owned");
            var optionalMarker = typeof(FailureContainmentTests).GetMethod("Marker", Private);
            optionalOwner.Patch(optionalMarker, prefix: new HarmonyMethod(typeof(FailureContainmentTests), "MarkerPrefix"));
            Set(notices, "eventHarmony", optionalOwner); Set(notices, "installed", true);
            fixture.Patch(notices.GetMethod("CloseOwnedPage", Private), prefix: new HarmonyMethod(typeof(FailureContainmentTests), "PageAttempt"));
            // RestoreContainer's Unity ECall cannot execute under CLR. Its real
            // method-entry failure is caught by Reset, which must still close/unpatch.
            logs.Clear(); State(true, true); pageAttempts = 0; failPage = true;
            Call(notices, "Fail", new InvalidOperationException("fixture_notice_primary"));
            Call(notices, "Fail", new InvalidOperationException("fixture_notice_repeat"));
            Assert((bool)Field(plugin, "Enabled") && (bool)Field(notices, "disabled") && !(bool)Field(notices, "loaded") &&
                !(bool)Field(notices, "dirty") && !(bool)Field(notices, "pageOpen") && !(bool)Field(notices, "nextPage") &&
                Field(notices, "owner") == null && Field(notices, "displayedData") == null && Field(notices, "displayedPage") == null &&
                (int)Field(notices, "generation") == generation + 1 && pageAttempts == 1 && ReferenceEquals(closedPage, view),
                "Notice failure leaves active ownership, repeats cleanup or disables research");
            Assert(unseen.GetParamInt("rar_notice_seen_fixture_unseen") == 0 && unseen.GetParamInt("rar_notice_initial") == 0,
                "Failure cleanup consumes undisplayed notice flags");
            Assert(!(bool)Field(notices, "installed") && Field(notices, "eventHarmony") == null &&
                !Harmony.GetPatchInfo(optionalMarker).Owners.Contains(optionalOwner.Id), "UI cleanup fault skips optional-owner unpatch");
            Assert(logs.FindAll(x => x.StartsWith("RAR_NOTICE_ERROR ")).Count == 1 &&
                logs.Exists(x => x.StartsWith("RAR_NOTICE_CLEANUP_ERROR feature=container ")) &&
                logs.Exists(x => x.StartsWith("RAR_NOTICE_CLEANUP_ERROR feature=page ") && x.Contains("fixture_page_fault")) && logs[0].Contains("fixture_notice_primary"),
                "Notice primary/secondary failure diagnostic missing or repeated");
            failPage = false;
            Set(notices, "disabled", false); Set(notices, "pageOpen", true); Set(notices, "nextPage", true); Set(notices, "owner", view);
            pageAttempts = 0;
            try { Call(notices, "WindowOpened", view); } // A native consumer replaces the same shared instance.
            catch (TargetInvocationException ex)
            {
                // The ownership/failure work precedes Request. CLR cannot JIT its
                // Time.frameCount ECall, even though disabled Request is inert in Unity.
                if (!(ex.InnerException is System.Security.SecurityException) ||
                    !ex.InnerException.StackTrace.Contains("AvailabilityNotifications.Request")) throw;
            }
            Assert((bool)Field(notices, "disabled") && Field(notices, "owner") == null && !(bool)Field(notices, "pageOpen") && pageAttempts == 0,
                "Restoration fault while relinquishing a shared window closes the foreign replacement");
            Console.WriteLine("PASS audit cleanup: actual guidance handler containment and teardown continuation; notice disable clears ownership before close, preserves unseen flags, isolates cleanup faults and unpatches only its owner; foreign replacement is not closed");
        }
        finally { fixture.UnpatchSelf(); }
    }
}
