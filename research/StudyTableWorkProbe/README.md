# Study Table Work Probe

Research only. No production mod or final interface. Original code is MPL-2.0.

## Current candidate: 0.1.1, two-tab composition

Question: can the existing Survey panel and craft-card list share a native CraftGUI frame/tabs with correct controller/mouse focus, resource-picker return and close/rollback? Work execution is already verified by 0.1.0; do not repeat its full save/cancel lifecycle just to test tabs.

Method: native tab creation/selection and BaseGUI stack semantics plus direct level2 inspection establish the static owners and geometry. Installed focus/layout with the user's mod set still requires observation. This candidate composes the existing ResourceBasedCraftGUI as a native top-of-stack subwindow, like MixedCraftGUI on the native alchemy tab; it does not clone Survey logic or replace its sample filter. It temporarily hides its outer frame/header/dimmer and positions its content inside the outer craft frame. Native picker remains a separate modal. Scoped next/previous-tab delegates and paired window close handlers bridge the two owners; originals and layout are restored on close/failure.

Inspected scene: level2 SHA-256 `1f75a0328dc4987f3020688a0feb0d310c2bf3f6e288c6e4a64ad09db1f39041`, matching installed host assembly identity. GUIElements owns CraftGUI component 24109 and ResourceBasedCraftGUI 22567; tab prefab width/height is 66/32 UI units. Paths/component existence are checked at entry; missing contracts disable this diagnostic rather than guess a hierarchy.

Minimum procedure: replace 0.1.0 with 0.1.1 (one installed copy under StudyTableWorkProbe.dll). Open the study table after alchemy unlock. Switch Изучение/Рецепты with LB/RB and, if available, mouse. On Изучение, open the real material picker, select an item or cancel, then switch again. Close the complete window from each tab and reopen. Check an unrelated workbench's original tabs. A short test job can be started to verify automatic window closing; it keeps the diagnostic 1 faith/1 science/10 energy inputs but now uses 2 work units. No recipe reward. After any job, F8 cleanup and save remain required as below.

Return a screenshot of each tab and LogOutput.log. Look for RAR_TABS_OPEN, SWITCH, RESTORE layout_restored=True and CLOSED outer=False study=False. RAR_WORK_ERROR or RAR_TABS_RESTORE_ERROR prevents acceptance. This is a two-tab candidate, not proof that its layout/focus is correct before the user test. Caption strings and final presentation remain diagnostic.

## Frozen 0.1.0 work-lifecycle baseline

The sections below describe the immutable first probe, source `6f4fd3bd21e6fee2247c0edfdc372bb1e1f988ef`; its artifact is not rebuilt. Returned evidence and current pending UI candidate identities are owned by docs/TEST_LOG.md.

## Question and method

Does one registered, sample-free manual job on the real `mf_alchemy_survey` table pass through native craft selection, payment, work, cancellation, completion and mid-work save/reload while ordinary Survey stays usable?

Static inspection establishes the owners but cannot prove installed patch order, live GUI transitions or restoration. There is no existing exact artifact for this job. A small additive job plus native two-option dialog and event observations is cheaper than a journal implementation or manually reading hidden state. The observer does not assign progress or completed state. Formula disclosure is excluded: it is a separate unresolved dependency.

## Boundaries

- Exact verified 1.407 assembly hash gate; fails closed on mismatch.
- Registers one unique job before ID caches; no ordinary craft or Survey definition edits.
- Table-local entry uses native DialogGUI and CraftGUI. Before The Beginning Of Alchemy, interaction remains ordinary Study.
- Job costs are **diagnostic inputs**, not adopted economics: 1 faith, 1 station science, 10 energy over 30 native work units. No sample, item output, formula or technology-point reward.
- Native cancellation is observed without a custom refund. Faith is returned as a world drop on cancellation, so the immediate inventory count alone is not a refund assertion. Science and spent energy are not restored by probe cleanup. Completed work consumes the diagnostic costs.
- The added definition is session-only, but starting/finishing it and saving can persist native craft/knowledge state. Use a disposable/test save. F8 opens the probe status/cleanup dialog after closing game menus. Use cleanup beside the tested study table: it cancels only this job, removes its completed marker and its stored last-craft IDs on loaded active tables. Save after cleanup before removing the DLL. It does not roll back real costs, unloaded object state or unrelated state.
- No recipe rows, inventory dumps, copied game source/assets or host DLLs are distributed.

## Minimum runtime matrix

| Scenario | Real path and evidence | Cleanup / acceptance |
| --- | --- | --- |
| Before alchemy unlock | Ordinary Study opens; gate logs `unlocked=False` | No job; preserve Survey |
| After unlock / normal mode | Table dialog -> ordinary Survey picker | Close picker; preserve input behavior |
| Research selection/start | Table dialog -> native card -> native craft start; `RAR_WORK_START` logs before/after costs and active job | Target sample absent by job definition; no hidden synthetic sample |
| Cancellation | Native cancel button or probe cleanup; `RAR_WORK_CANCEL` logs actual deltas | Native science distinction recorded; no refund policy inferred |
| Mid-work reload | Save during work, quit/reload with DLL; `RAR_WORK_RESTORE` records resolved ID and progress | Continue actual work; unresolved ID fails acceptance |
| Completion | Native manual work -> real `GameSave.OnFinishedCraft`; `RAR_WORK_COMPLETE` confirms native marker | No formula reward; completion appears once |
| Cleanup | F8 -> remove test state; `RAR_WORK_CLEANUP` reports remaining active/marker/last references | Save cleaned state before uninstall |

Return BepInEx `LogOutput.log` and note whether both menus were usable. No exact formulas are required. The first two gate states can be checked on existing suitable saves; do not replay Clotho just for this probe.

## Local build

`build.ps1 -GameRoot <installation> -DotnetRoot <dotnet installation> -ExpectedSource <commit SHA>` invokes SDK 6.0.406 Roslyn directly against local host/BepInEx references, without package restore. `-CheckOnly` permits a development-tree compilation and writes an explicitly unnumbered check artifact; that output is never handed as a candidate. The handoff mode requires the exact commit and clean tree, refuses to overwrite numbered output, verifies the assembly version and records hashes of its local references. No proprietary references are copied into the repository or output. Output is under ignored `artifacts/`; the installed filename is `StudyTableWorkProbe.dll`, the handed filename `StudyTableWorkProbe-0.1.0.dll`.

Runtime acceptance is pending. The compile gate proves binding only, not the UI or saved job lifecycle.
