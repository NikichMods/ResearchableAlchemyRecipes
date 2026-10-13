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
                { "alchemy_1_yellow_electro", "7B751B" }, { "ash", "686375" },
                { "blood", "B12A39" }, { "drop_alcohol", "486B92" }, { "drop_oil", "98721C" },
                { "powder_gold", "966B1E" }, { "powder_graphite", "536C80" },
                { "powder_silver", "547890" }, { "powder_white", "777561" },
                { "taste_booster:salt", "77704E" }, { "water", "21799E" },
                { "memory_powder", "955E36" }
            };
            var families = new Dictionary<string, string> {
                { "brown", "905322" }, { "d_blue", "3049AE" }, { "d_green", "477442" },
                { "d_violet", "7A2AA5" }, { "green", "507B20" }, { "red", "AA342A" },
                { "violet", "7729A3" }, { "yellow", "827019" }
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
