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
        private static bool revealed;
        private static UIWidget background;
        private static UIWidget[] chrome;
        private static UIWidget[] amountWidgets;
        private static Vector4 lastLayout;
        private static int settled;
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
            // Verified 1.407 corner groups contain only tails/attachment points.
            // The rectangular background is independent, despite back_spr being null.
            foreach (var corner in bubble.corners) corner.SetActive(false);
            var back = bubble.transform.Find("back and points/back");
            background = back == null ? null : back.GetComponent<UIWidget>();
            if (background == null) throw new InvalidOperationException("Missing native tooltip background");
            chrome = bubble.GetComponentsInChildren<UIWidget>(true);
            amountWidgets = row.amount_buttons == null ? new UIWidget[0] : row.amount_buttons.GetComponentsInChildren<UIWidget>(true);
            float scale = PixelsPerUnit(bubble.transform);
            gap = row.label_descr.fontSize * scale * 0.5f;
            UpdateAnchors();
            // Measure the actual title/section headings with the current native font.
            // Long source/condition lines wrap; they do not dictate empty panel width.
            int preferred = MeasureHeadings();
            float padding = (Bounds(background).width - Bounds(bubble.widget).width) / scale;
            float right = Screen.width - RightEdge(Bounds(frame)) - gap * 2f;
            int width = Mathf.FloorToInt(Mathf.Min(preferred, right / scale - padding));
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
            UpdateAnchors();
            StudyTableWorkProbe.Write("RAR_GUIDANCE_TOOLTIP product=" + formula.GetFirstRealOutput().id + " text_width=" + width + " renderer=native independent_list=True");
        }
        private static int MeasureHeadings()
        {
            float width = 0f;
            foreach (var label in bubble.GetComponentsInChildren<UILabel>(true))
            {
                string original = label.text;
                var headings = new List<string>();
                string[] lines = original.Split('\n');
                headings.Add(lines[0]);
                foreach (string line in lines)
                    if (line.StartsWith("  ", StringComparison.Ordinal) && !line.StartsWith("    ", StringComparison.Ordinal)) headings.Add(line);
                label.overflowMethod = UILabel.Overflow.ResizeFreely;
                label.text = string.Join("\n", headings.ToArray());
                label.ProcessText(false, true);
                width = Mathf.Max(width, label.printedSize.x);
                label.text = original;
            }
            return Mathf.CeilToInt(width);
        }
        private static void UpdateAnchors()
        {
            foreach (var widget in chrome)
                if (widget != null && widget.gameObject.activeInHierarchy) widget.UpdateAnchors();
            foreach (var widget in amountWidgets)
                if (widget != null && widget.gameObject.activeInHierarchy) widget.UpdateAnchors();
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
        private static void PrepareGeometry()
        {
            UpdateAnchors();
            Rect box = Bounds(background), host = Bounds(frame);
            float x = RightEdge(host) + gap;
            x = Mathf.Clamp(x, gap, Mathf.Max(gap, Screen.width - gap - box.width));
            float bottom = host.yMin;
            if (box.height <= Screen.height - gap * 2f) bottom = Mathf.Clamp(bottom, gap, Screen.height - gap - box.height);
            var current = camera.WorldToScreenPoint(bubble.transform.position);
            var desired = current + new Vector3(x - box.xMin, bottom - box.yMin, 0f);
            bubble.transform.position += camera.ScreenToWorldPoint(desired) - camera.ScreenToWorldPoint(current);
            var layout = new Vector4(Mathf.Round(x), Mathf.Round(bottom), Mathf.Round(box.width), Mathf.Round(box.height));
            settled = layout == lastLayout ? settled + 1 : 0;
            lastLayout = layout;
            bubble.widget.alpha = settled > 0 ? 1f : 0f;
            if (settled > 0 && !revealed)
            {
                revealed = true;
                StudyTableWorkProbe.Write("RAR_GUIDANCE_TOOLTIP_READY phase=before_geometry anchor=window_lower_right tail=False x=" + layout.x + " y=" + layout.y + " width=" + layout.z + " height=" + layout.w + " exceeds_screen=" + (box.height > Screen.height - gap * 2f));
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
            selected = null; formula = null; frame = null; camera = null; chrome = null; amountWidgets = null; background = null; revealed = false;
            lastLayout = Vector4.zero; settled = 0;
            if (old != null) old.DestroyBubble();
        }
        [HarmonyPatch(typeof(WidgetsBubbleGUI), "Update")]
        private static class Position
        {
            [HarmonyPriority(Priority.Last)]
            private static void Postfix(WidgetsBubbleGUI __instance)
            {
                if (!ReferenceEquals(__instance, bubble)) return;
                // Native collider-mode Update raises zero alpha. Keep creation hidden;
                // only the render-geometry consumer may reveal the settled instance.
                try { bubble.widget.alpha = settled > 0 ? 1f : 0f; }
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
        [HarmonyPatch(typeof(UIPanel), "UpdateSelf")]
        private static class Ready
        {
            [HarmonyPriority(Priority.Last)]
            private static void Prefix(UIPanel __instance)
            {
                // 1.407 UpdateSelf builds widget geometry/draw calls. A bubble's LateUpdate
                // is not that final consumer; placement there can miss this frame's mesh.
                // Other panel contents and the native UpdateSelf are left intact.
                if (bubble == null || background == null || !ReferenceEquals(__instance, background.panel)) return;
                try
                {
                    PrepareGeometry();
                }
                catch (Exception ex) { Hide(); StudyTableWorkProbe.Fail(ex); }
            }
        }
    }
}
