// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

[assembly: AssemblyVersion("0.1.0.0")]
[assembly: AssemblyFileVersion("0.1.0.0")]

namespace NikichMods.Research
{
    [BepInPlugin("nikichmods.research.sourceeligibility", "Source Eligibility Probe", Version)]
    public sealed class SourceEligibilityProbe : BaseUnityPlugin
    {
        public const string Version = "0.1.0";
        private const string Builder = "keeper_room_builddesk";
        private const string Spice = "taste_booster:alchemy_1_spice";
        private static readonly string[] Paints = { "yellow_paint", "green_paint", "brown_paint", "red_paint", "d_green_paint", "d_violet_paint", "violet_paint" };
        private static ManualLogSource log;
        private static bool ready;
        private GameSave observed;
        private Harmony harmony;
        private void Awake()
        {
            log = Logger;
            try
            {
                string hash;
                using (var sha = SHA256.Create()) using (var stream = File.OpenRead(typeof(GameSave).Assembly.Location))
                    hash = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                if (hash != "e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8") throw new InvalidOperationException("Unverified host: " + hash);
                harmony = new Harmony("nikichmods.research.sourceeligibility");
                harmony.PatchAll(typeof(SourceEligibilityProbe).Assembly);
                ready = true;
                Write("RAR_SOURCE_READY version=" + Version + " read_only=True save_mutation=False");
            }
            catch (Exception ex) { Fail(ex); }
        }
        private void OnDestroy() { ready = false; if (harmony != null) harmony.UnpatchSelf(); }
        private void Update()
        {
            if (!ready || !MainGame.game_started || MainGame.me == null || MainGame.me.save == null || MainGame.me.player == null || GameBalance.me == null || observed == MainGame.me.save) return;
            observed = MainGame.me.save;
            Capture("loaded_world");
        }
        private static void Write(string s) { if (log != null) log.LogInfo(s); }
        private static void Fail(Exception ex) { ready = false; if (log != null) log.LogError("RAR_SOURCE_ERROR disabled=True " + ex); }
        private static int Uses(CraftDefinition craft, string item)
        {
            foreach (var need in craft.needs) if (need.id == item) return 1;
            return 0;
        }
        private static void Knowledge(string product, out int total, out int known)
        {
            total = 0; known = 0;
            foreach (var c in GameBalance.me.craft_data)
            {
                if (c.craft_type != CraftDefinition.CraftType.MixedCraft) continue;
                var output = c.GetFirstRealOutput();
                if (output == null || output.id != product) continue;
                total++; if (MainGame.me.save.completed_one_time_crafts.Contains(c.id)) known++;
            }
        }
        private static void Capture(string phase)
        {
            if (!ready) return;
            try
            {
                var save = MainGame.me.save;
                bool alchemy = save.unlocked_techs.Contains("The Beginning Of Alchemy");
                var builder = WorldMap.GetWorldGameObjectByObjId(Builder, true);
                Write("RAR_SOURCE_SNAPSHOT phase=" + phase + " alchemy=" + alchemy + " builder_present=" + (builder != null) +
                    " builder_removed=" + (builder != null && builder.is_removed) + " builder_disabled=" + (builder != null && builder.GetParamInt("disabled_interactions") != 0) +
                    " tutorial_blocks=" + (MainGame.me.player.GetParamInt("in_tutorial") == 1 && MainGame.me.player.GetParamInt("tut_shown_tut_1") == 0));
                foreach (string paint in Paints)
                {
                    int sources = 0, visible = 0, total, known;
                    foreach (var c in GameBalance.me.craft_obj_data)
                        if (c.builder_ids.Contains(Builder) && Uses(c, paint) != 0) { sources++; if (save.IsCraftVisible(c)) visible++; }
                    Knowledge(paint, out total, out known);
                    Write("RAR_SOURCE_PAINT product=" + paint + " sources=" + sources + " native_visible=" + visible + " formulas=" + total + " known=" + known +
                        " remaining=" + (total - known) + " reachability_proven=False");
                }
                var merchant = save.known_npcs.GetNPC("npc_merchant");
                int spiceTotal, spiceKnown; Knowledge(Spice, out spiceTotal, out spiceKnown);
                Write("RAR_SOURCE_SPICE task=" + (merchant == null ? "Unknown" : merchant.GetQuestState("merchant_curse").ToString()) +
                    " cure_phrase_unlocked=" + save.unlocked_phrases.Contains("@merchant_on_cure") +
                    " cure_phrase_blacklisted=" + save.black_list_of_phrases.Contains("@merchant_on_cure") +
                    " formulas=" + spiceTotal + " known=" + spiceKnown + " remaining=" + (spiceTotal - spiceKnown));
            }
            catch (Exception ex) { Fail(ex); }
        }
        [HarmonyPatch(typeof(CraftGUI), "OpenAsBuild")]
        private static class NativeBuilderList
        {
            private static void Postfix(CraftGUI __instance, WorldGameObject build_desk)
            {
                if (!ready || build_desk == null || build_desk.obj_id != Builder) return;
                try
                {
                    Capture("native_builder_open");
                    foreach (string paint in Paints)
                    {
                        int cards = 0;
                        foreach (var cell in __instance.list_items) if (cell.craft_definition != null && Uses(cell.craft_definition, paint) != 0) cards++;
                        Write("RAR_SOURCE_NATIVE_BUILD product=" + paint + " cards=" + cards + " builder_reached=True");
                    }
                }
                catch (Exception ex) { Fail(ex); }
            }
        }
        [HarmonyPatch(typeof(MultiAnswerOptionGUI), "Show")]
        private static class NativeDialogueOption
        {
            private static void Postfix(AnswerVisualData answer)
            {
                if (!ready || answer == null || answer.id != "@merchant_on_cure") return;
                Write("RAR_SOURCE_NATIVE_DIALOG phrase=merchant_on_cure rendered=True can_be_picked=" + answer.can_be_picked);
                Capture("native_spice_option_shown");
            }
        }
        [HarmonyPatch(typeof(GUIElements), "OpenCraftGUI")]
        private static class StudySnapshot
        {
            private static void Prefix(WorldGameObject craftery_wgo)
            {
                if (ready && craftery_wgo != null && craftery_wgo.obj_id == "mf_alchemy_survey") Capture("study_table_open");
            }
        }
    }
}
