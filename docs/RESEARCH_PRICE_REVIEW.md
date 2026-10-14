# Research price and utility review — 2026-10-14

Status: current reduced science tariff adopted as the working balance; historical higher ranges remain exploratory evidence. Research runtime costs stay diagnostic. The prior quantitative study counted material Study expenditure only; it did not value failed mixtures, material acquisition/processing time, certainty of outcome or convenience. Its 2–3 science result is a comparison anchor, not a ceiling on a richer player-facing fee.

## Current reduced-price working list

Current working science tariff is 1/2/4/8/10. The user approves the grouped list and moves the fertilizer energy elixir from 8 to 4 because its first use belongs to earlier progression. Decorative paint 1 is the explicit exception to whole-paper even pricing. Stage names describe approximate role/value groups, not automatic availability gates or a dynamic price rule. The complete assignments below are the working balance; installed runtime costs remain diagnostic and faith is fixed at zero; exact energy/work interpolation and partial treatment remain unresolved. Historical higher-price tables below are comparison evidence.

| Product | Science |
| --- | ---: |
| Yellow paint | 1 |
| Green paint | 1 |
| Brown paint | 1 |
| Red paint | 1 |
| Dark green paint | 1 |
| Dark violet paint | 1 |
| Violet paint | 1 |
| Blue paint | 1 |
| Black paint | 2 |
| White paint | 2 |
| Glue | 4 |
| Acid | 2 |
| Alkali | 4 |
| Aromatizer | 8 |
| Dark elixir | 8 |
| Energy elixir (fertilizer) | 4 |
| Balsamic solution | 4 |
| Flavor enhancer | 8 |
| Freshener | 4 |
| Golden elixir | 10 |
| Growth enhancer | 4 |
| Preservative | 4 |
| Silver elixir | 10 |
| Tanning agent | 8 |
| Appetite potion | 4 |
| Armour potion | 4 |
| Berserk potion | 4 |
| Damage potion | 4 |
| Healing potion | 2 |
| Long restoration potion | 4 |
| Energy potion (drinkable) | 4 |
| Speed potion | 8 |
| Zombie juice | 4 |

Totals after the subsequent explicit corrections: 17 practical topics 90, eight decorative topics 8, eight drinkable potions 34; all 33 topics 132 science = 66 sheets of paper-equivalent value. Black/white paint, growth/flavor enhancers and zombie juice total 20 science. These are research fees only. The user adopts flavor enhancer 8, healing 2, dark elixir 8, growth enhancer 4, freshener (incense I) 4 and aromatizer (incense II) 8. The earlier request for both tier-II fertilizer inputs at 8 is superseded: energy_elixir for boost I/II now costs 4; flavor_enhancer for quality II stays 8. These serve separate fertilizers, not one combined recipe. A single energy-elixir research serves both boost tiers. Native consumer census confirms energy_elixir -> sack_clock_silver/gold, growth_enhancer -> sack_star_silver, flavor_enhancer -> sack_star_gold, freshener -> incense_1 and aromatizer -> incense_2. The other assignments retain the approved working list; no runtime costs changed. Gold/silver 10 is a modest premium over 8 for advanced corpse improvement, not a measured objective value. Price by usefulness/progression rather than valuable-sounding names alone. Final faith, work, energy and partial-knowledge treatment still affect total player burden.

## User's personal priority split of the reduced list

The user identifies 20 products as personally important, not universally mandatory for completion: black/white paint, glue, acid, alkali, aromatizer, dark elixir, fertilizer energy elixir, balsamic solution, flavor enhancer, freshener, golden elixir, growth enhancer, preservative, silver elixir, tanning agent, healing potion, drinkable energy potion, speed potion and zombie juice. At current fees these total 104 science (52 paper): four topics at 2, nine at 4, five at 8 and two at 10. The remaining 13 topics are optional for this player: eight decorative paints at 1 (8 total) and appetite/armour/berserk/damage/long-restoration potions at 4 (20 total), totaling 28 science (14 paper). Full catalog is now 132 science (66 paper). This personal split replaces neither the earlier computational 17-product population nor source/eligibility rules; it is a new pricing-burden lens. These figures exclude ordinary material Study and all non-science research/production costs; already-known topics can reduce actual expenditure.

## Faith and native energy/work gradient

Adopted: science tariff fixed; recipe research faith cost 0. Ordinary material Survey costs remain unchanged. Adopted direction: map cheapest ordinary material Study work/energy endpoints to the science-1 group, largest endpoints to science-10, with smooth intermediate costs. No price scaling by formula count follows from this direction.

Read-only 1.407 game_data census on the accepted resource/assembly hashes, ignored study-time-energy-private.py/json: 223 paid single-input Survey/Alchemy definitions have work/energy pairs 0/0 (1), 1/0 (29), 1/1 (18), 1/2 (3), 2/5 (64), 2/10 (2), 3/5 (84), 10/10 (3), 10/15 (19). The zero-work exception is the authored Astrologer quest-key study, not an ordinary material lower endpoint. The previously verified 38 non-slime alchemy-source materials have 1/0 (9), 1/1 (1), 1/2 (3), 3/5 (24), 10/15 (1). Thus ordinary source endpoints are work 1/energy 0 and work 10/energy 15. All these expressions are simplified numeric constants; reading default_value alone would incorrectly report zero. Shared STUDY_RESOURCE_ECONOMY.md owns the reusable corpus facts.

Proposed numeric mapping: let s be the topic's science price, x=(s-1)/9; work=1+9x=s, total authored energy=round(15x), nearest integer. Interpolate by science price, not equally spaced group index. Endpoints exactly match ordinary material studies; integer rounding only affects energy at s=2/8.

| Science | Faith | Native craft_time/work | Authored total energy |
| ---: | ---: | ---: | ---: |
| 1 | 0 | 1 | 0 |
| 2 | 0 | 2 | 2 |
| 4 | 0 | 4 | 5 |
| 8 | 0 | 8 | 12 |
| 10 | 0 | 10 | 15 |

These are proposed rounded values, not installed or specifically user-reviewed intermediate assignments. Native manual CraftComponent owns work/proportional energy spending; energy is the definition's whole-job budget, not per action. craft_time is a native work/progress parameter, not a measured wall-clock duration or promised number of key presses: animation/actor/other installed work modifiers affect actual time. Full catalog totals under this proposal are 132 science, 132 native work and 168 energy; personal important set 104/104/143, optional set 28/28/25. This is expenditure over separate jobs across progression, not one session. Existing diagnostic 0.1.41 remains unchanged (faith/science/work/energy 1/1/2/10). Do not mutate its frozen DLL. Final tariff integration and lifecycle/refund checks require a later exact candidate.



## Historical higher-price proposal

The user explicitly describes these values as preliminary and requests a usefulness review, especially for embalming products. Decorative research should have a small but meaningful price; one science feels token-like to the user. Early/middle themes suggest 5–10, middle roughly 10, late roughly 15. Speed is considered especially valuable and proposed at least 10. A native quest asking for an alchemical product must establish immediate awareness; that source rule is adopted independently of these prices.

| Product / group | User's preliminary science | Interpretation / open point |
| --- | ---: | --- |
| Eight decorative-only paints | 2 each | White/black have separate functional roles |
| White paint | 5 | Paper production |
| Black paint | Not individually priced | Illustrative 5 below follows the early-functional range; not a user-selected value |
| Glue, acid, alkali | 5 each | Acid's diary quest value is separate from acid-injection usefulness |
| Aromatizer | 10 | Incense II is a real consumer; ordinary story perfume is obtainable from NPCs and does not require manufacturing aromatizer |
| Dark elixir | 10–15 | Conditional utility deserves comparison with gold/silver |
| Energy elixir for fertilizer | 5 | Distinct from drinkable energy potion |
| Balsamic solution | 5–10 | Restore injection; depends on body condition |
| Flavor enhancer | 10–15 | Quality fertilizer II; progression/production benefit |
| Freshener | 5–10 | Incense I/soap; start of range was proposed at 5 |
| Golden elixir | 15 | Core advanced corpse improvement |
| Growth enhancer | 5–10 | Quality fertilizer I |
| Preservative | 5–10 | Preservation; value decreases with refrigeration |
| Silver elixir | 10–15 | Core advanced corpse improvement |
| Tanning agent | Around 10 | Held at 10 for the illustrative totals, not a resolved exact price |
| Five optional potions: appetite, armour, berserk, damage, long restoration | 5–10 each | User groups these as optional effects |
| Healing potion | No exact fee selected | Illustrative 5 below; its initial Clotho quest source is adopted |
| Drinkable energy potion | 5 | Separate topic from fertilizer elixir |
| Speed potion | At least 10 | Totals below use 10; not an upper bound |
| Zombie juice | 5 | Early practical/story-adjacent role |

## Community utility evidence

Player reports are qualitative, self-selected and progress/DLC-dependent. They are not a poll or proof of consensus. Mechanical role descriptions were cross-checked against the target's existing native consumer census; detailed corpse-state numerical rules are not newly runtime-verified here.

- Glue, lye/alkali, silver and gold recur in recommended corpse-improvement sets. [26-white-skull discussion](https://www.reddit.com/r/GraveyardKeeper/comments/1609uz2/), [organ modification discussion](https://www.reddit.com/r/GraveyardKeeper/comments/1i8ev1i/). Their individual use still depends on the body and player's goal.
- Dark injection is a way to prepare red-skull requirements for later silver/gold use; players also describe avoiding it through organ selection. It is useful flexibility, not necessarily a universally required top-tier research. [Alternative without dark injection](https://www.reddit.com/r/GraveyardKeeper/comments/1609uz2/), [zombie-efficiency discussion](https://www.reddit.com/r/GraveyardKeeper/comments/1ak9nlk/).
- Acid injection is often criticized for sacrificing white skulls when maximizing the body. Rare situational uses exist. That does not make acid itself low-value: it is directly requested for the Astrologer's diary. [Acid-injection discussion](https://steamcommunity.com/app/599140/discussions/0/1642038749332545930/). Native quest owner is verified separately.
- Restore and preservative have narrower niches: salvaging a decayed valuable body, or holding bodies before refrigeration. Some players rarely use them once storage/preparation facilities improve. [Restore/preservative discussion](https://steamcommunity.com/app/599140/discussions/0/1742228532895463588/), [practical upgrade path](https://www.reddit.com/r/GraveyardKeeper/comments/106r174/), [embalming discussion](https://www.reddit.com/r/GraveyardKeeper/comments/1bg3rvi/). Avoid the absolute label useless; player workflow matters.
- Speed is repeatedly described as major comfort improvement. Pricing it by practical value instead of optional/combat category is reasonable. [Early speed discovery](https://www.reddit.com/r/GraveyardKeeper/comments/eh9xpa/), [energy/comfort discussion](https://www.reddit.com/r/GraveyardKeeper/comments/1uxruv0/). No representative willingness-to-pay estimate exists.

Design judgment: glue/alkali 5, silver around 10 and gold 15 fit the user's proposed hierarchy. Consider dark around 5–10 rather than automatically 15, and balsamic/preservative around the lower 5 end, because late unlock is not itself sustained utility. Acid 5 is justified by its independent task. Aromatizer 10 should be justified through the optional manufacturing/church benefit, not a supposed compulsory perfume-crafting quest.

## Total burden and the comfort premium

Using the illustrative fills above (black 5, healing 5, tanning 10, speed 10), the user's ranges give:

| Population | Research fee | Paper-equivalent value |
| --- | ---: | ---: |
| 17 broad practical themes | 120–155 science | 60–77.5 sheets |
| Eight decorative themes | 16 | 8 sheets |
| Eight drinkable potions | 45–70 | 22.5–35 sheets |
| All 33 themes | 181–241 | 90.5–120.5 sheets |

The upper figure is not a true upper bound when speed can exceed 10 or tanning is moved above 10. These are catalog-wide sums for unknown topics; partial knowledge/previously learned formulas lower actual paid-topic counts. They exclude faith, energy and work costs.

With the earlier directed material-study minima, practical totals become 144–179 with monster jelly or 156–191 without it. The earlier narrow random/catchup comparator cost 66–74 / 70–77 science. Therefore this scale changes the product goal from approximately science-neutral information to a deliberate investment in certainty and comfort. That is a legitimate design choice, but must not be described as quantitatively recovered wasted-science cost.

For five infrastructure themes (black/white paint, quality fertilizers I/II and zombie juice), research alone is 30–40 science. Including directed material Study is 34–46 depending on routes: about 17–23 sheets of paper-equivalent value. These topics occur over progression rather than all at first access; the comparison still cautions against charging too much for the paper/ink route that is meant to make later research affordable.

One native paper-press run supplies 60 science. The fee for the whole catalog corresponds to about 3–4 runs of that output, or 4–5 whole runs including the material-study allowance. This is an output equivalence only: wheat, water, paint, technology, construction and other uses of paper are still costs, and skin/wing paper routes have their own opportunity costs.

As a scale illustration only, the verified 35-input ordinary corpus permits 228 two-slot tuples and 2736 three-slot tuples under native category-position rules if every input is available. A completely uniform search without replacement would take (N+1)/(v+1) attempts on average to encounter one of v target variants. Native goo hints, stock limitations, partial knowledge, biased experimentation and the free wiki change that model radically. Do not turn these spaces into an estimate of actual wasted ingredients or convert them directly into a precise science surcharge. They merely show why the material-Study comparison omits a potentially large benefit of targeted knowledge.

No material/time equivalence or comfort markup is measured yet. The next useful choice is whether the player-facing price should feel like one short preparation or a substantial knowledge investment. The user's richer 2/5/10/15 scale is the current proposal to review; prior 1/2/4 remains historical comparison evidence, not the selected direction.
