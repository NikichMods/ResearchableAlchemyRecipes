// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using HarmonyLib;

namespace NikichMods.Research
{
    internal static class ResearchRepetitionGuard
    {
        internal static bool Allow(bool active, bool finished, bool missingKnowledge) { return !active && !finished && missingKnowledge; }
        internal static void RepairDefinition(CraftDefinition craft)
        {
            if (!StudyTableWorkProbe.IsJob(craft)) return;
            if (!craft.disable_multi_craft || craft.force_multi_craft || craft.enqueue_type != CraftDefinition.EnqueueType.NeverEnqueue)
                StudyTableWorkProbe.Write("RAR_RESEARCH_SINGLE action=definition_repaired disable_multi=" + craft.disable_multi_craft + " force_multi=" + craft.force_multi_craft);
            craft.disable_multi_craft = true;
            craft.force_multi_craft = false;
            craft.enqueue_type = CraftDefinition.EnqueueType.NeverEnqueue;
        }
        private static bool Ready(CraftComponent component, CraftDefinition craft)
        {
            var save = MainGame.me.save;
            return Allow(component.is_crafting, save.completed_one_time_crafts.Contains(craft.id),
                SourceResearch.IsJob(craft.id) ? SourceResearch.ReadyToStart(craft.id, save) :
                craft.id != FormulaKnowledge.Job || FormulaKnowledge.HasRemaining(save));
        }
        internal static int RemoveQueued(List<CraftComponent.CraftQueueItem> queue)
        {
            return queue == null ? 0 : queue.RemoveAll(item => StudyTableWorkProbe.IsJobId(item.id));
        }
        [HarmonyPatch(typeof(CraftItemGUI), "Draw")]
        private static class Display
        {
            [HarmonyPriority(Priority.Last)]
            private static void Prefix(CraftDefinition craft_definition)
            {
                if (StudyTableWorkProbe.enabledProbe) RepairDefinition(craft_definition);
            }
        }
        // QueueEverything's verified postfix can force true even after definition flags reject it.
        [HarmonyPatch(typeof(CraftDefinition), "CanCraftMultiple")]
        private static class MultipleResult
        {
            [HarmonyPriority(Priority.Last)]
            private static void Postfix(CraftDefinition __instance, ref bool __result)
            {
                if (StudyTableWorkProbe.enabledProbe && StudyTableWorkProbe.IsJob(__instance)) __result = false;
            }
        }
        [HarmonyPatch(typeof(CraftComponent), "CraftReally")]
        private static class Start
        {
            [HarmonyPriority(Priority.Last)]
            private static bool Prefix(CraftComponent __instance, CraftDefinition craft, ref int amount, ref bool __result)
            {
                if (!StudyTableWorkProbe.enabledProbe || !StudyTableWorkProbe.IsJob(craft)) return true;
                if (amount <= 0 || !Ready(__instance, craft))
                {
                    __result = false;
                    StudyTableWorkProbe.Write("RAR_RESEARCH_SINGLE action=start_rejected before_payment=True"); return false;
                }
                if (amount != 1) StudyTableWorkProbe.Write("RAR_RESEARCH_SINGLE action=amount_clamped requested=" + amount + " effective=1");
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
                if (!StudyTableWorkProbe.enabledProbe || !StudyTableWorkProbe.IsJob(craft)) return true;
                bool queued = __instance.craft_queue != null && __instance.craft_queue.Exists(q => q.id == craft.id);
                if (amount <= 0 || !Ready(__instance, craft) || queued)
                {
                    StudyTableWorkProbe.Write("RAR_RESEARCH_SINGLE action=enqueue_rejected before_payment=True"); return false;
                }
                amount = 1; return true;
            }
        }
        [HarmonyPatch(typeof(CraftComponent), "TryStartCraftFromQueue")]
        private static class OldQueue
        {
            private static void Prefix(CraftComponent __instance)
            {
                if (!StudyTableWorkProbe.enabledProbe || __instance.craft_queue == null) return;
                if (!__instance.craft_queue.Exists(q => StudyTableWorkProbe.IsJobId(q.id))) return;
                var completed = MainGame.me.save.completed_one_time_crafts;
                int removed = __instance.craft_queue.RemoveAll(q => StudyTableWorkProbe.IsJobId(q.id) && completed.Contains(q.id));
                foreach (var q in __instance.craft_queue)
                    if (StudyTableWorkProbe.IsJobId(q.id)) { q.n = 1; q.infinite = false; }
                if (removed > 0) StudyTableWorkProbe.Write("RAR_RESEARCH_SINGLE action=completed_queue_removed count=" + removed);
            }
        }
        [HarmonyPatch(typeof(CraftComponent), "FinishCurrentCraft")]
        private static class PrepaidBatch
        {
            private static void Prefix(CraftComponent __instance)
            {
                if (!StudyTableWorkProbe.enabledProbe || !StudyTableWorkProbe.IsJob(__instance.current_craft)) return;
                try
                {
                // Existing craft_amount repeats were paid at start; queued entries are not prepaid.
                int extras = Math.Max(0, __instance.craft_amount - 1);
                __instance.craft_amount = 1;
                int removed = RemoveQueued(__instance.craft_queue);
                if (extras > 0)
                {
                    var craft = __instance.current_craft;
                    foreach (var item in craft.needs) __instance.wgo.DropItem(new Item(item.id, checked(item.value * extras)), Direction.None, default(UnityEngine.Vector3), 1f, true);
                    foreach (var item in craft.needs_from_wgo) __instance.wgo.data.AddItem(item.id, checked(item.value * extras));
                }
                StudyTableWorkProbe.Write("RAR_RESEARCH_SINGLE action=finish_once prepaid_extras_refunded=" + extras + " queued_entries_removed=" + removed);
                }
                catch (Exception ex) { StudyTableWorkProbe.Fail(ex); }
            }
        }
    }
}
