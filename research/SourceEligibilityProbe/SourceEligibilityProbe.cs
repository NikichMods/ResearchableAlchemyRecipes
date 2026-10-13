// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using BepInEx;
using BepInEx.Logging;
using UnityEngine;

[assembly: AssemblyVersion("0.1.3.0")]
[assembly: AssemblyFileVersion("0.1.3.0")]

namespace NikichMods.Research
{
    [BepInPlugin("nikichmods.research.sourceeligibility", "Source Eligibility Probe", Version)]
    public sealed class SourceEligibilityProbe : BaseUnityPlugin
    {
        public const string Version = "0.1.3";
        private static readonly string[] Potions = { "pot_appetite", "pot_armor", "pot_berserk", "pot_damage", "pot_heal", "pot_heal_long", "pot_mana", "pot_speed" };
        private const string Builder = "keeper_room_builddesk";
        private const string Church = "table_book_constr";
        private static readonly string[] ChurchProducts = { "acid", "alkali", "aromatizer", "black_paint", "dark_elixir", "energy_elixir", "flask_green_l_plus", "flavor_enhancer", "freshener", "glue", "golden_elixir", "growth_enhancer", "preservative", "silver_elixir", "tanning_agent", "white_paint" };
        private const string Spice = "taste_booster:alchemy_1_spice";
        private static readonly string[] Paints = { "yellow_paint", "green_paint", "brown_paint", "red_paint", "d_green_paint", "d_violet_paint", "violet_paint" };
        private static ManualLogSource log;
        private static bool ready;
        private static readonly FieldInfo BalanceInstance = typeof(GameBalance).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly FieldInfo NativeCrafts = typeof(BaseCraftGUI).GetField("crafts", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo CardCrafts = typeof(CraftItemGUI).GetField("_possible_crafts", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo VendorObject = typeof(VendorGUI).GetField("_vendor_obj", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo VendorWidgets = typeof(InventoryPanelGUI).GetField("_widgets", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo InventoryCells = typeof(InventoryWidget).GetField("items", BindingFlags.Instance | BindingFlags.NonPublic);
        internal static void ValidateBindings()
        {
            if (BalanceInstance == null || NativeCrafts == null || CardCrafts == null || VendorObject == null || VendorWidgets == null || InventoryCells == null ||
                InventoryCells.FieldType != typeof(List<BaseItemCellGUI>) ||
                VendorObject.FieldType != typeof(WorldGameObject) || VendorWidgets.FieldType != typeof(List<InventoryWidget>) ||
                NativeCrafts.FieldType != typeof(List<CraftDefinition>) || CardCrafts.FieldType != typeof(List<CraftDefinition>))
                throw new InvalidOperationException("Missing verified native source/card fields");
        }
        private void Awake()
        {
            log = Logger;
            try
            {
                string hash;
                using (var sha = SHA256.Create()) using (var stream = File.OpenRead(typeof(GameSave).Assembly.Location))
                    hash = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                if (hash != "e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8") throw new InvalidOperationException("Unverified host: " + hash);
                ValidateBindings();
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
                var vendorGui = GUIElements.me.vendor;
                if (vendorGui.is_shown)
                {
                    CaptureVendor(balance, vendorGui, phase);
                    Write("RAR_SOURCE_DONE phase=" + phase + " source=vendor");
                    return;
                }
                var activeGui = GUIElements.me.craft;
                var activeStation = activeGui.GetCrafteryWGO();
                if (activeGui.is_shown && activeStation != null && activeStation.obj_id == Church)
                {
                    CaptureChurch(balance, activeGui, activeStation, phase);
                    Write("RAR_SOURCE_DONE phase=" + phase + " source=church_workbench");
                    return;
                }
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
        internal static HashSet<string> SourceIds(IEnumerable<CraftDefinition> crafts, string product)
        {
            var ids = new HashSet<string>();
            foreach (var craft in crafts) if (Uses(craft, product) != 0) ids.Add(craft.id);
            return ids;
        }
        internal static int StockCount(IEnumerable<Item> items, string product)
        {
            int count = 0;
            foreach (var item in items) if (item != null && item.id == product && item.value > 0) count += item.value;
            return count;
        }
        private static void CaptureVendor(GameBalance balance, VendorGUI gui, string phase)
        {
            var shown = (WorldGameObject)VendorObject.GetValue(gui);
            // Only read the trading object already created by the player's native window.
            var trader = gui.trading == null ? null : gui.trading.trader;
            if (shown == null || shown.obj_id != "npc_witch" || trader == null)
            { Write("RAR_SOURCE_VENDOR_SKIPPED reason=clotho_not_shown"); return; }
            Write("RAR_SOURCE_CALL phase=before action=existing_vendor_lists");
            var widgets = (List<InventoryWidget>)VendorWidgets.GetValue(gui.vendor_panel);
            var instances = WorldMap.GetWorldGameObjectsByObjId("npc_witch");
            foreach (var station in instances)
                Write("RAR_SOURCE_VENDOR_INSTANCE id=" + station.unique_id + " shown=" + ReferenceEquals(station, shown) +
                    " removed=" + station.is_removed + " gd_disabled=" + station.IsDisabled() +
                    " disabled_interactions=" + station.GetParamInt("disabled_interactions"));
            foreach (string product in Potions)
            {
                int stock = 0, drawing = 0, lockedDrawing = 0, cells = 0, lockedCells = 0;
                foreach (var inventory in trader.inventory.all) stock += StockCount(inventory.data.inventory, product);
                foreach (var inventory in trader.drawing_inventory.all)
                {
                    int n = StockCount(inventory.data.inventory, product); drawing += n;
                    if (inventory.is_locked) lockedDrawing += n;
                }
                foreach (var widget in widgets)
                    foreach (var cell in (List<BaseItemCellGUI>)InventoryCells.GetValue(widget))
                        if (cell.gameObject.activeInHierarchy && cell.item != null && cell.item.id == product && cell.item.value > 0)
                        {
                            cells++;
                            if (widget.inventory_lock_widget != null && widget.inventory_lock_widget.gameObject.activeInHierarchy) lockedCells++;
                        }
                int total, known; Knowledge(balance, product, out total, out known);
                Write("RAR_SOURCE_VENDOR_PRODUCT product=" + product + " stock=" + stock + " drawing=" + drawing +
                    " locked_drawing=" + lockedDrawing + " active_cells=" + cells + " cells_under_lock=" + lockedCells +
                    " formulas=" + total + " known=" + known + " remaining=" + (total - known));
            }
            Write("RAR_SOURCE_CALL phase=after action=existing_vendor_lists");
            Write("RAR_SOURCE_VENDOR_RESULT phase=" + phase + " source_window_reached=True shown_indexed=" +
                instances.Exists(station => ReferenceEquals(station, shown)) + " tier=" + trader.cur_tier +
                " trade_phrase_unlocked=" + MainGame.me.save.unlocked_phrases.Contains("@clotho_trade") +
                " trade_phrase_blacklisted=" + MainGame.me.save.black_list_of_phrases.Contains("@clotho_trade") +
                " products=" + Potions.Length + " mutations=0 visual_lock_occlusion_unverified=True");
        }
        internal static HashSet<string> RenderedIds(IEnumerable<CraftItemGUI> cells, string product)
        {
            var ids = new HashSet<string>();
            foreach (var cell in cells)
                foreach (string id in SourceIds((List<CraftDefinition>)CardCrafts.GetValue(cell), product)) ids.Add(id);
            return ids;
        }
        private static void CaptureChurch(GameBalance balance, CraftGUI gui, WorldGameObject shown, string phase)
        {
            var save = MainGame.me.save;
            Write("RAR_SOURCE_CALL phase=before action=church_instances");
            var instances = WorldMap.GetWorldGameObjectsByObjId(Church);
            Write("RAR_SOURCE_CALL phase=after action=church_instances count=" + instances.Count);
            foreach (var station in instances)
            {
                var component = station.components.craft;
                Write("RAR_SOURCE_CHURCH_INSTANCE id=" + station.unique_id + " shown=" + ReferenceEquals(station, shown) +
                    " removed=" + station.is_removed + " removing=" + station.is_removing + " gd_disabled=" + station.IsDisabled() +
                    " disabled_interactions=" + station.GetParamInt("disabled_interactions") +
                    " tutorial_blocks=" + (!station.obj_def.interactive_in_tutorial && save.IsInTutorial()) +
                    " native_can_interact=" + (component != null && component.CanInteractCraft()) +
                    " busy=" + (component != null && component.is_crafting));
            }
            Write("RAR_SOURCE_CALL phase=before action=church_source_cards");
            var expected = new List<CraftDefinition>();
            foreach (var craft in balance.craft_data)
                if (craft.craft_in.Contains(Church) && craft.craft_type == CraftDefinition.CraftType.None && save.IsCraftVisible(craft)) expected.Add(craft);
            var native = (List<CraftDefinition>)NativeCrafts.GetValue(gui);
            bool shownIndexed = instances.Exists(station => ReferenceEquals(station, shown));
            bool allAgree = shownIndexed;
            foreach (string product in ChurchProducts)
            {
                var predicted = SourceIds(expected, product);
                var consumed = SourceIds(native, product);
                var rendered = RenderedIds(gui.list_items, product);
                bool agree = predicted.SetEquals(consumed) && predicted.SetEquals(rendered);
                allAgree &= agree;
                int total, known; Knowledge(balance, product, out total, out known);
                Write("RAR_SOURCE_CHURCH_PRODUCT product=" + product + " native_visible=" + predicted.Count +
                    " native_consumed=" + consumed.Count + " rendered_definitions=" + rendered.Count + " exact_ids_agree=" + agree +
                    " formulas=" + total + " known=" + known + " remaining=" + (total - known));
            }
            Write("RAR_SOURCE_CALL phase=after action=church_source_cards");
            Write("RAR_SOURCE_CHURCH_RESULT phase=" + phase + " products=" + ChurchProducts.Length + " exact_ids_agree=" + allAgree +
                " source_window_reached=True shown_indexed=" + shownIndexed + " alchemy=" + save.unlocked_techs.Contains("The Beginning Of Alchemy") + " mutations=0");
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
