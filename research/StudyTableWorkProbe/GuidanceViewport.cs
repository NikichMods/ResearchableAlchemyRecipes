// SPDX-License-Identifier: MPL-2.0
using UnityEngine;

namespace NikichMods.Research
{
    // Resize the one native owner; frame, clipping panel and tips retain their anchors.
    internal static class GuidanceViewport
    {
        private static UIWidget window;
        private static Vector3 position;
        private static int height;
        internal static void Enlarge(CraftGUI gui)
        {
            Restore();
            var transform = gui.transform.Find("window");
            var owner = transform == null ? null : transform.GetComponent<UIWidget>();
            var back = transform == null ? null : transform.Find("back");
            var frame = back == null ? null : back.GetComponent<UIWidget>();
            var tips = gui.GetButtonTips().GetComponent<UIWidget>();
            var camera = NGUITools.FindCameraForLayer(gui.gameObject.layer);
            if (owner == null || frame == null || tips == null || camera == null || owner.pivot != UIWidget.Pivot.Center ||
                owner.topAnchor.target != null || owner.bottomAnchor.target != null ||
                frame.topAnchor.target != owner.transform || frame.bottomAnchor.target != owner.transform)
            { StudyTableWorkProbe.Write("RAR_GUIDANCE_VIEWPORT status=unchanged reason=unsupported_native_owner"); return; }
            gui.transform.BroadcastMessage("UpdateAnchors", SendMessageOptions.DontRequireReceiver);
            var parent = owner.transform.parent;
            float low = float.PositiveInfinity, high = float.NegativeInfinity;
            foreach (var widget in new[] { frame, tips }) foreach (var corner in widget.worldCorners)
            {
                float y = parent.InverseTransformPoint(corner).y;
                low = Mathf.Min(low, y); high = Mathf.Max(high, y);
            }
            float depth = camera.WorldToScreenPoint(owner.transform.position).z;
            Vector3 bottom = parent.InverseTransformPoint(camera.ViewportToWorldPoint(new Vector3(0.5f, 0.1f, depth)));
            Vector3 top = parent.InverseTransformPoint(camera.ViewportToWorldPoint(new Vector3(0.5f, 0.9f, depth)));
            int growth = Mathf.FloorToInt((top.y - bottom.y - (high - low)) / owner.transform.localScale.y);
            if (growth <= 0) return;
            window = owner; height = owner.height; position = owner.transform.localPosition;
            owner.height += growth;
            var adjusted = position;
            adjusted.y += (top.y + bottom.y - high - low) * 0.5f;
            owner.transform.localPosition = adjusted;
            gui.transform.BroadcastMessage("UpdateAnchors", SendMessageOptions.DontRequireReceiver);
            gui.scroll_view.InvalidateBounds();
            StudyTableWorkProbe.Write("RAR_GUIDANCE_VIEWPORT status=enlarged old_height=" + height + " height=" + owner.height + " screen_fraction=0.8 owner=native_window");
        }
        internal static void Restore()
        {
            if (window == null) return;
            var owner = window; window = null;
            owner.height = height; owner.transform.localPosition = position;
            owner.transform.parent.BroadcastMessage("UpdateAnchors", SendMessageOptions.DontRequireReceiver);
        }
    }
}
