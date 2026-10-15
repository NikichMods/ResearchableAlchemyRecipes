// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace NikichMods.Research
{
    // Research only: observe the exact native warning before CheckLayer repairs it.
    // Never suppress the original, change a layer, or enumerate scene objects.
    [HarmonyPatch(typeof(UIWidget), "CheckLayer")]
    internal static class PickerLayerObservation
    {
        private static int remaining = 16;
        private static bool failed;

        private static void Prefix(UIWidget __instance)
        {
            if (failed || remaining == 0 || !StudyTableWorkProbe.enabledProbe || !MainGame.game_started) return;
            try
            {
                if (GUIElements.me == null || GUIElements.me.resource_picker == null ||
                    !GUIElements.me.resource_picker.is_shown || __instance.panel == null) return;
                int widgetLayer = __instance.gameObject.layer;
                int panelLayer = __instance.panel.gameObject.layer;
                if (widgetLayer == panelLayer) return;
                remaining--;
                StudyTableWorkProbe.Write("RAR_PICKER_LAYER widget=" + Path(__instance.transform) +
                    " type=" + __instance.GetType().Name + " layer=" + widgetLayer +
                    " panel=" + Path(__instance.panel.transform) + " panel_layer=" + panelLayer +
                    " remaining=" + remaining);
                if (remaining == 15)
                    StudyTableWorkProbe.Write("RAR_PICKER_LAYER_STACK " + new System.Diagnostics.StackTrace(1, false));
            }
            catch (Exception ex)
            {
                failed = true;
                StudyTableWorkProbe.Write("RAR_PICKER_LAYER_OBSERVER_DISABLED " + ex.GetType().Name + ": " + ex.Message);
            }
        }

        private static string Path(Transform target)
        {
            var names = new List<string>();
            while (target != null && names.Count < 24)
            {
                names.Add(target.name);
                target = target.parent;
            }
            names.Reverse();
            return string.Join("/", names.ToArray());
        }
    }
}
