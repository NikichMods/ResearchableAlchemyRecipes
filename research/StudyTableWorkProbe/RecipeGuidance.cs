// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace NikichMods.Research
{
    // Guidance for the already-known native list at both manual alchemy tiers.
    internal static class RecipeGuidance
    {
        internal const string StudyMarker = "[693606][не изучено][-]";
        internal const string StudyExplanation = "Получение ингредиентов из отмеченных материалов требует их изучения на исследовательском столе.";
        private static readonly HashSet<string> catalogueEntries = new HashSet<string>();
        private static readonly Dictionary<string, List<CraftDefinition>> routes = new Dictionary<string, List<CraftDefinition>>();
        private static readonly Dictionary<string, string> familyNames = new Dictionary<string, string>();
        private static CraftGUI guidanceWindow;
        private static bool previewUnstudied;
        internal static void Register(GameBalance balance)
        {
            routes.Clear();
            catalogueEntries.Clear();
            var definitions = new Dictionary<string, ItemDefinition>();
            foreach (var item in balance.items_data) definitions[item.id] = item;
            familyNames.Clear();
            var products = new HashSet<string>();
            foreach (var formula in balance.craft_data)
            {
                if (!IsListCandidate(formula)) continue;
                catalogueEntries.Add(formula.id);
                products.Add(formula.GetFirstRealOutput().id);
                foreach (var ingredient in formula.needs)
                {
                    if (routes.ContainsKey(ingredient.id)) continue;
                    routes.Add(ingredient.id, FindRoutes(balance.craft_data, ingredient.id));
                }
            }
            foreach (var producers in routes.Values) foreach (var producer in producers) foreach (var input in producer.needs)
            {
                string member = ResolveFamilyMember(input.id, definitions);
                if (member != null) familyNames[input.id] = member;
            }
            AdditionalSources.Register(balance, new HashSet<string>(routes.Keys), DLCEngine.IsDLCAvailable(DLCEngine.DLCVersion.Souls), DLCEngine.IsDLCAvailable(DLCEngine.DLCVersion.Refugees));
            StudyTableWorkProbe.Write("RAR_GUIDANCE_REGISTER formulas=" + catalogueEntries.Count + " products=" + products.Count + " ingredients=" + routes.Count + " scope=known_native_list");
        }
        internal static bool IsListCandidate(CraftDefinition craft)
        {
            return craft != null && craft.craft_type == CraftDefinition.CraftType.MixedCraft && !craft.hidden &&
                craft.craft_in.Exists(SupportedStation);
        }
        internal static string ResolveFamilyMember(string id, IDictionary<string, ItemDefinition> definitions)
        {
            if (definitions.ContainsKey(id)) return null;
            string prefix = id + ":", first = null;
            foreach (string candidate in definitions.Keys)
                if (candidate.StartsWith(prefix, StringComparison.Ordinal) && (first == null || string.CompareOrdinal(candidate, first) < 0)) first = candidate;
            return first;
        }
        internal static List<CraftDefinition> FindRoutes(IEnumerable<CraftDefinition> crafts, string ingredient)
        {
            var result = new List<CraftDefinition>();
            foreach (var craft in crafts)
            {
                if (craft.hidden || craft.dont_show_in_hint || craft.needs.Count == 0 || craft.craft_in.Count == 0) continue;
                if (craft.craft_type != CraftDefinition.CraftType.AlchemyDecompose && craft.craft_type != CraftDefinition.CraftType.None) continue;
                // Automated mirror crafts are separate paths, not a second manual alternative.
                if (craft.craft_in.Exists(id => id.Contains("zombie"))) continue;
                if (craft.output.Exists(item => item.id == ingredient)) result.Add(craft);
            }
            result.Sort((a, b) => string.CompareOrdinal(a.id, b.id));
            return result;
        }
        private static bool Supports(CraftItemGUI row)
        {
            if (!StudyTableWorkProbe.enabledProbe || row == null || row.current_craft == null) return false;
            var save = MainGame.me.save;
            var table = row.craft_gui_interface.GetCrafteryWGO();
            return table != null && KnownForTable(row.current_craft, table.obj_id, save);
        }
        internal static bool SupportedStation(string id) { return id == "mf_alchemy_craft_02" || id == "mf_alchemy_craft_03"; }
        internal static bool KnownForTable(CraftDefinition formula, string station, GameSave save)
        {
            return formula != null && SupportedStation(station) && formula.craft_in.Contains(station) &&
                save.unlocked_techs.Contains(SourceResearch.Technology) && save.completed_one_time_crafts.Contains(formula.id);
        }
        internal static string Name(string id)
        {
            string member;
            string native = familyNames.TryGetValue(id, out member) ? member : id;
            var definition = GameBalance.me.GetDataOrNull<ItemDefinition>(native);
            return definition == null ? GJL.L(id) : definition.GetItemName(true);
        }
        internal static string BuildText(CraftDefinition formula, Dictionary<string, List<CraftDefinition>> catalogue, GameSave save, Func<string, string> name, bool preview)
        { return BuildTextCore(formula, catalogue, save, name, preview, false); }
        private static string BuildTextCore(CraftDefinition formula, Dictionary<string, List<CraftDefinition>> catalogue, GameSave save, Func<string, string> name, bool preview, bool additional)
        {
            var text = new StringBuilder("[4B3422]Как получить ингредиенты");
            bool pending = false;
            foreach (var ingredient in formula.needs)
            {
                text.Append("\n\n").Append(ReagentHeadingPalette.Heading(ingredient.id, name(ingredient.id)));
                bool hasAdditional = additional && AdditionalSources.Append(text, ingredient.id, save, name);
                List<CraftDefinition> producers;
                if (!catalogue.TryGetValue(ingredient.id, out producers) || producers.Count == 0)
                { if (!hasAdditional) text.Append("\nИсточник пока не проверен"); continue; }
                var sections = new List<SourceSection>();
                foreach (var group in GroupRoutes(producers, save))
                {
                    var route = group[0];
                    var line = new StringBuilder();
                    for (int i = 0; i < route.needs.Count; i++)
                    {
                        if (i > 0) line.Append(" + ");
                        line.Append(name(route.needs[i].id));
                    }
                    var stations = new List<string>();
                    var yields = new List<int>();
                    foreach (var member in group) foreach (string station in member.craft_in) if (!stations.Contains(station))
                    {
                        stations.Add(station);
                        var produced = member.output.Find(item => item.id == ingredient.id);
                        yields.Add(produced == null ? 0 : AdditionalSources.FixedYield(produced));
                    }
                    string key = string.Join("|", stations.ToArray()) + ":" + string.Join("|", yields.ConvertAll(n => n.ToString()).ToArray());
                    var section = sections.Find(x => x.key == key);
                    if (section == null)
                    {
                        var equipment = new StringBuilder();
                        bool tiers = stations.Count == 2 && stations[0] == "mf_distcube_2_clay" && stations[1] == "mf_distcube_2_cuprum" &&
                            yields[0] == yields[1] && name(stations[1]) == name(stations[0]) + " II";
                        if (tiers)
                        {
                            equipment.Append(name(stations[0])).Append(" I, II");
                        }
                        else for (int i = 0; i < stations.Count; i++)
                        {
                            if (i > 0) equipment.Append(" / ");
                            equipment.Append(name(stations[i]));
                        }
                        section = new SourceSection { key = key, equipment = equipment.ToString() };
                        sections.Add(section);
                    }
                    bool unstudied = route.craft_type == CraftDefinition.CraftType.AlchemyDecompose &&
                        ((!save.IsSurveyComplete(CraftDefinition.CraftSubType.Alchemy, route.needs[0].id)) || (preview && route.needs[0].id == "sulfur"));
                    if (unstudied) { pending = true; line.Append(" ").Append(StudyMarker); }
                    if (!save.IsCraftVisible(route)) line.Append(" (рецепт закрыт)");
                    if (route.condition.has_expression) line.Append(" (есть дополнительное условие)");
                    bool goo = route.needs.Exists(item => item.id.StartsWith("goo_", StringComparison.Ordinal));
                    if (goo) section.gooLines.Add("[AA]" + line + "[FF]");
                    else section.lines.Add(line.ToString());
                }
                foreach (var section in sections)
                {
                    text.Append("\n  ").Append(section.equipment).Append(":");
                    foreach (string line in section.lines) text.Append("\n    ").Append(line);
                    foreach (string line in section.gooLines) text.Append("\n    ").Append(line);
                }
            }
            if (pending) text.Append("\n\n[693606]").Append(StudyExplanation).Append("[-]");
            return text.Append("[-]").ToString();
        }
        private sealed class SourceSection
        {
            internal string key, equipment;
            internal readonly List<string> lines = new List<string>();
            internal readonly List<string> gooLines = new List<string>();
        }
        internal static List<List<CraftDefinition>> GroupRoutes(List<CraftDefinition> producers, GameSave save)
        {
            var result = new List<List<CraftDefinition>>();
            foreach (var route in producers)
            {
                var group = result.Find(g => EquivalentSources(g[0], route) && save.IsCraftVisible(g[0]) == save.IsCraftVisible(route));
                if (group == null) result.Add(new List<CraftDefinition> { route }); else group.Add(route);
            }
            return result;
        }
        private static bool EquivalentSources(CraftDefinition a, CraftDefinition b)
        {
            if (a.craft_type != b.craft_type || a.is_auto != b.is_auto || a.condition.has_expression || b.condition.has_expression || a.needs.Count != b.needs.Count || a.output.Count != b.output.Count || a.needs_from_wgo.Count != 0 || b.needs_from_wgo.Count != 0) return false;
            for (int i = 0; i < a.needs.Count; i++) if (!SameItem(a.needs[i], b.needs[i])) return false;
            bool sameStations = a.craft_in.Exists(station => b.craft_in.Contains(station));
            for (int i = 0; i < a.output.Count; i++)
            {
                var x = a.output[i]; var y = b.output[i];
                if (x.id != y.id || x.chance_group != y.chance_group || !SameExpression(x.self_chance, y.self_chance) || !SameExpression(x.common_chance, y.common_chance) || (sameStations && !SameItem(x, y))) return false;
            }
            return true;
        }
        private static bool SameItem(Item a, Item b)
        {
            return a.id == b.id && a.value == b.value && a.chance_group == b.chance_group &&
                SameExpression(a.min_value, b.min_value) && SameExpression(a.max_value, b.max_value) &&
                SameExpression(a.self_chance, b.self_chance) && SameExpression(a.common_chance, b.common_chance);
        }
        private static bool SameExpression(SmartExpression a, SmartExpression b)
        {
            return ReferenceEquals(a, b) || (a != null && b != null && a.default_value == b.default_value && a.GetRawExpressionString() == b.GetRawExpressionString());
        }
        internal static string Text(CraftDefinition formula) { return BuildTextCore(formula, routes, MainGame.me.save, Name, previewUnstudied, true); }
        internal static string QuantityUnavailableReason()
        {
            if (!StudyTableWorkProbe.enabledProbe || guidanceWindow == null || !guidanceWindow.is_shown) return null;
            var table = guidanceWindow.GetCrafteryWGO();
            if (table == null || !SupportedStation(table.obj_id)) return null;
            var focused = CraftItemGUI.current_overed;
            string reason = CardUnavailableReason(true, focused, guidanceWindow.list_items);
            if (reason == null) QuantityInputCompatibility.Synchronize(focused, table);
            return reason;
        }
        internal static string CardUnavailableReason(bool owns, CraftItemGUI card, List<CraftItemGUI> rows)
        {
            if (!owns) return null;
            if (ReferenceEquals(card, null)) return "guidance_missing_card";
            if (!rows.Exists(row => ReferenceEquals(row, card))) return "guidance_stale_card";
            // Check only initialized live-list cards; no Unity component lookup or fake focus.
            var craft = card.current_craft;
            if (craft == null || craft.needs == null) return "guidance_missing_formula";
            return null;
        }
        internal static void PreviewMenu()
        {
            GUIElements.me.dialog.Open("Проверка отметки изучения", previewUnstudied ? "Обычный вид" : "Сера не изучена: просмотр",
                delegate {
                    previewUnstudied = !previewUnstudied;
                    bool nativeStudied = MainGame.me.save.IsSurveyComplete(CraftDefinition.CraftSubType.Alchemy, "sulfur");
                    StudyTableWorkProbe.Write("RAR_GUIDANCE_PREVIEW enabled=" + previewUnstudied + " native_sulfur_studied=" + nativeStudied + " save_mutation=False");
                    GUIElements.me.dialog.OpenOK("Откройте известный рецепт жёлтой краски. Изменён только просмотр; знания сохранения и доступность на мельнице остаются настоящими.");
                }, "Закрыть", null, null, GameKey.Select, GameKey.Back,
                "Временный просмотр добавляет отметку только к сере и исчезает после перезапуска. Он не заменяет проверку настоящего изучения.");
        }
        internal static void Refresh()
        {
            if (!StudyTableWorkProbe.enabledProbe || guidanceWindow == null || !guidanceWindow.is_shown_and_top)
            { GuidanceTooltip.Hide(); return; }
            var row = CraftItemGUI.current_overed;
            if (row == null || !guidanceWindow.list_items.Contains(row) || !Supports(row))
            { GuidanceTooltip.Hide(); return; }
            GuidanceTooltip.Show(guidanceWindow, row);
        }
        internal static void Restore()
        {
            guidanceWindow = null;
            GuidanceTooltip.Hide();
        }
        [HarmonyPatch(typeof(CraftGUI), "OpenAsAlchemy")]
        private static class Open
        {
            private static void Postfix(CraftGUI __instance)
            {
                if (!StudyTableWorkProbe.enabledProbe) return;
                guidanceWindow = __instance.list_items.Exists(Supports) ? __instance : null;
            }
        }
        [HarmonyPatch(typeof(CraftGUI), "Hide")]
        private static class Close
        {
            private static void Prefix() { Restore(); }
        }
    }
}
