// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace NikichMods.Research
{
    // Research-only native window composition. Survey still owns all sample/craft behavior.
    internal static class StudyTabs
    {
        internal const string StudyTab = "rar_study";
        internal const string ResearchTab = "rar_research";
        private static readonly FieldInfo TabIds = AccessTools.Field(typeof(CraftGUI), "_tabs_ids");
        private static readonly FieldInfo Tabs = AccessTools.Field(typeof(CraftGUI), "_tabs");
        private static readonly FieldInfo Expanded = AccessTools.Field(typeof(CraftGUI), "_exanded_item");
        private static readonly MethodInfo Collapse = AccessTools.Method(typeof(CraftGUI), "CollapseItem");
        private static readonly FieldInfo Keys = AccessTools.Field(typeof(BaseGUI), "gamekey_delegates");
        private static WorldGameObject table;
        private static bool switching;
        private static bool restoring;
        private static Snapshot snapshot;
        private static string selected;

        private static CraftGUI Craft { get { return GUIElements.me.craft; } }
        private static ResourceBasedCraftGUI Study { get { return GUIElements.me.resource_based_craft; } }
        private static bool Active { get { return table != null && snapshot != null; } }
        private static bool Owns(CraftGUI gui) { return Active && gui == Craft && gui.GetCrafteryWGO() == table; }

        internal static void Open(WorldGameObject owner)
        {
            if (Active) Restore();
            if (Craft.is_shown || Study.is_shown) throw new InvalidOperationException("Study tab entry requires closed native windows");
            if (TabIds == null || Tabs == null || Expanded == null || Collapse == null || Keys == null ||
                Craft.tabs_go == null || Craft.craft_tab_prefab == null || Craft.mixed_craft_go == null)
                throw new InvalidOperationException("Missing verified native tab contract");
            snapshot = new Snapshot(Study, Craft);
            table = owner;
            selected = StudyTab;
            ReplaceTabKeys(Study);
            ReplaceTabKeys(Craft);
            StudyTableWorkProbe.Write("RAR_TABS_OPEN phase=before table=" + owner.unique_id);
            Craft.OpenCraftList(owner);
            StudyTableWorkProbe.Write("RAR_TABS_OPEN phase=after outer=" + Craft.is_shown + " study=" + Study.is_shown);
        }

        private static void ReplaceTabKeys(BaseGUI gui)
        {
            var keys = (Dictionary<GameKey, GJCommons.BoolDelegate>)Keys.GetValue(gui);
            snapshot.keyMaps.Add(new KeyMap(keys));
            keys[GameKey.PrevTab] = delegate { return Toggle(gui); };
            keys[GameKey.NextTab] = delegate { return Toggle(gui); };
        }

        private static bool Toggle(BaseGUI owner)
        {
            if (!Active || !owner.is_shown_and_top || GUIElements.me.resource_picker.is_shown) return false;
            try { Switch(selected == StudyTab ? ResearchTab : StudyTab); return true; }
            catch (Exception ex) { StudyTableWorkProbe.Fail(ex); return true; }
        }

        private static void Switch(string id)
        {
            if (!Active || switching || GUIElements.me.resource_picker.is_shown) return;
            if (id != StudyTab && id != ResearchTab) throw new InvalidOperationException("Unexpected diagnostic tab");
            switching = true;
            try
            {
                if (Expanded.GetValue(Craft) != null) Collapse.Invoke(Craft, new object[] { null });
                if (Study.is_shown) Study.Hide(false);
                selected = id;
                foreach (CraftItemGUI item in Craft.list_items)
                    item.gameObject.SetActive(id == ResearchTab && item.craft_definition.tab_id == ResearchTab);
                foreach (CraftTabGUI tab in (List<CraftTabGUI>)Tabs.GetValue(Craft)) tab.SetSelectedState(tab.tab_id == id);
                Craft.scroll_view.gameObject.SetActive(id == ResearchTab);
                Craft.mixed_craft_go.SetActive(false);
                if (id == StudyTab)
                {
                    snapshot.HideStudyChrome();
                    // Native mixed-tab composition keeps a separate top-of-stack window as well.
                    // The inspected Survey container is offset +3 UI units from its root.
                    Study.transform.position = Craft.mixed_craft_go.transform.position - Study.transform.parent.TransformVector(new Vector3(0, 3, 0));
                    Study.GetComponent<UIPanel>().depth = Craft.GetComponent<UIPanel>().depth + 1;
                    Study.Open(table, CraftDefinition.CraftType.Survey);
                    Study.GetButtonTips().transform.position = Craft.GetButtonTips().transform.position;
                    Craft.GetButtonTips().gameObject.SetActive(false);
                }
                else
                {
                    Craft.GetButtonTips().gameObject.SetActive(true);
                    Craft.scroll_view.ResetPosition();
                    if (BaseGUI.for_gamepad) { Craft.GetGamepadController().Enable(); Craft.GetGamepadController().ReinitItems(true); }
                }
                Craft.UpdateAllAnchors();
                Sounds.OnGUITabClick();
                StudyTableWorkProbe.Write("RAR_TABS_SWITCH tab=" + id + " outer=" + Craft.is_shown + " study=" + Study.is_shown + " study_top=" + Study.is_shown_and_top);
            }
            finally { switching = false; }
        }

        internal static void Restore()
        {
            if (snapshot == null || restoring) return;
            restoring = true;
            try
            {
                var saved = snapshot;
                table = null;
                snapshot = null;
                if (Study.is_shown) Study.Hide(false);
                saved.Restore(Study, Craft);
                bool restored = saved.Matches(Study, Craft);
                StudyTableWorkProbe.Write("RAR_TABS_RESTORE study_shown=" + Study.is_shown + " layout_restored=" + restored);
                if (!restored) throw new InvalidOperationException("Survey layout rollback did not match snapshot");
            }
            catch (Exception ex)
            {
                StudyTableWorkProbe.enabledProbe = false;
                StudyTableWorkProbe.Write("RAR_TABS_RESTORE_ERROR action=disabled " + ex);
            }
            finally { restoring = false; }
        }

        private sealed class KeyMap
        {
            private readonly Dictionary<GameKey, GJCommons.BoolDelegate> map;
            private readonly GJCommons.BoolDelegate previous;
            private readonly GJCommons.BoolDelegate next;
            internal KeyMap(Dictionary<GameKey, GJCommons.BoolDelegate> owner) { map = owner; previous = map[GameKey.PrevTab]; next = map[GameKey.NextTab]; }
            internal void Restore() { map[GameKey.PrevTab] = previous; map[GameKey.NextTab] = next; }
        }

        private sealed class Snapshot
        {
            internal readonly List<KeyMap> keyMaps = new List<KeyMap>();
            private readonly Vector3 position;
            private readonly Vector3 tipsPosition;
            private readonly int panelDepth;
            private readonly bool craftTipsActive;
            private readonly Dictionary<GameObject, bool> chrome = new Dictionary<GameObject, bool>();
            internal Snapshot(ResourceBasedCraftGUI study, CraftGUI craft)
            {
                position = study.transform.localPosition;
                tipsPosition = study.GetButtonTips().transform.localPosition;
                panelDepth = study.GetComponent<UIPanel>().depth;
                craftTipsActive = craft.GetButtonTips().gameObject.activeSelf;
                foreach (string path in new[] { "window/back", "window/decore_back", "window/header", "window/Universal Object Info", "dark back" })
                {
                    Transform child = study.transform.Find(path);
                    if (child == null) throw new InvalidOperationException("Missing inspected Survey chrome: " + path);
                    chrome.Add(child.gameObject, child.gameObject.activeSelf);
                }
            }
            internal void HideStudyChrome() { foreach (GameObject child in chrome.Keys) child.SetActive(false); }
            internal void Restore(ResourceBasedCraftGUI study, CraftGUI craft)
            {
                foreach (KeyMap map in keyMaps) map.Restore();
                study.transform.localPosition = position;
                study.GetButtonTips().transform.localPosition = tipsPosition;
                study.GetComponent<UIPanel>().depth = panelDepth;
                foreach (var pair in chrome) pair.Key.SetActive(pair.Value);
                craft.GetButtonTips().gameObject.SetActive(craftTipsActive);
            }
            internal bool Matches(ResourceBasedCraftGUI study, CraftGUI craft)
            {
                if (study.transform.localPosition != position || study.GetButtonTips().transform.localPosition != tipsPosition ||
                    study.GetComponent<UIPanel>().depth != panelDepth || craft.GetButtonTips().gameObject.activeSelf != craftTipsActive) return false;
                foreach (var pair in chrome) if (pair.Key.activeSelf != pair.Value) return false;
                return true;
            }
        }

        [HarmonyPatch(typeof(CraftGUI), "RedrawCraftTabs")]
        private static class TabList
        {
            private static void Prefix(CraftGUI __instance)
            {
                if (!Owns(__instance)) return;
                var ids = (List<string>)TabIds.GetValue(__instance);
                ids.Clear(); ids.Add(StudyTab); ids.Add(ResearchTab);
            }
        }

        [HarmonyPatch(typeof(CraftGUI), "SwitchTab")]
        private static class TabSwitch
        {
            private static bool Prefix(CraftGUI __instance, string tab_id)
            {
                if (!Owns(__instance)) return true;
                try { Switch(tab_id); } catch (Exception ex) { StudyTableWorkProbe.Fail(ex); }
                return false;
            }
        }

        [HarmonyPatch(typeof(CraftTabGUI), "Draw")]
        private static class TabCaption
        {
            private static void Postfix(CraftTabGUI __instance, WorldGameObject craftery_wgo, string tab_id)
            {
                if (!Active || craftery_wgo != table || (tab_id != StudyTab && tab_id != ResearchTab)) return;
                try
                {
                    // Keep the native clickable tab; replace only its icon presentation with a short caption.
                    __instance.spr_icon.gameObject.SetActive(false);
                    UILabel label = UnityEngine.Object.Instantiate(Craft.LB);
                    label.name = "RAR tab caption";
                    label.transform.SetParent(__instance.transform, false);
                    label.transform.localPosition = Vector3.zero;
                    label.transform.localScale = Vector3.one;
                    label.width = 62; label.height = 24; label.fontSize = 10;
                    label.alignment = NGUIText.Alignment.Center;
                    label.depth = __instance.spr_icon.depth + 2;
                    label.text = tab_id == StudyTab ? "Изучение" : "Рецепты";
                    label.gameObject.SetActive(true);
                }
                catch (Exception ex) { StudyTableWorkProbe.Fail(ex); }
            }
        }

        [HarmonyPatch(typeof(CraftGUI), "Hide")]
        private static class OuterClose
        {
            private static void Prefix(CraftGUI __instance, out bool __state)
            {
                __state = Active && __instance == Craft;
                if (__state) Restore();
            }
            private static void Postfix(CraftGUI __instance, bool __state)
            {
                if (__state)
                    StudyTableWorkProbe.Write("RAR_TABS_CLOSED outer=" + __instance.is_shown + " study=" + Study.is_shown);
            }
        }

        [HarmonyPatch(typeof(ResourceBasedCraftGUI), "Hide")]
        private static class StudyClose
        {
            private static void Postfix(ResourceBasedCraftGUI __instance)
            {
                if (!Active || switching || restoring || __instance != Study) return;
                if (Craft.is_shown) Craft.Hide(false);
            }
        }
    }
}
