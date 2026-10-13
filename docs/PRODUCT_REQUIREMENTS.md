# Product requirements

Status: adopted working concept. Project name and public repository approved on 2026-10-11. Research-only mechanism candidates exist; production implementation is not yet started.

## Outcome

For an alchemical product known to the character, the player can follow a predictable in-game route from missing knowledge to ingredients and manufacture, without mandatory brute force, recipe deduction or an external wiki. The player remains responsible for ordinary material acquisition, vanilla material study, equipment and production.

## Adopted player flow

1. An available, verified in-game source mentions a product. This counts as character awareness even if the player has not opened that source's window.
2. After the character has unlocked the vanilla The Beginning Of Alchemy technology, eligible products appear as research topics. Earlier awareness alone does not open the mod's research/information access. External player knowledge does not reveal the entire catalog.
3. The player selects, pays for and performs research at the study table. Possession of the target product is not required.
4. Research reveals all ordinary alternative formulas for the product and the acquisition methods of their ingredients.
5. The player chooses suitable material sources, performs any required vanilla material studies, produces ingredients and uses ordinary mixed alchemy.

## Knowledge and payment

- Reveal ingredient-source information with product research; no separate paid reagent-source research is required.
- Show all source alternatives within the supported host-data scope, each material, processing method, equipment and required conditions. Include ordinary producer paths where relevant, not just alchemical decomposition. Do not explain world acquisition of the source material itself.
- Show alternative sources without automatically completing their vanilla studies or charging for all of them.
- Respect previously known formulas and material studies, irrespective of whether knowledge came from crafting, vanilla authored disclosure or this mod.
- Recipe research is a single work operation. Quantity/queue extensions must not charge for repeated copies of the same research, and fully known products must not start another paid investigation.
- One product research reveals its remaining formulas. If some are already known, display them immediately and label the remaining topic as studying other manufacturing methods.
- Knowledge remains available for repeated reference and across recipes. Ordinary repeat manufacture pays production costs, not another information fee.
- Research uses faith, science, energy and work time in the style of vanilla study. Exact values are deferred. Recipe knowledge and guidance are the reward; additional red/green/blue technology-point rewards are not required.

## Study-table presentation

- Present ordinary material Study and recipe research as two tabs in a common native-style frame. Tabs use icons, as on vanilla workbenches, rather than permanent text captions. Exact icon choice and final presentation remain subject to visual acceptance.

## Contextual recipe guidance

- Final mod presentation must support all game languages and the complete supported ingredient population. Current research prototypes use Russian; localization implementation is deferred, not removed from final scope. Visual grouping must not depend on Russian names or one representative reagent's colours/line lengths.
- First-version guidance lives with known formula components at the native alchemy tables; a standalone journal/character-window tab is deferred and is not required for the initial feature. Reconsider it later as optional consolidated browsing, not a prerequisite.
- Research starts at the study table. Known formulas remain available through ordinary alchemy-table lists.
- Ingredient guidance must be available for repeated reference while inspecting a known formula: reagent -> source alternatives -> processing method/equipment/conditions -> remaining material-study requirement.
- Recipe information and product research become accessible only after The Beginning Of Alchemy is actually unlocked. Apply the same progression gate to contextual recipe guidance and the research entry at the study table. A pre-unlock locked entry is still an open presentation choice.
- Product entries use native item icons and names; a known product shows its known alternative formulas and ingredient composition. Partial knowledge must not reveal still-unknown variants before the remaining product research.
- Product -> ingredient -> acquisition methods supplies the relevant material information; an unrelated all-material encyclopedia is not required.
- Display remaining vanilla material-study requirements clearly and account for completed studies.
- For a source blocked by native material Study, identify the material and make the next action clear: study that source at the study table before processing. Do not automatically study it as part of recipe research.
- Use a compact "не изучено" marker on each affected source route and one conditional explanation below the block. Adopted Russian prototype wording: "Получение ингредиентов из отмеченных материалов требует их изучения на исследовательском столе." This explains a condition rather than commanding the player to study every alternative. Keep source/equipment/marker together on one line when the available width permits; first use the available width without shrinking the native font. Final long-row behavior remains subject to visual evaluation.
- Ingredient section names are headings with readable hues associated with their icons. The user adopts the reviewed complete 35-ingredient palette in REAGENT_HEADING_PALETTE.md; stable item IDs own colours. Source alternatives remain neutral and pending-source marks remain amber. Live rendering acceptance is separate from palette approval.
- Preserve the native recipe card/window geometry, ingredient/energy positions, navigation and quantity controls. Display source guidance separately beside the window in the familiar native tooltip visual style, immediately following mouse hover or gamepad focus. Inline expanding cards and enlarging the shared alchemy window are rejected after wider usability testing.
- The next visual prototype uses height determined by text content, with no scrolling implementation yet. A competing workbench-style panel and two-sided comparison are dropped; native-tooltip appearance is chosen. Exact placement, fit and input behavior still require inspection and usability verification.
- Do not add an "any quality" note. Show a specific quality only where genuinely required. The one-time verified current source corpus has no such requirement, so show plain crop-family names without per-display quality filtering; no quality-policy subsystem is required now.
- Anchor the tooltip beside the native window's lower-right corner, growing upward with measured clearance for quantity arrows. Use a rectangular tooltip without a tail. Avoid revealing intermediate layout/position passes; eliminating the visible jump takes priority over retaining an extra clearance correction. Add separation between product title and source-information body; use readable, visibly coloured foregrounds for parchment.
- Use a narrow vertical layout with equipment subheadings and individual material rows. Preserve extraction/burning/inventory-use explanations for now; their removal is not adopted. Compatible combined equipment levels use Roman digits and comma-space, e.g. `I, II`.
- Latest visual feedback prioritizes immediately scannable reagent/equipment/material indentation over maximum narrowness. Do not split study-marker words/brackets or quantities into orphan fragments. The user requests Roman equipment levels (e.g. `I, II`), superseding the preceding numeric presentation choice. English and Russian are the first localization/acceptance targets; preserve the same structure for all native game languages, using current-language font/text measurements rather than Russian fixed lengths. This does not authorize or require full translation implementation in this visual iteration.
- Reduce repeated equipment names by equipment subheadings and material lists. Preserve each source's native study/availability conditions and output counts; combine equipment tiers only where their applicable conditions/results are compatible. Omit DLC-exclusive source alternatives when the native DLC availability check says that DLC is absent.

## Existing saves and invariants

- Installing the mod after progression must not require restarting or replaying technology unlocks merely to register research topics. Reconstruct current eligibility and knowledge from verified save-backed/native state on load.
- Read the existing technology unlock state on old saves; opening access must not depend solely on observing a new Clotho conversation or a future unlock event.
- Awareness of a past disappeared item or interaction cannot be promised unless a persistent host record is established. Current-source eligibility is the selected rule.
- Preserve vanilla source-material study and ordinary manufacturing. Unknown formulas are not automatically revealed just because the product becomes a research topic.
- Targeted recipe purchasing from Clotho is a separate possible future mod, outside initial scope.
- No logic minigame, deduction requirement, generator or UI is inherited from AlchemyRiddle.

## Acceptance envelope

A representative unknown product can be researched without owning it; the native alchemy-table list shows usable formulas and contextual component guidance supplies their acquisition routes. A chosen unstudied source is explicitly identified as needing vanilla study; unrelated alternatives remain unstudied and unpaid. Previously known formulas/materials are recognized. Loading an existing save yields correct current topics. Repeated manufacture and repeated information viewing do not charge another knowledge fee. Save reload preserves the resulting knowledge. Exact research economics and interface acceptance will be defined before the corresponding candidate.
