// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace NikichMods.Research
{
    // First visible-layout experiment: one already-known product, manual producer paths only.
    internal static class RecipeGuidance
    {
        private const string Product = "yellow_paint";
        // User-selected optical correction exported twice by 0.1.15 Done.
        private const int HeaderDown = 9;
        private static readonly Dictionary<string, List<CraftDefinition>> routes = new Dictionary<string, List<CraftDefinition>>();
        private static CraftItemGUI selected;
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
            private bool measured;
            internal Layout(CraftItemGUI row)
            {
                icons = row.ingredients_table.GetComponent<UIWidget>();
                if (icons == null || row.txt_spend == null) throw new InvalidOperationException("Missing inspected recipe header widgets");
                iconTop = new Anchor(icons.topAnchor); iconBottom = new Anchor(icons.bottomAnchor);
                energyTop = new Anchor(row.txt_spend.topAnchor); energyBottom = new Anchor(row.txt_spend.bottomAnchor);
                textRight = new Anchor(row.label_descr.rightAnchor);
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
                if (Math.Abs(ingredientError) > 0.1f || Math.Abs(energyError) > 0.1f)
                    throw new InvalidOperationException("Measured header alignment did not converge");
                if (!measured && row.label_descr.width > 40)
                {
                    measured = true;
                    StudyTableWorkProbe.Write("RAR_GUIDANCE_HEADER ingredient_before=" + ingredientBefore + " energy_before=" + energyBefore + " ingredient_after=" + ingredientError + " energy_after=" + energyError + " down_ui_pixels=" + HeaderDown + " owner=final_widget_bounds");
                }
            }
            internal void Restore(CraftItemGUI row)
            {
                iconTop.Restore(icons.topAnchor); iconBottom.Restore(icons.bottomAnchor);
                energyTop.Restore(row.txt_spend.topAnchor); energyBottom.Restore(row.txt_spend.bottomAnchor);
                textRight.Restore(row.label_descr.rightAnchor);
                row.label_descr.UpdateAnchors();
            }
        }
        internal static void Register(GameBalance balance)
        {
            routes.Clear();
            foreach (var formula in balance.craft_data)
            {
                if (!IsTarget(formula)) continue;
                foreach (var ingredient in formula.needs)
                {
                    if (routes.ContainsKey(ingredient.id)) continue;
                    routes.Add(ingredient.id, FindRoutes(balance.craft_data, ingredient.id));
                }
            }
            if (routes.Count != 2) throw new InvalidOperationException("Bounded known paint ingredient count changed");
            StudyTableWorkProbe.Write("RAR_GUIDANCE_REGISTER ingredients=" + routes.Count + " scope=known_yellow_manual_sources");
        }
        internal static bool IsTarget(CraftDefinition craft)
        {
            if (craft == null || craft.craft_type != CraftDefinition.CraftType.MixedCraft) return false;
            var output = craft.GetFirstRealOutput();
            return output != null && output.id == Product;
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
            return table != null && table.obj_id == "mf_alchemy_craft_02" &&
                save.unlocked_techs.Contains(SourceResearch.Technology) && save.completed_one_time_crafts.Contains(row.current_craft.id);
        }
        private static string Name(string id) { return GJL.L(id); }
        internal static string BuildText(CraftDefinition formula, Dictionary<string, List<CraftDefinition>> catalogue, GameSave save, Func<string, string> name, bool preview)
        {
            var text = new StringBuilder("[C9D8DC]Как получить ингредиенты[-]");
            bool pending = false;
            foreach (var ingredient in formula.needs)
            {
                text.Append("\n\n").Append(ReagentHeadingPalette.Heading(ingredient.id, name(ingredient.id)));
                List<CraftDefinition> producers;
                if (!catalogue.TryGetValue(ingredient.id, out producers) || producers.Count == 0)
                { text.Append("\nИсточник пока не проверен"); continue; }
                foreach (var route in producers)
                {
                    text.Append("\n• ");
                    for (int i = 0; i < route.needs.Count; i++)
                    {
                        if (i > 0) text.Append(" + ");
                        text.Append(name(route.needs[i].id));
                        if (route.needs[i].value > 1) text.Append(" ×").Append(route.needs[i].value);
                    }
                    text.Append(" → ");
                    for (int i = 0; i < route.craft_in.Count; i++)
                    { if (i > 0) text.Append(", "); text.Append(name(route.craft_in[i])); }
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
        private static string Text(CraftDefinition formula) { return BuildText(formula, routes, MainGame.me.save, Name, previewUnstudied); }
        internal static string QuantityCardUnavailable(CraftItemGUI card)
        {
            bool owns = selected != null && GUIElements.me != null && GUIElements.me.craft.is_shown && Supports(selected);
            return CardUnavailableReason(owns, card, owns ? GUIElements.me.craft.list_items : null, CraftItemGUI.current_overed);
        }
        internal static string CardUnavailableReason(bool owns, CraftItemGUI card, List<CraftItemGUI> rows, CraftItemGUI focused)
        {
            if (!owns) return null;
            if (ReferenceEquals(card, null)) return "guidance_missing_card";
            if (!rows.Exists(row => ReferenceEquals(row, card))) return "guidance_stale_card";
            if (!ReferenceEquals(card, focused)) return "guidance_card_not_focused";
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
                    StudyTableWorkProbe.Write("RAR_GUIDANCE_SHOW native_description=True height=" + next.GetComponent<UIWidget>().height + " text_width=" + next.label_descr.width + " text_height=" + next.label_descr.height);
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
                if (!StudyTableWorkProbe.enabledProbe || BaseGUI.for_gamepad) return;
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
