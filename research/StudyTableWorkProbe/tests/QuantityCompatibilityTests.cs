// SPDX-License-Identifier: MPL-2.0
using System;
using System.IO;
using System.Reflection;
using HarmonyLib;

class QuantityCompatibilityTests
{
    private static void ForceMultiple(ref bool __result) { __result = true; }
    private static bool scoped;
    public static int Main(string[] args)
    {
        AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e) {
            foreach (string dir in args) { string path = Path.Combine(dir, new AssemblyName(e.Name).Name + ".dll"); if (File.Exists(path)) return Assembly.LoadFrom(path); }
            return null;
        };
        try { Run(args); Console.WriteLine("PASS actual installed checker: original null failure, scoped guard, fallthrough and unpatch restoration"); return 0; }
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
        var harmony = new Harmony("rar.offline.quantity-test");
        try
        {
            install.Invoke(null, new object[] { harmony, new Func<bool>(() => scoped) });
            scoped = true;
            if (!(bool)method.Invoke(null, null)) throw new Exception("Scoped invalid-input result not returned");
            scoped = false; NullFailure(method);
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
