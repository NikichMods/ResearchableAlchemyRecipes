# Design and next work

Current direction: study-table research plus an informational alchemy journal. This is design work, not a verified implementation architecture.

## Current recovery entry point — 2026-10-12

The concept/naming/bootstrap chat, two bounded static passes and the first installed work-lifecycle test are complete. Continue from the findings below; do not restart product-direction comparison or repeat the completed probe merely for recovery.

- Adopted: Researchable Alchemy Recipes; public NikichMods/ResearchableAlchemyRecipes; Graveyard Keeper 1.407 / Windows PC / BepInEx 5.
- Adopted progression gate: journal information and product research open after vanilla The Beginning Of Alchemy is unlocked. Earlier product awareness does not by itself open access. A visible locked entry with explanatory text is proposed, not a finalized UI.
- The selected behavior is owned by PRODUCT_REQUIREMENTS.md: character awareness from an available verified source; research selected/performed at the study table; one product research reveals all remaining ordinary formulas and ingredient-source guidance; informational journal; chosen vanilla material studies and ordinary manufacture.
- Research access before later manufacturing-equipment availability (after the adopted foundational alchemy gate), exact prices/partial-knowledge pricing and the final UI remain open. Do not silently resolve them during technical work.
- The stable baseline is documentation-only. There is no production source/candidate or chosen production build setup. Research-only Study Table Work Probe 0.1.0 on research/study-table-work verified native start/cancel/save-menu-load/completion/cleanup. Its identity and evidence limits are in TEST_LOG.md. Production and final UI remain unaccepted.
- Existing awareness research is partial (26/34 structural/vendor candidate coverage, not proof of first exposure for every save). Existing mixed-output/ingredient intersection evidence is complete for the inspected ordinary 1.407 corpus and should not be repeated merely to recover context.
- Read VERIFIED_RUNTIME_DATA.md for accepted evidence identities, then inspect the corresponding shared/previous research. LOCAL_ENVIRONMENT.md locates local inputs; it is intentionally untracked.

First-pass findings: ordinary Survey is sample-dependent at selection, completion dialog and sample return. The common work owner is CraftComponent, and an independent work definition can be investigated without changing material Survey. Native craft restoration resolves a saved ID through GameBalance, so a transient UI-only definition is insufficient. The seven paint gaps have 26 keeper-room blueprint consumers and native persistent unlock/migration owners; Spices has an already accepted Merchant task source. Neither finding establishes full current-save source availability.

Second-pass findings: CraftComponent is a statically supported execution mechanism. Registration before GameBalanceBase.CreateIDsCache, restricted to GameBalance and idempotent by stable job ID, lets the subsequent native ID/station cache construction include the jobs. GUIElements.OpenCraftGUI selects the Survey inventory window when Survey definitions are present; job insertion alone cannot supply research selection. The normal tech_alchemy_begins graph covers all seven paint outputs through 22 of their 26 blueprint consumers; they are not established as initial-game topics. The native unlocked_techs list supplies the adopted access gate without requiring a new perk.

Runtime findings: controller selection, sample-free payment/manual work, native cancellation, in-progress save/menu-load resumption, completion marker and cleanup are confirmed. Partial progress after reload is user-observed; full process restart is not tested. The temporary A/B dialog is usable but not the desired final UX. 0.1.1 demonstrates the two-tab frame, keyboard/mouse and controller switching, picker select/cancel and reopening in the installed game. It remains unaccepted because the returned log reveals a key-map restoration defect and other UI diagnostics. Next bounded questions: (1) verify the 0.1.2 close fix/native icon presentation, then native formula-knowledge reward/persistence; (2) current builder/dialogue source predicates. Cancellation returns faith as a world drop, while station science and spent energy are retained. Research refund semantics remain open. Reuse prior evidence before diagnostics.

This recovery checkpoint authorizes no new production behavior by itself. Record the selected technical mechanism only after evidence supports it, and request a user decision only if the evidence changes the adopted product scope or exposes a real unresolved choice.

## Working interaction model

- The study table presents eligible product research topics, required resources and work. Its native integration seam must be verified; ordinary item-insertion UI must not force the player to possess the unknown product.
- The journal lists available research and studied products. Selecting a known product shows all known formula variants; selecting an ingredient shows its acquisition routes and required vanilla material study.
- Short tooltips may be sufficient. Do not presume mouse/gamepad problems or require a separate panel without representative layout evidence; several source routes should remain readable and controllable.
- Actual save-backed formula knowledge takes priority. A partially known product can show its existing formulas and offer study of the remainder. How to price partial knowledge is unresolved; do not present an already-known formula as a new reward.

## Remaining user-owned decisions

1. After The Beginning Of Alchemy, can recipe research precede access to further manufacturing equipment/technology, particularly Advanced alchemy? The foundational access gate is adopted; later production capability remains separate.
2. Exact faith/science/energy/work costs and treatment of partially known products.
3. Final journal/table layout after representative UI evidence; whether a locked entry explains the alchemy prerequisite before access opens.
4. Cancellation behavior for paid research, including station science, after representative native behavior is verified.

These are open decisions, not implicitly accepted defaults. The separate paid reagent-source stage, per-formula research buttons, automatic study of all sources, automatic formula reveal on awareness and a combined targeted-purchasing feature are not the chosen direction.

## Technical questions and intended evidence

| Question | Why it matters | First evidence path |
| --- | --- | --- |
| Which sources are available to this save for every supported product? | Prevent unreachable topics and leaked global catalog; support old saves | Prior AlchemyRiddle census, shared crafting/dialogue/trading evidence, then bounded static inspection of uncovered cases |
| How does native formula knowledge render and persist after authored disclosure? | Recognize vanilla knowledge and display new research results without duplicate state | Shared UnlockAlchemy/completion evidence, actual final consumers and save reload path |
| How can a topic be offered and work completed at the study table without a target item? | The product is the research goal, not an input sample | Existing native Survey/GUI/craft owners; compare narrow native mechanisms before selecting one |
| How are ingredient routes and material-study readiness represented? | Cover ordinary producer paths as well as decomposition and show remaining gates correctly | Shared AlchemyDecompose/Study evidence, authored producer data, native visibility consumers |
| Which journal UI seam supports mouse and gamepad? | Readable formula variants and source details, correct input and layout lifecycle | Existing game UI/mod evidence and bounded representative UI inspection |

Research-method choice follows DevRules: reuse accepted evidence first, inspect direct source/data next, write a narrow diagnostic only if it closes a residual uncertainty more reliably. No new probe or production framework is required merely because the above list exists.

## Immediate next step

Test the research-only 0.1.2 correction after its exact identity is recorded in TEST_LOG.md. The 0.1.1 user report confirms keyboard/mouse/gamepad tab switching, native picker select/cancel, reopening, short work and F8 cleanup. Ten closes restore layout and hide both owners, but eight closes also produce Dictionary enumeration exceptions in BaseGUI.Update. Native formula disclosure remains a separate next evidence gate. Finish current keeper-room builder and Merchant-source predicates independently.

The close defect is local: restoring PrevTab/NextTab values mutates the dictionary while BaseGUI.Update enumerates its Keys. Retain the native window composition; install a separate key-map copy and restore the field reference, without mutating either enumerated dictionary. A .NET Framework regression reproduces the old failure and verifies the replacement strategy; installed-runtime confirmation is still required. This is cheaper and narrower than replacing BaseGUI.Update or postponing all close behavior.

Adopted presentation direction from the latest screenshots: icon tabs like vanilla workbenches. Direct inspection of the verified resources.assets found tab_book (33x25, quill/scroll) and tab_custom_alch (40x23, paired flasks). Candidate 0.1.2 uses native IDs book / ?alch and unchanged CraftTabGUI.Draw/SetSelectedState for icons and selection tint. Scoped SwitchTab interception continues to own this study-table composition; the ?alch name does not delegate its content to native MixedCraftGUI. No extracted artwork is distributed. The specific pair is a visual candidate, not final art acceptance.

The 0.1.1 log also contains six NGUI widget/panel layer warnings around a picker opening. Their cause is unassigned; the key-map fix does not prove their removal. Check the next capture before advancing UI readiness, and investigate remaining warnings through their actual owners rather than broad layer mutation.

Chosen research mechanism: native CraftGUI frame and CraftTabGUI tabs with the actual ResourceBasedCraftGUI Survey as a top-of-stack content subwindow, matching the inspected native mixed-alchemy composition. Keep the original sample filter and craft callbacks; temporarily hide only inspected Survey frame/header/dimmer, align content and bridge tab/close actions. Snapshot/restore changed layout and key delegates. The 0.1.1 composition has user-observed usability but unresolved log defects; 0.1.2 is its correction candidate. This is research UI, not production. Alternatives were moving only controls into a new hierarchy (more ownership/rebinding assumptions) or swapping two complete windows (different frames/sizes, less faithful to the desired two-tab UX). Independent formula reward is excluded from this candidate. The old 0.1.0 bytes stay frozen; 0.1.1 shortens only the diagnostic work-time input, not accepted economics.

Preferred table UX from the user's 0.1.0 feedback: two tabs, ordinary material Study and our recipe research, rather than an A/B modal before every interaction. Keep alternatives available only if the verified implementation cost warrants a real tradeoff. No final prefab/layout is selected. Avoid excessive work duration in future diagnostics; the user found this probe's 30 native work units uncomfortably long, and those values are not product economics.

The frozen 0.1.0 diagnostic entry intercepted only GUIElements.OpenCraftGUI for an idle mf_alchemy_survey after the native alchemy unlock. Normal selection calls the original dispatch under a recursion guard; research opens CraftGUI.OpenCraftList on the same table. Alternatives were adding controls directly to ResourceBasedCraftGUI (requires unverified prefab/layout ownership) or replacing Survey selection (changes its sample contract). The native dialog is the smallest test entry with already-inspected input/callback behavior, not the adopted final table design. User approval on 2026-10-12 accepted the research direction and next experiment; layout and cancellation economics remain open.

## Alchemy access and proposed onboarding — 2026-10-12

The user selected access after the vanilla alchemy introduction. Use actual `GameSave.unlocked_techs.Contains("The Beginning Of Alchemy")`, not merely revealed/visible technology state, a Merchant quest, possession of a product or a remembered conversation. Existing accepted Clotho research identifies the early memory-help branch and its Heal Potion/reagent alternatives, with The Beginning Of Alchemy as the granted technology; it does not make Merchant/Spices a universally first or mandatory entry.

Proposed presentation: retain a discoverable journal entry before access, showing a short alchemy-prerequisite explanation without product/formula lists. This lets a new player recognize that the mod is installed. Final location, wording and whether it is a tab remain open. A new research perk is not needed to encode the gate; consider one only if a separate player-facing purpose justifies it, not as a duplicate persistent flag. Do not assume exactly eight initial research topics: source eligibility and actual formula knowledge determine the list.

## First mechanism comparison — 2026-10-12

Goal: choose a known product at the study table, pay and perform work without its sample, preserve material Study, and resume correctly after save reload.

| Mechanism family | Evidence and consequence | Current judgment |
| --- | --- | --- |
| Reuse normal Survey with altered needs | ResourceBasedCraftGUI selects by first input; Survey completion reads item details and returns that input. Removing the sample alone is insufficient and requires changing several special-case consumers. | Do not pursue as a simple input edit. |
| Separate native work definition on the study table, topic selection using native craft cards | Common CraftComponent handles costs, manual work and completion; CraftGUI cards support explicit name/icon without a manufactured target output. Registration, entry UI, knowledge reward and reload still need verification. | Preferred research direction, not an accepted implementation architecture. |
| Separate mod work/payment/save machinery | Would duplicate already identified native owners and introduce independent persistence/progress state. | Defer unless native work proves unable to satisfy the contract. |

Shared static findings and their exact applicability limits are linked from VERIFIED_RUNTIME_DATA.md. No new UI layout, economics, equipment-access rule or production behavior was adopted in this pass.

## Rejected/deferred options and rationale

- Separate paid source lookup plus vanilla material study: an extra information-payment stage between recipe and selected material.
- Automatically study every alternative source: changes native progression/rewards and makes players pay for unnecessary alternatives.
- Separate research per formula: asks the player to choose unknown alternatives before knowing which is useful; selected unit is the product.
- Targeted Clotho purchasing: interesting independent mod; combining it would blur this mod's central feature.
- Technology-tree research: not the selected research interface after study-table choice.
