// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using HarmonyLib;

public static class SourceTopicTests
{
    private const BindingFlags Private = BindingFlags.NonPublic | BindingFlags.Static;
    private static Type source;
    // The standalone CLR has no Unity Resources ECalls. Bag-item initialization is unrelated
    // to source visibility/reward; only this engine-dependent constructor helper is skipped.
    private static bool SkipBagInitialization() { return false; }
    private static object Call(string name, params object[] args) { return source.GetMethod(name, Private).Invoke(null, args); }
    private static void Assert(bool value, string message) { if (!value) throw new Exception(message); }
    private static GameSave Save()
    {
        var save = (GameSave)FormatterServices.GetUninitializedObject(typeof(GameSave));
        save.completed_one_time_crafts = new List<string> { "unrelated_knowledge" };
        save.unlocked_techs = new List<string> { "The Beginning Of Alchemy" };
        save.unlocked_crafts = new List<string>(); save.locked_crafts = new List<string>();
        return save;
    }
    public static int Main(string[] args)
    {
        AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e) {
            foreach (string dir in args) { string path = Path.Combine(dir, new AssemblyName(e.Name).Name + ".dll"); if (File.Exists(path)) return Assembly.LoadFrom(path); }
            return null;
        };
        try { RunWithFixture(args[0]); Console.WriteLine("PASS source topics: native visibility, independent gates, remaining reward, save isolation, cleanup and registration identity"); return 0; }
        catch (Exception ex) { Console.WriteLine(ex); return 1; }
    }
    private static void RunWithFixture(string dir)
    {
        var fixture = new Harmony("rar.offline.item-fixture");
        try
        {
            fixture.Patch(AccessTools.Method(typeof(Item), "CheckAndInitBagItem"), prefix: new HarmonyMethod(typeof(SourceTopicTests), "SkipBagInitialization"));
            Run(dir);
        }
        finally { fixture.UnpatchSelf(); }
    }
    private static void Run(string dir)
    {
        var asm = Assembly.LoadFrom(Path.Combine(dir, "compile-check.dll"));
        source = asm.GetType("NikichMods.Research.SourceResearch", true);
        var host = (MainGame)FormatterServices.GetUninitializedObject(typeof(MainGame));
        MainGame.me = host; host.save = Save();
        var balance = (GameBalance)FormatterServices.GetUninitializedObject(typeof(GameBalance));
        balance.craft_data = new List<CraftDefinition>(); balance.craft_obj_data = new List<ObjectCraftDefinition>();
        balance.items_data = new List<ItemDefinition>();
        var products = new[] { "yellow_paint", "green_paint", "brown_paint", "red_paint", "d_green_paint", "d_violet_paint", "violet_paint" };
        foreach (string product in products)
        {
            balance.items_data.Add(new ItemDefinition { id = product, icon = "fixture_native_icon_" + product });
            balance.craft_data.Add(new CraftDefinition { id = "fixture_" + product, craft_type = CraftDefinition.CraftType.MixedCraft, output = new List<Item> { new Item(product, 1) } });
            balance.craft_obj_data.Add(new ObjectCraftDefinition { id = "source_" + product, builder_ids = new List<string> { "keeper_room_builddesk" }, needs = new List<Item> { new Item(product, 1) } });
        }
        Call("Register", balance); Call("Register", balance);
        Assert(balance.craft_data.Count == 14, "Registration duplicated native jobs");
        var topics = (IDictionary)source.GetField("topics", Private).GetValue(null);
        Assert(topics.Count == 7, "Bounded registration count");
        var topic = topics["rar_probe_source_paint_0110_yellow_paint"];
        var job = (CraftDefinition)topic.GetType().GetField("job", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(topic);
        var visible = topic.GetType().GetField("visible", BindingFlags.NonPublic | BindingFlags.Instance);
        var formulas = (List<CraftDefinition>)topic.GetType().GetField("formulas", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(topic);
        Assert(job.needs.Count == 1 && job.needs[0].id == "faith" && job.output.Count == 0 && job.one_time_craft && !job.CanCraftMultiple(), "Sample-free single job contract");
        Assert(job.icon == "fixture_native_icon_yellow_paint", "Native item icon metadata ignored");
        var enabled = asm.GetType("NikichMods.Research.StudyTableWorkProbe").GetField("enabledProbe", Private);
        enabled.SetValue(null, true);
        var harmony = new Harmony("rar.offline.source-topics");
        var postfix = source.GetNestedType("Visibility", BindingFlags.NonPublic).GetMethod("Postfix", Private);
        harmony.Patch(typeof(GameSave).GetMethod("IsCraftVisible"), postfix: new HarmonyMethod(postfix));
        try
        {
            var save = host.save;
            Assert(!save.IsCraftVisible(job), "Uncaptured sources offered");
            source.GetField("snapshotSave", Private).SetValue(null, save);
            source.GetField("sourceAvailable", Private).SetValue(null, true); visible.SetValue(topic, 1);
            Assert(save.IsCraftVisible(job), "Positive native topic hidden");
            save.unlocked_techs.Clear(); Assert(!save.IsCraftVisible(job), "Alchemy gate ignored"); save.unlocked_techs.Add("The Beginning Of Alchemy");
            source.GetField("sourceAvailable", Private).SetValue(null, false); Assert(!save.IsCraftVisible(job), "Absent/disabled source offered"); source.GetField("sourceAvailable", Private).SetValue(null, true);
            visible.SetValue(topic, 0); Assert(!save.IsCraftVisible(job), "Locked blueprints offered"); visible.SetValue(topic, 1);
            job.hidden = true; Assert(!save.IsCraftVisible(job), "Native hidden result overwritten"); job.hidden = false;
            save.locked_crafts.Add(job.id); Assert(!save.IsCraftVisible(job), "Native locked result overwritten"); save.locked_crafts.Clear();
            var otherSave = Save(); Assert(!otherSave.IsCraftVisible(job), "Source snapshot crossed saves");
            var learned = (List<string>)Call("GrantRemaining", save, formulas);
            Assert(learned.Count == 1 && !save.IsCraftVisible(job), "Known formula still offered");
            Assert(((List<string>)Call("GrantRemaining", save, formulas)).Count == 0, "Repeated formula reward");
            save.completed_one_time_crafts.Add(job.id);
            // Model list-backed restart, not a claim of native serializer execution.
            var reloaded = Save(); reloaded.completed_one_time_crafts = new List<string>(save.completed_one_time_crafts);
            Call("Cleanup", reloaded);
            Assert(reloaded.completed_one_time_crafts.Contains(formulas[0].id) && reloaded.completed_one_time_crafts.Contains("unrelated_knowledge") && !reloaded.completed_one_time_crafts.Contains(job.id), "Cleanup erased real knowledge or left work marker");
            Assert((int)Call("Cleanup", reloaded) == 0, "Cleanup not idempotent");
            // Test shared remaining-only grant with partial knowledge and repeated/duplicate definitions.
            var partial = Save(); partial.completed_one_time_crafts.Add("a");
            var variants = new[] { new CraftDefinition { id = "a" }, new CraftDefinition { id = "b" }, new CraftDefinition { id = "b" } };
            Assert(((List<string>)Call("GrantRemaining", partial, variants)).Count == 1, "Partial reward duplicated known variant");
        }
        finally { harmony.UnpatchSelf(); enabled.SetValue(null, false); }
        for (int mask = 0; mask < 16; mask++)
            Assert((bool)Call("Eligible", (mask & 1) != 0, (mask & 2) != 0, (mask & 4) != 0 ? 1 : 0, (mask & 8) != 0 ? 1 : 0) == (mask == 15), "Independent eligibility gate");
        var guidance = asm.GetType("NikichMods.Research.RecipeGuidance", true);
        var producer = new CraftDefinition { id = "manual", craft_type = CraftDefinition.CraftType.AlchemyDecompose,
            craft_in = new List<string> { "mill" }, needs = new List<Item> { new Item("source", 1) }, output = new List<Item> { new Item("reagent", 1) } };
        var automatic = new CraftDefinition { id = "automatic", craft_in = new List<string> { "alchemy_table_zombie" },
            needs = producer.needs, output = producer.output };
        var mixed = new CraftDefinition { id = "mixed", craft_type = CraftDefinition.CraftType.MixedCraft,
            craft_in = producer.craft_in, needs = producer.needs, output = producer.output };
        var hidden = new CraftDefinition { id = "hidden", hidden = true, craft_in = producer.craft_in, needs = producer.needs, output = producer.output };
        var found = (List<CraftDefinition>)guidance.GetMethod("FindRoutes", Private).Invoke(null, new object[] { new[] { automatic, producer, mixed, hidden }, "reagent" });
        Assert(found.Count == 1 && ReferenceEquals(found[0], producer), "Guidance includes hidden/mixed/automated routes");
        var render = guidance.GetMethod("BuildText", Private);
        var formula = new CraftDefinition { needs = new List<Item> { new Item("reagent", 1) } };
        var catalogue = new Dictionary<string, List<CraftDefinition>> { { "reagent", new List<CraftDefinition> { producer } } };
        host.save = Save();
        Func<string, string> names = id => id;
        var pending = (string)render.Invoke(null, new object[] { formula, catalogue, host.save, names, false });
        Assert(pending.Contains("[не изучено]") && pending.Contains("исследовательском столе"), "Unstudied native source lacks marker/instruction");
        catalogue["reagent"].Add(producer);
        var multiplePending = (string)render.Invoke(null, new object[] { formula, catalogue, host.save, names, false });
        Assert(multiplePending.IndexOf("Для использования", StringComparison.Ordinal) == multiplePending.LastIndexOf("Для использования", StringComparison.Ordinal), "Shared instruction duplicated per alternative");
        catalogue["reagent"].RemoveAt(1);
        host.save.completed_one_time_crafts.Add("surv:source");
        var studied = (string)render.Invoke(null, new object[] { formula, catalogue, host.save, names, false });
        Assert(!studied.Contains("[не изучено]") && !studied.Contains("исследовательском столе"), "Studied native source still marked");
        producer.needs = new List<Item> { new Item("sulfur", 1) };
        host.save.completed_one_time_crafts.Add("surv:sulfur");
        var baselineKnowledge = new HashSet<string>(host.save.completed_one_time_crafts);
        var preview = (string)render.Invoke(null, new object[] { formula, catalogue, host.save, names, true });
        Assert(!preview.Contains("Просмотр:") && preview.Contains("[не изучено]") && baselineKnowledge.SetEquals(host.save.completed_one_time_crafts), "Preview changes knowledge or leaks diagnostic heading into recipe");
        producer.craft_type = CraftDefinition.CraftType.None;
        host.save.completed_one_time_crafts.Remove("surv:sulfur");
        var ordinary = (string)render.Invoke(null, new object[] { formula, catalogue, host.save, names, false });
        Assert(!ordinary.Contains("[не изучено]"), "Ordinary production incorrectly requires material Study");
        // UI method detours cannot be compiled by the standalone CLR: Unity ECalls require
        // the real engine. Resolve exact contracts here; installed patching remains a live gate.
        foreach (string name in new[] { "Redraw", "OnOver", "OnMouseOvered" })
            Assert(AccessTools.Method(typeof(CraftItemGUI), name, Type.EmptyTypes) != null, "Missing exact UI method: " + name);
        Assert(AccessTools.Method(typeof(CraftGUI), "OpenAsAlchemy", new[] { typeof(WorldGameObject) }) != null, "Missing alchemy UI entry");
        Assert(AccessTools.Method(typeof(CraftGUI), "Hide", new[] { typeof(bool) }) != null, "Missing UI close contract");
    }
}
