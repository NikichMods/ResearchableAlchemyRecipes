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

- First-version guidance lives with known formula components at the native alchemy tables; a standalone journal/character-window tab is deferred and is not required for the initial feature. Reconsider it later as optional consolidated browsing, not a prerequisite.
- Research starts at the study table. Known formulas remain available through ordinary alchemy-table lists.
- Ingredient guidance must be available for repeated reference while inspecting a known formula: reagent -> source alternatives -> processing method/equipment/conditions -> remaining material-study requirement.
- Recipe information and product research become accessible only after The Beginning Of Alchemy is actually unlocked. Apply the same progression gate to contextual recipe guidance and the research entry at the study table. A pre-unlock locked entry is still an open presentation choice.
- Product entries use native item icons and names; a known product shows its known alternative formulas and ingredient composition. Partial knowledge must not reveal still-unknown variants before the remaining product research.
- Product -> ingredient -> acquisition methods supplies the relevant material information; an unrelated all-material encyclopedia is not required.
- Display remaining vanilla material-study requirements clearly and account for completed studies.
- For a source blocked by native material Study, identify the material and make the next action clear: study that source at the study table before processing. Do not automatically study it as part of recipe research.
- In the selected recipe card, ingredient widgets sit opposite the product name, with energy on the same horizontal line as the ingredients; source guidance follows below.
- Tooltip vs detail-panel presentation and the exact mouse/gamepad layout remain subject to inspection and usability verification.

## Existing saves and invariants

- Installing the mod after progression must not require restarting or replaying technology unlocks merely to register research topics. Reconstruct current eligibility and knowledge from verified save-backed/native state on load.
- Read the existing technology unlock state on old saves; opening access must not depend solely on observing a new Clotho conversation or a future unlock event.
- Awareness of a past disappeared item or interaction cannot be promised unless a persistent host record is established. Current-source eligibility is the selected rule.
- Preserve vanilla source-material study and ordinary manufacturing. Unknown formulas are not automatically revealed just because the product becomes a research topic.
- Targeted recipe purchasing from Clotho is a separate possible future mod, outside initial scope.
- No logic minigame, deduction requirement, generator or UI is inherited from AlchemyRiddle.

## Acceptance envelope

A representative unknown product can be researched without owning it; the native alchemy-table list shows usable formulas and contextual component guidance supplies their acquisition routes. A chosen unstudied source is explicitly identified as needing vanilla study; unrelated alternatives remain unstudied and unpaid. Previously known formulas/materials are recognized. Loading an existing save yields correct current topics. Repeated manufacture and repeated information viewing do not charge another knowledge fee. Save reload preserves the resulting knowledge. Exact research economics and interface acceptance will be defined before the corresponding candidate.
