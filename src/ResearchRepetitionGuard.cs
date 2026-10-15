// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using HarmonyLib;

namespace NikichMods.ResearchableAlchemyRecipes
{
    internal static class ResearchRepetitionGuard
    {
        internal static bool Allow(bool active, bool finished, bool missingKnowledge) { return !active && !finished && missingKnowledge; }
        internal static void RepairDefinition(CraftDefinition craft)
        {
            if (!Plugin.IsJob(craft)) return;
            craft.disable_multi_craft = true;
            craft.force_multi_craft = false;
            craft.enqueue_type = CraftDefinition.EnqueueType.NeverEnqueue;
        }
        private static bool Ready(CraftComponent component, CraftDefinition craft)
        {
            var save = MainGame.me.save;
            return Allow(component.is_crafting, save.completed_one_time_crafts.Contains(craft.id),
                SourceResearch.ReadyToStart(craft.id, save));
        }
        internal static int RemoveQueued(List<CraftComponent.CraftQueueItem> queue)
        {
            return queue == null ? 0 : queue.RemoveAll(item => Plugin.IsJobId(item.id));
        }
        [HarmonyPatch(typeof(CraftItemGUI), "Draw")]
        private static class Display
        {
            [HarmonyPriority(Priority.Last)]
            private static void Prefix(CraftDefinition craft_definition)
            {
                if (Plugin.Enabled) RepairDefinition(craft_definition);
            }
        }
        // QueueEverything's verified postfix can force true even after definition flags reject it.
        [HarmonyPatch(typeof(CraftDefinition), "CanCraftMultiple")]
        private static class MultipleResult
        {
            [HarmonyPriority(Priority.Last)]
            private static void Postfix(CraftDefinition __instance, ref bool __result)
            {
                if (Plugin.Enabled && Plugin.IsJob(__instance)) __result = false;
            }
        }
        [HarmonyPatch(typeof(CraftComponent), "CraftReally")]
        private static class Start
        {
            [HarmonyPriority(Priority.Last)]
            private static bool Prefix(CraftComponent __instance, CraftDefinition craft, ref int amount, ref bool __result)
            {
                if (!Plugin.Enabled || !Plugin.IsJob(craft)) return true;
                if (amount <= 0 || !Ready(__instance, craft))
                {
                    __result = false;
                    Plugin.Write("RAR_RESEARCH_SINGLE action=start_rejected before_payment=True"); return false;
                }
                if (amount != 1) Plugin.Write("RAR_RESEARCH_SINGLE action=amount_clamped requested=" + amount + " effective=1");
                amount = 1;
                return true;
            }
        }
        [HarmonyPatch(typeof(CraftComponent), "EnqueueCraft")]
        private static class Enqueue
        {
            [HarmonyPriority(Priority.Last)]
            private static bool Prefix(CraftComponent __instance, CraftDefinition craft, ref int amount)
            {
                if (!Plugin.Enabled || !Plugin.IsJob(craft)) return true;
                bool queued = __instance.craft_queue != null && __instance.craft_queue.Exists(q => q.id == craft.id);
                if (amount <= 0 || !Ready(__instance, craft) || queued)
                {
                    Plugin.Write("RAR_RESEARCH_SINGLE action=enqueue_rejected before_payment=True"); return false;
                }
                amount = 1; return true;
            }
        }
        [HarmonyPatch(typeof(CraftComponent), "TryStartCraftFromQueue")]
        private static class QueueEntry
        {
            private static void Prefix(CraftComponent __instance)
            {
                if (!Plugin.Enabled || __instance.craft_queue == null) return;
                if (!__instance.craft_queue.Exists(q => Plugin.IsJobId(q.id))) return;
                var completed = MainGame.me.save.completed_one_time_crafts;
                int removed = __instance.craft_queue.RemoveAll(q => Plugin.IsJobId(q.id) && completed.Contains(q.id));
                foreach (var q in __instance.craft_queue)
                    if (Plugin.IsJobId(q.id)) { q.n = 1; q.infinite = false; }
                if (removed > 0) Plugin.Write("RAR_RESEARCH_SINGLE action=completed_queue_removed count=" + removed);
            }
        }
        [HarmonyPatch(typeof(CraftComponent), "FinishCurrentCraft")]
        private static class PrepaidBatch
        {
            private static void Prefix(CraftComponent __instance)
            {
                if (!Plugin.Enabled || !Plugin.IsJob(__instance.current_craft)) return;
                try
                {
                // Existing craft_amount repeats were paid at start; queued entries are not prepaid.
                int extras = Math.Max(0, __instance.craft_amount - 1);
                __instance.craft_amount = 1;
                RemoveQueued(__instance.craft_queue);
                if (extras > 0)
                {
                    var craft = __instance.current_craft;
                    foreach (var item in craft.needs) __instance.wgo.DropItem(new Item(item.id, checked(item.value * extras)), Direction.None, default(UnityEngine.Vector3), 1f, true);
                    foreach (var item in craft.needs_from_wgo) __instance.wgo.data.AddItem(item.id, checked(item.value * extras));
                }
                }
                catch (Exception ex) { Plugin.Fail(ex); }
            }
        }
    }
}
