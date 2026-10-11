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

[assembly: AssemblyVersion("0.1.0.0")]
[assembly: AssemblyFileVersion("0.1.0.0")]

namespace NikichMods.Research
{
    [BepInPlugin("nikichmods.research.studytablework", "Study Table Work Probe", Version)]
    public sealed class StudyTableWorkProbe : BaseUnityPlugin
    {
        public const string Version = "0.1.0";
        internal const string JobId = "rar_probe_work_010";
        private const string TableId = "mf_alchemy_survey";
        private const string Technology = "The Beginning Of Alchemy";
        private const string HostHash = "e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8";
        private static ManualLogSource Log;
        private static bool enabledProbe;
        private static bool normalDispatch;
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
                harmony = new Harmony("nikichmods.research.studytablework");
                harmony.PatchAll(typeof(StudyTableWorkProbe).Assembly);
                enabledProbe = true;
                Write("RAR_WORK_READY version=" + Version + " hash=" + actual + " save_persistent=True formulas=0");
            }
            catch (Exception ex) { Fail(ex); }
        }

        private void OnDestroy()
        {
            enabledProbe = false;
            if (harmony != null) harmony.UnpatchSelf();
        }

        private void Update()
        {
            if (!enabledProbe || !Input.GetKeyDown(KeyCode.F8) || !MainGame.game_started || !BaseGUI.all_guis_closed)
                return;
            try
            {
                DumpActive("RAR_WORK_STATUS");
                GUIElements.me.dialog.Open("Проверка работы на столе 0.1.0", "Убрать тестовое состояние", Cleanup,
                    "Закрыть", null, null, GameKey.Select, GameKey.Back,
                    "Тест может сохраняться в игре. Очистка удаляет только тестовое задание и его отметку; потраченные ресурсы не возвращает. После очистки сохраните игру перед удалением DLL.");
            }
            catch (Exception ex) { Fail(ex); }
        }

        internal static void Write(string message) { if (Log != null) Log.LogInfo(message); }
        internal static void Fail(Exception ex)
        {
            enabledProbe = false;
            if (Log != null) Log.LogError("RAR_WORK_ERROR action=disabled fallback=vanilla " + ex);
        }
        private static bool IsJob(CraftDefinition craft) { return craft != null && craft.id == JobId; }
        private static bool IsTable(WorldGameObject table) { return table != null && table.obj_id == TableId; }

        private static void Cleanup()
        {
            try
            {
                int canceled = 0;
                int cleared = 0;
                foreach (WorldGameObject table in UnityEngine.Object.FindObjectsOfType<WorldGameObject>())
                {
                    if (!IsTable(table)) continue;
                    CraftComponent cc = table.components.craft;
                    if (cc.is_crafting && IsJob(cc.current_craft)) { cc.Cancel(); canceled++; }
                    if ((string)Last.GetValue(cc) == JobId) { Last.SetValue(cc, ""); cleared++; }
                    if ((string)Last2.GetValue(cc) == JobId) { Last2.SetValue(cc, ""); cleared++; }
                }
                int removed = MainGame.me.save.completed_one_time_crafts.RemoveAll(id => id == JobId);
                Write("RAR_WORK_CLEANUP canceled=" + canceled + " last_cleared=" + cleared + " markers_removed=" + removed);
                DumpActive("RAR_WORK_CLEAN");
                GUIElements.me.dialog.OpenOK("Тестовое состояние убрано. Сохраните игру перед удалением пробника.");
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
                            custom_name = "Проверка исследования без образца",
                            icon = "i_pot_heal",
                            craft_type = CraftDefinition.CraftType.None,
                            needs = new List<Item> { new Item("faith", 1) },
                            needs_from_wgo = new List<Item> { new Item("science", 1) },
                            output = new List<Item>(),
                            condition = Constant(1),
                            craft_time = Constant(30),
                            energy = Constant(10),
                            sanity = Constant(0),
                            is_auto = false,
                            one_time_craft = true,
                            disable_multi_craft = true,
                            enqueue_type = CraftDefinition.EnqueueType.NeverEnqueue,
                            hide_quality_icon = true
                        });
                    }
                    registrationSeen = true;
                    Write("RAR_WORK_REGISTER phase=before_id_cache added=" + (existing == null));
                }
                catch (Exception ex) { Fail(ex); }
            }
            private static void Postfix(GameBalanceBase __instance)
            {
                if (!enabledProbe || !(__instance is GameBalance)) return;
                if (__instance.GetDataOrNull<CraftDefinition>(JobId) == null)
                    Fail(new InvalidOperationException("Registered job is not resolvable through native ID cache"));
                else Write("RAR_WORK_REGISTER phase=after_id_cache resolved=True");
            }
        }

        [HarmonyPatch(typeof(GUIElements), "OpenCraftGUI")]
        private static class TableEntry
        {
            private static bool Prefix(GUIElements __instance, WorldGameObject craftery_wgo)
            {
                if (!enabledProbe || normalDispatch || !IsTable(craftery_wgo)) return true;
                try
                {
                    bool unlocked = MainGame.me.save.unlocked_techs.Contains(Technology);
                    Write("RAR_WORK_GATE unlocked=" + unlocked + " registered=" + registrationSeen);
                    if (!unlocked || craftery_wgo.components.craft.is_crafting) return true;
                    if (!registrationSeen || GameBalance.me.GetDataOrNull<CraftDefinition>(JobId) == null)
                        throw new InvalidOperationException("Job registration missed the real load lifecycle");
                    __instance.dialog.Open("Исследовательский стол: проверка 0.1.0", "Обычное изучение", delegate {
                        try { normalDispatch = true; __instance.OpenCraftGUI(craftery_wgo); }
                        catch (Exception ex) { Fail(ex); }
                        finally { normalDispatch = false; }
                    }, "Проверка исследования", delegate {
                        try {
                            Write("RAR_WORK_UI before=native_cards");
                            __instance.craft.OpenCraftList(craftery_wgo);
                            Write("RAR_WORK_UI after=native_cards shown=" + __instance.craft.is_shown);
                        } catch (Exception ex) { Fail(ex); }
                    }, null, GameKey.Select, GameKey.Back,
                        "Это диагностическая работа: 1 вера, 1 наука, без образца и без раскрытия рецептов. Используйте тестовое сохранение. F8 после закрытия меню — очистка.");
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
                if (!enabledProbe || !IsJob(craft)) return;
                try {
                    bool sampleFree = craft.needs.Count == 1 && craft.needs[0].id == "faith" && craft.item_needs.Count == 0;
                    __state = Resources.Capture(__instance.wgo);
                    Write("RAR_WORK_START phase=before sample_free=" + sampleFree + " output_count=" + craft.output.Count + __state.Summary());
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
                if (!enabledProbe || !IsJob(__instance.current_craft)) return;
                try { __state = Resources.Capture(__instance.wgo); Write("RAR_WORK_CANCEL phase=before" + __state.Summary()); }
                catch (Exception ex) { Fail(ex); }
            }
            private static void Postfix(CraftComponent __instance, Resources __state)
            {
                if (__state == null) return;
                try { Write("RAR_WORK_CANCEL phase=after active=" + __instance.is_crafting + " refund_as_drops=True" + Resources.Capture(__instance.wgo).Summary()); }
                catch (Exception ex) { Fail(ex); }
            }
        }

        [HarmonyPatch(typeof(GameSave), "OnFinishedCraft")]
        private static class CompletionObserver
        {
            private static void Postfix(GameSave __instance, CraftDefinition craft)
            {
                if (!enabledProbe || !IsJob(craft)) return;
                Write("RAR_WORK_COMPLETE native_marker=" + __instance.completed_one_time_crafts.Contains(JobId) + " formulas=0");
            }
        }

        [HarmonyPatch(typeof(CraftComponent), "DeserializeCraftComponent")]
        private static class RestoreObserver
        {
            private static void Postfix(CraftComponent __instance, SerializableWGO.SerializableCraft data)
            {
                if (!enabledProbe || data.cur_craft_id != JobId) return;
                Write("RAR_WORK_RESTORE saved_active=" + data.is_crafting + " resolved=" + IsJob(__instance.current_craft) + " active=" + __instance.is_crafting);
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
