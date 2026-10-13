// SPDX-License-Identifier: MPL-2.0
using System;
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
        private static bool onLeft;
        private static float gap;
        private static bool logged;
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
                new BubbleWidgetTextData(Name(formula), UITextStyles.TextStyle.HintTitle, NGUIText.Alignment.Left),
                new BubbleWidgetTextData(RecipeGuidance.Text(formula), UITextStyles.TextStyle.TinyDescription, NGUIText.Alignment.Left));
            // Collider mode provides native lifetime/layout on both input modes; this bubble
            // is independent of the manager's ordinary item tooltip and never takes focus.
            bubble = TooltipBubbleGUI.Show(data, true, collider);
            if (bubble == null) throw new InvalidOperationException("Native tooltip creation failed");
            float scale = PixelsPerUnit(bubble.transform);
            gap = row.label_descr.fontSize * scale * 0.5f;
            Rect rect = Bounds(frame);
            float left = rect.xMin - gap * 2f, right = Screen.width - rect.xMax - gap * 2f;
            onLeft = left >= right;
            int width = Mathf.FloorToInt(Mathf.Min(row.GetComponent<UIWidget>().width, Mathf.Max(left, right) / scale - row.label_descr.fontSize));
            if (width < row.label_descr.fontSize * 4) { Hide(); StudyTableWorkProbe.Write("RAR_GUIDANCE_TOOLTIP status=hidden reason=insufficient_side_space"); return; }
            foreach (var label in bubble.GetComponentsInChildren<UILabel>(true))
            {
                label.leftAnchor.target = null; label.rightAnchor.target = null;
                label.topAnchor.target = null; label.bottomAnchor.target = null;
                label.ResetAnchors();
                label.overflowMethod = UILabel.Overflow.ResizeHeight;
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
            Rect box = Bounds(bubble.back_spr), host = Bounds(frame);
            float x = onLeft ? host.xMin - gap - box.width : host.xMax + gap;
            x = Mathf.Clamp(x, gap, Mathf.Max(gap, Screen.width - gap - box.width));
            float top = Mathf.Min(host.yMax, Screen.height - gap);
            if (box.height <= Screen.height - gap * 2f) top = Mathf.Max(top, box.height + gap);
            var current = camera.WorldToScreenPoint(bubble.transform.position);
            var desired = current + new Vector3(x - box.xMin, top - box.yMax, 0f);
            bubble.transform.position += camera.ScreenToWorldPoint(desired) - camera.ScreenToWorldPoint(current);
            if (!logged)
            {
                logged = true;
                StudyTableWorkProbe.Write("RAR_GUIDANCE_TOOLTIP_POSITION side=" + (onLeft ? "left" : "right") + " height_screen=" + box.height + " exceeds_screen=" + (box.height > Screen.height - gap * 2f));
            }
        }
        internal static void Hide()
        {
            var old = bubble; bubble = null;
            selected = null; formula = null; frame = null; camera = null; logged = false;
            if (old != null) old.DestroyBubble();
        }
        [HarmonyPatch(typeof(WidgetsBubbleGUI), "Update")]
        private static class Position
        {
            [HarmonyPriority(Priority.Last)]
            private static void Postfix(WidgetsBubbleGUI __instance)
            {
                if (!ReferenceEquals(__instance, bubble)) return;
                try { Place(); } catch (Exception ex) { Hide(); StudyTableWorkProbe.Fail(ex); }
            }
        }
    }
}
