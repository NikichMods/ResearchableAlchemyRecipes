// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Text;

namespace NikichMods.Research
{
    // Native channels which are not a selectable fixed-input workstation recipe.
    internal static class AdditionalSources
    {
        internal sealed class Route
        {
            internal string source, equipment, method, phrase;
            internal int yield;
            internal CraftDefinition extraction;
            internal ObjectCraftDefinition blueprint;
            internal readonly List<string> technologies = new List<string>();
        }
        private static readonly Dictionary<string, List<Route>> routes = new Dictionary<string, List<Route>>();
        internal static void Register(GameBalance balance, HashSet<string> ingredients, bool soulsAvailable, bool croneAvailable)
        {
            routes.Clear();
            foreach (string id in ingredients) routes.Add(id, new List<Route>());
            // Verified npc_witch root answer #18: dialogue purchase, not vendor stock.
            if (croneAvailable && ingredients.Contains("memory_powder") && balance.items_data.Exists(item => item.id == "memory_powder"))
                routes["memory_powder"].Add(new Route { source = "npc_witch", method = "Покупка в диалоге", phrase = "@buy_memory_powder" });
            foreach (var item in balance.items_data)
                if (item.can_be_used && !item.autouse && string.IsNullOrEmpty(item.on_use_script))
                    foreach (var output in item.drop_on_use)
                        if (ingredients.Contains(output.id)) routes[output.id].Add(new Route {
                            source = item.id, method = "Использовать в инвентаре", yield = FixedYield(output)
                        });
            foreach (var craft in balance.craft_data)
                if (!craft.hidden && !craft.dont_show_in_hint && craft.IsBodyPartExtractionCraft() && craft.needs.Count == 0)
                    foreach (var output in craft.output)
                        if (ingredients.Contains(output.id))
                            foreach (string station in craft.craft_in) routes[output.id].Add(new Route {
                                source = "body", equipment = station, method = "извлечение из тела", yield = FixedYield(output), extraction = craft
                            });
            AddCremation(balance, ingredients, "corp_burning", "mf_pyre_placed", "cremation_builddesk:p:mf_pyre_placed");
            if (soulsAvailable) AddCremation(balance, ingredients, "crematorium_corp_burning", "mf_crematorium", "souls_builddesk:p:mf_crematorium");
            if (balance.techs_data != null)
                foreach (var sources in routes.Values) foreach (var source in sources)
                {
                    string unlock = source.extraction != null ? source.extraction.id : source.blueprint != null ? source.blueprint.id : null;
                    if (unlock == null) continue;
                    foreach (var technology in balance.techs_data) if (technology.crafts.Contains(unlock)) source.technologies.Add(technology.id);
                }
            int uses = 0, extractions = 0, cremations = 0, purchases = 0;
            foreach (var sources in routes.Values) foreach (var source in sources)
                if (source.phrase != null) purchases++; else if (source.extraction != null) extractions++; else if (source.blueprint != null) cremations++; else uses++;
            StudyTableWorkProbe.Write("RAR_GUIDANCE_EXTRA_REGISTER item_use=" + uses + " extraction=" + extractions + " cremation=" + cremations + " dialogue_purchase=" + purchases + " world_load=False");
        }
        private static void AddCremation(GameBalance balance, HashSet<string> ingredients, string craftId, string station, string blueprintId)
        {
            var craft = balance.craft_data.Find(c => c.id == craftId);
            var blueprint = balance.craft_obj_data.Find(c => c.id == blueprintId);
            if (craft == null && blueprint == null && craftId == "crematorium_corp_burning") return;
            if (craft == null || blueprint == null || !craft.is_auto || !craft.hidden || blueprint.out_obj != station)
                throw new InvalidOperationException("Missing verified cremation channel: " + craftId);
            foreach (var output in craft.output)
                if (ingredients.Contains(output.id)) routes[output.id].Add(new Route {
                    source = "body", equipment = station, method = "сожжение тела", yield = FixedYield(output), blueprint = blueprint
                });
        }
        internal static int FixedYield(Item output)
        {
            if (output.min_value == null) return output.max_value == null ? output.value : 0;
            float minimum;
            string raw = output.min_value.GetRawExpressionString();
            if (string.IsNullOrEmpty(raw)) return output.value;
            else if (!float.TryParse(raw, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out minimum)) return 0;
            if (minimum != output.value) return 0;
            if (output.max_value == null) return 0;
            string maximum = output.max_value.GetRawExpressionString();
            float upper;
            if (string.IsNullOrEmpty(maximum)) upper = output.max_value.default_value;
            else if (!float.TryParse(maximum, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out upper)) return 0;
            if (upper > minimum) return 0;
            return output.value;
        }
        internal static bool Append(StringBuilder text, string ingredient, GameSave save, Func<string, string> name)
        {
            List<Route> sources;
            if (!routes.TryGetValue(ingredient, out sources) || sources.Count == 0) return false;
            var remaining = new List<Route>(sources);
            var headings = new List<string>();
            var sections = new Dictionary<string, List<string>>();
            while (remaining.Count > 0)
            {
                var source = remaining[0]; remaining.RemoveAt(0);
                Route paired = remaining.Find(other => CanCombine(source, other, save));
                if (paired != null) remaining.Remove(paired);
                string equipment;
                if (source.equipment != null)
                {
                    equipment = name(source.equipment);
                    if (paired != null)
                    {
                        string first = name("mf_preparation_1"), second = name("mf_preparation_2");
                        equipment = second == first + " II" ? first + " I, II" : first + " / " + second;
                    }
                }
                else equipment = source.method;
                var line = new StringBuilder(name(source.source));
                if (source.phrase != null && (!save.unlocked_phrases.Contains(source.phrase) || save.black_list_of_phrases.Contains(source.phrase)))
                    line.Append(" (покупка пока недоступна)");
                if (source.equipment != null) line.Append(" (").Append(source.method).Append(")");
                if (source.extraction != null)
                {
                    if (!save.IsCraftVisible(source.extraction)) Closed(line, source, save, name, "рецепт закрыт");
                }
                if (source.blueprint != null && !save.IsCraftVisible(source.blueprint)) Closed(line, source, save, name, "чертёж закрыт");
                List<string> entries;
                if (!sections.TryGetValue(equipment, out entries))
                {
                    entries = new List<string>(); sections.Add(equipment, entries); headings.Add(equipment);
                }
                entries.Add(line.ToString());
            }
            foreach (string heading in headings)
            {
                text.Append("\n  ").Append(heading).Append(":");
                foreach (string line in sections[heading]) text.Append("\n    ").Append(line);
            }
            return true;
        }
        private static bool CanCombine(Route a, Route b, GameSave save)
        {
            bool tables = (a.equipment == "mf_preparation_1" && b.equipment == "mf_preparation_2") ||
                (b.equipment == "mf_preparation_1" && a.equipment == "mf_preparation_2");
            return tables && a.extraction != null && b.extraction != null && a.source == b.source && a.method == b.method &&
                !a.extraction.condition.has_expression && !b.extraction.condition.has_expression &&
                a.yield == b.yield && save.IsCraftVisible(a.extraction) && save.IsCraftVisible(b.extraction);
        }
        private static void Closed(StringBuilder text, Route source, GameSave save, Func<string, string> name, string fallback)
        {
            var missing = source.technologies.FindAll(id => !save.unlocked_techs.Contains(id));
            if (missing.Count == 0) { text.Append(" (").Append(fallback).Append(")"); return; }
            text.Append(" (требуется: ");
            for (int i = 0; i < missing.Count; i++) { if (i > 0) text.Append(", "); text.Append(name(missing[i])); }
            text.Append(")");
        }
    }
}
