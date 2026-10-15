// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;

namespace NikichMods.ResearchableAlchemyRecipes
{
    internal static class ResearchEconomics
    {
        private struct Cost
        {
            internal readonly int science, work, energy;
            internal Cost(int science, int work, int energy)
            { this.science = science; this.work = work; this.energy = energy; }
        }
        // Science is independent of the accepted short work/energy budgets.
        private static readonly Cost Decorative = new Cost(1, 2, 2);
        private static readonly Cost Early = new Cost(2, 2, 3);
        private static readonly Cost White = new Cost(4, 2, 3);
        private static readonly Cost Acid = new Cost(6, 2, 3);
        private static readonly Cost Practical = new Cost(8, 3, 6);
        private static readonly Cost Advanced = new Cost(10, 5, 12);
        private static readonly Cost Premium = new Cost(14, 8, 15);
        private static readonly Dictionary<string, Cost> Costs = new Dictionary<string, Cost> {
            { "yellow_paint", Decorative }, { "green_paint", Decorative }, { "brown_paint", Decorative }, { "red_paint", Decorative },
            { "d_green_paint", Decorative }, { "d_violet_paint", Decorative }, { "violet_paint", Decorative }, { "blue_paint", Decorative },
            { "black_paint", Early }, { "white_paint", White }, { "acid", Acid }, { "pot_heal", Early },
            { "glue", Practical }, { "alkali", Practical }, { "energy_elixir", Practical }, { "flask_green_l_plus", Practical },
            { "freshener", Practical }, { "growth_enhancer", Practical }, { "preservative", Practical }, { "zombie_juise", Practical },
            { "pot_appetite", Practical }, { "pot_armor", Practical }, { "pot_berserk", Practical }, { "pot_damage", Practical },
            { "pot_heal_long", Practical }, { "pot_mana", Practical },
            { "aromatizer", Advanced }, { "dark_elixir", Advanced }, { "flavor_enhancer", Advanced },
            { "tanning_agent", Advanced }, { "pot_speed", Advanced },
            { "silver_elixir", Premium }, { "golden_elixir", Premium }
        };
        private static Cost Get(string product)
        {
            Cost cost;
            if (!Costs.TryGetValue(product, out cost)) throw new InvalidOperationException("Unknown research cost: " + product);
            return cost;
        }
        internal static int Science(string product) { return Get(product).science; }
        internal static void Apply(CraftDefinition job, string product)
        {
            var cost = Get(product);
            job.needs = new List<Item>();
            job.needs_from_wgo = new List<Item> { new Item("science", cost.science) };
            job.craft_time = new SmartExpression { default_value = cost.work };
            job.energy = new SmartExpression { default_value = cost.energy };
        }
    }
}
