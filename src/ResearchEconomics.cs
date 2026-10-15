// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;

namespace NikichMods.ResearchableAlchemyRecipes
{
    internal static class ResearchEconomics
    {
        private static readonly Dictionary<string, int> Prices = new Dictionary<string, int> {
            { "yellow_paint", 1 }, { "green_paint", 1 }, { "brown_paint", 1 }, { "red_paint", 1 },
            { "d_green_paint", 1 }, { "d_violet_paint", 1 }, { "violet_paint", 1 }, { "blue_paint", 1 },
            { "black_paint", 2 }, { "white_paint", 2 }, { "acid", 2 }, { "pot_heal", 2 },
            { "glue", 4 }, { "alkali", 4 }, { "energy_elixir", 4 }, { "flask_green_l_plus", 4 },
            { "freshener", 4 }, { "growth_enhancer", 4 }, { "preservative", 4 }, { "zombie_juise", 4 },
            { "pot_appetite", 4 }, { "pot_armor", 4 }, { "pot_berserk", 4 }, { "pot_damage", 4 },
            { "pot_heal_long", 4 }, { "pot_mana", 4 },
            { "aromatizer", 8 }, { "dark_elixir", 8 }, { "flavor_enhancer", 8 },
            { "tanning_agent", 8 }, { "pot_speed", 8 },
            { "silver_elixir", 10 }, { "golden_elixir", 10 }
        };
        internal static int Science(string product)
        {
            int price;
            if (!Prices.TryGetValue(product, out price)) throw new InvalidOperationException("Unknown research price: " + product);
            return price;
        }
        internal static int Work(int science)
        {
            switch (science)
            {
                case 1: case 2: return 2;
                case 4: return 3;
                case 8: return 5;
                case 10: return 8;
                default: throw new InvalidOperationException("Unknown research work group: " + science);
            }
        }
        internal static int Energy(int science) { return (int)Math.Round(2 + 13 * (science - 1) / 9.0); }
        internal static void Apply(CraftDefinition job, string product)
        {
            int price = Science(product);
            job.needs = new List<Item>();
            job.needs_from_wgo = new List<Item> { new Item("science", price) };
            job.craft_time = new SmartExpression { default_value = Work(price) };
            job.energy = new SmartExpression { default_value = Energy(price) };
        }
    }
}
