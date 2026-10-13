// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

public static class CardObserverTests
{
    private const BindingFlags Private = BindingFlags.NonPublic | BindingFlags.Static;
    private static void Assert(bool condition, string message) { if (!condition) throw new Exception(message); }
    public static int Main(string[] args)
    {
        AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e) {
            foreach (string directory in args) { string path = Path.Combine(directory, new AssemblyName(e.Name).Name + ".dll"); if (File.Exists(path)) return Assembly.LoadFrom(path); }
            return null;
        };
        try { Run(args[0]); Console.WriteLine("PASS source observer: native field bindings, grouped card variants, duplicate isolation and equal-count ID mismatch"); return 0; }
        catch (Exception ex) { Console.WriteLine(ex); return 1; }
    }
    private static void Run(string directory)
    {
        var type = Assembly.LoadFrom(Path.Combine(directory, "compile-check.dll")).GetType("NikichMods.Research.SourceEligibilityProbe", true);
        type.GetMethod("ValidateBindings", Private).Invoke(null, null);
        var item = (Item)FormatterServices.GetUninitializedObject(typeof(Item)); item.id = "fixture_product";
        var first = new CraftDefinition { id = "source_a", needs = new List<Item> { item } };
        var second = new CraftDefinition { id = "source_b", needs = new List<Item> { item } };
        var unrelated = new CraftDefinition { id = "unrelated", needs = new List<Item>() };
        var cell = (CraftItemGUI)FormatterServices.GetUninitializedObject(typeof(CraftItemGUI));
        cell.craft_definition = first;
        typeof(CraftItemGUI).GetField("_possible_crafts", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(cell, new List<CraftDefinition> { first, second, unrelated });
        var rendered = (HashSet<string>)type.GetMethod("RenderedIds", Private).Invoke(null, new object[] { new[] { cell, cell }, "fixture_product" });
        Assert(rendered.SetEquals(new[] { "source_a", "source_b" }), "Observer missed nonselected variant, duplicated a card or counted an unrelated recipe");
        var expected = (HashSet<string>)type.GetMethod("SourceIds", Private).Invoke(null, new object[] { new[] { first, second }, "fixture_product" });
        var incorrect = new HashSet<string> { "source_a", "wrong_source" };
        Assert(expected.Count == incorrect.Count && !expected.SetEquals(incorrect), "Equal counts were treated as source agreement");
        Assert(cell.craft_definition == first && first.needs.Count == 1 && second.needs.Count == 1, "Observation mutated native/card inputs");
        var stock = type.GetMethod("StockCount", Private);
        var stocked = (Item)FormatterServices.GetUninitializedObject(typeof(Item)); stocked.id = "fixture_product"; stocked.value = 3;
        var depleted = (Item)FormatterServices.GetUninitializedObject(typeof(Item)); depleted.id = "fixture_product"; depleted.value = 0;
        Assert((int)stock.Invoke(null, new object[] { new[] { stocked, depleted, null }, "fixture_product" }) == 3, "Stock observer counts empty/null entries incorrectly");
        Assert((int)stock.Invoke(null, new object[] { new[] { stocked }, "other_product" }) == 0 && stocked.value == 3, "Stock observation changes inventory or counts unrelated goods");
    }
}
