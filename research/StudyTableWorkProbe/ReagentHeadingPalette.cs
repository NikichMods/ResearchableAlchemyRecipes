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
                { "alchemy_1_yellow_electro", "E8E39B" }, { "ash", "CECDD4" },
                { "blood", "E0A0A5" }, { "drop_alcohol", "C3CEDB" }, { "drop_oil", "E0CB8C" },
                { "powder_gold", "E5CA88" }, { "powder_graphite", "A8B6C2" },
                { "powder_silver", "CCD8DF" }, { "powder_white", "E5E5DA" },
                { "taste_booster:salt", "D8D5C4" }, { "water", "A6CFE2" }
            };
            var families = new Dictionary<string, string> {
                { "brown", "DAB792" }, { "d_blue", "A8B4E3" }, { "d_green", "9FBF9A" },
                { "d_violet", "BDA0CD" }, { "green", "B2D091" }, { "red", "E3A39C" },
                { "violet", "D0B4E8" }, { "yellow", "E6D78D" }
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
