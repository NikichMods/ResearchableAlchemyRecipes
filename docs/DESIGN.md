# Design and next work

Current direction: study-table research plus an informational alchemy journal. This is design work, not a verified implementation architecture.

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
