# Diagnostic runtime tests

## Study Table Work Probe 0.1.1 — two-tab candidate, 2026-10-12

**Status:** local compile/identity gate passed; installed UI/layout/input test pending. No production UI acceptance.

- Exact compiled source: `d72441d25ed868ac069180077d5e38f73fb8dba1`, research/study-table-work.
- Artifact: `StudyTableWorkProbe-0.1.1.dll`; replace the previous installed copy as `StudyTableWorkProbe.dll`, do not install side by side.
- SHA-256: `f7772345f6195073030872fbdfa6c2e3279d07cbedb15bfc92148f557f2fda31`.
- Assembly/file versions: `0.1.1.0`; plugin/canonical source version: `0.1.1`. Build script derives artifact/manifest version from the code constant and asserts both binary versions.
- Build: same local SDK 6.0.406 Roslyn and verified host/BepInEx references as 0.1.0; clean exact source, optimized deterministic output, warnings as errors. No proprietary inputs distributed.
- Question: native two-tab composition, controller/mouse switching, material-picker return, paired close and layout restoration. The established work lifecycle is baseline, not a reason to repeat save/reload.
- Changed diagnostic input: manual work time 30 -> 2 units. Other diagnostic costs stay 1 faith/1 science/10 energy; no formula disclosure or product balance decision.
- Setup modifies only session UI layout and next/previous-tab delegates, restored on close. Starting the existing test job still creates save-persistent native craft state; use the test save and F8 cleanup after any job.

Minimum user procedure:

1. Replace 0.1.0 with this DLL, launch and open the study table on the existing suitable test save. There should be one common craft frame with Изучение / Рецепты tabs and no A/B entry dialog.
2. Switch a few times with LB/RB. On Изучение open the real material picker, select an item or cancel, then switch again. If using a mouse, test tab clicks and return from the picker.
3. Close from each tab and reopen. Check that ordinary Study remains usable and an unrelated workbench's tabs remain normal. Provide a screenshot of each tab and note overlap, clipping or lost focus.
4. Optionally start the short test job to verify that the entire composed window closes on launch. A normal material Study can be performed if convenient. If a test job was started/completed, close menus, use F8 cleanup beside the table and save as in 0.1.0. No mid-work save/reload repetition is required.
5. Return LogOutput.log. Expected markers are RAR_TABS_OPEN, RAR_TABS_SWITCH, RAR_TABS_RESTORE layout_restored=True and RAR_TABS_CLOSED outer=False study=False. RAR_WORK_ERROR or RAR_TABS_RESTORE_ERROR blocks acceptance. Visual/controller outcomes still require user observations even when log state looks correct.

Nothing in the build result establishes correct layout/focus in the installed game. Formula reward, final text/art, source eligibility and refund economics remain separate gates. Numbered 0.1.0 and 0.1.1 artifacts are immutable.

## Study Table Work Probe 0.1.0 — work lifecycle verified, 2026-10-12

**Status:** local compile gate passed; bounded installed-runtime work lifecycle confirmed by the returned log and user observations. This is not production/final-UI acceptance.

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

### Returned evidence

Returned LogOutput.log snapshot SHA-256: `c21fcb3a4c23589b85ac25b1ccddff00b9c72f1f7cf0280daf8f93f343f8c732`. The locally installed probe DLL hash matched the exact handed artifact above. Raw log remains in ignored local evidence storage.

- Native ID-cache registration resolves successfully; gate is unlocked; native craft cards open.
- First start: `sample_free=True`, `output_count=0`, faith 6 -> 5, station science 21 -> 20. Observer did not assign progress/completion.
- Before cancellation: table 3471 progress 0.268, energy 97.323. Native cancel leaves no active job and science remains 20. User saw the faith return; the next start independently records faith back at 6. Immediate post-cancel inventory still reads 5 because refund is a world drop.
- Second start: faith 6 -> 5, science 20 -> 19. After saving and returning to the menu, the native deserializer reports `saved_active=True resolved=True active=True`. User reports the partial progress visually preserved and then finishes the work. This establishes same-process save/menu-load resumption; a full process restart was not reported.
- Native completion records `native_marker=True formulas=0`. Final cleanup removes one completed marker and one last-craft reference, leaving `active=0 completed=False`; a subsequent SaveGame/serialization block is present.
- No `RAR_WORK_ERROR` occurs. `RAR_WORK_LOADED` is absent: this loading route reaches the native craft deserializer but not the probed MainGame.OnGameLoaded postfix. There is no post-load numeric progress sample; the user's visual observation supplies that part of the evidence.
- Ordinary material Study was completed; screenshot shows the native completion dialog and decomposition-form hint. The log also contains `Can't give back a craft item because it is null` at ordinary Survey completion before the first diagnostic job. Its cause is not established; do not characterize the entire modded session as error-free or attribute this to the research job.

Installed environment includes Alchemy Research Redux 0.1.8, Decomp Delight! 0.1.9 and Save Now! 2.5.14. These are representative coexistence observations, not a clean-vanilla run or general compatibility guarantee. The user tested controller menu actions A/B; mouse input and pre-alchemy access were not independently demonstrated.

UX feedback: the diagnostic work is too long for comfortable play; its 30 work units/10 energy remain test inputs, not adopted balance. Ending without a reward dialog is expected because this probe has no reward/notification. The modal A/B chooser works but is not desired final UX. Preferred next design is two tabs, normal Study and recipe research, with native-style tab navigation; exact integration remains to be verified. Do not rebuild 0.1.0 with changed timing or tabs.
