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
        var palette = asm.GetType("NikichMods.Research.ReagentHeadingPalette", true).GetMethod("Heading", Private);
        var heading = (string)palette.Invoke(null, new object[] { "alchemy_1_yellow", "localized example" });
        Assert(heading.StartsWith("[c][") && heading.Contains("localized example:") && heading.EndsWith("[-][/c]"), "Palette must preserve localized heading and close colour scope");
        Assert((string)palette.Invoke(null, new object[] { "unverified_future_reagent", "plain example" }) == "plain example:", "Unverified reagent assigned a guessed colour");
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
        const string footer = "Получение ингредиентов из отмеченных материалов требует их изучения на исследовательском столе.";
        Assert(multiplePending.IndexOf(footer, StringComparison.Ordinal) >= 0 && multiplePending.IndexOf(footer, StringComparison.Ordinal) == multiplePending.LastIndexOf(footer, StringComparison.Ordinal), "Shared explanation absent or duplicated per alternative");
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
        CheckBroadGuidance(asm, host.save);
        // UI method detours cannot be compiled by the standalone CLR: Unity ECalls require
        // the real engine. Resolve exact contracts here; installed patching remains a live gate.
        foreach (string name in new[] { "Redraw", "OnOver", "OnMouseOvered" })
            Assert(AccessTools.Method(typeof(CraftItemGUI), name, Type.EmptyTypes) != null, "Missing exact UI method: " + name);
        Assert(AccessTools.Method(typeof(CraftGUI), "OpenAsAlchemy", new[] { typeof(WorldGameObject) }) != null, "Missing alchemy UI entry");
        Assert(AccessTools.Method(typeof(CraftGUI), "Hide", new[] { typeof(bool) }) != null, "Missing UI close contract");
    }
    private static void CheckBroadGuidance(Assembly asm, GameSave save)
    {
        var guidance = asm.GetType("NikichMods.Research.RecipeGuidance", true);
        var classify = guidance.GetMethod("IsListCandidate", Private);
        var definitions = new Dictionary<string, ItemDefinition> {
            { "fixture_powder", new ItemDefinition { alch_type = ItemDefinition.AlchemyType.Powder } },
            { "fixture_fluid", new ItemDefinition { alch_type = ItemDefinition.AlchemyType.Fluid } },
            { "fixture_essence", new ItemDefinition { alch_type = ItemDefinition.AlchemyType.Essence } },
            { "fixture_universal", new ItemDefinition { alch_type = ItemDefinition.AlchemyType.Universal } }
        };
        var a = new CraftDefinition { id = "fixture_known_variant", craft_type = CraftDefinition.CraftType.MixedCraft,
            craft_in = new List<string> { "mf_alchemy_craft_02" }, needs = new List<Item> { new Item("fixture_powder", 1), new Item("fixture_fluid", 1) },
            output = new List<Item> { new Item("fixture_product", 1) } };
        Assert((bool)classify.Invoke(null, new object[] { a }), "Ordinary two-slot formula rejected");
        a.needs[0].id = "fixture_fluid";
        Assert((bool)classify.Invoke(null, new object[] { a }), "Visible nonstandard formula incorrectly rejected");
        a.needs[0].id = "fixture_universal";
        Assert((bool)classify.Invoke(null, new object[] { a }), "Native universal reagent rejected");
        a.needs.Add(new Item("fixture_essence", 1)); a.craft_in[0] = "mf_alchemy_craft_03";
        Assert((bool)classify.Invoke(null, new object[] { a }), "Three-slot table unsupported");
        a.craft_in[0] = "alchemy_table_zombie";
        Assert(!(bool)classify.Invoke(null, new object[] { a }), "Automated station included");
        a.craft_in[0] = "mf_alchemy_craft_03"; a.output[0].id = "goo";
        Assert((bool)classify.Invoke(null, new object[] { a }), "Known visible failure product incorrectly rejected by display catalogue");
        a.output[0].id = "fixture_product";
        definitions.Add("fixture_crop:1", new ItemDefinition());
        definitions.Add("fixture_crop:2", new ItemDefinition());
        var family = guidance.GetMethod("ResolveFamilyMember", Private);
        Assert((string)family.Invoke(null, new object[] { "fixture_crop", definitions }) == "fixture_crop:1", "Native family member not resolved deterministically");
        Assert(family.Invoke(null, new object[] { "fixture_crop:2", definitions }) == null, "Concrete variant replaced by family");
        Assert(family.Invoke(null, new object[] { "fixture_missing", definitions }) == null, "Missing material invented");
        a.hidden = true;
        Assert(!(bool)classify.Invoke(null, new object[] { a }), "Hidden catalogue entry included");
        a.hidden = false;
        var b = new CraftDefinition { id = "fixture_unknown_variant", craft_in = a.craft_in, needs = a.needs, output = a.output };
        save.completed_one_time_crafts.Add(a.id);
        var eligible = guidance.GetMethod("KnownForTable", Private);
        Assert((bool)eligible.Invoke(null, new object[] { a, "mf_alchemy_craft_03", save }), "Known exact formula unavailable");
        Assert(!(bool)eligible.Invoke(null, new object[] { b, "mf_alchemy_craft_03", save }), "Partial knowledge leaks another formula for the same product");
        Assert(!(bool)eligible.Invoke(null, new object[] { a, "mf_alchemy_craft_02", save }), "Wrong manufacturing station accepted");
        save.unlocked_techs.Clear();
        Assert(!(bool)eligible.Invoke(null, new object[] { a, "mf_alchemy_craft_03", save }), "Pre-alchemy guidance disclosed");
        save.unlocked_techs.Add("The Beginning Of Alchemy");
        var first = new CraftDefinition { id = "fixture_source_I", craft_type = CraftDefinition.CraftType.AlchemyDecompose,
            craft_in = new List<string> { "mf_distcube_2_clay" }, needs = new List<Item> { new Item("fixture_source", 1) }, output = new List<Item> { new Item("fixture_reagent", 1) } };
        var second = new CraftDefinition { id = "fixture_source_II", craft_type = first.craft_type,
            craft_in = new List<string> { "mf_distcube_2_cuprum" }, needs = first.needs, output = first.output };
        var grouping = guidance.GetMethod("GroupRoutes", Private);
        var sources = new List<CraftDefinition> { first, second };
        Assert(((List<List<CraftDefinition>>)grouping.Invoke(null, new object[] { sources, save })).Count == 1, "Equivalent tiers repeat the source");
        save.locked_crafts.Add(second.id);
        Assert(((List<List<CraftDefinition>>)grouping.Invoke(null, new object[] { sources, save })).Count == 2, "Different source visibility lost in grouping");
        save.locked_crafts.Remove(second.id); second.output = new List<Item> { new Item("fixture_reagent", 2) };
        Assert(((List<List<CraftDefinition>>)grouping.Invoke(null, new object[] { sources, save })).Count == 1, "Distinct tiers cannot share their common material");
        var tierFormula = new CraftDefinition { needs = new List<Item> { new Item("fixture_reagent", 1) } };
        var tierCatalogue = new Dictionary<string, List<CraftDefinition>> { { "fixture_reagent", sources } };
        Func<string, string> tierNames = id => id;
        string tiers = (string)guidance.GetMethod("BuildText", Private).Invoke(null, new object[] { tierFormula, tierCatalogue, save, tierNames, false });
        Assert(tiers.Contains("mf_distcube_2_clay") && tiers.Contains("mf_distcube_2_cuprum") && tiers.Contains("2 ед."), "Equipment/yield alternative lost in common-material grouping");
        second.craft_in[0] = first.craft_in[0];
        Assert(((List<List<CraftDefinition>>)grouping.Invoke(null, new object[] { sources, save })).Count == 2, "Different recipes at the same station collapsed");
        var bonusYield = new Item("fixture_reagent", 1) { min_value = new SmartExpression(), max_value = new SmartExpression() };
        bonusYield.min_value.FromString("2+Ppar(\"p_scientist\")");
        var fixedYield = asm.GetType("NikichMods.Research.AdditionalSources").GetMethod("FixedYield", Private);
        Assert((int)fixedYield.Invoke(null, new object[] { bonusYield }) == 0, "Dynamic yield presented as a fixed quantity");
        bonusYield.min_value.FromString("1"); bonusYield.max_value.default_value = 3;
        Assert((int)fixedYield.Invoke(null, new object[] { bonusYield }) == 0, "Random yield range presented as fixed");
        var balance = (GameBalance)FormatterServices.GetUninitializedObject(typeof(GameBalance));
        balance.items_data = new List<ItemDefinition> {
            new ItemDefinition { id = "fixture_water_container", can_be_used = true, drop_on_use = new List<Item> { new Item("water", 20) } },
            new ItemDefinition { id = "fixture_not_usable", drop_on_use = new List<Item> { new Item("water", 20) } }
        };
        balance.craft_data = new List<CraftDefinition> {
            new CraftDefinition { id = "ex:fixture:blood", needs_unlock = true, craft_in = new List<string> { "fixture_autopsy" }, output = new List<Item> { new Item("blood", 1) } },
            new CraftDefinition { id = "corp_burning", hidden = true, is_auto = true, output = new List<Item> { new Item("ash", 5) } },
            new CraftDefinition { id = "crematorium_corp_burning", hidden = true, is_auto = true, output = new List<Item> { new Item("ash", 5) } }
        };
        balance.craft_obj_data = new List<ObjectCraftDefinition> {
            new ObjectCraftDefinition { id = "cremation_builddesk:p:mf_pyre_placed", out_obj = "mf_pyre_placed", enabled = true },
            new ObjectCraftDefinition { id = "souls_builddesk:p:mf_crematorium", out_obj = "mf_crematorium", enabled = true }
        };
        balance.techs_data = new List<TechDefinition> { new TechDefinition { id = "fixture_required_skill", crafts = new List<string> { "ex:fixture:blood" } } };
        var extras = asm.GetType("NikichMods.Research.AdditionalSources", true);
        extras.GetMethod("Register", Private).Invoke(null, new object[] { balance, new HashSet<string> { "water", "blood", "ash" }, true });
        var core = guidance.GetMethod("BuildTextCore", Private);
        var empty = new Dictionary<string, List<CraftDefinition>>();
        Func<string, string> names = id => id;
        foreach (string ingredient in new[] { "water", "blood", "ash" })
        {
            var formula = new CraftDefinition { needs = new List<Item> { new Item(ingredient, 1) } };
            string text = (string)core.Invoke(null, new object[] { formula, empty, save, names, false, true });
            Assert(!text.Contains("Источник пока") && !text.Contains("[не изучено]"), "Procedural source absent or incorrectly needs material Study");
            if (ingredient == "water") Assert(text.Contains("fixture_water_container") && text.Contains("20 ед.") && !text.Contains("fixture_not_usable"), "Item-use eligibility/yield wrong");
            if (ingredient == "blood") Assert(text.Contains("извлечение") && text.Contains("извлечение из тела") && text.Contains("fixture_required_skill"), "Body-part or declared technology condition absent");
            if (ingredient == "ash") Assert(text.Contains("сожжение") && text.Contains("mf_pyre_placed") && text.Contains("mf_crematorium"), "Hidden native automatic channels missing or incorrectly shown as menu recipes");
        }
        // Equipment is stated once without losing independent source-study requirements.
        var another = new CraftDefinition { id = "fixture_other_material", craft_type = first.craft_type,
            craft_in = first.craft_in, needs = new List<Item> { new Item("fixture_other_source", 1) }, output = first.output };
        var sameEquipment = new Dictionary<string, List<CraftDefinition>> { { "fixture_reagent", new List<CraftDefinition> { first, another } } };
        string grouped = (string)guidance.GetMethod("BuildText", Private).Invoke(null, new object[] { tierFormula, sameEquipment, save, tierNames, false });
        Assert(grouped.Split(new[] { "mf_distcube_2_clay" }, StringSplitOptions.None).Length == 2 && grouped.Contains("fixture_source") && grouped.Contains("fixture_other_source"), "Repeated equipment or missing source in equipment section");
        extras.GetMethod("Register", Private).Invoke(null, new object[] { balance, new HashSet<string> { "ash" }, false });
        var ashFormula = new CraftDefinition { needs = new List<Item> { new Item("ash", 1) } };
        string noDlc = (string)core.Invoke(null, new object[] { ashFormula, empty, save, names, false, true });
        Assert(noDlc.Contains("mf_pyre_placed") && !noDlc.Contains("mf_crematorium"), "Absent DLC source displayed or ordinary cremation lost");
        var extraction1 = new CraftDefinition { id = "ex:fixture_I:blood", craft_in = new List<string> { "mf_preparation_1" }, output = new List<Item> { new Item("blood", 1) } };
        var extraction2 = new CraftDefinition { id = "ex:fixture_II:blood", craft_in = new List<string> { "mf_preparation_2" }, output = extraction1.output };
        balance.craft_data.Add(extraction1); balance.craft_data.Add(extraction2);
        extras.GetMethod("Register", Private).Invoke(null, new object[] { balance, new HashSet<string> { "blood" }, true });
        Func<string, string> preparationNames = id => id == "mf_preparation_1" ? "fixture table" : id == "mf_preparation_2" ? "fixture table II" : id;
        var bloodFormula = new CraftDefinition { needs = new List<Item> { new Item("blood", 1) } };
        string combined = (string)core.Invoke(null, new object[] { bloodFormula, empty, save, preparationNames, false, true });
        Assert(combined.Contains("fixture table I–II"), "Available equivalent preparation tiers not combined");
        save.locked_crafts.Add(extraction2.id);
        string separate = (string)core.Invoke(null, new object[] { bloodFormula, empty, save, preparationNames, false, true });
        Assert(!separate.Contains("fixture table I–II") && separate.Contains("рецепт закрыт"), "Closed extraction tier hidden in combined label");
        save.locked_crafts.Remove(extraction2.id);
    }
}
