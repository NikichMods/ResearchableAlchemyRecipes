// SPDX-License-Identifier: MPL-2.0
namespace NikichMods.ResearchableAlchemyRecipes
{
    // Exact authored request owners in the verified 1.407 NPC graphs.
    // Read the already-serialized journal; GetOrCreateNPC would initialize balance
    // and could add encounter/achievement state, so it is deliberately not used.
    internal static class QuestResearchSources
    {
        internal static bool Known(GameSave save, string product)
        {
            if (save == null || save.known_npcs == null) return false;
            string npcId, taskId;
            if (product == "pot_heal") { npcId = "npc_witch"; taskId = "npc_clotho_task_1"; }
            else if (product == "acid") { npcId = "npc_astrologer"; taskId = "astrologer_acid"; }
            else return false;
            var npc = save.known_npcs.GetNPC(npcId);
            if (npc == null) return false;
            var state = npc.GetQuestState(taskId);
            // A fulfilled request remains evidence that the product was named;
            // purchasing/receiving the item need not have taught its formulas.
            return state == KnownNPC.TaskState.State.Visible || state == KnownNPC.TaskState.State.Complete;
        }
    }
}
