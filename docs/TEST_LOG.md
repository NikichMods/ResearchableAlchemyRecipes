# Diagnostic runtime tests

## Study Table Work Probe 0.1.0 — candidate, 2026-10-12

**Status:** local compile gate passed; installed-runtime test pending. This is not production or accepted behavior.

- Purpose: prove one registered, sample-free native manual job on the study table, temporary table-local mode entry, cancellation observation, midway save/reload and cleanup. Formula disclosure is excluded.
- Branch: `research/study-table-work`.
- Exact compiled source: `6f4fd3bd21e6fee2247c0edfdc372bb1e1f988ef`.
- Build: `research/StudyTableWorkProbe/build.ps1`, handoff mode, SDK 6.0.406 Roslyn, C# 7.3, optimized deterministic library, warnings as errors, no restore. Clean tree and exact HEAD checked before compilation; local proprietary references used only for binding.
- Handoff: `StudyTableWorkProbe-0.1.0.dll`; installed basename: `StudyTableWorkProbe.dll`.
- Assembly version: `0.1.0.0`; BepInPlugin version: `0.1.0` (same code constant).
- DLL SHA-256: `319a0a277c418587b71f0195cede9346ffd27d65774cb8d0c3cbdeef0b550612`.
- Target Assembly-CSharp.dll SHA-256: `e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8`; mismatches disable this save-affecting probe.
- Local binding references: BepInEx assembly 5.4.23.5 and Harmony assembly 2.9.0.0. Full reference hashes are in the ignored build-identity manifest; no dependency binaries are distributed.

Minimum user procedure on a disposable/test save with alchemy unlocked and an existing study table:

1. Install the exact DLL as `BepInEx/plugins/StudyTableWorkProbe.dll`, then launch/load the save. Normal table interaction presents the temporary two-option dialog.
2. Choose ordinary Study once and verify the sample picker still works. Close it.
3. Choose research, start the single job and do a little native work. Close menus and press F8 beside this table; choose cleanup to exercise native cancellation. Collect the returned faith drop if needed. Cancellation consumes 1 station science and spent energy; this is observed native behavior, not adopted economics.
4. Start the job again, perform some work but leave it incomplete, then save through the normal game save path (for example sleep), quit and reload with the probe installed. Return to the table and finish the work. Each start costs 1 faith and 1 science; full completion uses 10 native energy before any native modifiers.
5. Close menus, press F8 beside the table and remove test state. Save after cleanup, then remove the DLL. Unloaded object state is not traversed; keep this test on the single table and disposable save.
6. Return BepInEx LogOutput.log and a short note about mouse/gamepad menu usability. If testing requires multiple launches, preserve each log before the next launch replaces it.

Expected diagnostic evidence: `RAR_WORK_READY`, cache registration `resolved=True`, `RAR_WORK_START sample_free=True output_count=0`, native cancellation, `RAR_WORK_RESTORE resolved=True` for the saved in-progress job, loaded progress observation, `RAR_WORK_COMPLETE native_marker=True formulas=0`, and cleanup with no remaining active job/completed marker. `RAR_WORK_ERROR` blocks acceptance. An absent marker is a missing observation, not a pass.

The pre-alchemy UI gate may be checked on an already available suitable save; replaying Clotho or opening a new playthrough is not required for this first work-lifecycle test. Native source availability, final UI, prices, refund policy and formula-knowledge persistence remain separate open checks.
