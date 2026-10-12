// SPDX-License-Identifier: MPL-2.0
using System;
using System.Globalization;
using UnityEngine;

namespace NikichMods.Research
{
    // Temporary session-only calibration; no save/config writes or per-frame object scans.
    internal static class IconCalibration
    {
        private static int materialX, materialY, recipeX, recipeY;
        private static bool shown;
        private static bool capturing;
        private static bool originalIgnoreEvents;
        private static int releaseFrame;
        private const int Limit = 8;
        internal static bool Capturing { get { return capturing; } }

        internal static Vector3 Extra(bool material)
        {
            return material ? new Vector3(materialX, materialY, 0) : new Vector3(recipeX, recipeY, 0);
        }

        internal static void Tick()
        {
            if (shown && (!StudyTableWorkProbe.enabledProbe || !StudyTabs.CanCalibrate)) Hide("table_unavailable");
            if (capturing && !shown && Time.frameCount > releaseFrame && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
            {
                UICamera.ignoreAllEvents = originalIgnoreEvents;
                capturing = false;
                StudyTableWorkProbe.Write("RAR_TAB_CALIBRATION_INPUT restored=True");
            }
            if (!StudyTableWorkProbe.enabledProbe || !Input.GetKeyDown(KeyCode.F2)) return;
            if (shown) { Hide("f2"); return; }
            if (!StudyTabs.CanCalibrate || capturing || Input.GetMouseButton(0) || Input.GetMouseButton(1)) return;
            originalIgnoreEvents = UICamera.ignoreAllEvents;
            UICamera.ignoreAllEvents = true;
            capturing = true;
            shown = true;
            StudyTableWorkProbe.Write("RAR_TAB_CALIBRATION_OPEN session_only=True step=1 range=8");
        }

        internal static void Hide(string reason)
        {
            if (!shown) return;
            shown = false;
            releaseFrame = Time.frameCount;
            Report(reason);
        }

        internal static void Shutdown()
        {
            Hide("plugin_destroyed");
            if (capturing) UICamera.ignoreAllEvents = originalIgnoreEvents;
            capturing = false;
        }

        private static void Report(string reason)
        {
            StudyTableWorkProbe.Write("RAR_TAB_CALIBRATION_VALUES reason=" + reason +
                " material_dx=" + materialX + " material_dy=" + materialY +
                " recipe_dx=" + recipeX + " recipe_dy=" + recipeY +
                " material_x=" + F(-3f + materialX) + " material_y=" + F(1f + materialY) +
                " recipe_x=" + F(-0.5f + recipeX) + " recipe_y=" + F(0.5f + recipeY));
        }

        private static string F(float value) { return value.ToString(CultureInfo.InvariantCulture); }

        internal static void Draw()
        {
            if (!shown) return;
            GUILayout.BeginArea(new Rect(20, 20, 440, 440), GUI.skin.window);
            try
            {
                GUILayout.Label("Положение значков — F2 закрывает панель");
                GUILayout.Label("Поправки к 0.1.4. Шаг: 1 пиксель интерфейса.");
                GUILayout.Label("X: вправо (+). Y: вверх (+). Без записи в сохранение.");
                GUILayout.Space(8);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Показать изучение")) StudyTabs.ShowCalibrationTab(true);
                if (GUILayout.Button("Показать рецепты")) StudyTabs.ShowCalibrationTab(false);
                GUILayout.EndHorizontal();
                GUILayout.Space(8);
                GUILayout.Label("Изучение вещества: лист, камень, бабочка");
                int mx = Slider("X", materialX);
                int my = Slider("Y", materialY);
                GUILayout.Space(8);
                GUILayout.Label("Исследование рецепта: свиток и колба");
                int rx = Slider("X", recipeX);
                int ry = Slider("Y", recipeY);
                if (mx != materialX || my != materialY || rx != recipeX || ry != recipeY)
                {
                    materialX = mx; materialY = my; recipeX = rx; recipeY = ry;
                    StudyTabs.RefreshIconPositions();
                }
                GUILayout.Space(8);
                if (GUILayout.Button("Сбросить поправки к 0.1.4"))
                {
                    materialX = materialY = recipeX = recipeY = 0;
                    StudyTabs.RefreshIconPositions();
                }
                if (GUILayout.Button("Готово — записать значения в лог и закрыть")) Hide("chosen");
                GUILayout.Label("Настройка действует до перезапуска игры.");
            }
            finally { GUILayout.EndArea(); }
        }

        private static int Slider(string label, int value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label + ": " + value, GUILayout.Width(55));
            if (GUILayout.Button("−", GUILayout.Width(28))) value--;
            value = Mathf.RoundToInt(GUILayout.HorizontalSlider(Mathf.Clamp(value, -Limit, Limit), -Limit, Limit));
            if (GUILayout.Button("+", GUILayout.Width(28))) value++;
            GUILayout.EndHorizontal();
            return Mathf.Clamp(value, -Limit, Limit);
        }
    }
}
