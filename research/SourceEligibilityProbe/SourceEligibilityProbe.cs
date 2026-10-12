// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using BepInEx;
using BepInEx.Logging;
using UnityEngine;

[assembly: AssemblyVersion("0.1.1.0")]
[assembly: AssemblyFileVersion("0.1.1.0")]

namespace NikichMods.Research
{
    [BepInPlugin("nikichmods.research.sourceeligibility", "Source Eligibility Probe", Version)]
    public sealed class SourceEligibilityProbe : BaseUnityPlugin
    {
        public const string Version = "0.1.1";
        private const string Builder = "keeper_room_builddesk";
        private const string Spice = "taste_booster:alchemy_1_spice";
        private static readonly string[] Paints = { "yellow_paint", "green_paint", "brown_paint", "red_paint", "d_green_paint", "d_violet_paint", "violet_paint" };
        private static ManualLogSource log;
        private static bool ready;
        private static readonly FieldInfo BalanceInstance = typeof(GameBalance).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);
        private void Awake()
        {
            log = Logger;
            try
            {
                string hash;
                using (var sha = SHA256.Create()) using (var stream = File.OpenRead(typeof(GameSave).Assembly.Location))
                    hash = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                if (hash != "e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8") throw new InvalidOperationException("Unverified host: " + hash);
                if (BalanceInstance == null) throw new InvalidOperationException("Missing verified balance instance field");
                ready = true;
                Write("RAR_SOURCE_READY version=" + Version + " read_only=True save_mutation=False patches=0 trigger=F9");
            }
            catch (Exception ex) { Fail(ex); }
        }
        private void OnDestroy() { ready = false; }
        private void Update()
        {
            // No host singleton or save is read until an explicit key press.
            if (!ready || !Input.GetKeyDown(KeyCode.F9)) return;
            Write("RAR_SOURCE_TRIGGER key=F9");
            if (!MainGame.game_started || MainGame.me == null || MainGame.me.save == null || MainGame.me.player == null)
            {
                Write("RAR_SOURCE_SKIPPED reason=world_not_ready"); return;
            }
            Capture("manual_F9");
        }
        private static void Write(string s) { if (log != null) log.LogInfo(s); }
        private static void Fail(Exception ex) { ready = false; if (log != null) log.LogError("RAR_SOURCE_ERROR disabled=True " + ex); }
        private static int Uses(CraftDefinition craft, string item)
        {
            foreach (var need in craft.needs) if (need.id == item) return 1;
            return 0;
        }
        private static void Knowledge(GameBalance balance, string product, out int total, out int known)
        {
            total = 0; known = 0;
            foreach (var c in balance.craft_data)
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
                Write("RAR_SOURCE_CALL phase=before action=read_existing_balance");
                var balance = (GameBalance)BalanceInstance.GetValue(null);
                Write("RAR_SOURCE_CALL phase=after action=read_existing_balance present=" + (balance != null));
                if (balance == null) { Write("RAR_SOURCE_SKIPPED reason=balance_not_loaded"); return; }
                var save = MainGame.me.save;
                bool alchemy = save.unlocked_techs.Contains("The Beginning Of Alchemy");
                Write("RAR_SOURCE_CALL phase=before action=builder_lookup");
                var builder = WorldMap.GetWorldGameObjectByObjId(Builder, true);
                Write("RAR_SOURCE_CALL phase=after action=builder_lookup");
                Write("RAR_SOURCE_SNAPSHOT phase=" + phase + " alchemy=" + alchemy + " builder_present=" + (builder != null) +
                    " builder_removed=" + (builder != null && builder.is_removed) + " builder_disabled=" + (builder != null && builder.GetParamInt("disabled_interactions") != 0) +
                    " tutorial_blocks=" + (MainGame.me.player.GetParamInt("in_tutorial") == 1 && MainGame.me.player.GetParamInt("tut_shown_tut_1") == 0));
                foreach (string paint in Paints)
                {
                    int sources = 0, visible = 0, total, known;
                    Write("RAR_SOURCE_CALL phase=before action=blueprint_visibility product=" + paint);
                    foreach (var c in balance.craft_obj_data)
                        if (c.builder_ids.Contains(Builder) && Uses(c, paint) != 0) { sources++; if (save.IsCraftVisible(c)) visible++; }
                    Write("RAR_SOURCE_CALL phase=after action=blueprint_visibility product=" + paint);
                    Write("RAR_SOURCE_CALL phase=before action=formula_counts product=" + paint);
                    Knowledge(balance, paint, out total, out known);
                    Write("RAR_SOURCE_CALL phase=after action=formula_counts product=" + paint);
                    Write("RAR_SOURCE_PAINT product=" + paint + " sources=" + sources + " native_visible=" + visible + " formulas=" + total + " known=" + known +
                        " remaining=" + (total - known) + " reachability_proven=False");
                }
                Write("RAR_SOURCE_CALL phase=before action=merchant_state");
                var merchant = save.known_npcs.GetNPC("npc_merchant");
                Write("RAR_SOURCE_CALL phase=after action=merchant_state");
                Write("RAR_SOURCE_CALL phase=before action=formula_counts product=spices");
                int spiceTotal, spiceKnown; Knowledge(balance, Spice, out spiceTotal, out spiceKnown);
                Write("RAR_SOURCE_CALL phase=after action=formula_counts product=spices");
                Write("RAR_SOURCE_SPICE task=" + (merchant == null ? "Unknown" : merchant.GetQuestState("merchant_curse").ToString()) +
                    " cure_phrase_unlocked=" + save.unlocked_phrases.Contains("@merchant_on_cure") +
                    " cure_phrase_blacklisted=" + save.black_list_of_phrases.Contains("@merchant_on_cure") +
                    " formulas=" + spiceTotal + " known=" + spiceKnown + " remaining=" + (spiceTotal - spiceKnown));
                Write("RAR_SOURCE_CALL phase=before action=shown_builder_cards");
                ObserveShownBuilder();
                Write("RAR_SOURCE_CALL phase=after action=shown_builder_cards");
                Write("RAR_SOURCE_DONE phase=" + phase);
            }
            catch (Exception ex) { Fail(ex); }
        }
        private static void ObserveShownBuilder()
        {
            var gui = GUIElements.me.craft;
            var desk = gui.GetCrafteryWGO();
            if (!gui.is_shown || desk == null || desk.obj_id != Builder)
            {
                Write("RAR_SOURCE_NATIVE_BUILD skipped=True reason=room_builder_not_shown"); return;
            }
            foreach (string paint in Paints)
            {
                int cards = 0;
                foreach (var cell in gui.list_items) if (cell.craft_definition != null && Uses(cell.craft_definition, paint) != 0) cards++;
                Write("RAR_SOURCE_NATIVE_BUILD product=" + paint + " cards=" + cards + " builder_reached=True");
            }
        }
    }
}
