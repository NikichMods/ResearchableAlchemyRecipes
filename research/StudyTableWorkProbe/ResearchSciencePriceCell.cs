// SPDX-License-Identifier: MPL-2.0
using System.Globalization;
using UnityEngine;

namespace NikichMods.Research
{
    // Plain owned view, created only after the player opens the research tab.
    // The owning table window restores native labels and disposes the children.
    internal sealed class ResearchSciencePriceCell
    {
        private UILabel nativeLabel, counter;
        private UI2DSprite icon;
        private bool nativeActive;
        // User selected in .50: UI units from the cost backing center.
        private const int IconX = 0, IconY = 0, IconSize = 40;
        private const int CountX = 17, CountY = -19, CountSize = 16;

        internal void Restore()
        {
            if (icon != null) icon.gameObject.SetActive(false);
            if (counter != null) counter.gameObject.SetActive(false);
            if (nativeLabel != null) nativeLabel.gameObject.SetActive(nativeActive);
        }

        internal void Dispose()
        {
            Restore();
            if (icon != null) Object.Destroy(icon.gameObject);
            if (counter != null) Object.Destroy(counter.gameObject);
        }

        internal void Show(CraftItemGUI row, int price, int available)
        {
            UILabel original = row.txt_spend;
            if (nativeLabel == null)
            {
                nativeLabel = original; nativeActive = original.gameObject.activeSelf;
                Transform box = original.transform.parent;
                // Native item textures contain blue replacement pixels. Preserve the
                // game's renderer/shader and tint; a bare UI2DSprite renders them blue.
                icon = Object.Instantiate(row.item_gui.x1.icon, box, false);
                icon.name = "Research science icon";
                icon.color = row.item_gui.colors.normal;
                icon.depth = original.depth;
                icon.sprite2D = EasySpritesCollection.GetSprite(new Item("science", 1).definition.GetIcon(), false, "");

                counter = Object.Instantiate(row.item_gui.x1.counter, box, false);
                counter.name = "Research science count";
                counter.depth = original.depth + 1;
                counter.pivot = UIWidget.Pivot.BottomRight;
                counter.alignment = NGUIText.Alignment.Right;
                counter.overflowMethod = UILabel.Overflow.ClampContent;
                counter.maxLineCount = 1;
            }
            nativeLabel.gameObject.SetActive(false);
            icon.gameObject.SetActive(true);
            counter.gameObject.SetActive(true);
            string value = price.ToString(CultureInfo.InvariantCulture);
            counter.text = available < price ? "[ff1111]" + value + "[-]" : value;
            ApplyGeometry();
        }

        private void ApplyGeometry()
        {
            if (icon == null || counter == null || nativeLabel == null) return;
            Transform box = nativeLabel.transform.parent;
            CenterAnchors(icon, box, IconX - IconSize / 2, IconX + IconSize / 2,
                IconY - IconSize / 2, IconY + IconSize / 2);
            counter.fontSize = CountSize;
            CenterAnchors(counter, box, CountX - 30, CountX, CountY, CountY + CountSize);
        }

        private static void CenterAnchors(UIWidget widget, Transform box, int left, int right, int bottom, int top)
        {
            widget.leftAnchor.target = box; widget.leftAnchor.relative = 0.5f; widget.leftAnchor.absolute = left;
            widget.rightAnchor.target = box; widget.rightAnchor.relative = 0.5f; widget.rightAnchor.absolute = right;
            widget.bottomAnchor.target = box; widget.bottomAnchor.relative = 0.5f; widget.bottomAnchor.absolute = bottom;
            widget.topAnchor.target = box; widget.topAnchor.relative = 0.5f; widget.topAnchor.absolute = top;
            widget.UpdateAnchors();
        }
    }
}
