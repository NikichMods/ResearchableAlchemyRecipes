// SPDX-License-Identifier: MPL-2.0
using System.Collections.Generic;

namespace NikichMods.Research
{
    // Reviewed 1.407 participant IDs; no dependence on translated names or other mods.
    internal static class ReagentHeadingPalette
    {
        private static readonly Dictionary<string, string> colours = Create();
        private static Dictionary<string, string> Create()
        {
            var result = new Dictionary<string, string> {
                { "alchemy_1_yellow_electro", "403D0F" }, { "ash", "3B3A43" },
                { "blood", "601F24" }, { "drop_alcohol", "2E3C4C" }, { "drop_oil", "463913" },
                { "powder_gold", "493910" }, { "powder_graphite", "313D46" },
                { "powder_silver", "2D3E48" }, { "powder_white", "3C3C2A" },
                { "taste_booster:salt", "3F3C2A" }, { "water", "1A4051" }
            };
            var families = new Dictionary<string, string> {
                { "brown", "50361B" }, { "d_blue", "1F2C60" }, { "d_green", "2C4029" },
                { "d_violet", "452C54" }, { "green", "2F401B" }, { "red", "63231C" },
                { "violet", "421E62" }, { "yellow", "433A0F" }
            };
            foreach (var family in families)
                for (int type = 1; type <= 3; type++) result.Add("alchemy_" + type + "_" + family.Key, family.Value);
            return result;
        }
        internal static string Heading(string id, string localized)
        {
            string hex;
            return colours.TryGetValue(id, out hex) ? "[c][" + hex + "]" + localized + ":[-][/c]" : localized + ":";
        }
    }
}
