// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NikichMods.ResearchableAlchemyRecipes
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
            var container = gui.subheader_container;
            bool priorActive = container.activeSelf;
            try
            {
                // Ordinary tech inspection can hide this container; info mode leaves it alone.
                // Activate before native layout, then restore on close/failure for other consumers.
                container.SetActive(true);
                gui.OpenAsItemsList(count, new List<Item> { new Item(product, 1) }, delegate {
                    container.SetActive(priorActive);
                    if (onClosed != null) onClosed();
                });
                // Native info mode owns the single product row, focus, font, resizing and close.
                // Set its public heading after the native generic-information writer.
                gui.label_header.text = Localization.Get(Localization.Key.ResearchCompleted);
                gui.UpdateAllAnchors();
                // Same native group/default path as a newly unlocked technology.
                Sounds.PlaySound("unlock");
            }
            catch { container.SetActive(priorActive); throw; }
        }
    }
}
