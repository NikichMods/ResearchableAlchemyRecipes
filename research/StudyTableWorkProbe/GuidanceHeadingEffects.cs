// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NikichMods.Research
{
    // Native text widgets give individual headings their own optional effect.
    internal static class GuidanceHeadingEffects
    {
        private static readonly Dictionary<string, string> light = new Dictionary<string, string> {
            { "alchemy_1_yellow", "F4D863" }, { "alchemy_2_yellow", "F4D863" },
            { "alchemy_3_yellow", "F4D863" }, { "powder_white", "FFF9E8" },
            { "taste_booster:salt", "EFE7CF" }, { "powder_silver", "D4E5EE" },
            { "powder_gold", "EFC35F" }, { "drop_oil", "F5DA77" }
        };
        internal sealed class Part
        {
            internal string text, outlined;
        }
        internal static List<Part> Split(string text, CraftDefinition formula, Func<string, string> name)
        {
            var headings = new Dictionary<string, string>();
            foreach (var ingredient in formula.needs)
            {
                string colour;
                if (light.TryGetValue(ingredient.id, out colour))
                    headings[ReagentHeadingPalette.Heading(ingredient.id, name(ingredient.id))] =
                        "[c][" + colour + "]" + name(ingredient.id) + ":[-][/c]";
            }
            const string body = "[4B3422]";
            // BuildTextCore owns exactly one outer body-colour scope.
            if (!text.StartsWith(body, StringComparison.Ordinal) || !text.EndsWith("[-]", StringComparison.Ordinal))
                return new List<Part> { new Part { text = text } };
            text = text.Substring(body.Length, text.Length - body.Length - 3);
            var result = new List<Part>();
            var rows = new List<string>();
            foreach (string row in text.Split('\n'))
            {
                string outlined;
                if (!headings.TryGetValue(row, out outlined)) { rows.Add(row); continue; }
                Flush(result, rows);
                result.Add(new Part { text = body + row + "[-]", outlined = body + outlined + "[-]" });
            }
            Flush(result, rows);
            return result;
        }
        private static void Flush(List<Part> parts, List<string> rows)
        {
            if (rows.Count == 0) return;
            parts.Add(new Part { text = "[4B3422]" + string.Join("\n", rows.ToArray()) + "[-]" });
            rows.Clear();
        }
        internal static bool SupportsOutline(bool fontAvailable, bool packedBitmap)
        { return fontAvailable && !packedBitmap; }

        internal static void Apply(UILabel label, string outlined)
        {
            string fallback = label.text;
            label.effectStyle = UILabel.Effect.None;
            try
            {
                var bitmap = label.bitmapFont;
                // All inspected native language fonts are UIFont bitmaps. An unknown
                // font path keeps B until its effect behavior is separately verified.
                bool supported = SupportsOutline(bitmap != null,
                    bitmap != null && bitmap.packedFontShader);
                if (!supported)
                {
                    StudyTableWorkProbe.Write("RAR_GUIDANCE_OUTLINE mode=palette_B reason=font_capability language=" + GJL.GetCurLng());
                    return;
                }
                label.effectColor = new Color32(0x4B, 0x34, 0x22, 255);
                label.effectDistance = new Vector2(0.5f, 0.5f);
                label.effectStyle = UILabel.Effect.Outline;
                label.text = outlined;
                StudyTableWorkProbe.Write("RAR_GUIDANCE_OUTLINE mode=outlined language=" + GJL.GetCurLng() + " font=" + bitmap.name);
            }
            catch (Exception ex)
            {
                label.effectStyle = UILabel.Effect.None;
                label.text = fallback;
                StudyTableWorkProbe.Write("RAR_GUIDANCE_OUTLINE mode=palette_B reason=" + ex.GetType().Name);
            }
        }
    }
}
