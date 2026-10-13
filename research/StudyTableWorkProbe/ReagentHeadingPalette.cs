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
                { "alchemy_1_yellow_electro", "504C11" }, { "ash", "4B4955" },
                { "blood", "871D25" }, { "drop_alcohol", "3B4C61" }, { "drop_oil", "5A4813" },
                { "powder_gold", "5D4814" }, { "powder_graphite", "3E4D58" },
                { "powder_silver", "394E5B" }, { "powder_white", "4C4C36" },
                { "taste_booster:salt", "504C35" }, { "water", "17516B" }
            };
            var families = new Dictionary<string, string> {
                { "brown", "6B4217" }, { "d_blue", "1D3287" }, { "d_green", "395235" },
                { "d_violet", "5F1D87" }, { "green", "365412" }, { "red", "87271D" },
                { "violet", "551D87" }, { "yellow", "564A12" }
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
