// SPDX-License-Identifier: MPL-2.0
using System;
using HarmonyLib;
using UnityEngine;

namespace NikichMods.Research
{
    // Bounded optical correction on the established final-widget geometry.
    // Session-only; reuse the input containment accepted by the earlier icon harness.
    internal static class HeaderCalibration
    {
        private const int Limit = 12;
        private static int down;
        private static bool shown, capturing, originalIgnoreEvents;
        private static int releaseFrame;
        internal static int Down { get { return down; } }

        internal static void Tick()
        {
            if (shown && (!StudyTableWorkProbe.enabledProbe || !RecipeGuidance.CanCalibrate)) Hide("card_unavailable");
            if (capturing && !shown && Time.frameCount > releaseFrame && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
            {
                UICamera.ignoreAllEvents = originalIgnoreEvents;
                capturing = false;
                StudyTableWorkProbe.Write("RAR_HEADER_CALIBRATION_INPUT restored=True");
            }
            if (!StudyTableWorkProbe.enabledProbe || !Input.GetKeyDown(KeyCode.F2)) return;
            if (shown) { Hide("f2"); return; }
            if (!RecipeGuidance.CanCalibrate || capturing || Input.GetMouseButton(0) || Input.GetMouseButton(1)) return;
            originalIgnoreEvents = UICamera.ignoreAllEvents;
            UICamera.ignoreAllEvents = true;
            capturing = shown = true;
            StudyTableWorkProbe.Write("RAR_HEADER_CALIBRATION_OPEN session_only=True step=1 range=12");
        }
        internal static void Hide(string reason)
        {
            if (!shown) return;
            shown = false; releaseFrame = Time.frameCount;
            StudyTableWorkProbe.Write("RAR_HEADER_CALIBRATION_VALUES reason=" + reason + " down_ui_pixels=" + down + " group=ingredients_and_energy baseline=measured_widget_centres");
        }
        internal static void Shutdown()
        {
            Hide("plugin_destroyed");
            if (capturing) UICamera.ignoreAllEvents = originalIgnoreEvents;
            capturing = false;
        }
        internal static void Draw()
        {
            if (!shown) return;
            GUILayout.BeginArea(new Rect(20, 20, 480, 240), GUI.skin.window);
            try
            {
                GUILayout.Label("Положение ингредиентов и энергии — F2 закрывает");
                GUILayout.Label("Одна общая поправка. Шаг: 1 пиксель интерфейса.");
                GUILayout.Label("Плюс — вниз, минус — вверх. 0 — положение 0.1.14.");
                GUILayout.Space(8);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Сдвиг: " + down, GUILayout.Width(90));
                int value = down;
                if (GUILayout.Button("−", GUILayout.Width(28))) value--;
                value = Mathf.RoundToInt(GUILayout.HorizontalSlider(Mathf.Clamp(value, -Limit, Limit), -Limit, Limit));
                if (GUILayout.Button("+", GUILayout.Width(28))) value++;
                GUILayout.EndHorizontal();
                if (GUILayout.Button("Сбросить поправку")) value = 0;
                value = Mathf.Clamp(value, -Limit, Limit);
                if (value != down) { down = value; RecipeGuidance.RefreshHeader(); }
                if (GUILayout.Button("Готово — записать значение в лог и закрыть")) Hide("chosen");
                GUILayout.Label("Настройка действует до перезапуска игры.");
            }
            finally { GUILayout.EndArea(); }
        }
        [HarmonyPatch(typeof(BaseGUI), "Update")]
        private static class Keys
        {
            private static bool Prefix(BaseGUI __instance)
            {
                return !(capturing && GUIElements.me != null &&
                    (__instance == GUIElements.me.craft || __instance == GUIElements.me.mixed_craft_tabbed));
            }
        }
    }
}
