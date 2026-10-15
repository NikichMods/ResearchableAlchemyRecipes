// SPDX-License-Identifier: MPL-2.0
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

[assembly: AssemblyVersion("0.1.0.0")]
[assembly: AssemblyFileVersion("0.1.0.0")]

namespace NikichMods.ResearchableAlchemyRecipes
{
    [BepInPlugin(Id, "Researchable Alchemy Recipes", Version)]
    [BepInDependency("p1xel8ted.gyk.maxbuttonsredux", BepInDependency.DependencyFlags.SoftDependency)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string Id = "nikichmods.researchablealchemyrecipes";
        public const string Version = "0.1.0";
        private const string HostHash = "e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8";
        private static ManualLogSource log;
        internal static bool Enabled;
        private static bool registered;
        private static bool failed;
        private Harmony harmony;

        private void Awake()
        {
            log = Logger;
            try
            {
                string actual;
                using (var sha = SHA256.Create())
                using (var stream = File.OpenRead(typeof(GameSave).Assembly.Location))
                    actual = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                // Persistent economy/knowledge paths retain the research baseline's exact-host gate.
                if (actual != HostHash) throw new InvalidOperationException("Unverified host: " + actual);
                TabArtwork.Load();
                harmony = new Harmony(Id);
                harmony.PatchAll(typeof(Plugin).Assembly);
                QuantityInputCompatibility.Install(harmony, StudyTabs.QuantityInputUnavailable, RecipeGuidance.QuantityUnavailableReason);
                Enabled = true;
                AvailabilityNotifications.Install();
                log.LogInfo("RAR_ACTIVATED version=" + Version + " host_sha256=" + actual + " notices=" + AvailabilityNotifications.Installed);
            }
            catch (Exception ex) { Fail(ex); }
        }

        private void OnDestroy()
        {
            Enabled = false;
            AvailabilityNotifications.Dispose();
            RecipeGuidance.Restore();
            StudyTabs.Restore();
            if (harmony != null) harmony.UnpatchSelf();
            TabArtwork.Dispose();
        }

        private void Update()
        {
            AvailabilityNotifications.Tick();
            if (!MainGame.game_started) return;
            try { RecipeGuidance.Refresh(); }
            catch (Exception ex) { RecipeGuidance.Restore(); Fail(ex); }
        }
        private void LateUpdate() { AvailabilityNotifications.ObserveShown(); }

        internal static void Write(string message)
        {
            if (log == null) return;
            if (message.Contains("ERROR ") || message.StartsWith("RAR_LOCALIZATION_INJECT error=", StringComparison.Ordinal)) log.LogError(message);
            else log.LogDebug(message);
        }
        internal static void Fail(Exception ex)
        {
            Enabled = false;
            if (failed) return;
            failed = true;
            if (log != null) log.LogError("RAR_DISABLED action=entry_disabled settlement=" + (registered ? "retained" : "unavailable") + " " + ex);
            // Keep core settlement hooks. Each owned view gets its own cleanup attempt.
            try { StudyTabs.Disable(); }
            catch (Exception cleanup) { CleanupError("tabs", cleanup); }
            try { RecipeGuidance.Restore(); }
            catch (Exception cleanup) { CleanupError("guidance", cleanup); }
            try { AvailabilityNotifications.Dispose(); }
            catch (Exception cleanup) { CleanupError("notices", cleanup); }
        }
        private static void CleanupError(string feature, Exception ex)
        {
            if (log != null) log.LogError("RAR_CLEANUP_FAILED feature=" + feature + " action=entry_remains_disabled " + ex);
        }
        internal static bool IsJob(CraftDefinition craft) { return craft != null && IsJobId(craft.id); }
        internal static bool IsJobId(string id) { return SourceResearch.IsJob(id); }

        [HarmonyPatch(typeof(GameBalanceBase), "CreateIDsCache")]
        private static class Registration
        {
            private static void Prefix(GameBalanceBase __instance)
            {
                if (!(__instance is GameBalance)) return;
                registered = false; // A rebuilt native cache needs its own validation.
                if (!Enabled) return;
                try
                {
                    SourceResearch.Register((GameBalance)__instance);
                    RecipeGuidance.Register((GameBalance)__instance);
                }
                catch (Exception ex) { Fail(ex); }
            }
            private static void Postfix(GameBalanceBase __instance)
            {
                if (!Enabled || !(__instance is GameBalance)) return;
                try { SourceResearch.VerifyCache((GameBalance)__instance); registered = true; }
                catch (Exception ex) { Fail(ex); }
            }
        }
        [HarmonyPatch(typeof(GUIElements), "OpenCraftGUI")]
        private static class TableEntry
        {
            private static bool Prefix(WorldGameObject craftery_wgo)
            {
                if (!Enabled || craftery_wgo == null || craftery_wgo.obj_id != "mf_alchemy_survey") return true;
                try
                {
                    if (!MainGame.me.save.unlocked_techs.Contains(SourceResearch.Technology) || craftery_wgo.components.craft.is_crafting) return true;
                    if (!registered) throw new InvalidOperationException("Research registry missed native load lifecycle");
                    SourceResearch.Refresh(MainGame.me.save);
                    StudyTabs.Open(craftery_wgo);
                    return false;
                }
                catch (Exception ex) { Fail(ex); return true; }
            }
        }
        [HarmonyPatch(typeof(GameSave), "OnFinishedCraft")]
        private static class Completion
        {
            private static void Postfix(GameSave __instance, CraftDefinition craft)
            {
                // Presentation disable must not discard an already-paid, verified native job.
                if (!registered || !IsJob(craft)) return;
                try { SourceResearch.Complete(craft.id, __instance); }
                catch (Exception ex) { Fail(ex); }
            }
        }
    }
}
