// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;

namespace NikichMods.Research
{
    // Native wrapping/font measurement are supplied by the owned label. This helper
    // preserves logical row indentation and keeps the localized Study marker atomic.
    internal static class GuidanceTextLayout
    {
        internal static string Format(string text, float width, string marker,
            Func<string, float> measure, Func<string, float, string> wrap)
        {
            var result = new List<string>();
            foreach (string row in text.Split('\n'))
            {
                int offset = 0;
                while (offset < row.Length && row[offset] == ' ') offset++;
                string indent = row.Substring(0, offset), content = row.Substring(offset);
                if (content.Length == 0) { result.Add(row); continue; }
                float capacity = width - measure(indent);
                int at = content.IndexOf(marker, StringComparison.Ordinal);
                var lines = new List<string>();
                if (at < 0) lines.AddRange(wrap(content, capacity).Split('\n'));
                else
                {
                    lines.AddRange(wrap(content.Substring(0, at).TrimEnd(), capacity).Split('\n'));
                    int last = lines.Count - 1;
                    string combined = lines[last] + (lines[last].Length == 0 ? "" : " ") + marker;
                    if (measure(combined) <= capacity) lines[last] = combined;
                    else lines.Add(marker);
                    string suffix = content.Substring(at + marker.Length).TrimStart();
                    if (suffix.Length > 0)
                    {
                        last = lines.Count - 1;
                        combined = lines[last] + " " + suffix;
                        if (measure(combined) <= capacity) lines[last] = combined;
                        else lines.AddRange(wrap(suffix, capacity).Split('\n'));
                    }
                }
                foreach (string line in lines) result.Add(indent + line);
            }
            return string.Join("\n", result.ToArray());
        }
    }
}
