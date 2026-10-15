// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace NikichMods.ResearchableAlchemyRecipes
{
    internal static class AvailabilityNotifications
    {
        private static readonly FieldInfo Tooltips = AccessTools.Field(typeof(TooltipBubbleGUI), "_all");
        private static readonly FieldInfo Speech = AccessTools.Field(typeof(SpeechBubbleGUI), "all");
        private static readonly MethodInfo PlayerControl = AccessTools.Method(typeof(BaseCharacterComponent), "PlayerControlIsDisabled");
        private static readonly FieldInfo UnlockRows = AccessTools.Field(typeof(TechUnlockDialogGUI), "_unlocks");
        private static readonly Type[] TargetTypes = { typeof(MainGame), typeof(MainGame), typeof(GameSave), typeof(GameSave), typeof(GameSave), typeof(GameSave), typeof(EnvironmentEngine), typeof(WorldMap) };
        private static readonly string[] TargetNames = { "OnGameLoaded", "OnGameStartedPlaying", "UnlockTech", "UnlockCraft", "SetTaskState", "OnFinishedCraft", "OnEndOfDay", "RescanWGOsList" };
        private static readonly Type[] HandlerTypes = { typeof(Loading), typeof(Loaded), typeof(Technology), typeof(CraftUnlock), typeof(Task), typeof(CraftFinished), typeof(Day), typeof(WorldRefresh) };
        private static Harmony eventHarmony;
        private static bool installed, loaded, disabled, dirty, pageOpen, nextPage;
        private static int quietFrame = -1, requestFrame, generation;
        private static GameSave boundSave;
        private static TechUnlockDialogGUI owner;
        private static GameObject countContainer;
        private static bool priorContainerActive;
        private static Item displayedData;
        private static List<string> displayedPage;
        private static int openedFrame;
        internal static bool Installed { get { return installed && !disabled; } }

        internal static void Install()
        {
            if (Tooltips == null || Speech == null || UnlockRows == null ||
                UnlockRows.FieldType != typeof(List<TechTreeGUIUnlockItem>) ||
                PlayerControl == null || PlayerControl.ReturnType != typeof(bool) || PlayerControl.GetParameters().Length != 0)
            { Fail(new InvalidOperationException("Missing verified notification overlay/control contract")); return; }
            try
            {
                // Optional hooks have separate ownership; a notice failure cannot abort the research patches.
                eventHarmony = new Harmony("nikichmods.researchablealchemyrecipes.notices");
                for (int i = 0; i < TargetTypes.Length; i++)
                {
                    var method = AccessTools.DeclaredMethod(TargetTypes[i], TargetNames[i]);
                    var handler = new HarmonyMethod(AccessTools.Method(HandlerTypes[i], i == 0 ? "Prefix" : "Postfix"));
                    if (i == 0) eventHarmony.Patch(method, prefix: handler);
                    else eventHarmony.Patch(method, postfix: handler);
                }
                BaseGUI.on_window_opened += WindowOpened;
                installed = true;
                BaseGUI.on_window_closed += WindowClosed;
            }
            catch (Exception ex) { Fail(ex); } // Any partially installed hooks become inert for this session.
        }
        internal static void Dispose()
        {
            if (installed)
            {
                BaseGUI.on_window_opened -= WindowOpened;
                BaseGUI.on_window_closed -= WindowClosed;
                installed = false;
            }
            Reset();
            if (eventHarmony != null)
            {
                try { eventHarmony.UnpatchSelf(); }
                catch (Exception ex) { Fail(ex); }
                eventHarmony = null;
            }
        }
        private static void RestoreContainer()
        {
            if (countContainer != null) countContainer.SetActive(priorContainerActive);
            countContainer = null;
        }
        private static void Reset()
        {
            var previousOwner = owner;
            bool closeOwnPage = pageOpen;
            generation++;
            RestoreContainer();
            loaded = false; dirty = false; pageOpen = false; nextPage = false;
            quietFrame = -1; boundSave = null; owner = null;
            displayedData = null; displayedPage = null;
            if (closeOwnPage && previousOwner != null && previousOwner.is_shown) previousOwner.Hide(false);
        }
        internal static void Request()
        {
            if (!loaded || disabled) return;
            dirty = true; quietFrame = -1; requestFrame = Time.frameCount;
        }
        private static void WindowOpened(BaseGUI gui)
        {
            // A foreign replacement of this same shared instance ends our ownership.
            if (pageOpen && gui == owner)
            {
                generation++; RestoreContainer(); pageOpen = false; nextPage = false;
                displayedData = null; displayedPage = null;
            }
            Request();
        }
        private static void WindowClosed(BaseGUI gui)
        {
            if (pageOpen && gui == owner) return; // Our on-hide callback decides Next versus dismissal.
            Request();
        }
        private static bool HasOverlay(FieldInfo field)
        {
            var collection = field.GetValue(null) as ICollection;
            if (collection == null) throw new InvalidOperationException("Notification overlay collection changed");
            return collection.Count != 0;
        }
        private static bool ManagedHintShown(GUIElements gui)
        {
            var bubble = gui.bubble_widgets_container;
            return bubble != null && bubble.gameObject.activeInHierarchy && bubble.widget != null && bubble.widget.alpha > 0f &&
                bubble.data != null && bubble.data.has_data;
        }
        internal static void Tick()
        {
            if (!Plugin.Enabled || !loaded || disabled || (!dirty && !nextPage)) return;
            try
            {
                if (pageOpen)
                {
                    if (nextPage) AdvancePage();
                    return;
                }
                var game = MainGame.me;
                if (!MainGame.game_started || game == null || game.player == null || game.save == null) return;
                if (!ReferenceEquals(boundSave, game.save))
                {
                    boundSave = game.save;
                    quietFrame = -1; requestFrame = Time.frameCount;
                }
                var gui = game.gui_elements;
                bool alchemy = game.save.unlocked_techs.Contains(SourceResearch.Technology);
                if (!alchemy) { dirty = false; quietFrame = -1; return; }
                bool tooltips = HasOverlay(Tooltips);
                bool managedHint = gui != null && ManagedHintShown(gui);
                bool speech = HasOverlay(Speech);
                bool controlDisabled = game.player_char == null || (bool)PlayerControl.Invoke(game.player_char, null);
                bool inputHeld = Input.anyKey || Input.GetMouseButton(0) || Input.GetMouseButton(1);
                bool safe = gui != null && AvailabilityNoticeState.Safe(MainGame.game_started && !MainGame.disable_all_game,
                    MainGame.game_starting, MainGame.paused, controlDisabled,
                    BaseGUI.all_guis_closed, tooltips || managedHint, speech, gui.corner_talk.is_shown,
                    gui.context_menu_bubble.is_shown, inputHeld);
                if (!safe || Time.frameCount <= requestFrame)
                {
                    quietFrame = -1;
                    return;
                }
                if (quietFrame < 0) { quietFrame = Time.frameCount; return; }
                if (Time.frameCount <= quietFrame) return;
                dirty = false; quietFrame = -1;
                Item data = game.player.data;
                SourceResearch.RefreshForNotices(game.save);
                var eligible = SourceResearch.AvailableProducts(game.save);
                var pending = AvailabilityNoticeState.Pending(data, alchemy, eligible);
                if (pending.Count == 0) return;
                ShowPage(data, pending);
            }
            catch (Exception ex) { Fail(ex); }
        }
        private static void AdvancePage()
        {
            var game = MainGame.me;
            if (game == null || !ReferenceEquals(game.save, boundSave) || !MainGame.game_started ||
                MainGame.game_starting || MainGame.paused || MainGame.disable_all_game ||
                owner == null || !owner.is_shown_and_top || Time.frameCount <= requestFrame ||
                Input.anyKey || Input.GetMouseButton(0) || Input.GetMouseButton(1) ||
                HasOverlay(Tooltips) || HasOverlay(Speech) || ManagedHintShown(game.gui_elements) ||
                game.gui_elements.corner_talk.is_shown || game.gui_elements.context_menu_bubble.is_shown) return;
            Item data = game.player.data;
            bool alchemy = game.save.unlocked_techs.Contains(SourceResearch.Technology);
            SourceResearch.RefreshForNotices(game.save);
            var eligible = SourceResearch.AvailableProducts(game.save);
            var pending = AvailabilityNoticeState.Pending(data, alchemy, eligible);
            if (pending.Count == 0) { ClosePage(false); return; }
            ShowPage(data, pending, true);
        }
        private static void ShowPage(Item data, List<string> pending, bool replaceContents = false)
        {
            var page = AvailabilityNoticeState.CausePage(pending, product => SourceResearch.IsClothoExpansion(data, product));
            bool expansion = page.Count > 0 && SourceResearch.IsClothoExpansion(data, page[0]);
            bool more = pending.Count > page.Count;
            bool initial = AvailabilityNoticeState.IsInitial(data);
            if (!replaceContents)
            {
                owner = MainGame.me.gui_elements.tech_dialog;
                countContainer = owner.subheader_container;
                priorContainerActive = countContainer.activeSelf;
                countContainer.SetActive(true);
            }
            string subheader = expansion ? Localization.Get(Localization.Key.ClothoAssortment).Replace("{0}", GJL.L(SourceResearch.Witch)) :
                initial ? Localization.Get(Localization.Key.NoticeHowTo) : "";
            if (more)
            {
                if (subheader.Length != 0) subheader += "\n";
                subheader += Localization.Get(Localization.Key.NoticeMore).Replace("{0}", (pending.Count - page.Count).ToString(CultureInfo.InvariantCulture));
            }
            var items = new List<Item>();
            foreach (string product in page) items.Add(new Item(product, 1));
            int token = generation;
            nextPage = false;
            if (!replaceContents)
                owner.OpenAsItemsList(subheader, items, delegate {
                    if (token != generation) return;
                    RestoreContainer(); pageOpen = false; nextPage = false;
                    displayedData = null; displayedPage = null;
                    dirty = false; quietFrame = -1;
                });
            else
            {
                // Native info row rendering, without closing/reopening the window.
                // Run on a later update, outside native button/key enumeration.
                var rows = (List<TechTreeGUIUnlockItem>)UnlockRows.GetValue(owner);
                if (rows.Count < page.Count) throw new InvalidOperationException("Native notice rows changed");
                owner.subheader.text = subheader;
                for (int i = 0; i < rows.Count; i++)
                {
                    if (i >= items.Count) { rows[i].Draw(null, null); continue; }
                    var definition = items[i].definition;
                    rows[i].Draw(new TechUnlock.TechUnlockData {
                        sprite = EasySpritesCollection.GetSprite(definition.GetIcon(), false, ""),
                        name = definition.GetItemName(true), quality_icon = definition.TryGetQualitySprite(),
                        description = definition.GetItemDescription(null)
                    }, null);
                }
                owner.table.Reposition();
            }
            owner.label_header.text = Localization.Get(Localization.Key.NoticeTitle);
            var buttons = owner.GetComponentInChildren<DialogButtonsGUI>(true);
            buttons.Set(more ? Localization.Get(Localization.Key.Next) : GJL.L("OK"), delegate { ClosePage(more); },
                more ? Localization.Get(Localization.Key.Close) : null, more ? new GJCommons.VoidDelegate(delegate { ClosePage(false); }) : null,
                null, null, GameKey.Select, GameKey.Back);
            owner.UpdateAllAnchors();
            // Match DialogGUI.OpenDialog: anchor layout can collapse a previously hidden
            // button label. Restore its native cached vertical offsets after layout.
            foreach (var button in buttons.GetComponentsInChildren<DialogButtonGUI>(true)) button.RestoreHeight();
            if (!owner.is_shown || !owner.add_to_opened_stack) throw new InvalidOperationException("Native notice page did not enter its window stack");
            pageOpen = true; dirty = false;
            nextPage = false;
            displayedData = data; displayedPage = page;
            openedFrame = Time.frameCount;
        }
        internal static void ObserveShown()
        {
            if (disabled || !pageOpen || displayedPage == null) return;
            try
            {
                // A later frame/top-of-stack observation avoids consuming a page hidden/replaced
                // by another native popup in the same opening frame.
                if (owner == null || !owner.is_shown_and_top || Time.frameCount <= openedFrame) return;
                AvailabilityNoticeState.MarkDisplayed(displayedData, displayedPage);
                displayedData = null; displayedPage = null;
            }
            catch (Exception ex) { Fail(ex); }
        }
        private static void ClosePage(bool continuePages)
        {
            if (!pageOpen) return;
            ObserveShown();
            nextPage = continuePages;
            if (continuePages) requestFrame = Time.frameCount;
            else owner.Hide(true);
        }
        private static void Fail(Exception ex)
        {
            if (disabled) return;
            disabled = true; dirty = false; RestoreContainer();
            Plugin.Write("RAR_NOTICE_ERROR action=disabled_for_session research_preserved=True " + ex);
        }

        private static class Loading
        {
            private static void Prefix() { if (disabled) return; try { Reset(); } catch (Exception ex) { Fail(ex); } }
        }
        private static class Loaded
        {
            private static void Postfix()
            {
                if (disabled) return;
                loaded = true; Request();
            }
        }
        private static class Technology
        {
            private static void Postfix() { Request(); }
        }
        private static class CraftUnlock
        {
            private static void Postfix() { Request(); }
        }
        private static class Task
        {
            private static void Postfix() { Request(); }
        }
        private static class CraftFinished
        {
            private static void Postfix(CraftDefinition craft)
            {
                if (!disabled && craft != null && (craft is ObjectCraftDefinition || SourceResearch.IsJob(craft.id) || craft.craft_type == CraftDefinition.CraftType.MixedCraft)) Request();
            }
        }
        private static class Day
        {
            private static void Postfix() { Request(); }
        }
        private static class WorldRefresh
        {
            private static void Postfix() { Request(); }
        }
    }
}
