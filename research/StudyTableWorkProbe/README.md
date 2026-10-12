# Study Table Work Probe

Research only. No production mod or final interface. Original code is MPL-2.0.

## Current candidate: 0.1.6, fixed chosen icon placement

The returned 0.1.5 panel screenshot and last two Done exports select Material Study correction (+1,+1) and Recipe Research (+2,+1). Fixed absolute offsets from native tab-background center are now (-2,+2) and (+1.5,+1.5). Full PNG canvases, native size/tint, key-map rollback, ordinary Study and work/save behavior remain unchanged.

The temporary F2 panel, session offsets, IMGUI reference and input-capture/Update guards are removed. F8 diagnostic job cleanup remains. Source review and compilation passed; this exact new binary is not installed-runtime accepted merely because the same positions were chosen in 0.1.5. A fresh-launch visual glance can confirm matching placement; no repeat of work/payment/save-load or calibration is required. Exact identity and remaining UI/reward/source gates are in docs/TEST_LOG.md and docs/DESIGN.md.

## Frozen 0.1.5 F2 live calibration baseline

Open the study table and press F2. The mouse-operated panel has independent X/Y sliders and +/- buttons for both icons, integer corrections -8..+8 relative to 0.1.4. +X moves right, +Y moves up; units are native UI pixels. Use panel page buttons to inspect active/dim states, Reset to return to the baseline, and Done to log selected values and close. F2 also closes. Values persist across table reopen during this process, not across game restarts or in saves/config.

Question: choose user-approved optical alignment now that the exact alpha-bounds centering path is proved but perceptually unsatisfactory. No PNG, scale, science-header, work, costs, reward or save change. The panel uses IMGUI while the native table stays visible. Native NGUI mouse events and table keys are temporarily contained, including Survey's direct Interaction shortcut. Original input flag is restored after mouse release or immediately on plugin destruction. UICamera.ignoreAllEvents and its early-return consumer were verified in the installed firstpass assembly; compile binds against the installed IMGUI module. No new GUI stack entries, key dictionary mutation, object scans or per-frame logging.

Minimum check: adjust both icons, compare pages using the panel buttons, press Done, then switch/close/reopen normally. Return RAR_TAB_CALIBRATION_VALUES reason=chosen, screenshots and the log including RAR_TAB_CALIBRATION_INPUT restored=True. No test job necessary. Final chosen values will be frozen and the calibration tool removed. Existing picker-layer warning ownership remains open. Exact candidate/log identities are in docs/TEST_LOG.md.

## Frozen 0.1.4 visible-art centering baseline

User-approved original icon appearance is preserved. The 0.1.3 screenshot reveals that centering full canvases leaves asymmetric opaque artwork off-center. Place each icon relative to the native background center, compensating exact alpha bounds: Material Study (-3,+1), Recipe Research (-0.5,+0.5) native UI units. Keep complete PNGs and native tint; no crop, scaling, new art, science-header or input/work change.

Direct scene back/icon anchors share a parent and Center pivot; the icon has no anchor targets. Validate this contract, preserve Z and assign absolute position, avoiding reopen accumulation. Fractional recipe offset reflects odd artwork dimensions. Check installed selected/dim centering and sharpness, switch/close/reopen and return screenshot/log. Live calibration is unnecessary for the first exact geometric correction; consider bounded optical tuning only if that correction does not satisfy the user. No test job required.

## Frozen 0.1.3 original artwork integration baseline

Question: render the two user-selected original PNGs in the existing native tabs, with exact canvases, sharp sampling and native selected/dim tint. This is a bounded presentation-only research candidate; no work, input, pricing, formula reward or picker-layer fix. The unchanged 0.1.2 composition/close evidence remains baseline.

Native owner/final writer: CraftTabGUI.Draw assigns UI2DSprite.sprite2D; subsequent SetSelectedState owns colour. A table-local Draw postfix replaces only the copied tab icon, preserving its inspected centered transform, and sets its widget to the complete 40x26 canvas. Direct level2 inspection finds native icon widgets at 54x32, pivot Center (4), pixel size 1, Simple fill. No custom label, panel, atlas registration or global sprite replacement is added. Existing book / ?alch IDs keep native lookup valid before the scoped replacement; other workbench tabs are unaffected.

TabArtwork decodes the embedded originals once after the verified-host check, validates PNG hashes and canvas size, creates centered FullRect sprites, uses Point filtering/Clamp wrapping and releases owned sprites/textures on plugin destruction. Both final PNGs are unchanged and tracked as original software-supporting assets under MPL-2.0; private drafts/game references remain ignored. The compiler adds the installed ImageConversionModule reference and embeds both PNGs, so delivery remains a single DLL. Local compile and byte-preserving embedded-resource checks passed; installed rendering remains pending.

Minimum check: replace the previous probe DLL, retaining one copy. Open both pages; switch by mouse/gamepad and close/reopen once. Return screenshots of both selected states and LogOutput.log. Check icons are sharp, fully visible, aligned and recognisable when dimmed. Expected RAR_TAB_ART_READY and RAR_TAB_ART_DRAW canvas=40x26, plus established paired restore/close markers. No work/save/reload repetition is needed. If a job is started, F8 cleanup and save remain required. Existing picker-layer warnings are not claimed fixed.

## Frozen 0.1.2 icon/close correction baseline, native icons and safe key-map restoration

Question: retain the user-tested two-tab Survey/craft-card composition while eliminating the identified close-time Dictionary enumeration failure and missing-icon errors. Native formula reward remains excluded. 0.1.1 source/artifact stays frozen; its returned results and limits are in docs/TEST_LOG.md.

Method: pinned BaseGUI.Update enumerates gamekey_delegates.Keys and invokes callbacks that can close the window. The old restoration replaced dictionary values inside that enumeration. 0.1.2 installs a cloned map and restores the original field reference; neither enumerated map is mutated on close. Layout restoration now also verifies original map identity. A .NET Framework regression reproduces the original exception and verifies the field-replacement strategy; the returned 0.1.2 run also confirms eleven paired restores/closes without the old enumeration failure.

Native icon presentation uses book / ?alch, resolving tab_book / tab_custom_alch via unchanged CraftTabGUI.Draw; native SetSelectedState supplies selected/dim tint. No cloned caption labels or extracted art are distributed. The table-local SwitchTab interception still handles both pages, so the icon ID does not activate vanilla MixedCraftGUI content. The quill/scroll and paired-flask pair remains subject to visual acceptance.

The composition retains the real ResourceBasedCraftGUI Survey owner, native picker/filter/craft callbacks and separate top-of-stack semantics, as in native mixed-alchemy composition. Only the inspected frame/header/dimmer, position, depth, tips and key-map field are temporarily changed and restored. Inspected level2 SHA-256: `1f75a0328dc4987f3020688a0feb0d310c2bf3f6e288c6e4a64ad09db1f39041`; tab prefab 66x32 units, CraftGUI component 24109 and ResourceBasedCraftGUI 22567.

Minimum procedure: replace the installed 0.1.1 copy with 0.1.2, keeping one probe DLL. Open the table, switch the quill/flask tabs using mouse and LB/RB, and close from each page using Back/Esc; reopen and also try right-click closing. On Study select/cancel the material picker and switch again. Provide a screenshot with the icons and LogOutput.log. Repeating the established work/save-load lifecycle is unnecessary. If any diagnostic job is started, use F8 cleanup beside the table and save afterwards as below.

Expected: native icons visible with selected tint, no tab_rar_* missing sprites, no Collection was modified exception, and RAR_TABS_RESTORE layout_restored=True plus RAR_TABS_CLOSED outer=False study=False. The returned 0.1.2 log still contains thirty NGUI layer warnings around picker interactions, with no assigned cause or reported visual malfunction. Their owner remains to be investigated before closing UI readiness. No production UI acceptance from compilation alone.

## Frozen 0.1.0 work-lifecycle baseline

The sections below describe the immutable first probe, source `6f4fd3bd21e6fee2247c0edfdc372bb1e1f988ef`; its artifact is not rebuilt. Returned evidence and current correction candidate identities are owned by docs/TEST_LOG.md.

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

The frozen 0.1.0 lifecycle was runtime-verified; the 0.1.2 close correction and native icon style are now runtime-observed; remaining NGUI warnings and the user's semantic icon choice remain open. The compile gate proves binding only.

## 0.1.7 bounded formula knowledge experiment

Research only, not a production catalog. Source in FormulaKnowledge.cs binds one two-variant product, preserves original knowledge with registered neutral metadata, prepares explicit synthetic unknown/partial input through F8, grants only remaining exact native knowledge at actual job completion and opens one native pseudotech reward. Native recipe lists and serializer are observed independently; no assigned work progress or fabricated manufacturing completion.

Use a disposable save with The Beginning Of Alchemy unlocked, a study table and a tier-I alchemy table. Keep the probe installed throughout the full restart. Do not discover/manufacture the tested elixir through other paths while the snapshot is active. F8 -> Actions -> Prepare -> Both unknown. Finish its short study-table recipe job; expect a native unlock popup and two recipe cards at the tier-I alchemy table. Save, fully exit and launch/load again; open the native table and F8 status. Preserve each launch log before the next launch overwrites it. Cleanup returns the original formula knowledge, removes diagnostic metadata/job markers and cancels loaded probe work; faith drops follow native cancellation, science/energy are not restored. Save after cleanup before removing DLL. Optionally repeat with One known; reward must add one variant and the finished topic disappear. Synthetic setup is not proof of reward.

Tests/KnowledgeStateTests.cs runs the actual state functions against native GameSave objects: 8 baseline/setup combinations, duplicate reward, restored snapshot on a fresh object, duplicate cleanup and missing-snapshot rejection. It proves diagnostic state logic, not serialization or UI. Compilation uses the build script's references and a .NET Framework executable; proprietary assemblies and test executables stay ignored.
