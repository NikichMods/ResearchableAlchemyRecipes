// SPDX-License-Identifier: MPL-2.0
using System.Globalization;
using UnityEngine;

namespace NikichMods.Research
{
    // Owned visuals only. Native row destruction owns these children; before each
    // redraw restore the original label so pooled/unrelated cards retain native UI.
    public sealed class ResearchSciencePriceCell : MonoBehaviour
    {
        private UILabel nativeLabel, counter;
        private UI2DSprite icon;
        private bool nativeActive;

        internal void Restore()
        {
            if (icon != null) icon.gameObject.SetActive(false);
            if (counter != null) counter.gameObject.SetActive(false);
            if (nativeLabel != null) nativeLabel.gameObject.SetActive(nativeActive);
        }

        internal void Show(UILabel original, int price, int available)
        {
            if (nativeLabel == null)
            {
                nativeLabel = original; nativeActive = original.gameObject.activeSelf;
                Transform box = original.transform.parent;
                var iconObject = new GameObject("Research science icon");
                iconObject.layer = original.gameObject.layer;
                iconObject.transform.SetParent(box, false);
                icon = iconObject.AddComponent<UI2DSprite>();
                icon.depth = original.depth;
                icon.sprite2D = EasySpritesCollection.GetSprite(new Item("science", 1).definition.GetIcon(), false, "");
                icon.color = Color.white;
                icon.leftAnchor.target = box; icon.leftAnchor.relative = 0.5f; icon.leftAnchor.absolute = -16;
                icon.rightAnchor.target = box; icon.rightAnchor.relative = 0.5f; icon.rightAnchor.absolute = 16;
                icon.bottomAnchor.target = box; icon.bottomAnchor.relative = 0.5f; icon.bottomAnchor.absolute = -16;
                icon.topAnchor.target = box; icon.topAnchor.relative = 0.5f; icon.topAnchor.absolute = 16;
                icon.width = 32; icon.height = 32;

                counter = Object.Instantiate(original, box, false);
                counter.name = "Research science count";
                counter.depth = original.depth + 1;
                counter.pivot = UIWidget.Pivot.BottomRight;
                counter.alignment = NGUIText.Alignment.Right;
                counter.overflowMethod = UILabel.Overflow.ClampContent;
                counter.maxLineCount = 1;
                counter.leftAnchor.target = box; counter.leftAnchor.relative = 0; counter.leftAnchor.absolute = 1;
                counter.rightAnchor.target = box; counter.rightAnchor.relative = 1; counter.rightAnchor.absolute = -1;
                counter.bottomAnchor.target = box; counter.bottomAnchor.relative = 0; counter.bottomAnchor.absolute = 1;
                counter.topAnchor.target = box; counter.topAnchor.relative = 0; counter.topAnchor.absolute = 17;
            }
            nativeLabel.gameObject.SetActive(false);
            icon.gameObject.SetActive(true);
            counter.gameObject.SetActive(true);
            string value = price.ToString(CultureInfo.InvariantCulture);
            counter.text = available < price ? "[ff1111]" + value + "[-]" : value;
            icon.UpdateAnchors(); counter.UpdateAnchors();
        }
    }
}
