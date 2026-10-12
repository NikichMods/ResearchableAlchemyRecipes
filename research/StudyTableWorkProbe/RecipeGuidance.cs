// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace NikichMods.Research
{
    // Known ordinary formulas at both native manual alchemy tiers.
    internal static class RecipeGuidance
    {
        private static readonly HashSet<string> ordinary = new HashSet<string>();
        // User-selected optical correction exported twice by 0.1.15 Done.
        private const int HeaderDown = 9;
        private static readonly Dictionary<string, List<CraftDefinition>> routes = new Dictionary<string, List<CraftDefinition>>();
        private static CraftItemGUI selected;
        private static CraftGUI guidanceWindow;
        private static CraftDefinition describing;
        private static readonly Dictionary<CraftItemGUI, int> anchors = new Dictionary<CraftItemGUI, int>();
        private static bool changing;
        private static bool previewUnstudied;
        private static readonly Dictionary<CraftItemGUI, Layout> layouts = new Dictionary<CraftItemGUI, Layout>();
        private sealed class Anchor
        {
            private readonly Transform target;
            private readonly float relative;
            private readonly int absolute;
            internal Anchor(UIRect.AnchorPoint point) { target = point.target; relative = point.relative; absolute = point.absolute; }
            internal void Restore(UIRect.AnchorPoint point) { point.target = target; point.relative = relative; point.absolute = absolute; }
        }
        private sealed class Layout
        {
            private readonly Anchor iconTop, iconBottom, energyTop, energyBottom, textRight;
            private readonly UIWidget icons;
            private readonly Transform amountGroup;
            private readonly Vector3 amountPosition;
            private bool measured;
            internal Layout(CraftItemGUI row)
            {
                icons = row.ingredients_table.GetComponent<UIWidget>();
                if (icons == null || row.txt_spend == null) throw new InvalidOperationException("Missing inspected recipe header widgets");
                iconTop = new Anchor(icons.topAnchor); iconBottom = new Anchor(icons.bottomAnchor);
                energyTop = new Anchor(row.txt_spend.topAnchor); energyBottom = new Anchor(row.txt_spend.bottomAnchor);
                textRight = new Anchor(row.label_descr.rightAnchor);
                // Native amount container is a plain transform on a vertically centred,
                // stretched selection frame. Preserve every child/button/mod-owned handler.
                var frame = row.selection_frame;
                var backing = frame == null || frame.topAnchor.target == null ? null : frame.topAnchor.target.GetComponent<UIWidget>();
                if (row.amount_buttons != null && frame != null &&
                    row.amount_buttons.transform.parent == frame.transform &&
                    frame.pivot == UIWidget.Pivot.Left && frame.bottomAnchor.target == frame.topAnchor.target &&
                    frame.topAnchor.relative == 1f && frame.bottomAnchor.relative == 0f &&
                    backing != null && backing.topAnchor.target == row.transform && backing.bottomAnchor.target == row.transform &&
                    backing.topAnchor.relative == 1f && backing.bottomAnchor.relative == 0f &&
                    backing.topAnchor.absolute == 0 && backing.bottomAnchor.absolute == 0 && row.no_multi_quality_height > 0)
                {
                    amountGroup = row.amount_buttons.transform;
                    amountPosition = amountGroup.localPosition;
                }
                else StudyTableWorkProbe.Write("RAR_GUIDANCE_AMOUNT status=unchanged reason=unsupported_native_group");
            }
            private static void Set(UIRect.AnchorPoint point, Transform target, float relative, int absolute)
            { point.target = target; point.relative = relative; point.absolute = absolute; }
            internal void Apply(CraftItemGUI row)
            {
                // Vertical group anchors are not the visible-cell geometry after UITable
                // layout. Preserve horizontal owners and align actual final widgets below.
                icons.topAnchor.target = null; icons.bottomAnchor.target = null;
                row.txt_spend.topAnchor.target = null; row.txt_spend.bottomAnchor.target = null;
                Set(row.label_descr.rightAnchor, row.transform, 1f, -8);
                row.label_descr.UpdateAnchors();
            }
            private static float CenterY(UIWidget widget, Transform common)
            {
                var corners = widget.worldCorners;
                return common.InverseTransformPoint((corners[0] + corners[2]) * 0.5f).y;
            }
            private static void MoveY(Transform target, Transform common, float delta)
            {
                target.localPosition += target.parent.InverseTransformVector(common.TransformVector(new Vector3(0, delta, 0)));
            }
            internal void AlignFinal(CraftItemGUI row)
            {
                var cells = row.ingredients_table.GetComponentsInChildren<BaseItemCellGUI>();
                UIWidget cell = null;
                foreach (var candidate in cells)
                    if (candidate.gameObject.activeInHierarchy) { cell = candidate.GetComponent<UIWidget>(); if (cell != null) break; }
                if (cell == null) throw new InvalidOperationException("No visible native ingredient widget to measure");
                float desired = CenterY(row.label_name, row.transform) - HeaderDown;
                float ingredientBefore = CenterY(cell, row.transform) - desired;
                float energyBefore = CenterY(row.txt_spend, row.transform) - desired;
                MoveY(icons.transform, row.transform, -ingredientBefore);
                MoveY(row.txt_spend.transform, row.transform, -energyBefore);
                float ingredientError = CenterY(cell, row.transform) - desired;
                float energyError = CenterY(row.txt_spend, row.transform) - desired;
                if (amountGroup != null)
                {
                    // Native frame centre moves down by half of the row-height growth.
                    // Restore the normal-row placement without guessing button coordinates.
                    amountGroup.localPosition = amountPosition;
                    float growth = row.GetComponent<UIWidget>().height - row.no_multi_quality_height;
                    MoveY(amountGroup, row.transform, growth * 0.5f);
                }
                if (Math.Abs(ingredientError) > 0.1f || Math.Abs(energyError) > 0.1f)
                    throw new InvalidOperationException("Measured header alignment did not converge");
                if (!measured && row.label_descr.width > 40)
                {
                    measured = true;
                    StudyTableWorkProbe.Write("RAR_GUIDANCE_HEADER ingredient_before=" + ingredientBefore + " energy_before=" + energyBefore + " ingredient_after=" + ingredientError + " energy_after=" + energyError + " down_ui_pixels=" + HeaderDown + " owner=final_widget_bounds");
                    if (amountGroup != null) StudyTableWorkProbe.Write("RAR_GUIDANCE_AMOUNT status=aligned owner=native_amount_container extra_height=" + (row.GetComponent<UIWidget>().height - row.no_multi_quality_height));
                }
            }
            internal void Restore(CraftItemGUI row)
            {
                iconTop.Restore(icons.topAnchor); iconBottom.Restore(icons.bottomAnchor);
                energyTop.Restore(row.txt_spend.topAnchor); energyBottom.Restore(row.txt_spend.bottomAnchor);
                textRight.Restore(row.label_descr.rightAnchor);
                if (amountGroup != null) amountGroup.localPosition = amountPosition;
                row.label_descr.UpdateAnchors();
            }
        }
        internal static void Register(GameBalance balance)
        {
            routes.Clear();
            ordinary.Clear();
            var definitions = new Dictionary<string, ItemDefinition>();
            foreach (var item in balance.items_data) definitions[item.id] = item;
            var products = new HashSet<string>();
            foreach (var formula in balance.craft_data)
            {
                if (!IsOrdinary(formula, definitions)) continue;
                ordinary.Add(formula.id);
                products.Add(formula.GetFirstRealOutput().id);
                foreach (var ingredient in formula.needs)
                {
                    if (routes.ContainsKey(ingredient.id)) continue;
                    routes.Add(ingredient.id, FindRoutes(balance.craft_data, ingredient.id));
                }
            }
            if (ordinary.Count != 43 || products.Count != 34 || routes.Count != 35)
                throw new InvalidOperationException("Verified ordinary guidance corpus changed");
            AdditionalSources.Register(balance, new HashSet<string>(routes.Keys));
            StudyTableWorkProbe.Write("RAR_GUIDANCE_REGISTER formulas=" + ordinary.Count + " products=" + products.Count + " ingredients=" + routes.Count + " scope=known_ordinary_manual_tables");
        }
        internal static bool IsTarget(CraftDefinition craft)
        {
            return craft != null && ordinary.Contains(craft.id);
        }
        internal static bool IsOrdinary(CraftDefinition craft, IDictionary<string, ItemDefinition> definitions)
        {
            if (craft == null || craft.craft_type != CraftDefinition.CraftType.MixedCraft || craft.hidden) return false;
            var output = craft.GetFirstRealOutput();
            if (output == null || output.id.StartsWith("goo", StringComparison.Ordinal) || craft.id.Contains(":_:")) return false;
            if (!(craft.needs.Count == 2 && craft.craft_in.Contains("mf_alchemy_craft_02")) &&
                !(craft.needs.Count == 3 && craft.craft_in.Contains("mf_alchemy_craft_03"))) return false;
            for (int i = 0; i < craft.needs.Count; i++)
            {
                ItemDefinition definition;
                if (!definitions.TryGetValue(craft.needs[i].id, out definition) ||
                    ((int)definition.alch_type != i + 1 && definition.alch_type != ItemDefinition.AlchemyType.Universal)) return false;
            }
            return true;
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
            if (!StudyTableWorkProbe.enabledProbe || row == null || !IsTarget(row.current_craft)) return false;
            var save = MainGame.me.save;
            var table = row.craft_gui_interface.GetCrafteryWGO();
            return table != null && KnownForTable(row.current_craft, table.obj_id, save);
        }
        internal static bool SupportedStation(string id) { return id == "mf_alchemy_craft_02" || id == "mf_alchemy_craft_03"; }
        internal static bool KnownForTable(CraftDefinition formula, string station, GameSave save)
        {
            return IsTarget(formula) && SupportedStation(station) && formula.craft_in.Contains(station) &&
                save.unlocked_techs.Contains(SourceResearch.Technology) && save.completed_one_time_crafts.Contains(formula.id);
        }
        private static string Name(string id) { return GJL.L(id); }
        internal static string BuildText(CraftDefinition formula, Dictionary<string, List<CraftDefinition>> catalogue, GameSave save, Func<string, string> name, bool preview)
        { return BuildTextCore(formula, catalogue, save, name, preview, false); }
        private static string BuildTextCore(CraftDefinition formula, Dictionary<string, List<CraftDefinition>> catalogue, GameSave save, Func<string, string> name, bool preview, bool additional)
        {
            var text = new StringBuilder("[C9D8DC]Как получить ингредиенты[-]");
            bool pending = false;
            foreach (var ingredient in formula.needs)
            {
                text.Append("\n\n").Append(ReagentHeadingPalette.Heading(ingredient.id, name(ingredient.id)));
                bool hasAdditional = additional && AdditionalSources.Append(text, ingredient.id, save, name);
                List<CraftDefinition> producers;
                if (!catalogue.TryGetValue(ingredient.id, out producers) || producers.Count == 0)
                { if (!hasAdditional) text.Append("\nИсточник пока не проверен"); continue; }
                foreach (var group in GroupRoutes(producers, save))
                {
                    var route = group[0];
                    text.Append("\n• ");
                    for (int i = 0; i < route.needs.Count; i++)
                    {
                        if (i > 0) text.Append(" + ");
                        text.Append(name(route.needs[i].id));
                        if (route.needs[i].value > 1) text.Append(" ×").Append(route.needs[i].value);
                    }
                    text.Append(" → ");
                    var stations = new List<string>();
                    var yields = new List<int>();
                    foreach (var member in group) foreach (string station in member.craft_in) if (!stations.Contains(station))
                    {
                        stations.Add(station);
                        var produced = member.output.Find(item => item.id == ingredient.id);
                        yields.Add(produced == null ? 0 : AdditionalSources.FixedYield(produced));
                    }
                    for (int i = 0; i < stations.Count; i++)
                    {
                        if (i > 0) text.Append(" / ");
                        string stationName = name(stations[i]);
                        if (i == 1 && stations[0] == "mf_distcube_2_clay" && stations[1] == "mf_distcube_2_cuprum" && stationName.StartsWith(name(stations[0]) + " ", StringComparison.Ordinal))
                            stationName = stationName.Substring(name(stations[0]).Length + 1);
                        text.Append(stationName);
                        if (yields[i] > 1) text.Append(" — ").Append(yields[i]).Append(" ед.");
                    }
                    bool unstudied = route.craft_type == CraftDefinition.CraftType.AlchemyDecompose &&
                        ((!save.IsSurveyComplete(CraftDefinition.CraftSubType.Alchemy, route.needs[0].id)) || (preview && route.needs[0].id == "sulfur"));
                    if (unstudied) { pending = true; text.Append(" [E5B86D][не изучено][-]"); }
                    if (!save.IsCraftVisible(route)) text.Append(" (рецепт закрыт)");
                    if (route.condition.has_expression) text.Append(" (есть дополнительное условие)");
                }
            }
            if (pending) text.Append("\n\n[E5B86D]Получение ингредиентов из отмеченных материалов требует их изучения на исследовательском столе.[-]");
            return text.ToString();
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
        private static string Text(CraftDefinition formula) { return BuildTextCore(formula, routes, MainGame.me.save, Name, previewUnstudied, true); }
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
        private static void Select(CraftItemGUI row)
        {
            if (changing) return;
            var next = Supports(row) ? row : null;
            if (selected == next) return;
            changing = true;
            try
            {
                var old = selected; selected = next;
                if (old != null) old.Redraw();
                if (next != null)
                {
                    if (next.label_descr == null || !next.auto_height || next.hgt_widget == null)
                        throw new InvalidOperationException("Missing native description/auto-height contract");
                    next.Redraw();
                    StudyTableWorkProbe.Write("RAR_GUIDANCE_SHOW product=" + next.current_craft.GetFirstRealOutput().id + " ingredients=" + next.current_craft.needs.Count + " native_description=True height=" + next.GetComponent<UIWidget>().height + " text_width=" + next.label_descr.width + " text_height=" + next.label_descr.height);
                    StudyTableWorkProbe.Write("RAR_GUIDANCE_STUDY native_sulfur_studied=" + MainGame.me.save.IsSurveyComplete(CraftDefinition.CraftSubType.Alchemy, "sulfur") + " preview=" + previewUnstudied);
                }
                GUIElements.me.craft.craft_list_grid.Reposition();
                if (next != null && BaseGUI.for_gamepad)
                {
                    // Native OnOver scrolls before our postfix grows the card. Native detail
                    // opening likewise waits 0.03s for anchors before scrolling its new bounds.
                    var requested = next;
                    GJTimer.AddTimer(0.03f, delegate
                    {
                        if (requested == null || !ReferenceEquals(selected, requested) || !StudyTableWorkProbe.enabledProbe || !BaseGUI.for_gamepad || !GUIElements.me.craft.is_shown) return;
                        try
                        {
                            requested.Redraw();
                            GUIElements.me.craft.craft_list_grid.Reposition();
                            GUIElements.me.craft.scroll_view.InvalidateBounds();
                            if (requested.auto_scroll == null) throw new InvalidOperationException("Missing native card autoscroll");
                            requested.auto_scroll.Perform(false);
                            StudyTableWorkProbe.Write("RAR_GUIDANCE_SCROLL phase=after final_height=" + requested.GetComponent<UIWidget>().height + " native_bounds=True");
                        }
                        catch (Exception ex) { StudyTableWorkProbe.Fail(ex); }
                    });
                }
            }
            catch (Exception ex) { StudyTableWorkProbe.Fail(ex); }
            finally { changing = false; }
        }
        internal static void Restore()
        {
            guidanceWindow = null;
            describing = null;
            var old = selected; selected = null;
            if (old != null) old.Redraw();
            anchors.Clear();
            layouts.Clear();
        }
        [HarmonyPatch(typeof(CraftItemGUI), "Redraw")]
        private static class Draw
        {
            private static void Prefix(CraftItemGUI __instance, out CraftDefinition __state)
            {
                __state = describing; describing = null;
                Layout layout;
                if (layouts.TryGetValue(__instance, out layout)) layout.Restore(__instance);
                if (__instance.label_descr != null)
                {
                    var parent = __instance.label_descr.transform.parent.GetComponent<UIWidget>();
                    int anchor;
                    if (parent != null && anchors.TryGetValue(__instance, out anchor)) parent.bottomAnchor.absolute = anchor;
                }
                if (__instance == selected && Supports(__instance))
                {
                    var parent = __instance.label_descr.transform.parent.GetComponent<UIWidget>();
                    if (parent == null) throw new InvalidOperationException("Missing native description parent");
                    if (!anchors.ContainsKey(__instance)) anchors.Add(__instance, parent.bottomAnchor.absolute);
                    if (!layouts.TryGetValue(__instance, out layout)) { layout = new Layout(__instance); layouts.Add(__instance, layout); }
                    layout.Apply(__instance);
                    describing = __instance.current_craft;
                }
            }
            private static Exception Finalizer(CraftItemGUI __instance, Exception __exception, CraftDefinition __state)
            {
                describing = __state;
                if (__exception == null && StudyTableWorkProbe.enabledProbe && __instance == selected)
                {
                    try { layouts[__instance].AlignFinal(__instance); }
                    catch (Exception ex) { StudyTableWorkProbe.Fail(ex); }
                }
                if (__exception != null && __instance == selected) StudyTableWorkProbe.Fail(__exception);
                return __exception;
            }
        }
        [HarmonyPatch(typeof(CraftDefinition), "GetDescription")]
        private static class Description
        {
            private static void Postfix(CraftDefinition __instance, ref string __result)
            {
                if (ReferenceEquals(__instance, describing))
                    __result = string.IsNullOrEmpty(__result) ? Text(__instance) : __result + "\n\n" + Text(__instance);
            }
        }
        [HarmonyPatch(typeof(CraftItemGUI), "OnOver")]
        private static class Focus
        {
            private static void Postfix(CraftItemGUI __instance) { if (BaseGUI.for_gamepad && StudyTableWorkProbe.enabledProbe) Select(__instance); }
        }
        [HarmonyPatch(typeof(CraftItemGUI), "OnMouseOvered")]
        private static class Mouse
        {
            private static void Postfix(CraftItemGUI __instance) { if (!BaseGUI.for_gamepad && StudyTableWorkProbe.enabledProbe) Select(__instance); }
        }
        [HarmonyPatch(typeof(CraftGUI), "OpenAsAlchemy")]
        private static class Open
        {
            private static void Postfix(CraftGUI __instance)
            {
                if (!StudyTableWorkProbe.enabledProbe) return;
                // Own the entire opened supported known-formula list, including other rows.
                guidanceWindow = __instance.list_items.Exists(Supports) ? __instance : null;
                if (BaseGUI.for_gamepad) return;
                foreach (var row in __instance.list_items) if (Supports(row)) { Select(row); break; }
            }
        }
        [HarmonyPatch(typeof(CraftGUI), "Hide")]
        private static class Close
        {
            private static void Prefix() { Restore(); }
        }
    }
}
