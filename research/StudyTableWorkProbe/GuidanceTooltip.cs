// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace NikichMods.Research
{
    internal static class GuidanceTooltip
    {
        private static TooltipBubbleGUI bubble;
        private static CraftItemGUI selected;
        private static CraftDefinition formula;
        private static UIWidget frame;
        private static Camera camera;
        private static float gap;
        private static bool logged;
        private static UIWidget[] chrome;
        private static UIWidget[] amountWidgets;
        private static int lastWidth, lastHeight, settled;
        internal static void Show(CraftGUI gui, CraftItemGUI row)
        {
            if (bubble != null && selected == row && ReferenceEquals(formula, row.current_craft)) return;
            Hide();
            var owner = gui.transform.Find("window/back");
            frame = owner == null ? null : owner.GetComponent<UIWidget>();
            camera = MainGame.me.gui_cam;
            var collider = row.GetComponent<Collider2D>();
            if (frame == null || camera == null || collider == null) throw new InvalidOperationException("Missing native tooltip/window geometry");
            selected = row; formula = row.current_craft;
            var data = new BubbleWidgetDataContainer(WidgetsBubbleGUI.Alignment.Left,
                new BubbleWidgetTextData("[241A13]" + Name(formula) + "[-]\n", UITextStyles.TextStyle.HintTitle, NGUIText.Alignment.Left),
                new BubbleWidgetTextData(RecipeGuidance.Text(formula), UITextStyles.TextStyle.TinyDescription, NGUIText.Alignment.Left));
            // Collider mode provides native lifetime/layout on both input modes; this bubble
            // is independent of the manager's ordinary item tooltip and never takes focus.
            bubble = TooltipBubbleGUI.Show(data, true, collider);
            if (bubble == null) throw new InvalidOperationException("Native tooltip creation failed");
            bubble.widget.alpha = 0f;
            // Native index LD: the bubble grows up/right from its lower-left tail.
            for (int i = 0; i < bubble.corners.Length; i++) bubble.corners[i].SetActive(i == 0);
            // Exact native tooltip prefab leaves BaseBubbleGUI.back_spr unset.
            // Its four corner families own the frame; cache them once, measure only the
            // currently active family plus the native content widget after native layout.
            var widgets = new List<UIWidget> { bubble.widget };
            foreach (var corner in bubble.corners)
                widgets.AddRange(corner.GetComponentsInChildren<UIWidget>(true));
            chrome = widgets.ToArray();
            amountWidgets = row.amount_buttons == null ? new UIWidget[0] : row.amount_buttons.GetComponentsInChildren<UIWidget>(true);
            float scale = PixelsPerUnit(bubble.transform);
            gap = row.label_descr.fontSize * scale * 0.5f;
            Rect rect = Bounds(frame);
            float right = Screen.width - RightEdge(rect) - gap * 2f;
            int width = Mathf.FloorToInt(Mathf.Min(row.GetComponent<UIWidget>().width, right / scale - row.label_descr.fontSize));
            if (width < row.label_descr.fontSize * 4) { Hide(); StudyTableWorkProbe.Write("RAR_GUIDANCE_TOOLTIP status=hidden reason=insufficient_side_space"); return; }
            foreach (var label in bubble.GetComponentsInChildren<UILabel>(true))
            {
                label.leftAnchor.target = null; label.rightAnchor.target = null;
                label.topAnchor.target = null; label.bottomAnchor.target = null;
                label.ResetAnchors();
                label.overflowMethod = UILabel.Overflow.ResizeHeight;
                label.color = Color.white;
                label.width = width;
                label.ProcessText(false, true);
            }
            bubble.UpdateSizeAndWidgetsPositions();
            Place();
            StudyTableWorkProbe.Write("RAR_GUIDANCE_TOOLTIP product=" + formula.GetFirstRealOutput().id + " text_width=" + width + " renderer=native independent_list=True");
        }
        private static string Name(CraftDefinition craft) { return GJL.L(craft.GetNameNonLocalized()); }
        private static float PixelsPerUnit(Transform transform)
        {
            var origin = camera.WorldToScreenPoint(transform.position);
            return (camera.WorldToScreenPoint(transform.TransformPoint(Vector3.right)) - origin).magnitude;
        }
        private static Rect Bounds(UIWidget widget)
        {
            float minX = float.PositiveInfinity, minY = minX, maxX = float.NegativeInfinity, maxY = maxX;
            foreach (var corner in widget.worldCorners)
            {
                Vector3 p = camera.WorldToScreenPoint(corner);
                minX = Mathf.Min(minX, p.x); minY = Mathf.Min(minY, p.y);
                maxX = Mathf.Max(maxX, p.x); maxY = Mathf.Max(maxY, p.y);
            }
            return Rect.MinMaxRect(minX, minY, maxX, maxY);
        }
        private static void Place()
        {
            if (bubble == null || frame == null) return;
            Rect box = Bounds(bubble.widget), host = Bounds(frame);
            foreach (var widget in chrome)
                if (widget != null && widget.gameObject.activeInHierarchy)
                {
                    Rect part = Bounds(widget);
                    box = Rect.MinMaxRect(Mathf.Min(box.xMin, part.xMin), Mathf.Min(box.yMin, part.yMin),
                        Mathf.Max(box.xMax, part.xMax), Mathf.Max(box.yMax, part.yMax));
                }
            float x = RightEdge(host) + gap;
            x = Mathf.Clamp(x, gap, Mathf.Max(gap, Screen.width - gap - box.width));
            float bottom = host.yMin;
            if (box.height <= Screen.height - gap * 2f) bottom = Mathf.Clamp(bottom, gap, Screen.height - gap - box.height);
            var current = camera.WorldToScreenPoint(bubble.transform.position);
            var desired = current + new Vector3(x - box.xMin, bottom - box.yMin, 0f);
            bubble.transform.position += camera.ScreenToWorldPoint(desired) - camera.ScreenToWorldPoint(current);
            if (!logged)
            {
                logged = true;
                StudyTableWorkProbe.Write("RAR_GUIDANCE_TOOLTIP_POSITION anchor=window_lower_right tail=LD height_screen=" + box.height + " exceeds_screen=" + (box.height > Screen.height - gap * 2f));
            }
        }
        private static float RightEdge(Rect host)
        {
            float right = host.xMax;
            foreach (var widget in amountWidgets)
                if (widget != null && widget.gameObject.activeInHierarchy) right = Mathf.Max(right, Bounds(widget).xMax);
            return right;
        }
        internal static void Hide()
        {
            var old = bubble; bubble = null;
            selected = null; formula = null; frame = null; camera = null; chrome = null; amountWidgets = null; logged = false;
            lastWidth = lastHeight = settled = 0;
            if (old != null) old.DestroyBubble();
        }
        [HarmonyPatch(typeof(WidgetsBubbleGUI), "Update")]
        private static class Position
        {
            [HarmonyPriority(Priority.Last)]
            private static void Postfix(WidgetsBubbleGUI __instance)
            {
                if (!ReferenceEquals(__instance, bubble)) return;
                try { Place(); bubble.widget.alpha = settled > 0 ? 1f : 0f; }
                catch (Exception ex) { Hide(); StudyTableWorkProbe.Fail(ex); }
            }
        }
        [HarmonyPatch(typeof(BaseBubbleGUI), "UpdateBubble")]
        private static class FixedCorner
        {
            // Retain native text/widget updates, but this one independent bubble no longer
            // follows collider-driven native corner/position selection.
            private static bool Prefix(BaseBubbleGUI __instance) { return !ReferenceEquals(__instance, bubble); }
        }
        [HarmonyPatch(typeof(WidgetsBubbleGUI), "LateUpdate")]
        private static class Ready
        {
            [HarmonyPriority(Priority.Last)]
            private static void Postfix(WidgetsBubbleGUI __instance)
            {
                if (!ReferenceEquals(__instance, bubble)) return;
                try
                {
                    int width = bubble.widget.width, height = bubble.widget.height;
                    settled = width == lastWidth && height == lastHeight ? settled + 1 : 0;
                    lastWidth = width; lastHeight = height;
                    Place();
                    bubble.widget.alpha = settled > 0 ? 1f : 0f;
                    if (settled == 1) StudyTableWorkProbe.Write("RAR_GUIDANCE_TOOLTIP_READY phase=visible width=" + width + " height=" + height);
                }
                catch (Exception ex) { Hide(); StudyTableWorkProbe.Fail(ex); }
            }
        }
    }
}
