// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Globalization;
using HarmonyLib;

namespace NikichMods.ResearchableAlchemyRecipes
{
    internal static class ResearchCostDisplay
    {
        private static readonly Dictionary<CraftItemGUI, ResearchSciencePriceCell> cells = new Dictionary<CraftItemGUI, ResearchSciencePriceCell>();

        // Called only by the owned study-table tab switch, after the native window
        // is open. No new global card hook or custom Unity component participates in load.
        internal static void Refresh(CraftGUI window)
        {
            if (!Plugin.Enabled || !MainGame.game_started || MainGame.game_starting ||
                window == null || !window.is_shown) return;
            var table = window.GetCrafteryWGO();
            if (ReferenceEquals(table, null) || table.obj_id != "mf_alchemy_survey") return;
            foreach (var row in window.list_items)
            {
                if (row == null || !row.gameObject.activeSelf || row.txt_spend == null) continue;
                var craft = row.current_craft;
                if (craft == null || !SourceResearch.IsJob(craft.id)) continue;
                var fee = craft.needs_from_wgo.Find(item => item.id == "science");
                if (fee == null) continue;
                ResearchSciencePriceCell cell;
                if (!cells.TryGetValue(row, out cell))
                {
                    cell = new ResearchSciencePriceCell(); cells.Add(row, cell);
                }
                cell.Show(row, window.object_info.right_item_prefab.x1.counter, fee.value, table.data.GetItemsCount("science", true));
            }
        }

        internal static void Restore()
        {
            foreach (var cell in cells.Values) cell.Dispose();
            cells.Clear();
        }

        internal static string Format(int price, int available, string nativeText)
        {
            if (price <= 0) return nativeText;
            string science = "[c](sci)[/c]" + price.ToString(CultureInfo.InvariantCulture);
            if (available < price) science = "[ff1111]" + science + "[-]";
            return string.IsNullOrEmpty(nativeText) ? science : science + "\n" + nativeText;
        }

        // Native spend text ignores non-fuel station inputs. Extend only our jobs;
        // needs_from_wgo remains the single source of the fee and native payment.
        [HarmonyPatch(typeof(CraftDefinition), "GetSpendTxt")]
        private static class SpendText
        {
            [HarmonyAfter("p1xel8ted.gyk.queueeverything")]
            [HarmonyPriority(Priority.Last)]
            private static void Postfix(CraftDefinition __instance, WorldGameObject wgo, ref string __result)
            {
                if (!Plugin.Enabled || !SourceResearch.IsJob(__instance.id) ||
                    ReferenceEquals(wgo, null) || wgo.obj_id != "mf_alchemy_survey") return;
                var fee = __instance.needs_from_wgo.Find(item => item.id == "science");
                if (fee != null) __result = Format(fee.value, wgo.data.GetItemsCount("science", true), __result);
            }
        }
    }
}
