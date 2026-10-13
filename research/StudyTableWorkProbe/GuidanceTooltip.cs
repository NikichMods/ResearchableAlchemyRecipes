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
        private static UILabel[] textLabels;
        private static int expectedStudyMarkers;
        private static Vector4 lastLayout;
        private static int settled;
        private static string language;
        private static UIPanel renderPanel;
        private static string studyMarker, studyMarkerText, studyExplanation;
        private static int observedFrames;
        private static float observedMinX, observedMaxX;
        internal static void Show(CraftGUI gui, CraftItemGUI row)
        {
            string currentLanguage = GJL.GetCurLng();
            if (bubble != null && selected == row && ReferenceEquals(formula, row.current_craft) && language == currentLanguage) return;
            Hide();
            language = currentLanguage;
            studyMarker = RecipeGuidance.StudyMarker;
            studyMarkerText = "[" + Localization.Get(Localization.Key.Unstudied) + "]";
            studyExplanation = RecipeGuidance.StudyExplanation;
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
            // The ordinary tooltip panel otherwise batches both bubbles' backgrounds
            // before both text groups. Keep this bubble's complete draw-call group below it.
            var sharedPanel = bubble.GetComponentInParent<UIPanel>();
            if (sharedPanel == null || frame.panel == null || sharedPanel.renderQueue != UIPanel.RenderQueue.Automatic ||
                bubble.GetComponent<UIPanel>() != null || sharedPanel.depth <= frame.panel.depth + 1)
                throw new InvalidOperationException("Missing verified native tooltip panel ordering");
            renderPanel = bubble.gameObject.AddComponent<UIPanel>();
            renderPanel.depth = sharedPanel.depth - 1;
            renderPanel.renderQueue = UIPanel.RenderQueue.Automatic;
            renderPanel.useSortingOrder = sharedPanel.useSortingOrder;
            renderPanel.sortingOrder = sharedPanel.sortingOrder;
            renderPanel.sortingLayerName = sharedPanel.sortingLayerName;
            // Verified 1.407 corner groups contain only tails/attachment points.
            // The rectangular background is independent, despite back_spr being null.
            foreach (var corner in bubble.corners) corner.SetActive(false);
            var back = bubble.transform.Find("back and points/back");
            background = back == null ? null : back.GetComponent<UIWidget>();
            if (background == null) throw new InvalidOperationException("Missing native tooltip background");
            chrome = bubble.GetComponentsInChildren<UIWidget>(true);
            foreach (var widget in chrome)
            {
                widget.RemoveFromPanel();
                // Not-yet-started widgets join this nearest panel in their native Start.
                widget.CreatePanel();
            }
            StudyTableWorkProbe.Write("RAR_GUIDANCE_RENDER_GROUP guidance_depth=" + renderPanel.depth + " native_tooltip_depth=" + sharedPanel.depth);
            amountWidgets = row.amount_buttons == null ? new UIWidget[0] : row.amount_buttons.GetComponentsInChildren<UIWidget>(true);
            float scale = PixelsPerUnit(bubble.transform);
            gap = row.label_descr.fontSize * scale * 0.5f;
            UpdateAnchors();
            // Full logical source rows determine width; shared introductory/footer
            // text may wrap normally without widening every short recipe.
            int preferred = MeasureRows();
            float padding = (Bounds(background).width - Bounds(bubble.widget).width) / scale;
            float right = Screen.width - RightEdge(Bounds(frame)) - gap * 2f;
            // Centered content needs even dimensions: its integer-anchored parchment
            // must not repeatedly resolve half-pixel edges on alternate sides.
            int width = Mathf.Min(((preferred + 1) / 2) * 2,
                Mathf.FloorToInt((right / scale - padding) / 2f) * 2);
            if (width < row.label_descr.fontSize * 4) { Hide(); StudyTableWorkProbe.Write("RAR_GUIDANCE_TOOLTIP status=hidden reason=insufficient_side_space"); return; }
            foreach (var label in bubble.GetComponentsInChildren<UILabel>(true))
            {
                label.leftAnchor.target = null; label.rightAnchor.target = null;
                label.topAnchor.target = null; label.bottomAnchor.target = null;
                label.ResetAnchors();
                string original = label.text;
                expectedStudyMarkers += Count(original, studyMarker);
                if (original.Contains(studyMarker))
                {
                    float minimum = Measure(label, "    " + studyMarker);
                    if (width < minimum) { Hide(); StudyTableWorkProbe.Write("RAR_GUIDANCE_TOOLTIP status=hidden reason=insufficient_marker_space"); return; }
                }
                string formatted = GuidanceTextLayout.Format(original, width, studyMarker,
                    value => Measure(label, value), (value, available) => Wrap(label, value, available));
                label.overflowMethod = UILabel.Overflow.ResizeHeight;
                label.color = Color.white;
                label.width = width;
                label.text = formatted;
                label.ProcessText(false, true);
            }
            bubble.UpdateSizeAndWidgetsPositions();
            UpdateAnchors();
            textLabels = bubble.GetComponentsInChildren<UILabel>(true);
            StudyTableWorkProbe.Write("RAR_GUIDANCE_TOOLTIP product=" + formula.GetFirstRealOutput().id + " text_width=" + width + " language=" + language + " renderer=native independent_list=True");
        }
        private static float Measure(UILabel label, string text)
        {
            // Native text-widget prefab has overflowWidth=150. ResizeFreely still wraps
            // at that ceiling; clear it on this owned label before natural-width sizing.
            label.overflowWidth = 0;
            label.overflowMethod = UILabel.Overflow.ResizeFreely;
            label.text = text;
            label.ProcessText(false, true);
            return label.printedSize.x;
        }
        private static string Wrap(UILabel label, string text, float available)
        {
            label.overflowMethod = UILabel.Overflow.ResizeHeight;
            label.width = Mathf.Max(1, Mathf.FloorToInt(available));
            label.text = text;
            label.ProcessText(false, true);
            return label.processedText;
        }
        private static int MeasureRows()
        {
            float width = 0f;
            foreach (var label in bubble.GetComponentsInChildren<UILabel>(true))
            {
                string original = label.text;
                foreach (string line in original.Split('\n'))
                    if (!line.Contains(studyExplanation) && !line.Contains(Localization.Get(Localization.Key.HowTo)))
                        width = Mathf.Max(width, Measure(label, line));
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
            // NGUI resolves the surrounding anchors in local UI pixels. Keep the
            // owner on that same grid rather than feeding subpixel corrections back.
            Vector3 local = bubble.transform.localPosition;
            local.x = Mathf.Round(local.x); local.y = Mathf.Round(local.y);
            if (bubble.transform.localPosition != local) bubble.transform.localPosition = local;
            var layout = new Vector4(Mathf.Round(x), Mathf.Round(bottom), Mathf.Round(box.width), Mathf.Round(box.height));
            settled = layout == lastLayout ? settled + 1 : 0;
            lastLayout = layout;
            bubble.widget.alpha = settled > 0 ? 1f : 0f;
            if (settled > 0 && !revealed)
            {
                revealed = true;
                StudyTableWorkProbe.Write("RAR_GUIDANCE_TOOLTIP_READY phase=before_geometry anchor=window_lower_right tail=False x=" + layout.x + " y=" + layout.y + " width=" + layout.z + " height=" + layout.w + " exceeds_screen=" + (box.height > Screen.height - gap * 2f));
                int intact = 0;
                foreach (var label in textLabels)
                    if (label != null && label.gameObject.activeInHierarchy) intact += Count(label.processedText, studyMarkerText);
                StudyTableWorkProbe.Write("RAR_GUIDANCE_TEXT language=" + language + " markers=" + expectedStudyMarkers + " rendered_whole=" + intact + " intact=" + (intact == expectedStudyMarkers));
            }
        }
        private static void ObserveGeometry()
        {
            // One bounded research observation per bubble, through the real geometry
            // consumer. A static screenshot cannot establish frame-to-frame stability.
            if (!revealed || observedFrames >= 120) return;
            float x = Bounds(background).xMin;
            if (observedFrames == 0) observedMinX = observedMaxX = x;
            else { observedMinX = Mathf.Min(observedMinX, x); observedMaxX = Mathf.Max(observedMaxX, x); }
            if (++observedFrames == 120)
                StudyTableWorkProbe.Write("RAR_GUIDANCE_MOTION language=" + language + " product=" + formula.GetFirstRealOutput().id +
                    " frames=120 screen_x_range=" + (observedMaxX - observedMinX).ToString("F4", System.Globalization.CultureInfo.InvariantCulture));
        }
        private static int Count(string text, string value) { return (text.Length - text.Replace(value, "").Length) / value.Length; }
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
            textLabels = null; expectedStudyMarkers = 0;
            language = null;
            renderPanel = null;
            studyMarker = null; studyMarkerText = null; studyExplanation = null;
            lastLayout = Vector4.zero; settled = 0;
            observedFrames = 0;
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
            private static void Postfix(UIPanel __instance)
            {
                if (bubble == null || background == null || !ReferenceEquals(__instance, background.panel)) return;
                try { ObserveGeometry(); }
                catch (Exception ex) { Hide(); StudyTableWorkProbe.Fail(ex); }
            }
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
