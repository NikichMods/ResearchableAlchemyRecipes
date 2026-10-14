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
            state = asm.GetType("NikichMods.Research.AvailabilityNoticeState", true);
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
            var safe = new object[] { true, false, false, false, true, false, false, false, false, false };
            Assert((bool)Call("Safe", safe), "Clear gameplay does not permit delivery");
            for (int i = 0; i < safe.Length; i++)
            {
                var blocked = (object[])safe.Clone(); blocked[i] = !(bool)blocked[i];
                Assert(!(bool)Call("Safe", blocked), "Unsafe UI/control/input boundary admitted: " + i);
            }
            Console.WriteLine("PASS notices: quiet/empty initial state, aggregated current eligibility, displayed-only deduplication, native Item roundtrip, source return/save isolation and ten delivery blockers");
        }
        finally { fixture.UnpatchSelf(); }
    }
}
