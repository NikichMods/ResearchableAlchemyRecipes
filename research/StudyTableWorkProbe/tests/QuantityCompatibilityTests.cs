// SPDX-License-Identifier: MPL-2.0
using System;
using System.IO;
using System.Reflection;
using HarmonyLib;

class QuantityCompatibilityTests
{
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
    }
}
