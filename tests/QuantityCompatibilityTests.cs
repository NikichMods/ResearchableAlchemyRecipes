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
    private static MethodInfo synchronize;
    private static List<CraftItemGUI> rows;
    private static CraftItemGUI focused;
    private static WorldGameObject craftery;
    private static string CardScope()
    {
        string reason = (string)cardPolicy.Invoke(null, new object[] { guidanceScoped, focused, rows });
        if (guidanceScoped && reason == null) synchronize.Invoke(null, new object[] { focused, craftery });
        return reason;
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
        try { Run(args); Console.WriteLine("PASS actual installed checker: native-focus repair of absent/stale caches, both cache fields, missing/empty/stale native cards, ordinary/one-time fallthrough, study guard, outside-scope isolation and unpatch restoration"); return 0; }
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
        foreach (var attr in probe.GetType("NikichMods.ResearchableAlchemyRecipes.Plugin").GetCustomAttributesData())
            if (attr.AttributeType.Name == "BepInDependency" && (string)attr.ConstructorArguments[0].Value == guid) dependency = true;
        if (!dependency) throw new Exception("Missing dependency on the actual installed plugin GUID");
        var install = probe.GetType("NikichMods.ResearchableAlchemyRecipes.QuantityInputCompatibility", true).GetMethod("Install", BindingFlags.NonPublic | BindingFlags.Static);
        cardPolicy = probe.GetType("NikichMods.ResearchableAlchemyRecipes.RecipeGuidance", true).GetMethod("CardUnavailableReason", BindingFlags.NonPublic | BindingFlags.Static);
        synchronize = probe.GetType("NikichMods.ResearchableAlchemyRecipes.QuantityInputCompatibility", true).GetMethod("Synchronize", BindingFlags.NonPublic | BindingFlags.Static);
        craftery = (WorldGameObject)FormatterServices.GetUninitializedObject(typeof(WorldGameObject));
        rows = new List<CraftItemGUI>();
        var foreignCard = plugin.GetField("_craftItemGui", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        var foreignCraftery = plugin.GetField("_crafteryWgo", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        foreignCard.SetValue(null, Card()); NullFailure(method);
        foreignCard.SetValue(null, null);
        var harmony = new Harmony("rar.offline.quantity-test");
        try
        {
            install.Invoke(null, new object[] { harmony, new Func<bool>(() => scoped), new Func<string>(CardScope) });
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
            foreignCraftery.SetValue(null, null);
            if ((bool)method.Invoke(null, null)) throw new Exception("Valid native focus did not repair stale cache");
            if (!ReferenceEquals(foreignCard.GetValue(null), valid) || !ReferenceEquals(foreignCraftery.GetValue(null), craftery)) throw new Exception("Focus projection did not set both exact foreign inputs");
            foreignCard.SetValue(null, null);
            if ((bool)method.Invoke(null, null) || !ReferenceEquals(foreignCard.GetValue(null), valid)) throw new Exception("Missing cache did not recover from native focus");
            focused = empty;
            if (!(bool)method.Invoke(null, null)) throw new Exception("Stale native focus not contained");
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
            foreignCard.SetValue(null, null); foreignCraftery.SetValue(null, null); NullFailure(method);
            if (foreignCard.GetValue(null) != null || foreignCraftery.GetValue(null) != null) throw new Exception("Outside-scope selection repaired globally");
        }
        finally { harmony.UnpatchSelf(); }
        NullFailure(method);
        var guard = probe.GetType("NikichMods.ResearchableAlchemyRecipes.ResearchRepetitionGuard", true);
        var openFlag = plugin.GetField("_craftGuiOpen", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        openFlag.SetValue(null, true);
        probe.GetType("NikichMods.ResearchableAlchemyRecipes.QuantityInputCompatibility").GetMethod("OnOurFrameClosed", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
        if ((bool)openFlag.GetValue(null)) throw new Exception("Foreign close notification did not clear open flag");
        var definition = new CraftDefinition { id = "rar_research_fixture", force_multi_craft = true, disable_multi_craft = false, one_time_craft = true };
        var source = probe.GetType("NikichMods.ResearchableAlchemyRecipes.SourceResearch", true);
        var registry = (System.Collections.IDictionary)source.GetField("topics", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        registry.Add(definition.id, Activator.CreateInstance(source.GetNestedType("Topic", BindingFlags.NonPublic), true));
        if (!definition.CanCraftMultiple()) throw new Exception("Forced multi fixture not reproduced");
        guard.GetMethod("RepairDefinition", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { definition });
        if (definition.CanCraftMultiple() || definition.enqueue_type != CraftDefinition.EnqueueType.NeverEnqueue) throw new Exception("Native multiple-craft gate remains enabled");
        var enabled = probe.GetType("NikichMods.ResearchableAlchemyRecipes.Plugin").GetField("Enabled", BindingFlags.NonPublic | BindingFlags.Static);
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
