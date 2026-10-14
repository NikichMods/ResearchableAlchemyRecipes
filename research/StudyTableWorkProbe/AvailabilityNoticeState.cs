// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;

namespace NikichMods.Research
{
    // Native player Item parameters record only successfully displayed topics.
    // There is no persisted pending list, source history, currency or recipe knowledge here.
    internal static class AvailabilityNoticeState
    {
        private const string Prefix = "rar_notice_seen_";
        private const string Initial = "rar_notice_initial";

        internal static bool IsInitial(Item playerData)
        {
            return playerData != null && playerData.GetParamInt(Initial) != 1;
        }

        internal static List<string> Pending(Item playerData, bool alchemy, IEnumerable<string> currentlyEligible)
        {
            var result = new List<string>();
            if (playerData == null || !alchemy) return result;
            var distinct = new HashSet<string>(StringComparer.Ordinal);
            foreach (string product in currentlyEligible)
                if (!string.IsNullOrEmpty(product) && distinct.Add(product) && playerData.GetParamInt(Prefix + product) != 1)
                    result.Add(product);
            return result;
        }

        internal static void MarkDisplayed(Item playerData, IEnumerable<string> products)
        {
            bool any = false;
            foreach (string product in products)
                if (!string.IsNullOrEmpty(product))
                {
                    playerData.SetParam(Prefix + product, 1f);
                    any = true;
                }
            if (any) playerData.SetParam(Initial, 1f);
        }

        internal static List<string> Page(List<string> pending)
        {
            return pending.GetRange(0, Math.Min(3, pending.Count));
        }

        internal static bool Safe(bool playing, bool starting, bool paused, bool playerControlDisabled,
            bool windowsClosed, bool tooltip, bool speech, bool cornerTalk, bool contextMenu, bool inputHeld)
        {
            return playing && !starting && !paused && !playerControlDisabled && windowsClosed &&
                !tooltip && !speech && !cornerTalk && !contextMenu && !inputHeld;
        }
    }
}
