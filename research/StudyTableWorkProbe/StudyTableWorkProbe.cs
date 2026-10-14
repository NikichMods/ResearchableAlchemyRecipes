// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

[assembly: AssemblyVersion("0.1.45.0")]
[assembly: AssemblyFileVersion("0.1.45.0")]

namespace NikichMods.Research
{
    [BepInPlugin("nikichmods.research.studytablework", "Study Table Work Probe", Version)]
    [BepInDependency("p1xel8ted.gyk.maxbuttonsredux", BepInDependency.DependencyFlags.SoftDependency)]
    public sealed class StudyTableWorkProbe : BaseUnityPlugin
    {
        public const string Version = "0.1.45";
        internal const string JobId = "rar_probe_work_010";
        private const string TableId = "mf_alchemy_survey";
        private const string Technology = "The Beginning Of Alchemy";
        private const string HostHash = "e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8";
        private static ManualLogSource Log;
        internal static bool enabledProbe;
        private static bool registrationSeen;
        private static readonly FieldInfo Last = AccessTools.Field(typeof(CraftComponent), "_last_craft_id");
        private static readonly FieldInfo Last2 = AccessTools.Field(typeof(CraftComponent), "_last_craft_id_2");
        private Harmony harmony;

        private void Awake()
        {
            Log = Logger;
            try
            {
                string actual;
                using (var sha = SHA256.Create())
                using (var stream = File.OpenRead(typeof(GameSave).Assembly.Location))
                    actual = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                if (actual != HostHash || Last == null || Last2 == null)
                    throw new InvalidOperationException("Unverified host or missing cleanup fields: " + actual);
                TabArtwork.Load();
                harmony = new Harmony("nikichmods.research.studytablework");
                harmony.PatchAll(typeof(StudyTableWorkProbe).Assembly);
                Localization.Inject();
                QuantityInputCompatibility.Install(harmony, StudyTabs.QuantityInputUnavailable, RecipeGuidance.QuantityUnavailableReason);
                enabledProbe = true;
                AvailabilityNotifications.Install();
                Write("RAR_WORK_READY version=" + Version + " hash=" + actual + " save_persistent=True reward_probe=True notice_hooks=" + AvailabilityNotifications.Installed);
            }
            catch (Exception ex) { Fail(ex); }
        }

        private void OnDestroy()
        {
            enabledProbe = false;
            AvailabilityNotifications.Dispose();
            RecipeGuidance.Restore();
            StudyTabs.Restore();
            if (harmony != null) harmony.UnpatchSelf();
            TabArtwork.Dispose();
        }

        private void Update()
        {
            AvailabilityNotifications.Tick();
            if (enabledProbe && MainGame.game_started && BaseGUI.all_guis_closed && Input.GetKeyDown(KeyCode.F6))
            {
                ResearchCompletion.Show("black_paint", 2, null, true); // Explicit session-only UI example; no grant.
                return;
            }
            if (enabledProbe && MainGame.game_started && BaseGUI.all_guis_closed && Input.GetKeyDown(KeyCode.F7))
            {
                AvailabilityNotifications.PreviewMenu();
                return;
            }
            if (registrationSeen && MainGame.game_started && BaseGUI.all_guis_closed && Input.GetKeyDown(KeyCode.F9))
            {
                try
                {
                    CraftComponent target = null; int count = 0;
                    foreach (var table in UnityEngine.Object.FindObjectsOfType<WorldGameObject>())
                    {
                        if (!IsTable(table) || table.is_removed) continue;
                        var cc = table.components.craft;
                        if (!cc.is_crafting || !ObserveStudy(cc, cc.current_craft)) continue;
                        target = cc; count++;
                    }
                    Write("RAR_STUDY_CANCEL_REQUEST diagnostic_key=F9 active_tables=" + count);
                    if (count == 1) target.Cancel(); // Actual native cancellation; no simulated progress or refund.
                }
                catch (Exception ex) { Fail(ex); }
                return;
            }
            if (MainGame.game_started)
            {
                try { RecipeGuidance.Refresh(); }
                catch (Exception ex) { RecipeGuidance.Restore(); Fail(ex); }
            }
            if (!registrationSeen || !Input.GetKeyDown(KeyCode.F8) || !MainGame.game_started || !BaseGUI.all_guis_closed)
                return;
            try
            {
                DumpActive("RAR_WORK_STATUS");
                if (enabledProbe) SourceResearch.Menu();
                else GUIElements.me.dialog.Open(Localization.Get(Localization.Key.Disabled), Localization.Get(Localization.Key.Cleanup), Cleanup,
                    Localization.Get(Localization.Key.Close), null, null, GameKey.Select, GameKey.Back, Localization.Get(Localization.Key.CleanupAvailable));
            }
            catch (Exception ex) { Fail(ex); }
        }

        private void LateUpdate()
        {
            AvailabilityNotifications.ObserveShown();
        }

        internal static void Write(string message)
        {
            if (Log == null) return;
            // Detailed source/card censuses remain opt-in through the host's Debug log level.
            if (message.StartsWith("RAR_TOPICS_PRODUCT ", StringComparison.Ordinal) ||
                message.StartsWith("RAR_TOPICS_CARD ", StringComparison.Ordinal) ||
                message.StartsWith("RAR_TOPICS_NATIVE_LIST ", StringComparison.Ordinal)) Log.LogDebug(message);
            else if (message.StartsWith("RAR_NOTICE_ERROR ", StringComparison.Ordinal)) Log.LogError(message);
            else Log.LogInfo(message);
        }
        internal static void Fail(Exception ex)
        {
            enabledProbe = false;
            StudyTabs.Restore();
            if (Log != null) Log.LogError("RAR_WORK_ERROR action=disabled fallback=vanilla " + ex);
        }
        internal static bool IsJob(CraftDefinition craft) { return craft != null && IsJobId(craft.id); }
        internal static bool IsJobId(string id) { return id == JobId || id == FormulaKnowledge.Job || SourceResearch.IsJob(id); }
        private static bool IsTable(WorldGameObject table) { return table != null && table.obj_id == TableId; }
        private static bool ObserveStudy(CraftComponent component, CraftDefinition craft)
        {
            return craft != null && IsTable(component.wgo) && (IsJob(craft) || craft.craft_type == CraftDefinition.CraftType.Survey);
        }

        internal static void Cleanup()
        {
            try
            {
                int canceled = 0;
                int cleared = 0;
                int queued = 0;
                foreach (WorldGameObject table in UnityEngine.Object.FindObjectsOfType<WorldGameObject>())
                {
                    if (!IsTable(table)) continue;
                    CraftComponent cc = table.components.craft;
                    queued += ResearchRepetitionGuard.RemoveQueued(cc.craft_queue);
                    if (cc.is_crafting && IsJob(cc.current_craft)) { cc.Cancel(); canceled++; }
                    if (IsJobId((string)Last.GetValue(cc))) { Last.SetValue(cc, ""); cleared++; }
                    if (IsJobId((string)Last2.GetValue(cc))) { Last2.SetValue(cc, ""); cleared++; }
                }
                int removed = MainGame.me.save.completed_one_time_crafts.RemoveAll(id => id == JobId);
                FormulaKnowledge.Restore(MainGame.me.save);
                int sourceMarkers = SourceResearch.Cleanup(MainGame.me.save);
                Write("RAR_TOPICS_CLEANUP job_markers_removed=" + sourceMarkers + " earned_native_knowledge_preserved=True");
                Write("RAR_WORK_CLEANUP canceled=" + canceled + " last_cleared=" + cleared + " markers_removed=" + removed + " queued_entries_removed=" + queued);
                DumpActive("RAR_WORK_CLEAN");
                GUIElements.me.dialog.OpenOK(Localization.Get(Localization.Key.CleanupDone));
            }
            catch (Exception ex) { Fail(ex); }
        }

        private static void DumpActive(string marker)
        {
            int active = 0;
            foreach (WorldGameObject table in UnityEngine.Object.FindObjectsOfType<WorldGameObject>())
            {
                if (!IsTable(table) || !table.components.craft.is_crafting || !IsJob(table.components.craft.current_craft)) continue;
                active++;
                Write(marker + " table=" + table.unique_id + " resolved=True progress=" + Number(table.progress));
            }
            Write(marker + " active=" + active + " completed=" + MainGame.me.save.completed_one_time_crafts.Contains(JobId));
        }

        private static string Number(float value) { return value.ToString("0.###", CultureInfo.InvariantCulture); }
        private static SmartExpression Constant(float value) { return new SmartExpression { default_value = value }; }

        private sealed class Resources
        {
            internal int faith;
            internal int science;
            internal float energy;
            internal static Resources Capture(WorldGameObject table)
            {
                return new Resources {
                    faith = MainGame.me.player.GetMultiInventoryForInteraction(null).GetTotalCount("faith", MultiInventory.DestinationType.AllFromFirst, true),
                    science = table.data.GetItemsCount("science", true),
                    energy = MainGame.me.player.energy
                };
            }
            internal string Summary() { return " faith=" + faith + " science=" + science + " energy=" + Number(energy); }
        }

        [HarmonyPatch(typeof(GameBalanceBase), "CreateIDsCache")]
        private static class Registration
        {
            private static void Prefix(GameBalanceBase __instance)
            {
                if (!enabledProbe || !(__instance is GameBalance)) return;
                try
                {
                    var balance = (GameBalance)__instance;
                    CraftDefinition existing = balance.craft_data.Find(c => c.id == JobId);
                    if (existing == null)
                    {
                        balance.craft_data.Add(new CraftDefinition {
                            id = JobId,
                            craft_in = new List<string> { TableId },
                            custom_name = Localization.Id(Localization.Key.SampleFreeProbeName),
                            icon = "i_pot_heal",
                            craft_type = CraftDefinition.CraftType.None,
                            needs = new List<Item> { new Item("faith", 1) },
                            needs_from_wgo = new List<Item> { new Item("science", 1) },
                            output = new List<Item>(),
                            condition = Constant(1),
                            craft_time = Constant(2),
                            energy = Constant(10),
                            sanity = Constant(0),
                            is_auto = false,
                            one_time_craft = true,
                            disable_multi_craft = true,
                            enqueue_type = CraftDefinition.EnqueueType.NeverEnqueue,
                            hide_quality_icon = true,
                            tab_id = StudyTabs.ResearchTab
                        });
                    }
                    FormulaKnowledge.Register(balance);
                    SourceResearch.Register(balance);
                    RecipeGuidance.Register(balance);
                    registrationSeen = true;
                    Write("RAR_WORK_REGISTER phase=before_id_cache added=" + (existing == null));
                }
                catch (Exception ex) { Fail(ex); }
            }
            private static void Postfix(GameBalanceBase __instance)
            {
                if (!enabledProbe || !(__instance is GameBalance)) return;
                if (__instance.GetDataOrNull<CraftDefinition>(JobId) == null || __instance.GetDataOrNull<CraftDefinition>(FormulaKnowledge.Job) == null)
                    Fail(new InvalidOperationException("Registered job is not resolvable through native ID cache"));
                else
                {
                    try { SourceResearch.VerifyCache((GameBalance)__instance); Write("RAR_WORK_REGISTER phase=after_id_cache resolved=True"); }
                    catch (Exception ex) { Fail(ex); }
                }
            }
        }

        [HarmonyPatch(typeof(GUIElements), "OpenCraftGUI")]
        private static class TableEntry
        {
            private static bool Prefix(GUIElements __instance, WorldGameObject craftery_wgo)
            {
                if (!enabledProbe || !IsTable(craftery_wgo)) return true;
                try
                {
                    bool unlocked = MainGame.me.save.unlocked_techs.Contains(Technology);
                    Write("RAR_WORK_GATE unlocked=" + unlocked + " registered=" + registrationSeen);
                    if (!unlocked || craftery_wgo.components.craft.is_crafting) return true;
                    if (!registrationSeen || GameBalance.me.GetDataOrNull<CraftDefinition>(JobId) == null)
                        throw new InvalidOperationException("Job registration missed the real load lifecycle");
                    SourceResearch.Refresh(MainGame.me.save);
                    StudyTabs.Open(craftery_wgo);
                    return false;
                }
                catch (Exception ex) { Fail(ex); return true; }
            }
        }

        [HarmonyPatch(typeof(CraftComponent), "CraftReally")]
        private static class StartObserver
        {
            private static void Prefix(CraftComponent __instance, CraftDefinition craft, out Resources __state)
            {
                __state = null;
                if (!enabledProbe || !ObserveStudy(__instance, craft)) return;
                try {
                    bool sampleFree = craft.needs.Count == 0 && craft.item_needs.Count == 0;
                    __state = Resources.Capture(__instance.wgo);
                    Write("RAR_WORK_START phase=before job=" + craft.id + " sample_free=" + sampleFree + " output_count=" + craft.output.Count + __state.Summary());
                }
                catch (Exception ex) { Fail(ex); }
            }
            private static void Postfix(CraftComponent __instance, bool __result, Resources __state)
            {
                if (__state == null) return;
                try { Write("RAR_WORK_START phase=after result=" + __result + " active=" + __instance.is_crafting + Resources.Capture(__instance.wgo).Summary()); }
                catch (Exception ex) { Fail(ex); }
            }
        }

        [HarmonyPatch(typeof(CraftComponent), "Cancel")]
        private static class CancelObserver
        {
            private static void Prefix(CraftComponent __instance, out Resources __state)
            {
                __state = null;
                if (!enabledProbe || !ObserveStudy(__instance, __instance.current_craft)) return;
                try { __state = Resources.Capture(__instance.wgo); Write("RAR_WORK_CANCEL phase=before job=" + __instance.current_craft.id + " reserved=" + ResearchScienceReserve.Read(__instance.wgo.data) + __state.Summary()); }
                catch (Exception ex) { Fail(ex); }
            }
            private static void Postfix(CraftComponent __instance, Resources __state)
            {
                if (__state == null) return;
                try { Write("RAR_WORK_CANCEL phase=after active=" + __instance.is_crafting + " refund_as_drops=True" + Resources.Capture(__instance.wgo).Summary()); }
                catch (Exception ex) { Fail(ex); }
            }
        }

        [HarmonyPatch(typeof(WorldGameObject), "DestroyMe")]
        private static class StudyRemovalObserver
        {
            private static void Prefix(WorldGameObject __instance)
            {
                if (!enabledProbe || !IsTable(__instance) || __instance.is_removed) return;
                try
                {
                    var cc = __instance.components.craft;
                    Write("RAR_STUDY_REMOVE observation_only=True active=" + cc.is_crafting + " job=" + (cc.current_craft == null ? "none" : cc.current_craft.id)
                        + " reserved=" + ResearchScienceReserve.Read(__instance.data) + " drop_inventory=" + __instance.obj_def.drop_inventory_after_remove + Resources.Capture(__instance).Summary());
                }
                catch (Exception ex) { Write("RAR_STUDY_REMOVE_OBSERVER_ERROR " + ex); }
            }
        }

        [HarmonyPatch(typeof(GameSave), "OnFinishedCraft")]
        private static class CompletionObserver
        {
            private static void Postfix(GameSave __instance, CraftDefinition craft)
            {
                if (!enabledProbe || !IsJob(craft)) return;
                try
                {
                    if (SourceResearch.IsJob(craft.id)) SourceResearch.Complete(craft.id, __instance);
                    else if (craft.id == FormulaKnowledge.Job) FormulaKnowledge.Complete(__instance);
                    else Write("RAR_WORK_COMPLETE native_marker=" + __instance.completed_one_time_crafts.Contains(JobId) + " formulas=0");
                }
                catch (Exception ex) { Fail(ex); }
            }
        }

        [HarmonyPatch(typeof(CraftComponent), "DeserializeCraftComponent")]
        private static class RestoreObserver
        {
            private static void Postfix(CraftComponent __instance, SerializableWGO.SerializableCraft data)
            {
                if (!enabledProbe || !ObserveStudy(__instance, __instance.current_craft)) return;
                Write("RAR_WORK_RESTORE job=" + data.cur_craft_id + " saved_active=" + data.is_crafting + " resolved=" + (__instance.current_craft != null)
                    + " active=" + __instance.is_crafting + " progress=" + Number(__instance.wgo.progress) + " reserved=" + ResearchScienceReserve.Read(__instance.wgo.data) + Resources.Capture(__instance.wgo).Summary());
            }
        }

        [HarmonyPatch(typeof(MainGame), "OnGameLoaded")]
        private static class LoadedObserver
        {
            private static void Postfix()
            {
                if (!enabledProbe) return;
                try { DumpActive("RAR_WORK_LOADED"); }
                catch (Exception ex) { Fail(ex); }
            }
        }
    }
}
