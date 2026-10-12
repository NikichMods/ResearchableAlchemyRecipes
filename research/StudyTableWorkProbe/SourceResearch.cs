// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using HarmonyLib;

namespace NikichMods.Research
{
    // Bounded room-paint research, not a global product catalogue or encounter ledger.
    internal static class SourceResearch
    {
        internal const string Builder = "keeper_room_builddesk";
        internal const string Technology = "The Beginning Of Alchemy";
        private const string JobPrefix = "rar_probe_source_paint_0110_";
        private static readonly string[] Products = { "yellow_paint", "green_paint", "brown_paint", "red_paint", "d_green_paint", "d_violet_paint", "violet_paint" };
        private sealed class Topic
        {
            internal string product;
            internal CraftDefinition job;
            internal readonly List<CraftDefinition> formulas = new List<CraftDefinition>();
            internal readonly List<ObjectCraftDefinition> sources = new List<ObjectCraftDefinition>();
            internal int visible;
        }
        private static readonly Dictionary<string, Topic> topics = new Dictionary<string, Topic>();
        private static GameSave snapshotSave;
        private static GameBalance registeredBalance;
        private static bool sourceAvailable;

        internal static bool IsJob(string id) { return id != null && topics.ContainsKey(id); }
        internal static bool Eligible(bool alchemy, bool source, int visible, int remaining)
        {
            return alchemy && source && visible > 0 && remaining > 0;
        }
        internal static void Register(GameBalance balance)
        {
            if (ReferenceEquals(registeredBalance, balance)) return;
            topics.Clear(); snapshotSave = null; sourceAvailable = false;
            // Resolve original definitions before appending any work to the host list.
            foreach (string product in Products)
            {
                var topic = new Topic { product = product };
                foreach (var craft in balance.craft_data)
                {
                    var output = craft.GetFirstRealOutput();
                    if (craft.craft_type == CraftDefinition.CraftType.MixedCraft && output != null && output.id == product)
                        topic.formulas.Add(craft);
                }
                foreach (var craft in balance.craft_obj_data)
                    if (craft.builder_ids.Contains(Builder) && craft.needs.Exists(item => item.id == product)) topic.sources.Add(craft);
                // Accepted slice: exactly one ordinary formula per paint, all room sources.
                if (topic.formulas.Count != 1 || topic.sources.Count == 0)
                    throw new InvalidOperationException("Verified paint group changed: " + product);
                topic.job = new CraftDefinition {
                    id = JobPrefix + product, craft_in = new List<string> { "mf_alchemy_survey" },
                    custom_name = product, icon = "i_" + product,
                    craft_type = CraftDefinition.CraftType.None,
                    needs = new List<Item> { new Item("faith", 1) }, needs_from_wgo = new List<Item> { new Item("science", 1) },
                    output = new List<Item>(), condition = new SmartExpression { default_value = 1 },
                    craft_time = new SmartExpression { default_value = 2 }, energy = new SmartExpression { default_value = 10 }, sanity = new SmartExpression(),
                    one_time_craft = true, disable_multi_craft = true, enqueue_type = CraftDefinition.EnqueueType.NeverEnqueue,
                    hide_quality_icon = true, tab_id = StudyTabs.ResearchTab
                };
                topics.Add(topic.job.id, topic);
            }
            foreach (var topic in topics.Values)
            {
                var existing = balance.craft_data.Find(c => c.id == topic.job.id);
                if (existing != null) throw new InvalidOperationException("Source research job ID collision");
                balance.craft_data.Add(topic.job);
            }
            registeredBalance = balance;
            StudyTableWorkProbe.Write("RAR_TOPICS_REGISTER products=" + topics.Count + " source=room_paints synthetic=False");
        }
        internal static void VerifyCache(GameBalance balance)
        {
            foreach (var topic in topics.Values)
                if (!ReferenceEquals(balance.GetDataOrNull<CraftDefinition>(topic.job.id), topic.job))
                    throw new InvalidOperationException("Source topic missing from native cache");
        }
        private static int Remaining(Topic topic, GameSave save)
        {
            int n = 0;
            foreach (var formula in topic.formulas) if (!save.completed_one_time_crafts.Contains(formula.id)) n++;
            return n;
        }
        internal static void Refresh(GameSave save)
        {
            snapshotSave = null; sourceAvailable = false;
            StudyTableWorkProbe.Write("RAR_TOPICS_CALL phase=before action=room_source_snapshot");
            // Lookup scans the existing WorldMap list; no singleton load, spawn or scene activation.
            var desk = WorldMap.GetWorldGameObjectByObjId(Builder, true);
            bool tutorial = MainGame.me.player.GetParamInt("in_tutorial") == 1 && MainGame.me.player.GetParamInt("tut_shown_tut_1") == 0;
            sourceAvailable = desk != null && !desk.is_removed && !desk.IsDisabled() && desk.GetParamInt("disabled_interactions") == 0 && !tutorial;
            foreach (var topic in topics.Values)
            {
                topic.visible = 0;
                foreach (var craft in topic.sources) if (save.IsCraftVisible(craft)) topic.visible++;
            }
            snapshotSave = save;
            StudyTableWorkProbe.Write("RAR_TOPICS_CALL phase=after action=room_source_snapshot builder_present=" + (desk != null) + " source_available=" + sourceAvailable + " tutorial_blocks=" + tutorial);
            Status("table_open", save);
        }
        internal static bool Available(string id, GameSave save)
        {
            Topic topic;
            return topics.TryGetValue(id, out topic) && ReferenceEquals(snapshotSave, save) &&
                Eligible(save.unlocked_techs.Contains(Technology), sourceAvailable, topic.visible, Remaining(topic, save));
        }
        internal static bool ReadyToStart(string id, GameSave save)
        {
            try
            {
                Refresh(save); // Recheck native inputs at the payment boundary, not only at menu creation.
                return Available(id, save);
            }
            catch (Exception ex) { StudyTableWorkProbe.Fail(ex); return false; }
        }
        internal static void Status(string phase, GameSave save)
        {
            int offered = 0;
            foreach (var topic in topics.Values)
            {
                bool eligible = Available(topic.job.id, save) && !save.completed_one_time_crafts.Contains(topic.job.id);
                if (eligible) offered++;
                StudyTableWorkProbe.Write("RAR_TOPICS_PRODUCT phase=" + phase + " product=" + topic.product + " native_visible=" + topic.visible +
                    " remaining=" + Remaining(topic, save) + " offered=" + eligible);
            }
            StudyTableWorkProbe.Write("RAR_TOPICS_STATUS phase=" + phase + " source_snapshot_current=" + ReferenceEquals(snapshotSave, save) + " offered=" + offered + " synthetic=False");
        }
        internal static List<string> GrantRemaining(GameSave save, IEnumerable<CraftDefinition> formulas)
        {
            var learned = new List<string>();
            foreach (var formula in formulas)
                if (!save.completed_one_time_crafts.Contains(formula.id)) { save.completed_one_time_crafts.Add(formula.id); learned.Add(formula.id); }
            return learned;
        }
        internal static void Complete(string id, GameSave save)
        {
            var topic = topics[id];
            var learned = GrantRemaining(save, topic.formulas);
            StudyTableWorkProbe.Write("RAR_TOPICS_REWARD product=" + topic.product + " learned=" + learned.Count + " remaining=" + Remaining(topic, save));
            if (learned.Count != 0)
            {
                // Same accepted native disclosure path as 0.1.7; no manufactured completion for formulas.
                var tech = new TechDefinition { id = topic.product, price = new GameRes() };
                tech.crafts.AddRange(learned);
                GUIElements.me.tech_dialog.Open(tech, delegate { Status("reward_dialog_closed", save); }, true, false, false, true);
            }
        }
        internal static int Cleanup(GameSave save)
        {
            // Earned vanilla knowledge remains. Only research-job metadata is removed.
            int removed = save.completed_one_time_crafts.RemoveAll(IsJob);
            snapshotSave = null; sourceAvailable = false;
            return removed;
        }
        internal static void Menu()
        {
            Status("F8", MainGame.me.save);
            GUIElements.me.dialog.Open("Исследования красок: тестовая сборка", "Убрать тестовые работы", StudyTableWorkProbe.Cleanup,
                "Закрыть", null, null, GameKey.Select, GameKey.Back,
                "Темы появляются автоматически по доступным чертежам комнаты. Полученные формулы сохраняются как обычные знания; очистка их не отменяет. Для проверки используйте тестовое сохранение.");
        }
        [HarmonyPatch(typeof(GameSave), "IsCraftVisible")]
        private static class Visibility
        {
            private static void Postfix(GameSave __instance, CraftDefinition craft, ref bool __result)
            {
                if (IsJob(craft.id)) __result = __result && StudyTableWorkProbe.enabledProbe && Available(craft.id, __instance);
            }
        }
        [HarmonyPatch(typeof(CraftGUI), "OpenAsAlchemy")]
        private static class NativeCards
        {
            private static void Postfix(CraftGUI __instance)
            {
                if (!StudyTableWorkProbe.enabledProbe) return;
                foreach (var topic in topics.Values)
                {
                    int cards = 0;
                    foreach (var cell in __instance.list_items)
                        if (cell.craft_definition != null && topic.formulas.Exists(c => c.id == cell.craft_definition.id)) cards++;
                    StudyTableWorkProbe.Write("RAR_TOPICS_NATIVE_LIST product=" + topic.product + " station=" + __instance.GetCrafteryWGO().obj_id + " cards=" + cards + " remaining=" + Remaining(topic, MainGame.me.save));
                }
            }
        }
        [HarmonyPatch(typeof(CraftGUI), "OpenCraftList")]
        private static class OfferedCards
        {
            private static void Postfix(CraftGUI __instance)
            {
                if (!StudyTableWorkProbe.enabledProbe) return;
                var table = __instance.GetCrafteryWGO();
                if (table == null || table.obj_id != "mf_alchemy_survey") return;
                foreach (var topic in topics.Values)
                {
                    int cards = 0;
                    foreach (var cell in __instance.list_items)
                        if (cell.craft_definition != null && cell.craft_definition.id == topic.job.id) cards++;
                    StudyTableWorkProbe.Write("RAR_TOPICS_CARD product=" + topic.product + " cards=" + cards + " eligible=" + Available(topic.job.id, MainGame.me.save));
                }
            }
        }
    }
}
