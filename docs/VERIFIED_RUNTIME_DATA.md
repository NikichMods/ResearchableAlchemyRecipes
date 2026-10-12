# Verified host evidence

This document records reusable evidence pointers and limits, not new production readiness claims. The canonical shared owner is [NikichMods/GraveyardKeeperResearch](https://github.com/NikichMods/GraveyardKeeperResearch), starting with its AGENTS.md and docs/RESEARCH_INDEX.md.

## Host identity

Accepted 1.407 research input identities:

- resources.assets SHA-256: 215c7981901a4b72d5db717666ba47ad3cc032527c95f58dc39d8af1293a69ca
- Assembly-CSharp.dll SHA-256: e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8

This project has installed research-probe evidence, recorded in TEST_LOG.md, but no production acceptance. Host evidence does not by itself prove the journal or formula-reward integration.

## Alchemy knowledge and sources

Owner: [ALCHEMY_SYSTEM.md](https://github.com/NikichMods/GraveyardKeeperResearch/blob/main/docs/ALCHEMY_SYSTEM.md).

- Ordinary alchemy uses positional mixed crafting, not a plain fixed-recipe card path.
- Native completed-craft and scripted UnlockAlchemy paths provide formula-knowledge state; actual GUI presentation and the research-triggered lifecycle still need verification for this consumer.
- Decomposition visibility depends on native material Survey state. Decomposition alone does not cover every reagent production method.
- There are 43 ordinary picker-compatible formulas for 34 outputs and 35 ingredients. Some products have alternative formulas. One additional success-classified definition is outside the ordinary picker contract; its role remains open.
- Direct static 1.407 intersection verification found zero overlap between successful mixed-product outputs and mixed ingredients, including both cross-tier directions and the picker exception. Exposing reagent acquisition does not disclose another mixed formula through a nested mixed-product dependency in this corpus. Other uses and mod-altered balance are outside that conclusion.
- The intersection result is preserved by [commit 8d6be7a](https://github.com/NikichMods/GraveyardKeeperResearch/commit/8d6be7ab6f0e0a1b31d5b963a46e15de35d3a5e3).

## Prior awareness/progression research

Accepted evidence may be reused from AlchemyRiddle; its former product decisions are not requirements here:

- [RUNTIME_CORPUS_ANALYSIS.md](https://github.com/NikichMods/AlchemyRiddle/blob/38feb900ed6c7151691aeccc129ab65798046478/docs/RUNTIME_CORPUS_ANALYSIS.md): structural/vendor candidate channels cover 26/34 ordinary products; 8 remain outside those measured channels. Seven of these have authored blueprint consumers whose actual visibility/unlock path was not established by that pass; one lacks measured ordinary consumer/vendor channels. This is channel-existence evidence, not a complete save-specific first-exposure proof.
- [PROGRESSION_RESEARCH.md](https://github.com/NikichMods/AlchemyRiddle/blob/38feb900ed6c7151691aeccc129ab65798046478/docs/PROGRESSION_RESEARCH.md): accepted bounded progression captures establish representative paths and distinguish visible need from production capability. Read accepted later sections rather than treating earlier pending notes as current conclusions.

## Shared native UI/craft references

- [CRAFTING_INVENTORY_AND_TRADING.md](https://github.com/NikichMods/GraveyardKeeperResearch/blob/main/docs/CRAFTING_INVENTORY_AND_TRADING.md): native craft/Survey rewards, current-save/world initialization, inventory boundaries and native item-tooltip ownership.
- [DIALOGUE_QUEST_AND_FLOWCANVAS.md](https://github.com/NikichMods/GraveyardKeeperResearch/blob/main/docs/DIALOGUE_QUEST_AND_FLOWCANVAS.md): authored dialogue/task availability and parent-route gates.
- [UI_INPUT_TIME_AND_ENVIRONMENT.md](https://github.com/NikichMods/GraveyardKeeperResearch/blob/main/docs/UI_INPUT_TIME_AND_ENVIRONMENT.md): applicable UI/gamepad/localization lifecycle findings; do not generalize one audited tooltip family to every new journal widget.

No concrete vanilla formulas, copied binaries or game assets are included in this repository.

## Mechanism verification — 2026-10-12

Read-only first pass; both local input hashes above were rechecked and match. Static source is the shared pinned `Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9`. No game/save mutation, new runtime probe or production code.

- Shared [crafting evidence](https://github.com/NikichMods/GraveyardKeeperResearch/blob/main/docs/CRAFTING_INVENTORY_AND_TRADING.md) now records the sample-dependent ResourceBasedCraftGUI/Survey completion contract, common CraftComponent work ownership and the requirement to register a stable GameBalance craft ID before save restoration. Merely removing the sample from Survey inputs is insufficient.
- Shared [alchemy evidence](https://github.com/NikichMods/GraveyardKeeperResearch/blob/main/docs/ALCHEMY_SYSTEM.md) now classifies the seven paint gaps: 26 unlocked-by-state keeper-room blueprint consumers, explicit old-save migration unlock writers, and no direct exact-record Technology mapping. Builder availability and normal new-save writers are still open.
- The accepted Merchant/Spices task source was recovered from prior progression research. Its current dialogue availability remains unverified; no full save-specific coverage claim is made.

The second static pass establishes the following (details and graph hash are in the shared owners above):

- Definition insertion before GameBalanceBase.CreateIDsCache, scoped to GameBalance and idempotent by ID, precedes native ID/station cache construction. `AddData` after cache creation is insufficient without cache maintenance. Stable definitions must exist even when a topic is not currently offered, for saved-job restoration.
- GUIElements.OpenCraftGUI routes the study table to its Survey sample picker; appending ordinary jobs does not create a research tab/entry. A narrow table-local entry is required. Hidden jobs also cannot be used as an eligibility shortcut because native manual DoAction rejects them.
- The real job's completion reaches GameSave.OnFinishedCraft before CraftComponent.End. Formula disclosure uses native completed_one_time_crafts as established by UnlockAlchemy; fabricated manufacturing completion would carry unrelated quest/achievement side effects.
- Native cancellation refunds item needs but does not symmetrically refund station needs_from_wgo science. No research refund rule has been selected.
- The normal tech_alchemy_begins graph opens 22 of the 26 paint-consumer blueprints, covering all seven paint outputs. These sources are not proved to be available from game start. Actual unlocked_techs state supplies the adopted journal/research gate; a new perk is not technically required.

Next evidence must verify the table-local entry, installed registration/load/resumption and formula-knowledge reload; separately establish current keeper-room builder and Merchant-source availability. Static control flow is not installed-runtime acceptance.

## Work-lifecycle runtime candidate — 2026-10-12

Study Table Work Probe 0.1.0 passed its local compile/identity gate and the bounded installed work-lifecycle check; see TEST_LOG.md for exact source, binary/log hashes and observed values. Registration, sample-free start/payment, native work/cancellation, save/menu-load restoration, completion and cleanup are confirmed with the user's installed mod set. Full process restart, mouse control and pre-alchemy gate are not runtime-proved by this capture. The probe did not assign progress/completion or disclose formulas. Its A/B dialog is not the desired final UI. This evidence does not close the journal/two-tab UI, current-source eligibility, pricing/refunds or formula reward gates.

## Two-tab UI candidate — 2026-10-12

Static source plus direct level2 scene inspection establishes native CraftTabGUI layout, CraftGUI tab ownership and MixedCraftGUI's separate top-of-stack subwindow model. BaseGUI.Update handles semantic keys only for the shown top window; opening another native window disables the previous window's controller. ResourceBasedCraftGUI remains the real Survey owner in the research-only 0.1.1 composition. Exact scene hash, references, mechanism and residual uncertainty are documented in research/StudyTableWorkProbe/README.md.

0.1.1 passed the compile/identity gate and now has installed user evidence for keyboard/mouse/gamepad switching, native picker selection/cancel, closing/reopening, short work and F8 cleanup. The exact installed DLL matches the handed hash. Ten opens, 28 switches and ten paired closes/layout restorations are recorded. This is bounded usability evidence, not UI acceptance: the log also contains eight BaseGUI.Update dictionary enumeration exceptions after closing, 20 missing tab_rar_* sprite errors, and six NGUI layer warnings around a picker opening.

Direct pinned BaseGUI source identifies live gamekey_delegates.Keys enumeration; the probe restores values during its callback, invalidating that enumerator. 0.1.2 installs a cloned map and restores the field reference instead. A deterministic .NET Framework regression reproduces the original defect and completes the corrected enumeration with the original map restored. The returned 0.1.2 run records eleven paired restores/closes and zero enumeration or missing-tab-sprite errors; user observations confirm both input modes and native icon appearance. Verified native tab_book / tab_custom_alch assets supply the icon-only candidate without copied artwork or caption widgets. Their selection tint remains owned by native CraftTabGUI.SetSelectedState. Formula reward is unchanged/excluded, and the layer-warning cause remains open.

Returned 0.1.2 limits and log identity are in TEST_LOG.md. Thirty picker-related NGUI layer warnings remain with unassigned cause. The user accepts the native icon style but requests alternative semantic artwork. The private contact-sheet inventory identifies 49 tab_* sprites in the verified resources.assets; presence alone does not prove use/nonuse. Neither production UI acceptance nor general mod compatibility is inferred.

## Original tab-art integration candidate

Research-only 0.1.3 embeds the user-selected original Material Study and Recipe Research PNGs, preserving the complete RGBA 40x26 canvases and byte identities recorded in research/StudyTableWorkProbe/assets/README.md. Native CraftTabGUI.Draw/SetSelectedState remain the sprite/tint owners; a table-local postfix changes only sprite and widget dimensions. Direct level2 inspection records the native icon widget as Center (4), 54x32, Simple (0), pixel-size 1. Startup creates two centered FullRect sprites with Point sampling; no global atlas edits. Local compile and exact-binary embedded-resource checks passed, but actual rendering/tint/layout acceptance is pending; exact candidate identity is in TEST_LOG.md. Existing picker-layer warnings remain independently unresolved.
