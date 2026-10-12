// SPDX-License-Identifier: MPL-2.0
using System;
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
        private static bool reported;
        private static MethodInfo closed;
        internal static void Install(Harmony harmony, Func<bool> scope)
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
            if (hash != VerifiedHash || target == null || target.ReturnType != typeof(bool) || target.GetParameters().Length != 0 || closed == null || closed.GetParameters().Length != 0)
                throw new InvalidOperationException("Unverified MaxButtonsRedux quantity-input contract: " + hash);
            unavailable = scope;
            reported = false;
            harmony.Patch(target, prefix: new HarmonyMethod(typeof(QuantityInputCompatibility), "Prefix"));
            StudyTableWorkProbe.Write("RAR_QUANTITY_COMPAT status=bound hash=" + hash + " scope=study_table_composition");
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
            if (unavailable == null || !unavailable()) return true;
            // The verified method's true result means invalid: its caller returns without Min/Max.
            __result = true;
            if (!reported)
            {
                StudyTableWorkProbe.Write("RAR_QUANTITY_COMPAT status=scoped_input_skipped native_quantity_unavailable=True");
                reported = true;
            }
            return false;
        }
    }
}
