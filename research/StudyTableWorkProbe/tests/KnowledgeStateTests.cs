// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

public static class KnowledgeStateTests
{
    public static int Main(string[] args)
    {
        AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e) {
            string name = new AssemblyName(e.Name).Name + ".dll";
            foreach (string dir in args) { string path = Path.Combine(dir, name); if (File.Exists(path)) return Assembly.LoadFrom(path); }
            return null;
        };
        try { Run(args[0]); Console.WriteLine("PASS: 8 original/setup combinations, duplicate reward, cleanup, snapshot reload and missing-snapshot rejection"); return 0; }
        catch (Exception ex) { Console.WriteLine(ex); return 1; }
    }
    private static void Assert(bool ok) { if (!ok) throw new Exception("State invariant failed"); }
    private static void Run(string dir)
    {
        var asm = Assembly.LoadFrom(Path.Combine(dir, "compile-check.dll"));
        var type = asm.GetType("NikichMods.Research.FormulaKnowledge", true);
        var flags = BindingFlags.Static | BindingFlags.NonPublic;
        var formulas = (List<CraftDefinition>)type.GetField("formulas", flags).GetValue(null);
        var originals = (List<string>)type.GetField("originals", flags).GetValue(null);
        formulas.Add(new CraftDefinition { id = "test_formula_a" }); formulas.Add(new CraftDefinition { id = "test_formula_b" });
        originals.Add("rar_probe_knowledge_original_017_test_a"); originals.Add("rar_probe_knowledge_original_017_test_b");
        var prepare = type.GetMethod("PrepareInput", flags); var grant = type.GetMethod("Grant", flags); var restore = type.GetMethod("Restore", flags);
        for (int mask = 0; mask < 4; mask++) for (int partial = 0; partial < 2; partial++)
        {
            var save = (GameSave)FormatterServices.GetUninitializedObject(typeof(GameSave));
            save.completed_one_time_crafts = new List<string> { "unrelated" };
            if ((mask & 1) != 0) save.completed_one_time_crafts.Add(formulas[0].id);
            if ((mask & 2) != 0) save.completed_one_time_crafts.Add(formulas[1].id);
            var baseline = new HashSet<string>(save.completed_one_time_crafts);
            prepare.Invoke(null, new object[] { save, partial != 0 });
            var learned = (List<string>)grant.Invoke(null, new object[] { save });
            Assert(learned.Count == (partial == 0 ? 2 : 1));
            Assert(((List<string>)grant.Invoke(null, new object[] { save })).Count == 0);
            // A fresh native object consumes only serialized list data; no process-local baseline is used.
            var loaded = (GameSave)FormatterServices.GetUninitializedObject(typeof(GameSave));
            loaded.completed_one_time_crafts = new List<string>(save.completed_one_time_crafts);
            restore.Invoke(null, new object[] { loaded });
            Assert(baseline.SetEquals(loaded.completed_one_time_crafts));
            restore.Invoke(null, new object[] { loaded }); Assert(baseline.SetEquals(loaded.completed_one_time_crafts));
            try { grant.Invoke(null, new object[] { loaded }); throw new Exception("Missing snapshot accepted"); }
            catch (TargetInvocationException e) { Assert(e.InnerException is InvalidOperationException); }
        }
    }
}
