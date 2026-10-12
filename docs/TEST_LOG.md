# Diagnostic runtime tests

## Study Table Work Probe 0.1.6 — fixed user-selected icon position

**Status:** clean-source compile/identity gate passed. Selected placement was observed in 0.1.5; exact 0.1.6 binary is not yet runtime-tested. The user explicitly defers a standalone 0.1.6 test and will inspect fixed placement in subsequent substantive candidates; do not request another dedicated run merely for these offsets.

- Exact source: 5322a458b50507c97b9f0adcaefe1a0d5d8573bb, research/study-table-work.
- Artifact: StudyTableWorkProbe-0.1.6.dll; SHA-256 dabe12e8c4d23d22d3e1b12286d6d989feda0b8df98ae9df9b97042dbdef160c. Plugin 0.1.6, assembly/file 0.1.6.0; deterministic optimized SDK 6.0.406 build, warnings as errors and both version assertions. Previous numbered artifacts frozen.

- Freeze final 0.1.5 calibration corrections: Material Study +1/+1 -> absolute (-2,+2); Recipe Research +2/+1 -> absolute (+1.5,+1.5), relative to native tab-background center. The last two chosen records agree.
- Remove IconCalibration, F2 polling/OnGUI, native input-capture flags/Update guards, page/tuning callbacks and IMGUI-module reference. F8 job cleanup remains. No PNG, size, native tint, ordinary Study, work/cost/save or science-header change.
- Static review/compile confirms no calibration class, F2 polling or input guards remain; offsets are constants applied at native Draw as before. No need to repeat the established work/save-load lifecycle. A fresh-launch glance can confirm the fixed placement matches the selected screenshot; whole UI/production acceptance remains separate.

## Study Table Work Probe 0.1.5 — F2 icon calibration candidate

**Status:** exact installed DLL matched; returned screenshot/log demonstrate the F2 panel, repeated chosen-value export and restoration/close path. Final optical corrections selected: material +1/+1, recipe +2/+1 relative to 0.1.4. This does not accept the whole production mod.

- Exact source: 737d4cd3c591eb47d2d55c7515a5cd3286bb51ed, research/study-table-work.
- Artifact: StudyTableWorkProbe-0.1.5.dll; SHA-256 771a4f3012fbed8c4f8113f0330b50e9d83772f3aa6589d8d6f06312a75583f2. Plugin 0.1.5, assembly/file 0.1.5.0; optimized deterministic SDK 6.0.406 build, warnings as errors, both versions asserted. Previous numbered artifacts unchanged.

- Goal: let the user choose optical alignment live rather than issue another guessed fixed offset. Native geometry/placement path is already established by 0.1.4.
- F2 opens an IMGUI panel only on the shown two-tab study-table composition, without an active resource picker/other modal. Four independent sliders and +/- buttons adjust Material Study and Recipe Research X/Y, step 1, bounded to -8..+8 native UI pixels relative to 0.1.4. +X is right, +Y is up. Both icons update immediately; panel buttons can show either native page for selected/dim comparison.
- Session-only values survive table closing/reopening, not process restart. No game-save/config writes. Reset returns to 0.1.4; Done logs corrections and resulting absolute offsets as RAR_TAB_CALIBRATION_VALUES reason=chosen and closes. F2 closing also records current values. Final accepted values will replace defaults and the temporary tool will be removed at a later candidate boundary.
- Input containment: the inspected public static UICamera.ignoreAllEvents early-return guard pauses native NGUI events while IMGUI owns input. Scoped BaseGUI.Update and ResourceBasedCraftGUI.Update prefixes suppress table keys (Survey handles Interaction before its base call). No native dictionaries/stack changes. Closing retains the guard until a later frame after mouse buttons are released, preventing click-through; the original flag is restored. Plugin destruction restores immediately. Failure/table loss closes the panel and releases capture through the next Update.
- API evidence: Assembly-CSharp-firstpass.dll SHA-256 9dc6def3b7715dd27eeb168ddc0af47e31c6f38d3fbee24bf592899392026498, exact UICamera.Update IL entry checks ignoreAllEvents and returns when true. Same verified host references plus installed UnityEngine.IMGUIModule.dll; no dependency binaries distributed. No recurring object scans or slider-change log spam.
- Runtime check: open table, F2; adjust each icon using sliders/buttons and compare both pages using panel buttons. Reset once if useful, choose final values and Done, then verify normal switching/close/reopen with mouse/gamepad. Return chosen-values log and final screenshots. RAR_TAB_CALIBRATION_INPUT restored=True and no probe errors are required. No work/save-load repetition. Existing diagnostic job remains save-persistent only if started; F8 cleanup rules unchanged.

### Returned 0.1.5 evidence and chosen values

Log SHA-256: 4a48481db590b8d0556e8c572167deabc4e3cc3502ff5907588f17bd088e88f8. Installed DLL SHA-256 matches the handed 0.1.5 artifact above. Screenshot shows the panel and selected corrections material_dx=1/material_dy=1/recipe_dx=2/recipe_dy=1.

- Four panel opens and four RAR_TAB_CALIBRATION_INPUT restored=True markers; six opens/paired closes/layout restorations and 25 tab switches. No probe errors or Collection was modified exceptions. This records the actual calibration/export/close path; no separate verbal input-mode acceptance is inferred.
- Earlier chosen recipe_dx=1 is superseded by the last two reason=chosen records, both material_dx=1/material_dy=1/recipe_dx=2/recipe_dy=1. Resulting offsets: material_x=-2/material_y=2/recipe_x=1.5/recipe_y=1.5. Treat explicit Done action plus returned screenshot/log as the selected working placement, not stable production approval.
- No NGUI layer warnings in this capture; native picker opening is not exercised. This does not resolve the earlier picker warning owner.
- Freeze values in 0.1.6 and remove the temporary calibration tool as previously agreed.

## Study Table Work Probe 0.1.4 — visible-art centering candidate

**Status:** clean-source compile/identity gate passed; returned log confirms the placement path, but user screenshots still find the icons too low/slightly left. Fixed placement is not accepted; next step is user-requested F2 calibration.

- Exact source: 850daeea753e85c164ddde1c8f1a355003944d0b, research/study-table-work.
- Artifact: StudyTableWorkProbe-0.1.4.dll; SHA-256 49344e23e386a7ee84148dbfe03539af8685f0ace0812da62083c3eada044cee. Plugin 0.1.4, assembly/file 0.1.4.0; optimized deterministic SDK 6.0.406 build with warnings as errors and both version checks. Frozen 0.1.3 is unchanged.

- Scope: icon placement only; original PNG bytes, 40x26 canvases, size, native tint, work/input/close paths and science header remain unchanged.
- Root cause: 0.1.3 centered complete PNGs. Material opaque bounds (9,3)-(37,25) center at (23,14) inside a (20,13) canvas center; recipe (5,2)-(36,25) centers at (20.5,13.5). PNG Y is inverted relative to UI Y.
- Corrected absolute offsets from the native tab-background center: Material Study x=-3/y=+1; Recipe Research x=-0.5/y=+0.5 UI units. Exact bounds centering; not a claim of subjective optical acceptance.
- Direct scene evidence: back/icon share parent and Center pivot; back 66x32, icon unanchored. Postfix validates same-parent/center-pivot contract, preserves icon Z and uses absolute placement. Missing anchors fail the probe rather than guess. Native SetSelectedState changes tint/background sprites, not icon position.
- Small installed check: open both tabs, inspect centering/clarity in selected and dim states, switch and close/reopen once. Return screenshots and log. Expected RAR_TAB_ART_DRAW offsets above and unchanged restore/close markers. No diagnostic job or repeated save/reload required. Existing picker warnings remain outside this correction.

### Returned 0.1.4 evidence

Log SHA-256: 537ecfc8c4beb905f5535fc48c6affd7525a2415e2ed2e31a489e961a4297340. Startup identifies 0.1.4/accepted host. Art READY once; 16 art draws record expected 0.1.4 offsets. Eight opens, forty switches and seven observed paired closes/layout_restored=True; the final open has no closing marker in the returned capture. No RAR_WORK_ERROR, RAR_TABS_RESTORE_ERROR or Collection was modified exception. Six picker layer warnings persist with unassigned cause. User reports slight improvement but still too low/slightly left and explicitly requests F2 sliders. This does not accept fixed positioning. Science-header roughness is no longer a requested change: user leaves the vanilla icon as it is.

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

## Candidate 0.1.7 — native formula knowledge reward (bounded runtime result accepted)

- Source: b9721dd1ad169f7561a6ac31e47ecd0477d631c3, research/study-table-work; clean exact-source handoff build.
- DLL: StudyTableWorkProbe-0.1.7.dll, assembly/file version 0.1.7.0.
- SHA-256: 439ab0329b37298d4efe932dee4a087fab5a991ce8bc0f1766a3ce1993d1bdfe.
- Build: existing deterministic SDK 6.0.406 / C# 7.3 warnings-as-errors gate; verified local 1.407 references and unchanged embedded artwork. No production toolchain or release is selected.
- Shared static evidence: GraveyardKeeperResearch commit 967a21d, native known-formula list/disclosure/save boundaries; pinned decompile identity remains in VERIFIED_RUNTIME_DATA.md.
- Automated state checks: 8 original-known/unknown-vs-partial setup combinations, remaining-only reward, duplicate reward/cleanup, original restoration from a fresh native save object's copied list, unrelated knowledge preservation and missing-snapshot rejection pass. Compilation passes. Native serializer, UI and process-restart operation are not tested by these checks.
- Changes: one pot_heal research topic after explicit F8 setup; real job completion grants remaining native exact formulas; grouped native forced pseudotech dialog; independent native card-count observer and save/load boundary logging. No ingredient formulas in probe logs/public docs; no native material Study or formula definitions changed. Final selected icon placement from 0.1.5 is carried forward without F2.
- Synthetic preparation can enter saves. Neutral registered metadata preserves original knowledge for cleanup across launches, not a production knowledge ledger. Keep DLL installed until cleanup + save. During the test do not independently unlock/manufacture this product or change the installed balance-mod set. F8 cleanup stays reachable if the probe disables itself after an error. Refund economics and unloaded-table cleanup scope remain as documented for prior probe.

Minimum installed procedure on a disposable save with foundational alchemy unlocked, existing study table and tier-I alchemy table:

1. Replace the previous probe DLL; retain one installed StudyTableWorkProbe.dll. F8 -> Actions -> Prepare -> Both unknown. F8's first status logs the original known count; setup logs known=0.
2. Finish the short recipe-research job. Expect native popup, learned=2, known=2 and native tier-I recipe list cards=2. No material sample is needed.
3. Save, preserve this launch's LogOutput.log, fully exit the game, relaunch/load with the same probe, open the tier-I list and F8 status. Expect known=2 and cards=2; no repeat paid research topic.
4. F8 -> Actions -> Remove test state; expect the original formula count restored. Then prepare One known, finish research; reward learned=1, known=2, native cards=2. Remove test state again and save.
5. Return both launch logs and notes/screenshots about native reward/list appearance. Preserve a second log before any further launch. Keep the test cleaned and saved before removing DLL.

No separate repetition of ordinary Study/input calibration is required for this downstream test. RAR_WORK_ERROR, an absent reward/list seam or incorrect cleanup/load state blocks this candidate's acceptance. Existing source-awareness, ingredient-guidance presentation, pricing/refunds and picker-layer-warning gates remain independent.


### Returned 0.1.7 evidence — 2026-10-12

Installed StudyTableWorkProbe-0.1.7.dll SHA-256 matches 439ab0329b37298d4efe932dee4a087fab5a991ce8bc0f1766a3ce1993d1bdfe. Source remains b9721dd1ad169f7561a6ac31e47ecd0477d631c3; frozen bytes are unchanged. Returned raw files are archived privately in ignored work/evidence/knowledge-0.1.7.

- Launch 1: user-pasted full Unity/player log text, SHA-256 2561de4deebe69a3ff2b8b1c1f3261a72c8dd36ddac4d2d56f46e8d360f49ae7. Startup reports version 0.1.7 and verified host hash. Native loaded/F8 state is prepared=False, known=0, completed=False. Synthetic unknown setup stays known=0. Actual completion records learned=2, known=2, native_job_marker=True; native dialog before/after/closed markers occur. SaveSlot manualsave.20260912_124029 is saved with prepared=True, known=2, completed=True.
- Launch 2: BepInEx LogOutput.log, SHA-256 19dab51bf0d522d4879b0ac563855c83ace453ce206371f8781ebe4fe84799ae. Separate startup and native PrepareAfterLoad restore prepared=True, known=2, completed=True. Tier-I OpenAsAlchemy observes cards=2, known=2. User reports full exit/relaunch and successful loaded presentation.
- First cleanup restores prepared=False, known=0, completed=False; clears one native last-craft reference. Partial setup then measures known=1. Actual completion records learned=1, known=2 and one popup unlock; native list cards=2. Final cleanup restores zero knowledge, removes the last-craft reference, repeated cleanup has nothing left to clear. Final ToBinary returns with prepared=False, known=0, completed=False on the same save slot.
- No RAR_WORK_ERROR or exception match occurs in either returned file. Native pseudotech lookup warns No data for object [TechDefinition] with id pot_heal once per reward (one in each launch). Broader game/world/asset warnings and errors also exist with unassigned ownership; this is not a clean-log or clean-vanilla compatibility claim. No picker-layer-warning match occurs here, but the relevant picker path is not established as exercised, so its prior gate stays open.
- Screenshots confirm the grouped two-row native reward and both native formula cards. Exact ingredients are not transcribed. User reports the post-cleanup tier-I list has no healing-elixir recipes and accepts functionality without global objections. They did not inspect the pre-research native list: initial zero knowledge is log-proved, initial zero rendered cards is not independently captured. The finished-topic hiding has no separate post-completion screenshot; do not overclaim it from successful reward alone.
- User flags generic technology-discovery wording as unsuitable; formula-discovery wording is the adopted presentation meaning, exact text remains open. The popup's two identical product rows are native craft-unlock rows, not proof that the two formulas were duplicated in knowledge.

Accepted envelope: one sample-free two-variant product, all missing variants, partial knowledge, native reward/list, full-process-restart retention and cleanup restoring an originally unknown baseline. Preexisting nonzero baseline restoration remains automated-state evidence, not an installed scenario. No actual manufacture, additional products, tier-II, changed mod set, source eligibility, final guidance or production release is accepted by this result.

## Source Eligibility Probe 0.1.0 — read-only source availability candidate

- Source: 8ac6051dcd31b6bd581bc766a0614173167d2483, research/study-table-work.
- DLL: SourceEligibilityProbe-0.1.0.dll; plugin/assembly/file versions 0.1.0 / 0.1.0.0 / 0.1.0.0.
- SHA-256: 58f02e675776228a38d23e81c4cbcb71d368522e4ad00468561e14960b25a485.
- Build: clean exact-source deterministic SDK 6.0.406 C# 7.3 warnings-as-errors compile; existing local identity pattern, no assets embedded/distributed. Exact reference hashes stay in ignored build-identity.json. Local compile check and handoff build pass; installed observation is pending.
- Shared source finding: GraveyardKeeperResearch f88344e, native room-builder filter, phrase rendering vs item-price pickability, and Clotho's specific Spices disclosure. No formula ingredients published.
- Probe has no save writers, craft registration, grants, synthetic inputs, F8 handler or UI changes. It records one first-playing-save snapshot and bounded native source/UI interactions. Strict verified-host gate keeps diagnostic claims within the inspected 1.407 envelope. This remains a separate observer, not a playable research implementation.

Minimum user procedure:

1. Add this one DLL to BepInEx/plugins; previous Study Table Work Probe may remain installed if its synthetic state is already cleaned. Launch/load the existing save.
2. Open the room-decoration builder in the keeper's house, then close it. Open the study table once for a second source snapshot. No crafting or spending is needed.
3. Return this launch's LogOutput.log. If the Merchant cure answer is already naturally available, optionally open that menu without handing in Spices; if completed/unavailable, do not replay or manipulate the quest. Native loaded task/phrase/known-formula state still supplies useful evidence.
4. Remove the observer DLL whenever desired; it requires no save cleanup. Formula-reward restart tests do not need repeating.

Expected evidence: RAR_SOURCE_READY read_only=True, current native visibility/knowledge counts for all seven paint channels, RAR_SOURCE_NATIVE_BUILD counts through the actual opened native builder, and Spices state/remaining formula count. Native cure-option rendering is an optional independent observation; affordability is logged separately. A missing builder observation or RAR_SOURCE_ERROR is not a pass. First-observation builder presence alone is not labeled verified physical reachability. Locked/new-save negative states and source coverage beyond these eight products remain untested.
