// SPDX-License-Identifier: MPL-2.0
using System;
using System.Globalization;
using HarmonyLib;

namespace NikichMods.Research
{
    internal static class ResearchCostDisplay
    {
        [HarmonyPatch(typeof(CraftItemGUI), "Redraw")]
        private static class PriceCell
        {
            private static void Prefix(CraftItemGUI __instance)
            {
                var previous = __instance.GetComponent<ResearchSciencePriceCell>();
                if (previous != null) previous.Restore();
            }
            [HarmonyPriority(Priority.Last)]
            private static void Postfix(CraftItemGUI __instance)
            {
                var craft = __instance.current_craft;
                if (!StudyTableWorkProbe.enabledProbe || craft == null || !SourceResearch.IsJob(craft.id) ||
                    __instance.txt_spend == null) return;
                var table = __instance.craft_gui_interface.GetCrafteryWGO();
                if (ReferenceEquals(table, null) || table.obj_id != "mf_alchemy_survey") return;
                var fee = craft.needs_from_wgo.Find(item => item.id == "science");
                if (fee == null) return;
                var cell = __instance.GetComponent<ResearchSciencePriceCell>();
                if (cell == null) cell = __instance.gameObject.AddComponent<ResearchSciencePriceCell>();
                cell.Show(__instance.txt_spend, fee.value, table.data.GetItemsCount("science", true));
            }
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
                if (!StudyTableWorkProbe.enabledProbe || !SourceResearch.IsJob(__instance.id) ||
                    ReferenceEquals(wgo, null) || wgo.obj_id != "mf_alchemy_survey") return;
                var fee = __instance.needs_from_wgo.Find(item => item.id == "science");
                if (fee != null) __result = Format(fee.value, wgo.data.GetItemsCount("science", true), __result);
            }
        }
    }
}
