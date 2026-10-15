// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using HarmonyLib;

namespace NikichMods.ResearchableAlchemyRecipes
{
    // Bounded room/church sources plus the adopted resurrection technology source.
    internal static class SourceResearch
    {
        internal const string Builder = "keeper_room_builddesk";
        internal const string Church = "table_book_constr";
        internal const string Technology = "The Beginning Of Alchemy";
        internal const string ResurrectionTechnology = "Second chance";
        internal const string ResurrectionProduct = "zombie_juise";
        internal const string Witch = "npc_witch";
        internal const string TradePhrase = "@clotho_trade";
        private const string DiscoveryPrefix = "rar_seen_clotho_";
        private const string Acquaintance = "rar_clotho_trade_tier";
        private const string ExpansionPrefix = "rar_clotho_expanded_";
        private const string JobPrefix = "rar_research_";
        // One production research job per adopted product.
        private static readonly string[] Products = { "yellow_paint", "green_paint", "brown_paint", "red_paint", "d_green_paint", "d_violet_paint", "violet_paint", "black_paint", "blue_paint", "white_paint", "glue", "acid", "alkali", "aromatizer", "dark_elixir", "energy_elixir", "flask_green_l_plus", "flavor_enhancer", "freshener", "golden_elixir", "growth_enhancer", "preservative", "silver_elixir", "tanning_agent", ResurrectionProduct, "pot_appetite", "pot_armor", "pot_berserk", "pot_damage", "pot_heal", "pot_heal_long", "pot_mana", "pot_speed" };
        private static readonly int[] FormulaCounts = { 1, 1, 1, 1, 1, 1, 1, 3, 1, 3, 1, 1, 1, 1, 1, 2, 1, 1, 1, 2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1 };
        private sealed class Topic
        {
            internal string product;
            internal CraftDefinition job;
            internal readonly List<CraftDefinition> formulas = new List<CraftDefinition>();
            internal readonly List<ObjectCraftDefinition> sources = new List<ObjectCraftDefinition>();
            internal readonly List<CraftDefinition> churchSources = new List<CraftDefinition>();
            internal int roomVisible;
            internal int churchVisible;
            internal ItemDefinition vendorItem;
            internal int vendorVisible;
            internal int questVisible;
            internal int visible;
        }
        private static readonly Dictionary<string, Topic> topics = new Dictionary<string, Topic>();
        private static GameSave snapshotSave;
        private static GameBalance registeredBalance;
        private static bool sourceAvailable;
        private static VendorDefinition witchDefinition;

        internal static bool IsJob(string id) { return id != null && topics.ContainsKey(id); }
        internal static bool Eligible(bool alchemy, bool source, int visible, int remaining)
        {
            return alchemy && source && visible > 0 && remaining > 0;
        }
        internal static void Register(GameBalance balance)
        {
            if (ReferenceEquals(registeredBalance, balance)) return;
            topics.Clear(); snapshotSave = null; sourceAvailable = false;
            witchDefinition = balance.vendors_data.Find(v => v.id == Witch);
            if (witchDefinition == null || witchDefinition.start_tire != 1 || witchDefinition.additional_types.Count != 0 ||
                witchDefinition.not_selling.Count != 0 || witchDefinition.count_modificators.Count != 0)
                throw new InvalidOperationException("Verified Clotho stock definition changed");
            // Resolve original definitions before appending any work to the host list.
            for (int productIndex = 0; productIndex < Products.Length; productIndex++)
            {
                string product = Products[productIndex];
                var topic = new Topic { product = product };
                if (product.StartsWith("pot_", StringComparison.Ordinal))
                {
                    topic.vendorItem = balance.items_data.Find(item => item.id == product);
                    if (topic.vendorItem == null || topic.vendorItem.base_count <= 0 || topic.vendorItem.product_tier < 1 || topic.vendorItem.product_tier > 3 ||
                        !topic.vendorItem.product_types.Exists(type => witchDefinition.GetProductTypes().Contains(type)))
                        throw new InvalidOperationException("Verified Clotho product changed: " + product);
                }
                foreach (var craft in balance.craft_data)
                {
                    var output = craft.GetFirstRealOutput();
                    if (craft.craft_type == CraftDefinition.CraftType.MixedCraft && output != null && output.id == product)
                        topic.formulas.Add(craft);
                }
                foreach (var craft in balance.craft_obj_data)
                    if (craft.builder_ids.Contains(Builder) && craft.needs.Exists(item => item.id == product)) topic.sources.Add(craft);
                foreach (var craft in balance.craft_data)
                    if (craft.craft_in.Contains(Church) && craft.craft_type == CraftDefinition.CraftType.None && craft.needs.Exists(item => item.id == product)) topic.churchSources.Add(craft);
                // Explicit audited groups; do not silently generalize to other product outputs.
                if (topic.formulas.Count != FormulaCounts[productIndex] || (topic.vendorItem == null && product != ResurrectionProduct && topic.sources.Count == 0 && topic.churchSources.Count == 0))
                    throw new InvalidOperationException("Verified source group changed: " + product);
                topic.job = new CraftDefinition {
                    id = JobPrefix + product, craft_in = new List<string> { "mf_alchemy_survey" },
                    custom_name = product, icon = balance.items_data.Find(item => item.id == product).GetIcon(),
                    craft_type = CraftDefinition.CraftType.None,
                    needs = new List<Item> { new Item("faith", 1) }, needs_from_wgo = new List<Item> { new Item("science", 1) },
                    output = new List<Item>(), condition = new SmartExpression { default_value = 1 },
                    craft_time = new SmartExpression { default_value = 2 }, energy = new SmartExpression { default_value = 10 }, sanity = new SmartExpression(),
                    one_time_craft = true, disable_multi_craft = true, enqueue_type = CraftDefinition.EnqueueType.NeverEnqueue,
                    hide_quality_icon = true, tab_id = StudyTabs.ResearchTab
                };
                ResearchEconomics.Apply(topic.job, product);
                topics.Add(topic.job.id, topic);
            }
            foreach (var topic in topics.Values)
            {
                var existing = balance.craft_data.Find(c => c.id == topic.job.id);
                if (existing != null) throw new InvalidOperationException("Source research job ID collision");
                balance.craft_data.Add(topic.job);
            }
            registeredBalance = balance;
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
        internal static string Describe(string id, GameSave save, string original)
        {
            Topic topic;
            if (save == null || !topics.TryGetValue(id, out topic)) return original;
            int remaining = Remaining(topic, save);
            if (remaining == 0 || remaining == topic.formulas.Count) return original;
            string label = Localization.Get(Localization.Key.OtherMethods);
            return string.IsNullOrEmpty(original) ? label : original + "\n" + label;
        }
        [HarmonyPatch(typeof(CraftDefinition), "GetDescription")]
        private static class PartialDescription
        {
            private static void Postfix(CraftDefinition __instance, ref string __result)
            {
                if (!Plugin.Enabled || MainGame.me == null || !IsJob(__instance.id)) return;
                __result = Describe(__instance.id, MainGame.me.save, __result);
            }
        }
        internal static int VisibleSources(bool roomAvailable, int roomVisible, bool churchAvailable, int churchVisible)
        {
            return (roomAvailable ? roomVisible : 0) + (churchAvailable ? churchVisible : 0);
        }
        internal static bool ChurchPhysical(bool removed, bool removing, bool gdDisabled, bool interactionsDisabled, bool tutorialBlocked, bool hasCrafts, bool craftInteraction)
        {
            // Temporary work is deliberately absent from this adopted awareness predicate.
            return !removed && !removing && !gdDisabled && !interactionsDisabled && !tutorialBlocked && hasCrafts && craftInteraction;
        }
        internal static void Refresh(GameSave save)
        {
            RefreshCore(save);
        }
        internal static void RefreshForNotices(GameSave save)
        {
            RefreshCore(save);
        }
        private static void RefreshCore(GameSave save)
        {
            snapshotSave = null; sourceAvailable = false;
            // Lookup scans the existing WorldMap list; no singleton load, spawn or scene activation.
            var desk = WorldMap.GetWorldGameObjectByObjId(Builder, true);
            bool tutorial = MainGame.me.player.GetParamInt("in_tutorial") == 1 && MainGame.me.player.GetParamInt("tut_shown_tut_1") == 0;
            bool roomAvailable = desk != null && !desk.is_removed && !desk.IsDisabled() && desk.GetParamInt("disabled_interactions") == 0 && !tutorial;
            bool churchAvailable = false;
            var churchInstances = WorldMap.GetWorldGameObjectsByObjId(Church);
            foreach (var station in churchInstances)
            {
                var component = station.components.craft;
                if (ChurchPhysical(station.is_removed, station.is_removing, station.IsDisabled(), station.GetParamInt("disabled_interactions") != 0,
                    !station.obj_def.interactive_in_tutorial && save.IsInTutorial(), component != null && component.has_visible_crafts,
                    station.obj_def.interaction_type == ObjectDefinition.InteractionType.Craft)) churchAvailable = true;
            }
            sourceAvailable = roomAvailable || churchAvailable;
            bool tradeKnown = TradeKnown(save);
            var witchInstances = WorldMap.GetWorldGameObjectsByObjId(Witch);
            // Reconcile actual higher tiers after loading or a missed native event.
            // This is the existing on-demand source read, never a new frame scan.
            foreach (var station in witchInstances)
                if (tradeKnown && WitchPhysical(station, save) &&
                    Math.Abs(station.data.GetParam("vendor_inited", 0f) - 1f) <= 0.01f)
                    RecordExpansion(MainGame.me.player.data, station.data.GetParamInt("vendor_tier"), station.data);
            foreach (var topic in topics.Values)
            {
                topic.roomVisible = 0; topic.churchVisible = 0; topic.vendorVisible = 0; topic.questVisible = 0;
                foreach (var craft in topic.sources) if (save.IsCraftVisible(craft)) topic.roomVisible++;
                foreach (var craft in topic.churchSources) if (save.IsCraftVisible(craft)) topic.churchVisible++;
                topic.visible = VisibleSources(roomAvailable, topic.roomVisible, churchAvailable, topic.churchVisible);
                if (topic.product == ResurrectionProduct) topic.visible = save.unlocked_techs.Contains(ResurrectionTechnology) ? 1 : 0;
                if (topic.vendorItem != null)
                {
                    foreach (var station in witchInstances)
                    {
                        bool physical = WitchPhysical(station, save);
                        var data = station.data;
                        bool initialized = Math.Abs(data.GetParam("vendor_inited", 0f) - 1f) <= 0.01f;
                        int tier = initialized ? data.GetParamInt("vendor_tier") : witchDefinition.start_tire;
                        if (HasDiscovery(MainGame.me.player.data, topic.product) && VendorVisible(tradeKnown, physical, tier, topic.vendorItem.product_tier, VendorStock(data, initialized, topic.vendorItem, tier))) topic.vendorVisible++;
                    }
                    topic.visible = topic.vendorVisible;
                }
                // An actually assigned native request is its own awareness source:
                // no sample, physical source station or trade visit is required.
                topic.questVisible = QuestResearchSources.Known(save, topic.product) ? 1 : 0;
                if (topic.questVisible > 0) topic.visible = System.Math.Max(topic.visible, topic.questVisible);
            }
            snapshotSave = save;
        }
        internal static bool Available(string id, GameSave save)
        {
            Topic topic;
            return topics.TryGetValue(id, out topic) && ReferenceEquals(snapshotSave, save) &&
                Eligible(save.unlocked_techs.Contains(Technology), QuestResearchSources.Known(save, topic.product) || (topic.vendorItem != null ? TradeKnown(save) && !ReferenceEquals(MainGame.me.player, null) && HasDiscovery(MainGame.me.player.data, topic.product) :
                    topic.product == ResurrectionProduct ? save.unlocked_techs.Contains(ResurrectionTechnology) : sourceAvailable), topic.visible, Remaining(topic, save));
        }
        internal static bool TradeKnown(GameSave save)
        {
            return save.unlocked_phrases.Contains(TradePhrase) && !save.black_list_of_phrases.Contains(TradePhrase);
        }
        internal static List<string> AvailableProducts(GameSave save)
        {
            var result = new List<string>();
            foreach (string product in Products)
            {
                string id = JobPrefix + product;
                if (Available(id, save) && !save.completed_one_time_crafts.Contains(id)) result.Add(product);
            }
            return result;
        }

        internal static bool HasDiscovery(Item playerData, string product)
        {
            return playerData != null && playerData.GetParamInt(DiscoveryPrefix + product) == 1;
        }
        private static bool WitchPhysical(WorldGameObject station, GameSave save)
        {
            return !station.is_removed && !station.is_removing && !station.IsDisabled() && station.GetParamInt("disabled_interactions") == 0 &&
                (station.obj_def.interactive_in_tutorial || !save.IsInTutorial()) && station.obj_def.interaction_type == ObjectDefinition.InteractionType.RunScript;
        }
        internal static int AcquaintanceTier(Item playerData)
        {
            if (playerData == null) return 0;
            int tier = playerData.GetParamInt(Acquaintance);
            if (tier > 0) return tier;
            return tier;
        }
        internal static bool IsClothoExpansion(Item playerData, string product)
        {
            return playerData != null && playerData.GetParamInt(ExpansionPrefix + product) == 1;
        }
        internal static int RecordExpansion(Item playerData, int tier, Item vendorData)
        {
            int knownTier = AcquaintanceTier(playerData);
            if (knownTier <= 0 || tier <= knownTier) return 0;
            // Persist first acquaintance independently of later discoveries.
            if (playerData.GetParamInt(Acquaintance) != knownTier) playerData.SetParam(Acquaintance, knownTier);
            int added = 0;
            foreach (var topic in topics.Values)
            {
                if (topic.vendorItem == null || topic.vendorItem.product_tier <= knownTier ||
                    topic.vendorItem.product_tier > tier || HasDiscovery(playerData, topic.product) ||
                    VendorStock(vendorData, true, topic.vendorItem, tier) <= 0) continue;
                playerData.SetParam(DiscoveryPrefix + topic.product, 1f);
                playerData.SetParam(ExpansionPrefix + topic.product, 1f);
                added++;
            }
            return added;
        }
        internal static int RecordTrade(Item playerData, int tier, IEnumerable<Item> stock)
        {
            int knownTier = AcquaintanceTier(playerData);
            playerData.SetParam(Acquaintance, knownTier > 0 ? knownTier : tier);
            int added = 0;
            foreach (var topic in topics.Values)
            {
                if (topic.vendorItem == null || topic.vendorItem.product_tier > tier || HasDiscovery(playerData, topic.product)) continue;
                bool present = false;
                foreach (var item in stock) if (item != null && item.id == topic.product && item.value > 0) { present = true; break; }
                if (!present) continue;
                playerData.SetParam(DiscoveryPrefix + topic.product, 1f);
                added++;
            }
            return added;
        }
        [HarmonyPatch(typeof(Vendor), "TryLevelUpVendor")]
        private static class TierDiscovery
        {
            private static void Prefix(Vendor __instance, out int __state)
            {
                __state = Plugin.Enabled && __instance.id == Witch ? __instance.cur_tier : -1;
            }
            private static void Postfix(Vendor __instance, int __state, bool __result)
            {
                if (__state < 0 || !__result || __instance.cur_tier <= __state || !MainGame.game_started ||
                    MainGame.game_starting || MainGame.me == null || MainGame.me.player == null) return;
                try
                {
                    // Defer discovery to the existing safe source read, after native
                    // day/trade work and physical/access gates have settled.
                    AvailabilityNotifications.Request();
                }
                catch (Exception ex) { Plugin.Fail(ex); }
            }
        }
        [HarmonyPatch(typeof(VendorGUI), "Open")]
        private static class TradeDiscovery
        {
            private static void Postfix(VendorGUI __instance)
            {
                if (!Plugin.Enabled || !__instance.is_shown || __instance.trading == null ||
                    __instance.trading.trader == null || __instance.trading.trader.id != Witch || MainGame.me == null || MainGame.me.player == null) return;
                try
                {
                    var trader = __instance.trading.trader;
                    var stock = new List<Item>();
                    foreach (var inventory in trader.inventory.all) stock.AddRange(inventory.data.inventory);
                    RecordExpansion(MainGame.me.player.data, trader.cur_tier, trader.vendor_data);
                    RecordTrade(MainGame.me.player.data, trader.cur_tier, stock);
                }
                catch (Exception ex) { Plugin.Fail(ex); }
            }
        }
        internal static bool VendorVisible(bool tradeKnown, bool physical, int tier, int productTier, int stock)
        {
            return tradeKnown && physical && productTier <= tier && stock > 0;
        }
        internal static int VendorStock(Item data, bool initialized, ItemDefinition item, int tier)
        {
            // Pure projection of the verified cold Vendor constructor; never initialize/fill a vendor here.
            int stock = 0;
            foreach (var entry in data.inventory) if (entry != null && entry.id == item.id && entry.value > 0) stock += entry.value;
            if (!initialized) stock += item.product_tier > tier ? 1 : item.base_count;
            return stock;
        }
        internal static bool ReadyToStart(string id, GameSave save)
        {
            try
            {
                Refresh(save); // Recheck native inputs at the payment boundary, not only at menu creation.
                return Available(id, save);
            }
            catch (Exception ex) { Plugin.Fail(ex); return false; }
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
            if (Plugin.Enabled && learned.Count != 0)
            {
                // Knowledge is already committed through GrantRemaining. Presentation adds no reward.
                ResearchCompletion.Show(topic.product, learned.Count, null);
            }
        }

        [HarmonyPatch(typeof(GameSave), "IsCraftVisible")]
        private static class Visibility
        {
            private static void Postfix(GameSave __instance, CraftDefinition craft, ref bool __result)
            {
                if (IsJob(craft.id)) __result = __result && Plugin.Enabled && Available(craft.id, __instance);
            }
        }

    }
}
