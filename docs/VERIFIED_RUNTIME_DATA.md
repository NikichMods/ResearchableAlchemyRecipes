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

0.1.3 returned visual evidence approves the original artwork appearance but shows displaced opaque artwork due to unequal PNG margins. No returned runtime log accompanies that report. The 0.1.4 correction centers the known alpha bounds on native tab background geometry; native pivots/parents and unchanged SetSelectedState placement ownership are inspected, with compiled binding but installed centering still pending. Science in the header is native additional_header_items data and native UniversalObjectInfoGUI item rendering; its perceived roughness has no established cause.

Returned 0.1.4 log identity/counts are in TEST_LOG.md: expected art offsets execute without probe errors, but optical placement remains unsatisfactory to the user. Research-only 0.1.5 therefore exposes bounded F2 calibration on the established anchor, with session-only offsets and chosen-value export. UICamera.ignoreAllEvents is a public static bool read at the installed firstpass Update entry with early return; it can temporarily contain NGUI events while IMGUI owns calibration input. Scoped table Update guards cover semantic keys and Survey's direct Interaction shortcut. Installed restoration/control operation is pending, not inferred from compilation.

Returned 0.1.5 log/screenshot (TEST_LOG.md) confirms visible calibration, final chosen export and four input-restoration paths, plus six paired closes without probe errors. Selected optical offsets are now fixed in research candidate 0.1.6; temporary calibration/input guards are removed. No picker opens/layer warnings appear in this capture, so the prior picker warning is not closed. Existing work/reward/save/source boundaries remain unchanged.

## Native formula knowledge final consumer — static 2026-10-12

Pinned source: Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9, Assembly-CSharp/{SmartExpression,CraftGUI,BaseCraftGUI,GameSave,TechUnlockDialogGUI,TechDefinition,CraftComponent}. Local 1.407 compilation binds the diagnostic to these owners; installed reward/persistence remains pending.

- Authored UnlockAlchemy adds an exact craft ID to completed_one_time_crafts and opens a forced pseudotech unlock dialog. It does not call manufactured OnFinishedCraft or unlock the simplified zombie craft. Random disclosure has additional achievement behavior and is not interchangeable.
- CraftGUI.OpenAsAlchemy builds its native list from those exact IDs, resolving each definition and requiring its craft_in to contain the opened station. CommonOpen further applies native IsCraftVisible and craft-type filtering. Thus research can supply native knowledge without a second formula ledger.
- TechUnlockDialogGUI.Open supports forced_unlock=true and pseudotech=true; OK then does not UnlockTech. A temporary TechDefinition can carry newly learned crafts, with native visible-unlock capacity three. The representative group has two formulas; combined presentation remains an installed UI check. Native generic technology-unlock wording is diagnostic, not selected final wording.
- GameSave.ToBinary captures the scene then calls SmartSerializer.Serialize<GameSave>; FromBinary deserializes GameSave. PrepareAfterLoad is the loaded-state boundary. completed_one_time_crafts is a public serializable list; native serialization/full-restart retention still requires real observation.
- CraftComponent reaches OnFinishedCraft before End; adding knowledge at the real research completion preserves native work/payment ownership. No fake manufacturing completion is needed.
- Native DialogButtonsGUI has three text slots but only two initialized key slots; its gamepad redraw can index past that pair for a third button. This probe uses only two-option dialogs, with explicit nested actions.

The ordinary representative pot_heal group has two nonhidden, needs_unlock=false formulas on mf_alchemy_craft_02 in the inspected local balance. Actual loaded group count is asserted before registration. No ingredient IDs/formulas are published. Shared native facts are promoted to GraveyardKeeperResearch/docs/ALCHEMY_SYSTEM.md.


## Native knowledge runtime closure — returned 0.1.7

Exact installed candidate, two returned launch logs and user screenshots/report (identities in TEST_LOG.md) confirm real sample-free completion -> remaining-only exact native formula knowledge -> native grouped popup and tier-I list -> full process restart -> retained native knowledge. Unknown setup learns two, partial setup learns one; both reach two native recipe cards. Cleanup returns the original zero knowledge and final native serialization records cleaned state. No duplicated production knowledge ledger is required for this proven path.

Limits: one product/station and current mod set; actual manufacture and loaded nonzero-baseline restoration were not separately exercised. Popup wording/pseudotech lookup warning need presentation refinement; source eligibility, component guidance, economics and prior picker warnings remain open. Initial zero knowledge is log-measured; no initial native-list screenshot was taken. This closes the bounded mechanism dependency, not production readiness.

## Source eligibility static follow-up — 2026-10-12

Pinned source remains Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9. The local Assembly-CSharp hash was rechecked and matches the accepted 1.407 identity. Read-only resources.assets graph inspection recovered these source owners (UTF-8 serialized graph hashes): npc_merchant, path 150541, fb8815cf25d381e3e3a7bc112cd93e45aea481a7c90338e8d3aea854dd6f88b2; npc_witch, path 150562, 3b1254898077562cffc9adba7cb339144dd6d5842ca2c522d522c72de8610108; keeper_room_builddesk, path 150502, b47867e06a6ce983b5549632d6063d465c5b516f7ea95880d69743e46a6785cf. Raw graphs remain private.

- MainGame.OpenBuildObjectGUI constructs a CraftsInventory from craft_obj_data where builder_ids contains the real desk definition and GameSave.IsCraftVisible passes. Its only explicit early-return gate is the current in_tutorial=1/tut_shown_tut_1=0 state. CraftGUI.OpenAsBuild then renders the native inventory. Unaffordable building costs do not remove the need from this source list.
- Keeper-room ObjectDefinition: Builder, zone home, unconditional interaction_1, no pre-interaction script and attached keeper_room_builddesk script. The attached graph handles first-use speech/intro; it contains no craft unlock or source filter. This does not itself prove that a particular save has a reachable desk. Loaded WorldMap presence, removed/disabled state and actual opened native cards must be observed.
- Merchant graph sets merchant_curse Visible, then unlocks the Clotho-directed phrase. That task alone does not prove that the player has been shown the Spices requirement. The root Merchant menu's cure answer has a SmartRes price of one Spices and no populated separate lock, and is filtered by phrase unlock/blacklist state.
- Important newly verified disclosure: Witch graph node 140 unlocks @merchant_on_cure and immediately node 142 runs Flow_RevealCrafth for the one ordinary Spices formula (silent=false, not_add_to_one_time=false). The exact craft resolves to taste_booster:alchemy_1_spice; ordinary group size is one. This contradicts treating Spices as a guaranteed still-unknown paid research topic after Clotho's explanation. Do not publish the concrete formula. Existing formula knowledge must exclude it from paid research at that point.
- Merchant menu cure answer is directly root-listed (#7); its successful branch blacklists @merchant_on_cure and marks merchant_curse Complete. UnlockPhrase does not delete its ID when the answer is blacklisted. These are persistent native records; day/current NPC movement and other root reachability remain distinct from phrase state.
- MultiAnswerGUI.ShowAnswers filters by nonempty ID, @-phrase unlock and blacklist, then renders even an unpickable answer. AnswerData resource sufficiency sets can_be_picked=false but does not hide that answer. For awareness of a displayed need, the product hand-in price must not become an ownership prerequisite. Parent navigation still needs enough resources to reach the actual mentioning surface where applicable.
- KnownNPCList.GetNPC is noncreating; GetOrCreateNPC would register a meeting and can invoke achievements. Eligibility readers must use the noncreating path.

Research-only Source Eligibility Probe 0.1.0 measures those native loaded inputs and actual final builder/dialogue presentation without modifying formulas, progression, work or save state. Scope: seven paint gaps and Spices; this is not a 34/34 eligibility claim or production topic registration. Installed positive availability evidence remains pending.

## Source observer regression limit

Returned Source Eligibility Probe 0.1.0 run fails to finish loading; removing only it restores loading per user report and native successful-baseline log. No source census is reached. Thus the current-source eligibility hypothesis is still unverified; prior static graph/visibility/disclosure findings are unaffected. 0.1.0 is withdrawn. GameBalance.me can invoke native loading/cache construction when its instance is absent, so it must not be used as a harmless readiness check. That source fact identifies a risk, not the demonstrated cause of this freeze. Zero-patch manual 0.1.1 containment uses the already-loaded field and explicit post-load trigger, with installed execution pending. Exact evidence/candidate identity belongs to TEST_LOG.md.

## Loaded source agreement — Source Eligibility Probe 0.1.1

Returned exact-DLL/log evidence in TEST_LOG.md confirms successful native loading and one completed manual F9 census. Native IsCraftVisible room-blueprint counts and already-rendered OpenAsBuild cards agree for all 26 records/seven paint products on this save. Actual shown room builder confirms the exercised positive access boundary. Two products are natively known and five remain researchable within this measured source subset. Spices has one known formula and a completed Merchant task, so no missing formula remains.

This closes the bounded positive room-source reconstruction dependency: read native unlocked technology, actual available room builder and its native blueprint visibility, then subtract existing formula knowledge. It does not establish negative/early states, every possible physical/source gate, all 34 product sources or production automatic-topic behavior. Manual containment avoids the prior load freeze in this returned run; original freeze cause remains unknown.

The same log reveals 78 repeated MaxButtonsRedux gamepad-input null-reference stacks after our Study window opens and before F9. Actual interaction/input state must be inspected before source-topic/UI expansion; root cause is not assigned. This compatibility gate is separate from the verified source-card agreement and is not a clean-vanilla result.

## Study / MaxButtonsRedux compatibility mechanism — 0.1.8

Installed MaxButtonsRedux.dll SHA-256 e811fc228ece102e389759d278bb66569884898b827369f99e11c984ffc2af30, MVID 830fce4e-6560-414e-a4db-3cc67ed26713. Direct bounded IL inspection (private scratch, no copied source in repository) establishes: CraftGUI.Open sets the foreign craft-open flag; only CraftItemGUI.OnOver assigns its selected-card reference; HandleGamepadInput routes to HandleCraftGuiInput when the flag is true; the checker immediately dereferences selected card/current craft/needs without null guards. Empty or non-card native content violates this assumption. True from the misleadingly named IsCraftGuiInputValid means invalid: the caller returns without handling quantity buttons. It rejects faith-input and one-time crafts.

Our composite frame intentionally keeps CraftGUI open while actual Survey owns the top window. Source0.1.1 capture opens it with no prepared diagnostic recipe card and then throws in that foreign checker. This establishes the incompatible empty/non-card assumption; the exact null receiver in that live frame was not captured. An offline invocation of the actual installed checker with its selected-card field null reproduces the NullReferenceException.

Chosen research-only correction: narrowly return that checker's native invalid-input result only while our exact study-table composition owns the frame. Both diagnostic tabs have no quantity operation: Survey selects one sample and research is faith-paid/one-time. Other tables and inactive/closed composition fall through to the original method. No foreign DLL edits, fake craft card, injected hovered item, global gamepad flag mutation or alternative UI hierarchy. Hiding the native frame would interfere with the selected composition; changing/restoring shared foreign flags would add another state owner. A scoped semantic input check is narrower.

Optional integration resolves the already-loaded MaxButtonsRedux assembly, verifies its exact inspected hash and static parameterless bool checker, then installs one prefix. Absent mod is ignored; a changed unverified contract disables this research probe rather than guessing foreign internals. This diagnostic exact-identity boundary does not select a general future production compatibility policy. Zero per-frame reflection/hash scans; only an existing callback asks our stable composition predicate. One session diagnostic records entering the scope.

QuantityCompatibilityTests runs against the actual installed foreign DLL: original null-card failure, scoped invalid result, inactive fallthrough preserving the original failure, and unpatch restoration all pass. This verifies real Harmony binding and skip/fallthrough polarity outside the game, not our runtime window/focus transitions. Compilation passes. Installed acceptance remains: ordinary Study/sample picker and both tab switches with gamepad, closing/reopening, plus normal Min/Max on an unrelated quantity-enabled workbench. Do not repeat source census or paid formula/restart matrix for this input-only change. Formula knowledge, costs, icons, native keys and save behavior are unchanged.

## Single-work protection and load-order correction — 0.1.9

Reopened compatibility evidence: returned 0.1.8 never bound MaxButtons because assembly lookup happened before its plugin loaded. Add an optional BepInDependency on the actual installed GUID p1xel8ted.gyk.maxbuttonsredux; BepInEx documents dependency-before-consumer ordering and SoftDependency permits absence (https://docs.bepinex.dev/v5.4.16/api/BepInEx.BepInDependency.html and DependencyFlags). Offline metadata regression checks that the dependency matches the real plugin attribute. Installed READY must now say bound; not_installed on this installation is a failure.

Actual Hide paths also need a closure notification: the inspected foreign CraftGUI_OnClosePressed callback has no parameters and only clears its craft-open flag. Invoke that already-owned semantic observer after our actual owned frame Hide, scoped by the captured ownership state. Offline real-DLL regression verifies the flag is cleared. Other-table/native callbacks and global gamepad behavior remain unchanged.

Native CraftDefinition.CanCraftMultiple reads disable_multi/force_multi/one_time, but the installed QueueEverything postfix may replace its result. Repair only our own registered job inputs before card Draw; then a lowest-priority result guard enforces false for our exact jobs. This result guard is justified because data-only projection is not authoritative after the foreign override. A forced-true fixture followed by the real guard proves own job remains false and an unrelated recipe retains true.

The actual payment writer is CraftComponent.CraftReally: craft_amount stores requested amount, item needs scale by it, station needs remove the same amount; End repeats until craft_amount<=1 and restarts queued work. Protect that pre-payment point with ref amount=1 and fail before resources for active, completed, no-missing-knowledge or nonpositive requests. EnqueueCraft deduplicates the same work, limits the initial entry to one, and rejects active/finished duplicates. TryStartCraftFromQueue removes completed own jobs and normalizes own finite/infinite counts. First finish drops all own pending queue entries and normalizes legacy prepaid craft_amount, restoring only the excess prepaid item/station costs. Cleanup removes queued test jobs before native cancellation. Native vanilla formula/source definitions and unrelated queued work are untouched.

Local compile and regressions pass: foreign null checker/guard/fallthrough/unpatch, actual plugin GUID dependency, own-close observer, repaired definition through native CanCraftMultiple, final forced-true-result containment and selective queue cleanup. Actual resources, game lifecycle, queue execution and old prepaid refund need installed evidence. All of this is research-only; no production readiness or general other-mod matrix is claimed.
