// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;

namespace NikichMods.ResearchableAlchemyRecipes
{
    internal static class Localization
    {
        internal enum Key
        {
            BlueprintLocked, Close, ClothoAssortment, Cremation, ExtraCondition, Extraction, HowTo, ItemUse, NewRecipes, Next, NoticeHowTo, NoticeMore, NoticeTitle, OtherMethods, Purchase, PurchaseUnavailable, RecipeLocked, Requires, ResearchCompleted, StudyExplanation, UnknownSource, Unstudied
        }
        private static readonly Dictionary<string, Dictionary<Key, string>> languages = Read();
        private static readonly FieldInfo CurrentLanguage = AccessTools.Field(typeof(GJL), "cur_lng");

        internal static string Get(Key key)
        {
            var current = (GJL)CurrentLanguage.GetValue(null);
            string value = Get(key, ReferenceEquals(current, null) ? "en" : current.id);
            return value;
        }
        internal static string Get(Key key, string language)
        {
            Dictionary<Key, string> values;
            if (language == null || !languages.TryGetValue(language, out values)) values = languages["en"];
            return values[key];
        }
        private static Dictionary<string, Dictionary<Key, string>> Read()
        {
            var result = new Dictionary<string, Dictionary<Key, string>>(StringComparer.Ordinal);
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RAR.Localization.tsv"))
            {
                if (stream == null) throw new InvalidOperationException("Missing localization resource");
                using (var reader = new StreamReader(stream, System.Text.Encoding.UTF8))
                {
                    string line;
                    do { line = reader.ReadLine(); } while (line != null && line.StartsWith("#", StringComparison.Ordinal));
                    if (line == null) throw new InvalidOperationException("Missing localization header");
                    string[] header = line.Split('\t');
                    for (int i = 1; i < header.Length; i++) result.Add(header[i], new Dictionary<Key, string>());
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal)) continue;
                        string[] cells = line.Split('\t');
                        if (cells.Length != header.Length) throw new InvalidOperationException("Localization column mismatch");
                        Key key = (Key)Enum.Parse(typeof(Key), cells[0]);
                        for (int i = 1; i < cells.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(cells[i])) throw new InvalidOperationException("Empty translation");
                            result[header[i]].Add(key, cells[i]);
                        }
                    }
                }
            }
            foreach (string locale in GJL.LANGUAGES)
                if (!result.ContainsKey(locale) || result[locale].Count != Enum.GetValues(typeof(Key)).Length)
                    throw new InvalidOperationException("Incomplete locale: " + locale);
            return result;
        }

    }
}
