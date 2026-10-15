// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;

internal static class NotificationStateTests
{
    private const BindingFlags Private = BindingFlags.Static | BindingFlags.NonPublic;
    private static Type state;
    private static bool SkipBagInitialization() { return false; }
    private static object Call(string name, params object[] args) { return state.GetMethod(name, Private).Invoke(null, args); }
    private static void Assert(bool value, string message) { if (!value) throw new Exception(message); }
    private static List<string> Pending(Item item, bool alchemy, params string[] products)
    {
        return (List<string>)Call("Pending", item, alchemy, products);
    }
    public static int Main(string[] args)
    {
        AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e) {
            foreach (string dir in args) { string path = Path.Combine(dir, new AssemblyName(e.Name).Name + ".dll"); if (File.Exists(path)) return Assembly.LoadFrom(path); }
            return null;
        };
        try { Run(args); return 0; }
        catch (Exception ex) { Console.WriteLine(ex); return 1; }
    }
    private static void Run(string[] args)
    {
        var fixture = new Harmony("rar.offline.notification-item-fixture");
        try
        {
            fixture.Patch(AccessTools.Method(typeof(Item), "CheckAndInitBagItem"), prefix: new HarmonyMethod(typeof(NotificationStateTests), "SkipBagInitialization"));
            var asm = Assembly.LoadFrom(Path.Combine(args[0], "compile-check.dll"));
            state = asm.GetType("NikichMods.ResearchableAlchemyRecipes.AvailabilityNoticeState", true);
            var player = new Item("fixture_notice_player", 1);
            player.SetParam("unrelated_flag", 7f);
            Assert(Pending(player, false, "acid").Count == 0 && (bool)Call("IsInitial", player), "Pre-alchemy consumes overview");
            Assert(Pending(player, true).Count == 0 && (bool)Call("IsInitial", player), "Empty eligibility consumes overview");
            Call("MarkDisplayed", player, new string[0]);
            Assert((bool)Call("IsInitial", player), "Empty displayed list consumes overview");
            var first = Pending(player, true, "acid", "pot_heal", "acid");
            Assert(first.Count == 2 && first[0] == "acid" && first[1] == "pot_heal", "First batch lost topics or duplicated a source");
            Assert((bool)Call("IsInitial", player), "Enqueue/query marks undelivered topics");
            // Eligibility can disappear or knowledge can change while another window is open.
            Assert(Pending(player, true, "pot_heal").Count == 1, "Revalidation keeps stale source/fully-known topic");
            Call("MarkDisplayed", player, first);
            Assert(!(bool)Call("IsInitial", player) && Pending(player, true, "acid", "pot_heal").Count == 0, "Display does not suppress repeats");
            Assert(Pending(player, true).Count == 0 && Pending(player, true, "acid").Count == 0, "Disappearance/reappearance re-announces topic");
            var restored = SmartSerializer.Deserialize<Item>(SmartSerializer.Serialize<Item>(player));
            Assert(!(bool)Call("IsInitial", restored) && Pending(restored, true, "acid", "pot_heal").Count == 0, "Native Item reload repeats overview");
            Assert(restored.GetParamInt("unrelated_flag") == 7, "Notice metadata changes unrelated native parameters");
            Assert(Pending(restored, true, "acid", "pot_mana").Count == 1, "Later new topic is lost");
            var secondSave = new Item("fixture_notice_player", 1);
            Assert((bool)Call("IsInitial", secondSave) && Pending(secondSave, true, "acid").Count == 1, "Notice state leaks between saves");
            var many = new List<string>();
            for (int i = 0; i < 33; i++) many.Add("fixture_topic_" + i);
            var pageData = new Item("fixture_notice_player", 1);
            var page = (List<string>)Call("Page", Pending(pageData, true, many.ToArray()));
            Assert(page.Count == 3, "Native three-row page capacity exceeded");
            Call("MarkDisplayed", pageData, page);
            pageData = SmartSerializer.Deserialize<Item>(SmartSerializer.Serialize<Item>(pageData));
            Assert(Pending(pageData, true, many.ToArray()).Count == 30, "Close/reload marks unseen pages as announced");
            int pages = 1;
            while (Pending(pageData, true, many.ToArray()).Count != 0)
            {
                page = (List<string>)Call("Page", Pending(pageData, true, many.ToArray()));
                Call("MarkDisplayed", pageData, page); pages++;
            }
            Assert(pages == 11, "Full 33-topic batch loses, repeats or exceeds native page capacity");
            Assert(((List<string>)Call("Page", new List<string>())).Count == 0, "Empty page is not empty");
            var causeData = new Item("fixture_notice_mixed", 1);
            var mixed = new List<string> { "ordinary1", "clotho1", "ordinary2", "clotho2", "clotho3", "ordinary3", "clotho4" };
            Func<string, bool> fromClotho = product => product.StartsWith("clotho", StringComparison.Ordinal);
            var causePage = (List<string>)Call("CausePage", Pending(causeData, true, mixed.ToArray()), fromClotho);
            Assert(causePage.Count == 3 && causePage.TrueForAll(product => fromClotho(product)),
                "Expansion explanation contaminates unrelated rows");
            Call("MarkDisplayed", causeData, causePage);
            causeData = SmartSerializer.Deserialize<Item>(SmartSerializer.Serialize<Item>(causeData));
            var remainder = Pending(causeData, true, mixed.ToArray());
            causePage = (List<string>)Call("CausePage", remainder, fromClotho);
            Assert(remainder.Count == 4 && causePage.Count == 1 && causePage[0] == "clotho4",
                "Reload skips unseen cause rows or fills them with unrelated products");
            Call("MarkDisplayed", causeData, causePage);
            var ordinaryPage = (List<string>)Call("CausePage", Pending(causeData, true, mixed.ToArray()), fromClotho);
            Assert(ordinaryPage.Count == 3 && ordinaryPage.TrueForAll(product => !fromClotho(product)),
                "Mixed-source batch loses generic rows");
            Call("MarkDisplayed", causeData, ordinaryPage);
            Assert(Pending(causeData, true, mixed.ToArray()).Count == 0, "Cause grouping repeats or loses topics");
            Console.WriteLine("PASS mixed-source notice pages: isolated cause text, max-three rows, short cause page, generic continuation and displayed-only reload");
            var safe = new object[] { true, false, false, false, true, false, false, false, false, false };
            Assert((bool)Call("Safe", safe), "Clear gameplay does not permit delivery");
            for (int i = 0; i < safe.Length; i++)
            {
                var blocked = (object[])safe.Clone(); blocked[i] = !(bool)blocked[i];
                Assert(!(bool)Call("Safe", blocked), "Unsafe UI/control/input boundary admitted: " + i);
            }
            var notices = asm.GetType("NikichMods.ResearchableAlchemyRecipes.AvailabilityNotifications", true);
            var rows = (FieldInfo)notices.GetField("UnlockRows", Private).GetValue(null);
            Assert(rows != null && rows.FieldType == typeof(List<TechTreeGUIUnlockItem>), "Native reusable notice rows changed");
            int bound = 0;
            var targets = (Type[])notices.GetField("TargetTypes", Private).GetValue(null);
            var targetNames = (string[])notices.GetField("TargetNames", Private).GetValue(null);
            var handlers = (Type[])notices.GetField("HandlerTypes", Private).GetValue(null);
            Assert(targets.Length == targetNames.Length && targets.Length == handlers.Length, "Notice binding map lengths differ");
            for (int i = 0; i < targets.Length; i++)
            {
                int matches = 0;
                foreach (var method in targets[i].GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
                    if (method.Name == targetNames[i]) matches++;
                Assert(matches == 1, "Missing/ambiguous native notification target: " + targets[i].Name + "." + targetNames[i]);
                Assert(handlers[i].GetMethod(i == 0 ? "Prefix" : "Postfix", Private) != null, "Notice handler is missing");
                Assert(handlers[i].GetCustomAttributes(typeof(HarmonyPatch), false).Length == 0, "Optional notice hooks entered mandatory research PatchAll");
                bound++;
            }
            Assert(bound == 8, "Notification native event binding coverage changed");
            foreach (string name in new[] { "on_window_opened", "on_window_closed" })
                Assert(typeof(BaseGUI).GetEvent(name, BindingFlags.Public | BindingFlags.Static) != null, "Native window event is missing: " + name);
            Console.WriteLine("PASS notices: quiet/empty initial state, aggregated current eligibility, displayed-only deduplication, native Item roundtrip, source return/save isolation and ten delivery blockers");
            Console.WriteLine("PASS notice metadata: eight unambiguous actual native targets and two window events; Unity-dependent detour execution remains an installed gate");
        }
        finally { fixture.UnpatchSelf(); }
    }
}
