// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using HarmonyLib;

namespace NikichMods.Research
{
    // One bounded diagnostic target. Native formula IDs are recovered from balance, never dumped.
    internal static class FormulaKnowledge
    {
        internal const string Job = "rar_probe_knowledge_heal_017";
        private const string Product = "pot_heal";
        private const string Prepared = "rar_probe_knowledge_prepared_017";
        private const string Prefix = "rar_probe_knowledge_original_017_";
        private static readonly List<CraftDefinition> formulas = new List<CraftDefinition>();
        private static readonly List<string> originals = new List<string>();
        private static GameSave Save { get { return MainGame.me.save; } }
        private static void Add(GameSave save, string id) { if (!save.completed_one_time_crafts.Contains(id)) save.completed_one_time_crafts.Add(id); }
        private static bool Has(GameSave save, string id) { return save.completed_one_time_crafts.Contains(id); }
        private static CraftDefinition Marker(string id)
        {
            return new CraftDefinition { id = id, hidden = true, craft_in = new List<string>(), output = new List<Item>() };
        }

        internal static void Register(GameBalance balance)
        {
            formulas.Clear(); originals.Clear();
            foreach (CraftDefinition c in balance.craft_data)
                if (c.craft_type == CraftDefinition.CraftType.MixedCraft && c.GetFirstRealOutput() != null && c.GetFirstRealOutput().id == Product)
                    formulas.Add(c);
            formulas.Sort((a, b) => string.CompareOrdinal(a.id, b.id));
            if (formulas.Count != 2) throw new InvalidOperationException("Representative native formula group changed; count=" + formulas.Count);
            foreach (CraftDefinition c in formulas)
            {
                string marker;
                using (var sha = SHA256.Create()) marker = Prefix + BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(c.id))).Replace("-", "").ToLowerInvariant();
                originals.Add(marker);
                if (balance.craft_data.Find(x => x.id == marker) == null) balance.craft_data.Add(Marker(marker));
            }
            if (balance.craft_data.Find(c => c.id == Prepared) == null) balance.craft_data.Add(Marker(Prepared));
            if (balance.craft_data.Find(c => c.id == Job) == null)
            {
                balance.craft_data.Add(new CraftDefinition {
                    id = Job, craft_in = new List<string> { "mf_alchemy_survey" },
                    custom_name = "Проверка открытия формул: эликсир исцеления", icon = "i_" + Product,
                    craft_type = CraftDefinition.CraftType.None,
                    needs = new List<Item> { new Item("faith", 1) }, needs_from_wgo = new List<Item> { new Item("science", 1) },
                    output = new List<Item>(), condition = new SmartExpression { default_value = 1 },
                    craft_time = new SmartExpression { default_value = 2 }, energy = new SmartExpression { default_value = 10 }, sanity = new SmartExpression(),
                    one_time_craft = true, disable_multi_craft = true, enqueue_type = CraftDefinition.EnqueueType.NeverEnqueue,
                    hide_quality_icon = true, tab_id = StudyTabs.ResearchTab
                });
            }
            StudyTableWorkProbe.Write("RAR_KNOWLEDGE_REGISTER formulas=" + formulas.Count + " metadata=3");
        }

        private static int Known(GameSave save)
        {
            int count = 0; foreach (var c in formulas) if (Has(save, c.id)) count++; return count;
        }
        internal static void Status(string phase, GameSave save)
        {
            if (formulas.Count != 2) return;
            StudyTableWorkProbe.Write("RAR_KNOWLEDGE_STATUS phase=" + phase + " prepared=" + Has(save, Prepared) + " known=" + Known(save) + " total=" + formulas.Count + " job_completed=" + Has(save, Job));
        }
        internal static void Menu()
        {
            Status("F8", Save);
            GUIElements.me.dialog.Open("Проверка открытия формул", "Действия", Actions, "Закрыть", null, null, GameKey.Select, GameKey.Back,
                "Только тестовое сохранение. Проверяется эликсир исцеления (2 формулы). Не открывайте и не готовьте его обычным способом во время проверки. Очистка вернёт исходные знания; ресурсы не возвращаются.");
        }
        private static void Actions()
        {
            GUIElements.me.dialog.Open("Тест сохраняется вместе с игрой", "Подготовить случай", Choose, "Убрать тестовое состояние", StudyTableWorkProbe.Cleanup,
                null, GameKey.Select, GameKey.Back, "После очистки сохраните игру перед удалением DLL.");
        }
        private static void Choose()
        {
            GUIElements.me.dialog.Open("Начальные знания для проверки", "Неизвестны обе", delegate { Prepare(false); }, "Известна одна", delegate { Prepare(true); },
                null, GameKey.Select, GameKey.Back, "Это подготовка входных данных, а не результат исследования. Исходные знания будут сохранены для очистки.");
        }
        private static void Prepare(bool partial)
        {
            try
            {
                if (Has(Save, Prepared) || Has(Save, Job))
                {
                    GUIElements.me.dialog.OpenOK("Сначала уберите предыдущее тестовое состояние через F8."); return;
                }
                PrepareInput(Save, partial);
                Status(partial ? "synthetic_partial" : "synthetic_unknown", Save);
                GUIElements.me.dialog.OpenOK("Подготовлено. Запустите работу на вкладке рецептов исследовательского стола.");
            }
            catch (Exception ex) { StudyTableWorkProbe.Fail(ex); }
        }
        internal static void PrepareInput(GameSave save, bool partial)
        {
            if (Has(save, Prepared) || Has(save, Job)) throw new InvalidOperationException("Cleanup required before preparation");
            for (int i = 0; i < formulas.Count; i++) if (Has(save, formulas[i].id)) Add(save, originals[i]);
            // Persist the restoration snapshot before removing any native knowledge.
            Add(save, Prepared);
            foreach (var c in formulas) save.completed_one_time_crafts.RemoveAll(id => id == c.id);
            if (partial) Add(save, formulas[0].id);
        }
        internal static void Restore(GameSave save)
        {
            if (Has(save, Prepared))
                for (int i = 0; i < formulas.Count; i++)
                {
                    save.completed_one_time_crafts.RemoveAll(id => id == formulas[i].id);
                    if (Has(save, originals[i])) Add(save, formulas[i].id);
                }
            save.completed_one_time_crafts.RemoveAll(id => id == Prepared || id == Job || id.StartsWith(Prefix, StringComparison.Ordinal));
            Status("cleanup_restored_original", save);
        }
        internal static void Complete(GameSave save)
        {
            var learned = Grant(save);
            StudyTableWorkProbe.Write("RAR_KNOWLEDGE_REWARD learned=" + learned.Count + " known=" + Known(save) + " native_job_marker=" + Has(save, Job));
            if (learned.Count == 0) return;
            var tech = new TechDefinition { id = Product, price = new GameRes() };
            tech.crafts.AddRange(learned);
            StudyTableWorkProbe.Write("RAR_KNOWLEDGE_DIALOG phase=before unlocks=" + learned.Count);
            GUIElements.me.tech_dialog.Open(tech, delegate { Status("reward_dialog_closed", save); }, true, false, false, true);
            StudyTableWorkProbe.Write("RAR_KNOWLEDGE_DIALOG phase=after");
        }
        internal static List<string> Grant(GameSave save)
        {
            if (!Has(save, Prepared)) throw new InvalidOperationException("Research completed without prepared snapshot");
            var learned = new List<string>();
            foreach (var c in formulas) if (!Has(save, c.id)) learned.Add(c.id);
            foreach (string id in learned) Add(save, id);
            return learned;
        }

        [HarmonyPatch(typeof(GameSave), "IsCraftVisible")]
        private static class Visibility
        {
            private static void Postfix(GameSave __instance, CraftDefinition craft, ref bool __result)
            {
                if (craft.id == StudyTableWorkProbe.JobId) __result = false;
                if (craft.id == Job) __result = __result && StudyTableWorkProbe.enabledProbe && Has(__instance, Prepared) && Known(__instance) < formulas.Count;
            }
        }
        [HarmonyPatch(typeof(GameSave), "PrepareAfterLoad")]
        private static class Loaded
        {
            private static void Postfix(GameSave __instance) { Status("native_prepare_after_load", __instance); }
        }
        [HarmonyPatch(typeof(GameSave), "ToBinary")]
        private static class Saving
        {
            private static void Postfix(GameSave __instance) { Status("native_to_binary_returned", __instance); }
        }
        [HarmonyPatch(typeof(CraftGUI), "OpenAsAlchemy")]
        private static class NativeCards
        {
            private static void Postfix(CraftGUI __instance)
            {
                if (!Has(Save, Prepared)) return;
                int cards = 0;
                foreach (var item in __instance.list_items)
                    if (item.craft_definition != null && formulas.Exists(c => c.id == item.craft_definition.id)) cards++;
                StudyTableWorkProbe.Write("RAR_KNOWLEDGE_NATIVE_LIST station=" + __instance.GetCrafteryWGO().obj_id + " cards=" + cards + " known=" + Known(Save));
            }
        }
    }
}
