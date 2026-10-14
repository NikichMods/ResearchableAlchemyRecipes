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
        save.unlocked_phrases = new List<string>(); save.black_list_of_phrases = new List<string>();
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
        var nativeLanguage = (GJL)FormatterServices.GetUninitializedObject(typeof(GJL));
        nativeLanguage.id = "ru"; nativeLanguage.dict = new Dictionary<string, string>();
        AccessTools.Field(typeof(GJL), "cur_lng").SetValue(null, nativeLanguage);
        var asm = Assembly.LoadFrom(Path.Combine(dir, "compile-check.dll"));
        source = asm.GetType("NikichMods.Research.SourceResearch", true);
        var host = (MainGame)FormatterServices.GetUninitializedObject(typeof(MainGame));
        MainGame.me = host; host.save = Save();
        var balance = (GameBalance)FormatterServices.GetUninitializedObject(typeof(GameBalance));
        balance.craft_data = new List<CraftDefinition>(); balance.craft_obj_data = new List<ObjectCraftDefinition>();
        balance.items_data = new List<ItemDefinition>();
        var products = new[] { "yellow_paint", "green_paint", "brown_paint", "red_paint", "d_green_paint", "d_violet_paint", "violet_paint", "black_paint", "blue_paint", "white_paint", "glue", "acid", "alkali", "aromatizer", "dark_elixir", "energy_elixir", "flask_green_l_plus", "flavor_enhancer", "freshener", "golden_elixir", "growth_enhancer", "preservative", "silver_elixir", "tanning_agent" };
        for (int productIndex = 0; productIndex < products.Length; productIndex++)
        {
            string product = products[productIndex];
            balance.items_data.Add(new ItemDefinition { id = product, icon = "fixture_native_icon_" + product });
            int variants = product == "black_paint" || product == "white_paint" ? 3 :
                product == "energy_elixir" || product == "golden_elixir" || product == "preservative" || product == "silver_elixir" ? 2 : 1;
            for (int variant = 0; variant < variants; variant++)
                balance.craft_data.Add(new CraftDefinition { id = "fixture_" + product + "_" + variant, craft_type = CraftDefinition.CraftType.MixedCraft, output = new List<Item> { new Item(product, 1) } });
            if (productIndex < 11)
                balance.craft_obj_data.Add(new ObjectCraftDefinition { id = "source_" + product, builder_ids = new List<string> { "keeper_room_builddesk" }, needs = new List<Item> { new Item(product, 1) } });
            if (productIndex >= 11 || product == "black_paint" || product == "white_paint" || product == "glue")
                balance.craft_data.Add(new CraftDefinition { id = "church_" + product, craft_in = new List<string> { "table_book_constr" },
                    needs_unlock = true, needs = new List<Item> { new Item(product, 1) } });
        }
        balance.items_data.Add(new ItemDefinition { id = "zombie_juise", icon = "fixture_resurrection_icon" });
        balance.craft_data.Add(new CraftDefinition { id = "fixture_resurrection_formula", craft_type = CraftDefinition.CraftType.MixedCraft, output = new List<Item> { new Item("zombie_juise", 1) } });
        var witch = new VendorDefinition { id = "npc_witch", start_tire = 1 };
        AccessTools.Field(typeof(VendorDefinition), "product_types").SetValue(witch, new List<string> { "pot_base", "pot_cool" });
        balance.vendors_data = new List<VendorDefinition> { witch };
        foreach (string product in new[] { "pot_appetite", "pot_armor", "pot_berserk", "pot_damage", "pot_heal", "pot_heal_long", "pot_mana", "pot_speed" })
        {
            int tier = product == "pot_heal" || product == "pot_mana" ? 1 : product == "pot_appetite" || product == "pot_damage" || product == "pot_speed" ? 2 : 3;
            balance.items_data.Add(new ItemDefinition { id = product, icon = "fixture_potion_icon", base_count = tier == 1 ? 10 : 5,
                product_tier = tier, product_types = new List<string> { tier == 1 ? "pot_base" : "pot_cool" } });
            for (int n = 0; n < (product == "pot_heal" ? 2 : 1); n++)
                balance.craft_data.Add(new CraftDefinition { id = "fixture_" + product + "_" + n, craft_type = CraftDefinition.CraftType.MixedCraft, output = new List<Item> { new Item(product, 1) } });
        }
        Call("Register", balance); Call("Register", balance);
        Assert(balance.craft_data.Count == 91, "Registration duplicated native jobs");
        var topics = (IDictionary)source.GetField("topics", Private).GetValue(null);
        Assert(topics.Count == 33, "Bounded registration count");
        var vendorData = new Item("fixture_vendor", 1);
        var healingItem = balance.items_data.Find(i => i.id == "pot_heal");
        Assert((int)Call("VendorStock", vendorData, false, healingItem, 1) == 10 && vendorData.inventory.Count == 0, "Cold stock projection mutates a vendor or misses first-tier stock");
        var armorItem = balance.items_data.Find(i => i.id == "pot_armor");
        Assert((int)Call("VendorStock", vendorData, false, armorItem, 1) == 1 && !(bool)Call("VendorVisible", true, true, 1, 3, 1), "Native cold higher-tier placeholder reveals a locked product");
        vendorData.inventory.Add(new Item("pot_heal", 3)); vendorData.inventory.Add(new Item("pot_mana", 2));
        Assert((int)Call("VendorStock", vendorData, true, healingItem, 1) == 3, "Initialized stock is replaced by theoretical replenishment");
        Assert((int)Call("VendorStock", vendorData, true, armorItem, 3) == 0, "Depleted initialized stock incorrectly gets cold placeholder");
        foreach (int tier in new[] { 1, 2, 3 })
            foreach (int required in new[] { 1, 2, 3 })
                Assert((bool)Call("VendorVisible", true, true, tier, required, 1) == (required <= tier), "Unlocked tier boundary changed");
        Assert(!(bool)Call("VendorVisible", false, true, 3, 1, 1) && !(bool)Call("VendorVisible", true, false, 3, 1, 1) &&
            !(bool)Call("VendorVisible", true, true, 3, 1, 0), "Trade/physical/current stock gates ignored");
        var churchOnly = topics["rar_probe_source_paint_0110_energy_elixir"];
        var churchSourceList = (List<CraftDefinition>)churchOnly.GetType().GetField("churchSources", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(churchOnly);
        Assert(churchSourceList.Count == 1 && ((IList)churchOnly.GetType().GetField("sources", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(churchOnly)).Count == 0, "Church-only topic incorrectly acquires room sources");
        Assert(!host.save.IsCraftVisible(churchSourceList[0]), "Locked church source visible");
        host.save.unlocked_crafts.Add(churchSourceList[0].id);
        Assert(host.save.IsCraftVisible(churchSourceList[0]), "Native unlocked church source hidden");
        host.save.locked_crafts.Add(churchSourceList[0].id);
        Assert(!host.save.IsCraftVisible(churchSourceList[0]), "Native locked override ignored");
        host.save.locked_crafts.Clear();
        var overlapping = topics["rar_probe_source_paint_0110_glue"];
        Assert(((IList)overlapping.GetType().GetField("sources", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(overlapping)).Count == 1 &&
            ((IList)overlapping.GetType().GetField("churchSources", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(overlapping)).Count == 1, "Overlapping source families were lost or made duplicate jobs");
        Assert((int)Call("VisibleSources", true, 2, false, 3) == 2 && (int)Call("VisibleSources", false, 2, true, 3) == 3 &&
            (int)Call("VisibleSources", true, 2, true, 3) == 5 && (int)Call("VisibleSources", false, 2, false, 3) == 0, "Unavailable family leaks visible recipes through another source");
        Assert((bool)Call("ChurchPhysical", false, false, false, false, false, true, true), "Enabled native church source unavailable");
        foreach (var blocked in new[] {
            new object[] { true, false, false, false, false, true, true },
            new object[] { false, true, false, false, false, true, true },
            new object[] { false, false, true, false, false, true, true },
            new object[] { false, false, false, true, false, true, true },
            new object[] { false, false, false, false, true, true, true },
            new object[] { false, false, false, false, false, false, true },
            new object[] { false, false, false, false, false, true, false } })
            Assert(!(bool)Call("ChurchPhysical", blocked), "Unavailable church source contributes topics");
        var busyFixture = (CraftComponent)FormatterServices.GetUninitializedObject(typeof(CraftComponent));
        busyFixture.crafts = new List<CraftDefinition> { churchSourceList[0] };
        foreach (bool busy in new[] { false, true })
        {
            busyFixture.is_crafting = busy;
            Assert((bool)Call("ChurchPhysical", false, false, false, false, false, busyFixture.has_visible_crafts, true), "Temporary native work hides the source");
        }
        var black = topics["rar_probe_source_paint_0110_black_paint"];
        var blackFormulas = (List<CraftDefinition>)black.GetType().GetField("formulas", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(black);
        string blackId = "rar_probe_source_paint_0110_black_paint";
        var descriptionSave = Save();
        Assert((string)Call("Describe", blackId, descriptionSave, "native description") == "native description", "Unknown topic marked as alternative research");
        descriptionSave.completed_one_time_crafts.Add(blackFormulas[0].id);
        var descriptionBefore = new HashSet<string>(descriptionSave.completed_one_time_crafts);
        Assert((string)Call("Describe", blackId, descriptionSave, "native description") == "native description\nЧасть рецептов известна. Открыть все варианты.", "Partial topic loses native description or explanation");
        var descriptionLanguage = (GJL)AccessTools.Field(typeof(GJL), "cur_lng").GetValue(null);
        foreach (string locale in GJL.LANGUAGES)
        {
            descriptionLanguage.id = locale;
            string description = (string)Call("Describe", blackId, descriptionSave, "");
            Assert(!string.IsNullOrWhiteSpace(description) && !description.Contains(blackFormulas[0].id), "Partial explanation absent or leaks formula identity: " + locale);
        }
        descriptionLanguage.id = "en";
        Assert((string)Call("Describe", blackId, descriptionSave, "") == "Some recipes are known. Unlock all variants.", "Partial description retained its old language");
        descriptionLanguage.id = "ru";
        Assert((string)Call("Describe", "ordinary_craft", descriptionSave, "unchanged") == "unchanged", "Partial label affects ordinary crafts");
        Assert((string)Call("Describe", blackId, null, "unchanged") == "unchanged", "Description requires loaded save");
        Assert(descriptionBefore.SetEquals(descriptionSave.completed_one_time_crafts), "Description changed knowledge");
        Call("GrantRemaining", descriptionSave, blackFormulas);
        Assert((string)Call("Describe", blackId, descriptionSave, "native description") == "native description", "Fully known group remains marked partial");
        var partiallyKnownBlack = Save(); partiallyKnownBlack.completed_one_time_crafts.Add(blackFormulas[0].id);
        Assert(((List<string>)Call("GrantRemaining", partiallyKnownBlack, blackFormulas)).Count == 2, "Registered three-variant group did not grant only its missing formulas");
        Assert(((List<string>)Call("GrantRemaining", partiallyKnownBlack, blackFormulas)).Count == 0, "Registered multi-variant reward repeated");
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
            var potionTopic = topics["rar_probe_source_paint_0110_pot_heal"];
            var potionJob = (CraftDefinition)potionTopic.GetType().GetField("job", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(potionTopic);
            potionTopic.GetType().GetField("visible", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(potionTopic, 1);
            Assert(!save.IsCraftVisible(potionJob), "Other source opens trade without Clotho explanation");
            save.unlocked_phrases.Add("@clotho_trade");
            source.GetField("sourceAvailable", Private).SetValue(null, false);
            Assert(save.IsCraftVisible(potionJob) && !save.IsCraftVisible(job), "Clotho source requires an unrelated station or enables room topics");
            save.black_list_of_phrases.Add("@clotho_trade");
            Assert(!save.IsCraftVisible(potionJob), "Blacklisted trade remains a source"); save.black_list_of_phrases.Clear();
            save.unlocked_techs.Remove("The Beginning Of Alchemy");
            Assert(!save.IsCraftVisible(potionJob), "Potion ignores alchemy gate"); save.unlocked_techs.Add("The Beginning Of Alchemy");
            var potionFormulas = (List<CraftDefinition>)potionTopic.GetType().GetField("formulas", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(potionTopic);
            save.completed_one_time_crafts.Add(potionFormulas[0].id);
            Assert(save.IsCraftVisible(potionJob) && ((List<string>)Call("GrantRemaining", save, potionFormulas)).Count == 1 && !save.IsCraftVisible(potionJob), "Partial potion knowledge is duplicated or full knowledge remains offered");
            source.GetField("sourceAvailable", Private).SetValue(null, true);
            var resurrection = topics["rar_probe_source_paint_0110_zombie_juise"];
            var resurrectionJob = (CraftDefinition)resurrection.GetType().GetField("job", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(resurrection);
            resurrection.GetType().GetField("visible", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(resurrection, 1);
            Assert(!save.IsCraftVisible(resurrectionJob), "Physical source bypasses resurrection technology");
            save.unlocked_techs.Add("Second chance");
            source.GetField("sourceAvailable", Private).SetValue(null, false);
            Assert(save.IsCraftVisible(resurrectionJob) && !save.IsCraftVisible(job), "Resurrection technology requires a station or enables unrelated physical sources");
            save.unlocked_techs.Remove("The Beginning Of Alchemy");
            Assert(!save.IsCraftVisible(resurrectionJob), "Resurrection topic bypasses alchemy access");
            save.unlocked_techs.Add("The Beginning Of Alchemy");
            var resurrectionFormulas = (List<CraftDefinition>)resurrection.GetType().GetField("formulas", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(resurrection);
            Assert(((List<string>)Call("GrantRemaining", save, resurrectionFormulas)).Count == 1 && !save.IsCraftVisible(resurrectionJob), "Known resurrection formula remains offered");
            save.unlocked_techs.Remove("Second chance");
            source.GetField("sourceAvailable", Private).SetValue(null, true);
            var churchJob = (CraftDefinition)churchOnly.GetType().GetField("job", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(churchOnly);
            var churchVisibleField = churchOnly.GetType().GetField("visible", BindingFlags.NonPublic | BindingFlags.Instance);
            churchVisibleField.SetValue(churchOnly, Call("VisibleSources", true, 0, false, 1));
            Assert(!save.IsCraftVisible(churchJob), "Room availability enables a church-only topic");
            churchVisibleField.SetValue(churchOnly, Call("VisibleSources", false, 0, true, 1));
            Assert(save.IsCraftVisible(churchJob), "Available church-only topic hidden");
            var churchFormulas = (List<CraftDefinition>)churchOnly.GetType().GetField("formulas", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(churchOnly);
            save.completed_one_time_crafts.Add(churchFormulas[0].id);
            Assert(save.IsCraftVisible(churchJob), "Partially known church-only group hidden");
            Assert(((List<string>)Call("GrantRemaining", save, churchFormulas)).Count == 1 && !save.IsCraftVisible(churchJob), "Church reward repeats known variant or leaves a fully known topic");
            var blackJob = (CraftDefinition)black.GetType().GetField("job", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(black);
            black.GetType().GetField("visible", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(black, 1);
            save.completed_one_time_crafts.Add(blackFormulas[0].id);
            Assert(save.IsCraftVisible(blackJob), "Partially known multi-variant topic hidden");
            Assert(((List<string>)Call("GrantRemaining", save, blackFormulas)).Count == 2 && !save.IsCraftVisible(blackJob), "Fully known multi-variant topic still offered");
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
            Assert(reloaded.completed_one_time_crafts.Contains(churchFormulas[1].id), "Cleanup erased earned church-source knowledge");
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
        const string footer = "Отмеченные материалы нужно сначала изучить на исследовательском столе.";
        Assert(multiplePending.IndexOf(footer, StringComparison.Ordinal) >= 0 && multiplePending.IndexOf(footer, StringComparison.Ordinal) == multiplePending.LastIndexOf(footer, StringComparison.Ordinal), "Shared explanation absent or duplicated per alternative");
        catalogue["reagent"].RemoveAt(1);
        var gooSource = new CraftDefinition { id = "first_goo", craft_type = producer.craft_type,
            craft_in = producer.craft_in, needs = new List<Item> { new Item("goo_yellow", 4) }, output = producer.output };
        var jellySource = new CraftDefinition { id = "jelly", craft_type = producer.craft_type,
            craft_in = producer.craft_in, needs = new List<Item> { new Item("jelly_green", 1) }, output = producer.output };
        catalogue["reagent"].Insert(0, gooSource);
        catalogue["reagent"].Add(jellySource);
        string gooText = (string)render.Invoke(null, new object[] { formula, catalogue, host.save, names, false });
        Assert(gooText.IndexOf("goo_yellow", StringComparison.Ordinal) > gooText.IndexOf("jelly_green", StringComparison.Ordinal), "Goo must follow ordinary sources even when catalogue order puts it first");
        Assert(gooText.Contains("[59]goo_yellow [693606][не изучено][-][FF]") && !gooText.Contains("[59]jelly_green"), "Goo row must dim its Study mark, preserve alpha scope and leave monster jelly ordinary");
        Assert(ReferenceEquals(catalogue["reagent"][0], gooSource), "Presentation reorders the native/source catalogue");
        Assert(!gooText.Contains("[59]mill:"), "Mixed equipment block heading was dimmed");
        gooSource.craft_in = new List<string> { "goo_station" };
        string gooOnly = (string)render.Invoke(null, new object[] { formula, catalogue, host.save, names, false });
        Assert(gooOnly.Contains("\n  [59]goo_station:[FF]") &&
            gooOnly.IndexOf("goo_station", StringComparison.Ordinal) > gooOnly.IndexOf("jelly_green", StringComparison.Ordinal),
            "Goo-only equipment block must have a dim heading and follow all ordinary sources");
        catalogue["reagent"].Remove(gooSource); catalogue["reagent"].Remove(jellySource);
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
        Assert(tiers.Contains("mf_distcube_2_clay") && tiers.Contains("mf_distcube_2_cuprum") && !tiers.Contains("ед."), "Equipment/yield alternative lost in common-material grouping");
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
        extras.GetMethod("Register", Private).Invoke(null, new object[] { balance, new HashSet<string> { "water", "blood", "ash" }, true, true });
        var core = guidance.GetMethod("BuildTextCore", Private);
        var empty = new Dictionary<string, List<CraftDefinition>>();
        Func<string, string> names = id => id;
        foreach (string ingredient in new[] { "water", "blood", "ash" })
        {
            var formula = new CraftDefinition { needs = new List<Item> { new Item(ingredient, 1) } };
            string text = (string)core.Invoke(null, new object[] { formula, empty, save, names, false, true });
            Assert(!text.Contains("Источник пока") && !text.Contains("[не изучено]"), "Procedural source absent or incorrectly needs material Study");
            if (ingredient == "water") Assert(text.Contains("\n  Использование предмета:\n    fixture_water_container") && !text.Contains("fixture_not_usable"), "Item-use equipment/source grouping or yield wrong");
            if (ingredient == "blood") Assert(text.Contains("Извлечение из тела") && !text.Contains("body (извлечение") && text.Contains("fixture_required_skill"), "Body-part or declared technology condition absent");
            if (ingredient == "ash") Assert(text.Contains("Сожжение тела") && text.Contains("mf_pyre_placed") && text.Contains("mf_crematorium"), "Hidden native automatic channels missing or incorrectly shown as menu recipes");
        }
        // Equipment is stated once without losing independent source-study requirements.
        var another = new CraftDefinition { id = "fixture_other_material", craft_type = first.craft_type,
            craft_in = first.craft_in, needs = new List<Item> { new Item("fixture_other_source", 1) }, output = first.output };
        var sameEquipment = new Dictionary<string, List<CraftDefinition>> { { "fixture_reagent", new List<CraftDefinition> { first, another } } };
        string grouped = (string)guidance.GetMethod("BuildText", Private).Invoke(null, new object[] { tierFormula, sameEquipment, save, tierNames, false });
        Assert(grouped.Split(new[] { "mf_distcube_2_clay" }, StringSplitOptions.None).Length == 2 && grouped.Contains("fixture_source") && grouped.Contains("fixture_other_source"), "Repeated equipment or missing source in equipment section");
        first.needs[0].value = 4;
        string quantityFree = (string)guidance.GetMethod("BuildText", Private).Invoke(null, new object[] { tierFormula, sameEquipment, save, tierNames, false });
        Assert(!quantityFree.Contains("×") && !quantityFree.Contains("ед.") && first.needs[0].value == 4, "Guide retains quantities or mutates native source input");
        extras.GetMethod("Register", Private).Invoke(null, new object[] { balance, new HashSet<string> { "ash" }, false, true });
        var ashFormula = new CraftDefinition { needs = new List<Item> { new Item("ash", 1) } };
        string noDlc = (string)core.Invoke(null, new object[] { ashFormula, empty, save, names, false, true });
        Assert(noDlc.Contains("mf_pyre_placed") && !noDlc.Contains("mf_crematorium"), "Absent DLC source displayed or ordinary cremation lost");
        var extraction1 = new CraftDefinition { id = "ex:fixture_I:blood", craft_in = new List<string> { "mf_preparation_1" }, output = new List<Item> { new Item("blood", 1) } };
        var extraction2 = new CraftDefinition { id = "ex:fixture_II:blood", craft_in = new List<string> { "mf_preparation_2" }, output = extraction1.output };
        balance.craft_data.Add(extraction1); balance.craft_data.Add(extraction2);
        extras.GetMethod("Register", Private).Invoke(null, new object[] { balance, new HashSet<string> { "blood" }, true, true });
        Func<string, string> preparationNames = id => id == "mf_preparation_1" ? "fixture table" : id == "mf_preparation_2" ? "fixture table II" : id;
        var bloodFormula = new CraftDefinition { needs = new List<Item> { new Item("blood", 1) } };
        string combined = (string)core.Invoke(null, new object[] { bloodFormula, empty, save, preparationNames, false, true });
        Assert(combined.Contains("\n  fixture table I, II:\n    Извлечение из тела") && !combined.Contains("I–II"), "Available preparation tiers/source format or readable numbering wrong");
        save.locked_crafts.Add(extraction2.id);
        string separate = (string)core.Invoke(null, new object[] { bloodFormula, empty, save, preparationNames, false, true });
        Assert(!separate.Contains("fixture table I, II") && separate.Contains("рецепт закрыт"), "Closed extraction tier hidden in combined label");
        save.locked_crafts.Remove(extraction2.id);
        var layout = asm.GetType("NikichMods.Research.GuidanceTextLayout").GetMethod("Format", Private);
        string marker = "[693606][не изучено][-]";
        Func<string, float> measure = value => System.Text.RegularExpressions.Regex.Replace(value, @"\[(?:[A-Fa-f0-9]{6}|[A-Fa-f0-9]{2})\]|\[-\]", "").Length;
        Func<string, float, string> wrap = (value, capacity) =>
        {
            var lines = new List<string>(); string line = "";
            foreach (string word in value.Split(' '))
            {
                string next = line.Length == 0 ? word : line + " " + word;
                if (line.Length > 0 && measure(next) > capacity) { lines.Add(line); line = word; }
                else line = next;
            }
            lines.Add(line); return string.Join("\n", lines.ToArray());
        };
        string narrow = (string)layout.Invoke(null, new object[] { "Heading:\n  Station:\n    long source name " + marker + " (closed route)", 18f, marker, measure, wrap });
        Assert(narrow.Contains("\n    long source\n    name\n    " + marker), "Material continuation lost indentation or atomic Study marker");
        Assert(narrow.Contains("\n    (closed route)"), "Source condition discarded or left-aligned by wrapping");
        string wide = (string)layout.Invoke(null, new object[] { "    石灰石 " + marker, 40f, marker, measure, wrap });
        Assert(wide == "    石灰石 " + marker, "Localized source name or fitting marker modified");
        string dimmed = (string)layout.Invoke(null, new object[] { "    [59]long goo source " + marker + "[FF]\n  Next equipment", 18f, marker, measure, wrap });
        Assert(dimmed.Contains(marker) && dimmed.IndexOf("[59]", StringComparison.Ordinal) < dimmed.IndexOf(marker, StringComparison.Ordinal) &&
            dimmed.IndexOf("[FF]", StringComparison.Ordinal) > dimmed.IndexOf(marker, StringComparison.Ordinal) && dimmed.EndsWith("\n  Next equipment", StringComparison.Ordinal),
            "Wrapping loses atomic Study marker or dimming/reset boundary");
        balance.items_data.Add(new ItemDefinition { id = "memory_powder" });
        var memoryFormula = new CraftDefinition { needs = new List<Item> { new Item("memory_powder", 1) } };
        extras.GetMethod("Register", Private).Invoke(null, new object[] { balance, new HashSet<string> { "memory_powder" }, false, true });
        string memory = (string)core.Invoke(null, new object[] { memoryFormula, empty, save, names, false, true });
        Assert(memory.Contains("Покупка") && memory.Contains("npc_witch") && memory.Contains("покупка пока недоступна") && !memory.Contains("Источник пока") && !memory.Contains("[не изучено]"), "Dialogue source/closed state missing or incorrectly needs Study");
        save.unlocked_phrases.Add("@buy_memory_powder");
        memory = (string)core.Invoke(null, new object[] { memoryFormula, empty, save, names, false, true });
        Assert(memory.Contains("npc_witch") && !memory.Contains("покупка пока недоступна"), "Unlocked purchase remains marked closed");
        save.black_list_of_phrases.Add("@buy_memory_powder");
        memory = (string)core.Invoke(null, new object[] { memoryFormula, empty, save, names, false, true });
        Assert(memory.Contains("покупка пока недоступна"), "Blacklisted dialogue purchase appears available");
        extras.GetMethod("Register", Private).Invoke(null, new object[] { balance, new HashSet<string> { "memory_powder" }, false, false });
        memory = (string)core.Invoke(null, new object[] { memoryFormula, empty, save, names, false, true });
        Assert(!memory.Contains("npc_witch"), "Absent Game of Crone still shows DLC dialogue source");
        CheckLocalization(asm, balance, save);
    }
    private static void CheckLocalization(Assembly asm, GameBalance balance, GameSave save)
    {
        var localization = asm.GetType("NikichMods.Research.Localization", true);
        var keyType = localization.GetNestedType("Key", BindingFlags.NonPublic);
        var get = localization.GetMethod("Get", Private, null, new[] { keyType, typeof(string) }, null);
        Func<string, string, string> text = (key, locale) => (string)get.Invoke(null, new[] { Enum.Parse(keyType, key), (object)locale });
        var current = (GJL)AccessTools.Field(typeof(GJL), "cur_lng").GetValue(null);
        current.dict["native_fixture"] = "preserved";
        var guidance = asm.GetType("NikichMods.Research.RecipeGuidance", true);
        var core = guidance.GetMethod("BuildTextCore", Private);
        var empty = new Dictionary<string, List<CraftDefinition>>();
        var extras = asm.GetType("NikichMods.Research.AdditionalSources", true);
        current.id = "ru";
        extras.GetMethod("Register", Private).Invoke(null, new object[] { balance, new HashSet<string> { "water", "blood", "ash", "memory_powder" }, false, true });
        var beforeKnowledge = new HashSet<string>(save.completed_one_time_crafts);
        var layout = asm.GetType("NikichMods.Research.GuidanceTextLayout", true).GetMethod("Format", Private);
        Func<string, float> measure = value => System.Text.RegularExpressions.Regex.Replace(value, @"\[(?:[A-Fa-f0-9]{6}|[A-Fa-f0-9]{2})\]|\[-\]", "").Length;
        Func<string, float, string> wrap = (value, width) =>
        {
            var lines = new List<string>(); string line = "";
            foreach (string word in value.Split(' '))
            {
                string next = line.Length == 0 ? word : line + " " + word;
                if (line.Length > 0 && measure(next) > width) { lines.Add(line); line = word; }
                else line = next;
            }
            lines.Add(line); return string.Join("\n", lines.ToArray());
        };
        foreach (string locale in GJL.LANGUAGES)
        {
            current.id = locale;
            current.dict["pot_heal"] = "native_item_name_" + locale;
            foreach (object key in Enum.GetValues(keyType))
            {
                string value = (string)get.Invoke(null, new[] { key, (object)locale });
                Assert(!string.IsNullOrWhiteSpace(value) && !value.Contains("nikichmods.rar.ui."), "Missing translation: " + locale + "/" + key);
            }
            localization.GetMethod("Inject", Private).Invoke(null, null);
            Assert(current.dict["native_fixture"] == "preserved" && current.dict["nikichmods.rar.ui.SampleFreeProbeName"] == text("SampleFreeProbeName", locale), "Namespaced injection overwrites native text or misses current locale");
            Assert(current.dict["nikichmods.rar.ui.FormulaProbeName"].Contains("native_item_name_" + locale) && !current.dict["nikichmods.rar.ui.FormulaProbeName"].Contains("{0}"), "Diagnostic item title bypasses native names or leaks placeholder");
            foreach (var pair in new[] { new[] { "water", "ItemUse" }, new[] { "blood", "Extraction" }, new[] { "ash", "Cremation" }, new[] { "memory_powder", "Purchase" } })
            {
                var formula = new CraftDefinition { needs = new List<Item> { new Item(pair[0], 1) } };
                Func<string, string> names = id => id;
                string rendered = (string)core.Invoke(null, new object[] { formula, empty, save, names, false, true });
                Assert(rendered.Contains(text("HowTo", locale)) && rendered.Contains(text(pair[1], locale)), "Cached source method retained its registration language: " + locale + "/" + pair[1]);
                if (pair[0] == "memory_powder") Assert(rendered.Contains(text("PurchaseUnavailable", locale)), "Closed purchase lost localized availability");
            }
            var ordinary = new CraftDefinition { id = "locale_normal", craft_type = CraftDefinition.CraftType.AlchemyDecompose,
                craft_in = new List<string> { "locale_mill" }, needs = new List<Item> { new Item("locale_material", 1) }, output = new List<Item> { new Item("alchemy_1_yellow", 1) } };
            var goo = new CraftDefinition { id = "locale_goo", craft_type = ordinary.craft_type,
                craft_in = ordinary.craft_in, needs = new List<Item> { new Item("goo_yellow", 1) }, output = ordinary.output };
            var target = new CraftDefinition { needs = new List<Item> { new Item("alchemy_1_yellow", 1) } };
            var catalogue = new Dictionary<string, List<CraftDefinition>> { { "alchemy_1_yellow", new List<CraftDefinition> { goo, ordinary } } };
            Func<string, string> identity = id => id;
            string guide = (string)guidance.GetMethod("BuildText", Private).Invoke(null, new object[] { target, catalogue, save, identity, false });
            string marker = "[693606][" + text("Unstudied", locale) + "][-]";
            Assert(guide.Contains(marker) && guide.Contains(text("StudyExplanation", locale)) && guide.IndexOf("locale_material", StringComparison.Ordinal) < guide.IndexOf("goo_yellow", StringComparison.Ordinal), "Localized study/source structure changed: " + locale);
            string formatted = (string)layout.Invoke(null, new object[] { "    [59]long material name " + marker + "[FF]", measure(marker) + 4, marker, measure, wrap });
            Assert(formatted.Contains(marker) && formatted.Contains("[59]") && formatted.Contains("[FF]"), "Localized marker/alpha split: " + locale);
        }
        Assert(beforeKnowledge.SetEquals(save.completed_one_time_crafts), "Localization changes save knowledge");
        Assert(text("HowTo", "unknown_locale") == text("HowTo", "en"), "Unknown locale must fall back to English");
        Assert(text("StudyExplanation", "ru") == "Отмеченные материалы нужно сначала изучить на исследовательском столе.", "Accepted Russian footer changed");
        Assert(text("HowTo", "en") == "How to obtain ingredients", "English guidance header changed");
        Assert(AccessTools.Method(typeof(GJL), "LoadLanguageResource", new[] { typeof(string) }) != null, "Native language-load boundary missing");
        current.id = "ru"; localization.GetMethod("Inject", Private).Invoke(null, null);
        Console.WriteLine("PASS localization: " + Enum.GetValues(keyType).Length + " keys x 11 languages, registered-source language changes, namespaced injection, English fallback, atomic markers and save isolation");
    }
}
