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
        private static readonly Dictionary<string, List<CraftDefinition>> routes = new Dictionary<string, List<CraftDefinition>>();
        private static CraftItemGUI selected;
        private static CraftDefinition describing;
        private static readonly Dictionary<CraftItemGUI, int> anchors = new Dictionary<CraftItemGUI, int>();
        private static bool changing;
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
        private static string Text(CraftDefinition formula)
        {
            var text = new StringBuilder("[C9D8DC]Как получить ингредиенты[-]");
            var save = MainGame.me.save;
            foreach (var ingredient in formula.needs)
            {
                text.Append("\n\n").Append(Name(ingredient.id)).Append(":");
                List<CraftDefinition> producers;
                if (!routes.TryGetValue(ingredient.id, out producers) || producers.Count == 0)
                { text.Append("\nИсточник пока не проверен"); continue; }
                foreach (var route in producers)
                {
                    text.Append("\n• ");
                    for (int i = 0; i < route.needs.Count; i++)
                    {
                        if (i > 0) text.Append(" + ");
                        text.Append(Name(route.needs[i].id));
                        if (route.needs[i].value > 1) text.Append(" ×").Append(route.needs[i].value);
                    }
                    text.Append(" → ");
                    for (int i = 0; i < route.craft_in.Count; i++)
                    { if (i > 0) text.Append(", "); text.Append(Name(route.craft_in[i])); }
                    if (route.craft_type == CraftDefinition.CraftType.AlchemyDecompose && !save.IsSurveyComplete(CraftDefinition.CraftSubType.Alchemy, route.needs[0].id))
                        text.Append(" [E5B86D](сначала изучить материал)[-]");
                    if (!save.IsCraftVisible(route)) text.Append(" (рецепт закрыт)");
                    if (route.condition.has_expression) text.Append(" (есть дополнительное условие)");
                }
            }
            return text.ToString();
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
        }
        [HarmonyPatch(typeof(CraftItemGUI), "Redraw")]
        private static class Draw
        {
            private static void Prefix(CraftItemGUI __instance, out CraftDefinition __state)
            {
                __state = describing; describing = null;
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
                    describing = __instance.current_craft;
                }
            }
            private static Exception Finalizer(CraftItemGUI __instance, Exception __exception, CraftDefinition __state)
            {
                describing = __state;
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
