// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using HarmonyLib;

namespace NikichMods.Research
{
    internal static class QuantityInputCompatibility
    {
        private const string VerifiedHash = "e811fc228ece102e389759d278bb66569884898b827369f99e11c984ffc2af30";
        private static Func<bool> unavailable;
        private static Func<string> cardUnavailable;
        private static readonly HashSet<string> reported = new HashSet<string>();
        private static MethodInfo closed;
        private static FieldInfo selectedCard;
        private static FieldInfo selectedCraftery;
        internal static void Install(Harmony harmony, Func<bool> scope, Func<string> cardScope)
        {
            Assembly installed = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                if (assembly.GetName().Name == "MaxButtonsRedux") { installed = assembly; break; }
            if (installed == null) { StudyTableWorkProbe.Write("RAR_QUANTITY_COMPAT status=not_installed"); return; }
            string hash;
            using (var sha = SHA256.Create()) using (var stream = File.OpenRead(installed.Location))
                hash = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
            var target = installed.GetType("MaxButtonsRedux.Plugin", true).GetMethod("IsCraftGuiInputValid", BindingFlags.Static | BindingFlags.NonPublic);
            closed = installed.GetType("MaxButtonsRedux.Patches", true).GetMethod("CraftGUI_OnClosePressed", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            selectedCard = installed.GetType("MaxButtonsRedux.Plugin", true).GetField("_craftItemGui", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            selectedCraftery = installed.GetType("MaxButtonsRedux.Plugin", true).GetField("_crafteryWgo", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            if (hash != VerifiedHash || target == null || target.ReturnType != typeof(bool) || target.GetParameters().Length != 0 || closed == null || closed.GetParameters().Length != 0 || selectedCard == null || selectedCard.FieldType != typeof(CraftItemGUI) || selectedCraftery == null || selectedCraftery.FieldType != typeof(WorldGameObject))
                throw new InvalidOperationException("Unverified MaxButtonsRedux quantity-input contract: " + hash);
            unavailable = scope;
            cardUnavailable = cardScope;
            reported.Clear();
            harmony.Patch(target, prefix: new HarmonyMethod(typeof(QuantityInputCompatibility), "Prefix"));
            StudyTableWorkProbe.Write("RAR_QUANTITY_COMPAT status=bound hash=" + hash + " scope=study_table_and_guidance_window_native_focus");
        }
        internal static void OnOurFrameClosed()
        {
            if (closed == null) return;
            // Our native Hide path is a real close but bypasses the mod's OnClosePressed observer.
            closed.Invoke(null, null);
            StudyTableWorkProbe.Write("RAR_QUANTITY_COMPAT status=own_frame_close_notified");
        }
        private static bool Prefix(ref bool __result)
        {
            string reason = unavailable != null && unavailable() ? "study_table_composition" :
                cardUnavailable == null ? null : cardUnavailable();
            if (reason == null) return true;
            // The verified method's true result means invalid: its caller returns without Min/Max.
            __result = true;
            if (reported.Add(reason))
            {
                StudyTableWorkProbe.Write("RAR_QUANTITY_COMPAT status=scoped_input_skipped reason=" + reason + " native_quantity_unavailable=True");
            }
            return false;
        }
        internal static void Synchronize(CraftItemGUI card, WorldGameObject craftery)
        {
            // Called only after owned-window/native-focus readiness is verified.
            // The exact foreign OnOver observer only caches these same two inputs.
            if (selectedCard == null || (ReferenceEquals(selectedCard.GetValue(null), card) && ReferenceEquals(selectedCraftery.GetValue(null), craftery))) return;
            selectedCard.SetValue(null, card);
            selectedCraftery.SetValue(null, craftery);
            if (reported.Add("native_focus_synchronized"))
                StudyTableWorkProbe.Write("RAR_QUANTITY_COMPAT status=native_focus_synchronized input_projection=True");
        }
    }
}
