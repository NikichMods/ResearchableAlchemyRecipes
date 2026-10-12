// SPDX-License-Identifier: MPL-2.0
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Generic;
using HarmonyLib;

class QuantityCompatibilityTests
{
    private static void ForceMultiple(ref bool __result) { __result = true; }
    private static bool scoped;
    private static bool guidanceScoped;
    private static MethodInfo cardPolicy;
    private static List<CraftItemGUI> rows;
    private static CraftItemGUI focused;
    private static string CardScope(CraftItemGUI card)
    {
        return (string)cardPolicy.Invoke(null, new object[] { guidanceScoped, card, rows, focused });
    }
    private static CraftItemGUI Card(params CraftDefinition[] definitions)
    {
        var card = (CraftItemGUI)FormatterServices.GetUninitializedObject(typeof(CraftItemGUI));
        AccessTools.Field(typeof(CraftItemGUI), "_possible_crafts").SetValue(card, new List<CraftDefinition>(definitions));
        AccessTools.Field(typeof(CraftItemGUI), "_current_craft_index").SetValue(card, 0);
        return card;
    }
    public static int Main(string[] args)
    {
        AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e) {
            foreach (string dir in args) { string path = Path.Combine(dir, new AssemblyName(e.Name).Name + ".dll"); if (File.Exists(path)) return Assembly.LoadFrom(path); }
            return null;
        };
        try { Run(args); Console.WriteLine("PASS actual installed checker: null/empty/stale/unfocused guidance cards, valid ordinary/one-time fallthrough, study guard, outside scope and unpatch restoration"); return 0; }
        catch (Exception ex) { Console.WriteLine(ex); return 1; }
    }
    private static void NullFailure(MethodInfo method)
    {
        try { method.Invoke(null, null); throw new Exception("Expected original null-card failure"); }
        catch (TargetInvocationException ex) { if (!(ex.InnerException is NullReferenceException)) throw; }
    }
    private static void Run(string[] dirs)
    {
        var foreign = Assembly.LoadFrom(Path.Combine(dirs[3], "MaxButtonsRedux.dll"));
        var plugin = foreign.GetType("MaxButtonsRedux.Plugin", true);
        var method = plugin.GetMethod("IsCraftGuiInputValid", BindingFlags.NonPublic | BindingFlags.Static);
        plugin.GetField("_craftItemGui", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).SetValue(null, null);
        NullFailure(method);
        var probe = Assembly.LoadFrom(Path.Combine(dirs[0], "compile-check.dll"));
        string guid = null;
        foreach (var attr in plugin.GetCustomAttributesData()) if (attr.AttributeType.Name == "BepInPlugin") guid = (string)attr.ConstructorArguments[0].Value;
        bool dependency = false;
        foreach (var attr in probe.GetType("NikichMods.Research.StudyTableWorkProbe").GetCustomAttributesData())
            if (attr.AttributeType.Name == "BepInDependency" && (string)attr.ConstructorArguments[0].Value == guid) dependency = true;
        if (!dependency) throw new Exception("Missing dependency on the actual installed plugin GUID");
        var install = probe.GetType("NikichMods.Research.QuantityInputCompatibility", true).GetMethod("Install", BindingFlags.NonPublic | BindingFlags.Static);
        cardPolicy = probe.GetType("NikichMods.Research.RecipeGuidance", true).GetMethod("CardUnavailableReason", BindingFlags.NonPublic | BindingFlags.Static);
        rows = new List<CraftItemGUI>();
        var foreignCard = plugin.GetField("_craftItemGui", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        foreignCard.SetValue(null, Card()); NullFailure(method);
        foreignCard.SetValue(null, null);
        var harmony = new Harmony("rar.offline.quantity-test");
        try
        {
            install.Invoke(null, new object[] { harmony, new Func<bool>(() => scoped), new Func<CraftItemGUI, string>(CardScope) });
            scoped = true;
            if (!(bool)method.Invoke(null, null)) throw new Exception("Scoped invalid-input result not returned");
            scoped = false; NullFailure(method);
            guidanceScoped = true;
            if (!(bool)method.Invoke(null, null)) throw new Exception("Missing guidance card not contained");
            var empty = Card(); rows.Add(empty); focused = empty; foreignCard.SetValue(null, empty);
            if (empty.current_craft != null || !(bool)method.Invoke(null, null)) throw new Exception("Empty native card not contained");
            var ordinary = new CraftDefinition { id = "ordinary", needs = new List<Item>() };
            var valid = Card(ordinary); rows.Clear(); rows.Add(valid); focused = valid;
            // Foreign checker reference can still point to a previous native window's row.
            foreignCard.SetValue(null, empty);
            if (!(bool)method.Invoke(null, null)) throw new Exception("Stale foreign card not contained");
            foreignCard.SetValue(null, valid); focused = null;
            if (!(bool)method.Invoke(null, null)) throw new Exception("Unfocused card not contained");
            focused = valid;
            if ((bool)method.Invoke(null, null)) throw new Exception("Ready ordinary quantity input was disabled");
            ordinary.one_time_craft = true;
            if (!(bool)method.Invoke(null, null)) throw new Exception("Original one-time rejection overwritten");
            ordinary.one_time_craft = false; ordinary.needs = null;
            if (!(bool)method.Invoke(null, null)) throw new Exception("Missing formula needs not contained");
            ordinary.needs = new List<Item>();
            guidanceScoped = false;
            if ((bool)method.Invoke(null, null)) throw new Exception("Ordinary input outside guidance altered");
            if (!ReferenceEquals(foreignCard.GetValue(null), valid)) throw new Exception("Foreign selection mutated by readiness guard");
            foreignCard.SetValue(null, null); NullFailure(method);
        }
        finally { harmony.UnpatchSelf(); }
        NullFailure(method);
        var guard = probe.GetType("NikichMods.Research.ResearchRepetitionGuard", true);
        var openFlag = plugin.GetField("_craftGuiOpen", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        openFlag.SetValue(null, true);
        probe.GetType("NikichMods.Research.QuantityInputCompatibility").GetMethod("OnOurFrameClosed", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
        if ((bool)openFlag.GetValue(null)) throw new Exception("Foreign close notification did not clear open flag");
        var definition = new CraftDefinition { id = "rar_probe_knowledge_heal_017", force_multi_craft = true, disable_multi_craft = false, one_time_craft = true };
        if (!definition.CanCraftMultiple()) throw new Exception("Forced multi fixture not reproduced");
        guard.GetMethod("RepairDefinition", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { definition });
        if (definition.CanCraftMultiple() || definition.enqueue_type != CraftDefinition.EnqueueType.NeverEnqueue) throw new Exception("Native multiple-craft gate remains enabled");
        var enabled = probe.GetType("NikichMods.Research.StudyTableWorkProbe").GetField("enabledProbe", BindingFlags.NonPublic | BindingFlags.Static);
        var uiHarmony = new Harmony("rar.offline.multiple-policy");
        try
        {
            enabled.SetValue(null, true);
            var canMultiple = typeof(CraftDefinition).GetMethod("CanCraftMultiple");
            uiHarmony.Patch(canMultiple, postfix: new HarmonyMethod(typeof(QuantityCompatibilityTests), "ForceMultiple"));
            var resultGuard = guard.GetNestedType("MultipleResult", BindingFlags.NonPublic).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static);
            uiHarmony.Patch(canMultiple, postfix: new HarmonyMethod(resultGuard));
            if (definition.CanCraftMultiple()) throw new Exception("Forced foreign result bypasses research restriction");
            if (!new CraftDefinition { id = "unrelated", force_multi_craft = true }.CanCraftMultiple()) throw new Exception("Unrelated craft result altered");
        }
        finally { uiHarmony.UnpatchSelf(); enabled.SetValue(null, false); }
        var queue = new System.Collections.Generic.List<CraftComponent.CraftQueueItem> {
            new CraftComponent.CraftQueueItem { id = definition.id, n = 2 },
            new CraftComponent.CraftQueueItem { id = "unrelated", n = 7 },
            new CraftComponent.CraftQueueItem { id = definition.id, infinite = true }
        };
        int removed = (int)guard.GetMethod("RemoveQueued", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { queue });
        if (removed != 2 || queue.Count != 1 || queue[0].id != "unrelated" || queue[0].n != 7) throw new Exception("Queue isolation failed");
    }
}
