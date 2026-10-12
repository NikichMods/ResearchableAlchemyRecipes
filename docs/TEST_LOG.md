# Diagnostic runtime tests

## Study Table Work Probe 0.1.4 — visible-art centering candidate

**Status:** compile check passed; exact handoff identity to be recorded after clean-source build. Installed centering acceptance pending.

- Scope: icon placement only; original PNG bytes, 40x26 canvases, size, native tint, work/input/close paths and science header remain unchanged.
- Root cause: 0.1.3 centered complete PNGs. Material opaque bounds (9,3)-(37,25) center at (23,14) inside a (20,13) canvas center; recipe (5,2)-(36,25) centers at (20.5,13.5). PNG Y is inverted relative to UI Y.
- Corrected absolute offsets from the native tab-background center: Material Study x=-3/y=+1; Recipe Research x=-0.5/y=+0.5 UI units. Exact bounds centering; not a claim of subjective optical acceptance.
- Direct scene evidence: back/icon share parent and Center pivot; back 66x32, icon unanchored. Postfix validates same-parent/center-pivot contract, preserves icon Z and uses absolute placement. Missing anchors fail the probe rather than guess. Native SetSelectedState changes tint/background sprites, not icon position.
- Small installed check: open both tabs, inspect centering/clarity in selected and dim states, switch and close/reopen once. Return screenshots and log. Expected RAR_TAB_ART_DRAW offsets above and unchanged restore/close markers. No diagnostic job or repeated save/reload required. Existing picker warnings remain outside this correction.

## Study Table Work Probe 0.1.3 — original tab artwork candidate

**Status:** local compile/identity and unchanged PNG embedding checks passed; returned screenshot and user report approve icon appearance but identify off-center opaque artwork. No returned runtime log for this segment; centering correction is 0.1.4.

- Exact compiled source: fc719ca0f9561f0f77bdcd1147416b8d1fde5ce2, research/study-table-work.
- Artifact: StudyTableWorkProbe-0.1.3.dll; SHA-256 7ac0a4bc80cd86f5114bd896f2ef36501cf5debf06e75b40e7c80afc164a8e09. Plugin version 0.1.3; assembly/file versions 0.1.3.0.
- Clean-source optimized deterministic handoff build, SDK 6.0.406 Roslyn, warnings as errors. Both resource byte hashes also checked in the exact handed binary; source canvases remain unchanged. Previous numbered artifacts remain frozen.

- Scope: two original user-selected 40x26 icons, preserved PNG bytes/full transparent canvases; native selected-state tint and existing input/work/close behavior.
- Resource identities and provenance: research/StudyTableWorkProbe/assets/README.md. Embeds RAR.MaterialStudy.png and RAR.RecipeResearch.png in the single DLL; no adjacent PNG installation.
- Mechanism/gate: scoped CraftTabGUI.Draw postfix assigns the sprite and 40x26 widget size only for the active study-table composition. Native lookup IDs remain valid, SetSelectedState remains the tint final writer. Direct scene icon widget is Center/54x32/Simple/pixel-size 1; full canvas is deliberately retained.
- Local validation: exact handoff PNG hashes, RGBA 40x26 and binary alpha confirmed; compile with warnings as errors and installed ImageConversionModule reference passed. Both development DLL resources match their source hashes byte for byte. No new production behavior.
- Minimum runtime check: inspect both active and dim icons, switch using mouse/gamepad and close/reopen. Return both page screenshots and log. RAR_TAB_ART_READY and RAR_TAB_ART_DRAW canvas=40x26 must be present; missing art, clipping/stretching, RAR_WORK_ERROR, restoration failure or changed unrelated tabs block acceptance. Existing picker-layer warnings remain a separate unresolved owner question.
- No need to repeat work/save/reload. Starting any diagnostic job still creates save-persistent test state; clean using F8 beside the table and save after cleanup.

## Study Table Work Probe 0.1.2 — icon/close correction candidate, 2026-10-12

**Status:** local compile/identity and deterministic key-map regression passed; returned 0.1.2 run confirms close correction and native icon rendering/usability. Remaining NGUI layer-warning investigation and final semantic icon choice are open. No production acceptance.

- Exact compiled source: `e8c0d4da66ec1280340113ba3d7bedb956a5f2a8`, research/study-table-work.
- Artifact: `StudyTableWorkProbe-0.1.2.dll`; SHA-256 `7b914ee5e4904241e8e4a0ea0aebd25f56d550c2399e93c2a2ea3141c708ad0f`. Assembly/file versions `0.1.2.0`, plugin version `0.1.2`.
- Build: clean exact source, same SDK 6.0.406 Roslyn/verified references, optimized deterministic output, warnings as errors; assembly/file versions asserted. Numbered 0.1.1 remains frozen.
- Changes: native quill/scroll and paired-flask tab icons via book / ?alch; removes the caption patch and nonexistent sprite requests. Installs a separate key-map copy and restores the field reference without modifying the live enumerated dictionary. Rollback assertion now checks original dictionary identity too.
- Local regression: .NET Framework reproduces the old existing-value mutation exception in the BaseGUI-style key loop; replacement completes the loop and restores the exact original map. This proves collection semantics, not Unity/Harmony integration.
- Diagnostic job ID, costs, 2-unit work time, no formula reward and F8 cleanup remain unchanged. Starting a job is save-persistent; use the test save, clean beside the table and save after cleanup before removal.

Minimum user check: replace the previous installed copy, retaining one probe DLL. Open the table, inspect both icons/selected tint and switch by mouse and LB/RB. Open/select/cancel the Study picker; close/reopen from each page with Back/Esc and also test right-click closing. Return an icon screenshot and LogOutput.log. No repeat of established mid-work save/load or long work is needed; a job is optional for launch-close coverage.

Acceptance blockers: any Collection was modified/BaseGUI.Update enumeration exception, missing tab sprites, RAR_WORK_ERROR/RAR_TABS_RESTORE_ERROR, layout_restored=False or an owner left shown. Inspect NGUI layer warnings separately: six occurred in 0.1.1 and their cause is unassigned; this candidate does not claim their removal. Native reward/persistence and current source eligibility remain independent future gates.

### Returned 0.1.2 evidence

Log snapshot SHA-256: `efd6f0ac9cd7234723ada05e0137560c315d859b79bad7433184f13226942682`. Startup reports version 0.1.2 and the accepted host hash. No probe DLL is present in the plugins directory at inspection, so the installed binary digest cannot be independently rechecked in this segment; the immutable handed artifact identity above is preserved.

- User reports good layout/native visual style; switching and selection with mouse/gamepad, diagnostic start and F8 cancellation. Screenshots show native selected/dim tab icons and both pages.
- 11 completed opens, 34 switches, 11 layout_restored=True restorations and 11 outer=False/study=False paired closes. Zero Collection was modified exceptions, missing tab sprites or RAR_WORK_ERROR/RAR_TABS_RESTORE_ERROR. This closes the identified close/key-map defect for this returned run.
- Start remains sample_free=True/output_count=0; faith 5 -> 4, station science 19 -> 18. F8 observes progress 0.401, native cancellation leaves active=False/science=18, then clears one last-craft reference; final active=0/completed=False. No formula disclosure.
- 30 widget/panel layer warnings remain around native picker interactions. No visible issue reported; ownership/cause not established. Do not call the entire log clean or infer that the remaining warning is harmless.
- User accepts icon presentation style but asks to review alternatives: existing writing/mixing symbols are semantically ambiguous on the study table. Swapping their meanings is a tentative suggestion, not an instruction to ship that reversal. A local preview catalogue of 49 native tab sprites and nine extra book/scroll/science candidates was prepared from verified resources.assets; no public extracted artwork. Final pair remains open.

## Study Table Work Probe 0.1.1 — two-tab candidate, 2026-10-12

**Status:** local compile/identity gate passed; installed usability demonstrated, but close/key-map errors block UI acceptance. No production UI acceptance.

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

The original build result alone did not establish installed layout/focus. Formula reward, final text/art, source eligibility and refund economics remain separate gates. Numbered 0.1.0 and 0.1.1 artifacts are immutable.

### Returned 0.1.1 evidence

Returned log SHA-256: `04aa127c681b8a3df008dc33324c27011d6eb64a577e682ca1d3f8c5d050b028`. Installed DLL hash matches the exact 0.1.1 artifact. Raw log and private asset inspection remain ignored local inputs.

- User checked tab switching with keyboard/mouse and gamepad; native material picker opening, selection/cancel, switching afterwards, closing and reopening. Screenshots show the common frame without reported clipping. The unrelated church workbench retains native icon tabs. This does not establish general compatibility with all UI mods.
- Log: 10 completed opens, 28 switches, 10 layout_restored=True restorations and 10 outer=False/study=False paired closes; no RAR_WORK_ERROR or RAR_TABS_RESTORE_ERROR.
- Short job starts sample_free=True/output_count=0, faith 5 -> 4, science 19 -> 18; native completion marker recorded with formulas=0. F8 removes one completed marker and one last-craft reference, active=0/completed=False. User screenshots show the completed job hidden and the cleaned job visible again.
- Blocking diagnostics: eight InvalidOperationException/Dictionary KeyCollection Enumerator stacks in BaseGUI.Update after paired closure. Inspection identifies restoration of the currently enumerated gamekey_delegates dictionary as the defect. 20 missing tab_rar_study/tab_rar_research errors are caused by the caption candidate requesting nonexistent native icons before hiding them.
- Six NGUI widget/panel layer warnings occur around a picker opening. Cause unassigned; do not call the overall log clean or claim that changing key maps removes these warnings. Other world/mod diagnostics are outside this scoped result.
- User requests icon-only tabs matching vanilla. The native icon pair selected for the next visual candidate is quill/scroll (tab_book) and paired flasks (tab_custom_alch). Final visual acceptance remains open.

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
