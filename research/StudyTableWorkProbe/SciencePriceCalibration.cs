// SPDX-License-Identifier: MPL-2.0
using UnityEngine;

namespace NikichMods.Research
{
    // Temporary visual calibration on the existing plugin, never a Unity component.
    // Reuses the accepted F2 capture/release pattern; no save or config writes.
    internal static class SciencePriceCalibration
    {
        internal static int IconX, IconY, IconSize = 32, CountX = 16, CountY = -14, CountSize = 16;
        private static bool shown, capturing, originalIgnoreEvents;
        private static int releaseFrame;
        internal static bool Capturing { get { return capturing; } }

        internal static void Tick()
        {
            if (shown && (!StudyTableWorkProbe.enabledProbe || !StudyTabs.CanCalibratePrice)) Hide("table_unavailable");
            if (capturing && !shown && Time.frameCount > releaseFrame && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
            {
                UICamera.ignoreAllEvents = originalIgnoreEvents;
                capturing = false;
                StudyTableWorkProbe.Write("RAR_PRICE_CALIBRATION_INPUT restored=True");
            }
            if (!StudyTableWorkProbe.enabledProbe || !Input.GetKeyDown(KeyCode.F2)) return;
            if (shown) { Hide("f2"); return; }
            if (!StudyTabs.CanCalibratePrice || capturing || Input.GetMouseButton(0) || Input.GetMouseButton(1)) return;
            originalIgnoreEvents = UICamera.ignoreAllEvents;
            UICamera.ignoreAllEvents = true;
            capturing = shown = true;
            StudyTableWorkProbe.Write("RAR_PRICE_CALIBRATION_OPEN session_only=True");
        }

        internal static void Hide(string reason)
        {
            if (!shown) return;
            shown = false; releaseFrame = Time.frameCount;
            StudyTableWorkProbe.Write("RAR_PRICE_CALIBRATION_VALUES reason=" + reason +
                " icon_x=" + IconX + " icon_y=" + IconY + " icon_size=" + IconSize +
                " count_x=" + CountX + " count_y=" + CountY + " count_size=" + CountSize);
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
            // Left side stays away from the right-hand price cells.
            GUILayout.BeginArea(new Rect(12, 12, Mathf.Min(420, Screen.width - 24), 470), GUI.skin.window);
            try
            {
                GUILayout.Label("Настройка стоимости — F2 закрывает панель");
                GUILayout.Label("Шаг: 1 единица игрового интерфейса.");
                GUILayout.Label("X: вправо (+), Y: вверх (+), от центра ячейки.");
                GUILayout.Label("Игровой шейдер и контур сохранены.");
                GUILayout.Space(8);
                bool changed = false;
                GUILayout.Label("Атом");
                changed |= Slider("X", ref IconX, -16, 16);
                changed |= Slider("Y", ref IconY, -16, 16);
                changed |= Slider("Размер", ref IconSize, 12, 48);
                GUILayout.Space(8);
                GUILayout.Label("Число — положение правого нижнего угла");
                changed |= Slider("X", ref CountX, -16, 18);
                changed |= Slider("Y", ref CountY, -20, 12);
                changed |= Slider("Размер", ref CountSize, 10, 24);
                GUILayout.Space(8);
                if (GUILayout.Button("Сбросить настройки"))
                {
                    IconX = IconY = 0; IconSize = 32; CountX = 16; CountY = -14; CountSize = 16;
                    changed = true;
                }
                if (changed) ResearchCostDisplay.ApplyGeometry();
                if (GUILayout.Button("Готово — записать значения в лог и закрыть")) Hide("chosen");
                GUILayout.Label("До перезапуска игры. Сохранение не меняется.");
            }
            finally { GUILayout.EndArea(); }
        }

        private static bool Slider(string label, ref int value, int min, int max)
        {
            int before = value;
            GUILayout.BeginHorizontal();
            GUILayout.Label(label + ": " + value, GUILayout.Width(100));
            if (GUILayout.Button("−", GUILayout.Width(28))) value--;
            value = Mathf.RoundToInt(GUILayout.HorizontalSlider(Mathf.Clamp(value, min, max), min, max));
            if (GUILayout.Button("+", GUILayout.Width(28))) value++;
            GUILayout.EndHorizontal();
            value = Mathf.Clamp(value, min, max);
            return value != before;
        }
    }
}
