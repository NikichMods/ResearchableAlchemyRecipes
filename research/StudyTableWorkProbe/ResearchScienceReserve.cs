// SPDX-License-Identifier: MPL-2.0
using HarmonyLib;

namespace NikichMods.Research
{
    // One native work per table; this records the actual prepaid fee, not a second science balance.
    internal static class ResearchScienceReserve
    {
        private const string Key = "rar_research_reserved_science";
        internal static int Read(Item table) { return table.GetParamInt(Key); }
        internal static void Record(Item table, int science) { table.SetParam(Key, science); }
        internal static int Refund(Item table, int legacyFee)
        {
            int fee = Read(table);
            if (fee <= 0) fee = legacyFee;
            if (fee > 0) table.AddItem("science", fee);
            Record(table, 0);
            return fee;
        }
        [HarmonyPatch(typeof(CraftComponent), "CraftReally")]
        private static class Start
        {
            private static void Postfix(CraftComponent __instance, CraftDefinition craft, bool __result)
            {
                if (!__result || !__instance.is_crafting || !SourceResearch.IsJob(craft.id)) return;
                Record(__instance.wgo.data, craft.needs_from_wgo[0].value);
                StudyTableWorkProbe.Write("RAR_SCIENCE_RESERVE action=paid job=" + craft.id + " reserved=" + Read(__instance.wgo.data));
            }
        }
        private sealed class CancelState
        {
            internal Item table;
            internal int fee;
        }
        [HarmonyPatch(typeof(CraftComponent), "Cancel")]
        private static class Cancel
        {
            private static void Prefix(CraftComponent __instance, out CancelState __state)
            {
                __state = __instance.is_crafting && __instance.current_craft != null && SourceResearch.IsJob(__instance.current_craft.id)
                    ? new CancelState { table = __instance.wgo.data, fee = Read(__instance.wgo.data) > 0 ? Read(__instance.wgo.data) : 1 } : null;
            }
            private static void Postfix(CraftComponent __instance, CancelState __state)
            {
                if (__state == null || __instance.is_crafting) return;
                // Older handed source jobs prepaid exactly one science; keep their paid fee on reload.
                int refunded = Refund(__state.table, __state.fee);
                StudyTableWorkProbe.Write("RAR_SCIENCE_RESERVE action=cancel_refund amount=" + refunded + " science=" + __state.table.GetItemsCount("science", true));
            }
        }
        [HarmonyPatch(typeof(CraftComponent), "ProcessFinishedCraft")]
        private static class Complete
        {
            private static void Prefix(CraftComponent __instance, out Item __state)
            {
                __state = __instance.current_craft != null && SourceResearch.IsJob(__instance.current_craft.id) ? __instance.wgo.data : null;
            }
            private static void Postfix(CraftComponent __instance, Item __state)
            {
                if (__state == null || __instance.is_crafting) return;
                Record(__state, 0);
                StudyTableWorkProbe.Write("RAR_SCIENCE_RESERVE action=completed refund=0");
            }
        }
    }
}
