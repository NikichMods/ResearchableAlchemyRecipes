# Design and next work

Current direction: study-table research plus an informational alchemy journal. This is design work, not a verified implementation architecture.

## Current recovery entry point — 2026-10-12

The concept/naming/bootstrap chat is complete. Continue concrete mechanism verification in the next project chat; do not restart the product-direction comparison or treat the previous chat transcript as required input.

- Adopted: Researchable Alchemy Recipes; public NikichMods/ResearchableAlchemyRecipes; Graveyard Keeper 1.407 / Windows PC / BepInEx 5.
- The selected behavior is owned by PRODUCT_REQUIREMENTS.md: character awareness from an available verified source; research selected/performed at the study table; one product research reveals all remaining ordinary formulas and ingredient-source guidance; informational journal; chosen vanilla material studies and ordinary manufacture.
- Research access before manufacturing-equipment availability, exact prices/partial-knowledge pricing and the final UI remain open. Do not silently resolve them during technical work.
- The project is documentation-only. There is no production source, candidate, chosen build setup or runtime acceptance. Implementation remains subject to the independent readiness gates.
- Existing awareness research is partial (26/34 structural/vendor candidate coverage, not proof of first exposure for every save). Existing mixed-output/ingredient intersection evidence is complete for the inspected ordinary 1.407 corpus and should not be repeated merely to recover context.
- Read VERIFIED_RUNTIME_DATA.md for accepted evidence identities, then inspect the corresponding shared/previous research. LOCAL_ENVIRONMENT.md locates local inputs; it is intentionally untracked.

Begin with two bounded questions: (1) what native study-table owner can offer and perform a target-product research job without a sample of that product; (2) what save-backed availability conditions cover the residual target-awareness gaps. Reuse prior evidence and direct inspection first. Before writing a diagnostic, state the residual uncertainty and why existing paths do not answer it. If a product has no verified awareness source, surface that gap instead of opening the full catalog or inventing a source.

This recovery checkpoint authorizes no new production behavior by itself. Record the selected technical mechanism only after evidence supports it, and request a user decision only if the evidence changes the adopted product scope or exposes a real unresolved choice.

## Working interaction model

- The study table presents eligible product research topics, required resources and work. Its native integration seam must be verified; ordinary item-insertion UI must not force the player to possess the unknown product.
- The journal lists available research and studied products. Selecting a known product shows all known formula variants; selecting an ingredient shows its acquisition routes and required vanilla material study.
- Short tooltips may be sufficient. Do not presume mouse/gamepad problems or require a separate panel without representative layout evidence; several source routes should remain readable and controllable.
- Actual save-backed formula knowledge takes priority. A partially known product can show its existing formulas and offer study of the remainder. How to price partial knowledge is unresolved; do not present an already-known formula as a new reward.

## Remaining user-owned decisions

1. Can recipe research precede access to the equipment/technology needed to manufacture the product? Awareness and production capability can differ.
2. Exact faith/science/energy/work costs and treatment of partially known products.
3. Final journal/table layout after representative UI evidence.

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

Classify the residual source-eligibility gaps using prior accepted evidence and direct data, and verify study-table topic/work ownership. Report what is proved and what remains unknown before production implementation. Detailed price tuning and visual polish can follow mechanism verification.

## Rejected/deferred options and rationale

- Separate paid source lookup plus vanilla material study: an extra information-payment stage between recipe and selected material.
- Automatically study every alternative source: changes native progression/rewards and makes players pay for unnecessary alternatives.
- Separate research per formula: asks the player to choose unknown alternatives before knowing which is useful; selected unit is the product.
- Targeted Clotho purchasing: interesting independent mod; combining it would blur this mod's central feature.
- Technology-tree research: not the selected research interface after study-table choice.
