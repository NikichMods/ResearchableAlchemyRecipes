// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;

namespace NikichMods.Research
{
    internal static class Localization
    {
        internal enum Key
        {
            Purchase, ItemUse, Extraction, Cremation, PurchaseUnavailable, RecipeLocked,
            BlueprintLocked, Requires, FormulaProbeName, FormulaProbeTitle, Actions, Close,
            FormulaProbeWarning, SavePersistent, PrepareCase, Cleanup, SaveBeforeRemoving,
            InitialKnowledge, NoneKnown, OneKnown, PreparationWarning, CleanupFirst, Prepared,
            Unstudied, StudyExplanation, HowTo, UnknownSource, ExtraCondition, StudyPreviewTitle,
            NormalView, SulfurPreview, PreviewReady, PreviewWarning, PaintProbeTitle,
            PaintProbeWarning, ProbeActions, CleanupWorks, ProbeActionsWarning, Disabled,
            CleanupAvailable, CleanupDone, SampleFreeProbeName
        }
        private static readonly Dictionary<string, Dictionary<Key, string>> languages = Read();
        private static readonly FieldInfo CurrentLanguage = AccessTools.Field(typeof(GJL), "cur_lng");
        internal static string Id(Key key) { return "nikichmods.rar.ui." + key; }
        internal static string Get(Key key)
        {
            var current = (GJL)CurrentLanguage.GetValue(null);
            string value = Get(key, ReferenceEquals(current, null) ? "en" : current.id);
            if (key == Key.FormulaProbeName || key == Key.FormulaProbeWarning)
            {
                string item = FormulaKnowledge.Product, translated;
                if (!ReferenceEquals(current, null) && current.dict != null && current.dict.TryGetValue(item, out translated)) item = translated;
                value = value.Replace("{0}", item);
            }
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
        internal static void Inject()
        {
            var current = (GJL)CurrentLanguage.GetValue(null);
            if (ReferenceEquals(current, null) || current.dict == null) return;
            foreach (Key key in Enum.GetValues(typeof(Key))) current.dict[Id(key)] = Get(key);
            StudyTableWorkProbe.Write("RAR_LOCALIZATION_READY language=" + current.id + " keys=" + Enum.GetValues(typeof(Key)).Length + " locales=" + languages.Count);
        }
        [HarmonyPatch(typeof(GJL), "LoadLanguageResource", new[] { typeof(string) })]
        private static class LanguageLoaded
        {
            private static void Postfix()
            {
                try { Inject(); }
                catch (Exception ex) { StudyTableWorkProbe.Write("RAR_LOCALIZATION_INJECT error=" + ex.GetType().Name); }
            }
        }
    }
}
