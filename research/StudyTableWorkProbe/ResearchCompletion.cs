// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NikichMods.Research
{
    internal static class ResearchCompletion
    {
        internal static string CountText(int count)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException("count");
            return Localization.Get(Localization.Key.NewRecipes).Replace("{0}", count.ToString(CultureInfo.InvariantCulture));
        }

        internal static void Show(string product, int learned, GJCommons.VoidDelegate onClosed)
        {
            if (learned == 0) return;
            string count = CountText(learned);
            var gui = GUIElements.me.tech_dialog;
            gui.OpenAsItemsList(count, new List<Item> { new Item(product, 1) }, onClosed);
            // Native info mode owns the single product row, focus, font, resizing and close.
            // Set its public heading after the native generic-information writer.
            gui.label_header.text = Localization.Get(Localization.Key.ResearchCompleted);
            gui.UpdateAllAnchors();
            StudyTableWorkProbe.Write("RAR_RESEARCH_RESULT product=" + product + " new_recipes=" + learned + " rows=1 native_info=True");
        }
    }
}
