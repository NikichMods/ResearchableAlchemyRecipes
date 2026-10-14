# Research economics: exploratory study — 2026-10-14

Status: evidence and proposals for discussion, not adopted prices or implementation. Installed immutable 0.1.40 remains unchanged. PRODUCT_REQUIREMENTS.md owns accepted semantics; DESIGN.md points here for the economics workstream.

## Question and user direction

How should optional target-directed formula knowledge be priced when vanilla material study, equipment and production remain payable, and an external wiki supplies the information without an in-game fee?

The user rejects pricing by formula count: additional variants need not mean a more valuable product; secondary/decorative research should remain attractive. Science is the user's preferred candidate for the main cost, with zero or a small faith fee still open. Early/late affordability should relate to progression; neither exact prices, a progression formula, partial discounts nor removal of faith is adopted. A global increasing research counter was explored but not selected.

The original faith/science/work/energy diagnostic values are not an economic baseline. Current scope is 33 potential paid product topics / 42 ordinary formulas; native story disclosure accounts for the additional Spices product/formula. The guidance audit's 44 formulas/36 inputs is a separate population.

## What the evidence establishes

Community accounts support three information routes: wiki lookup, random recipe scrolls, and experimentation using goo hints. See [Tips for Alchemy?](https://www.reddit.com/r/GraveyardKeeper/comments/11yh7dc/), [Alchemy bench recipes ingame?](https://www.reddit.com/r/GraveyardKeeper/comments/nii2n3/), and [Potions (no spoilers)](https://www.reddit.com/r/GraveyardKeeper/comments/17v5vcj/). These are self-selected qualitative reports, not a representative measurement of the percentage using the wiki. Do not repeat the user's 99% estimate as an established statistic. Older threads deliberately avoid sequel contamination.

Accepted native research shows failed alchemy hints relate to some structurally nearby valid recipe, not a player-selected target. They therefore do not establish a reliable target-directed discovery path. Random authored unlocks and successful manufacture are separate native knowledge channels. Actual scroll selection probabilities and replacement behavior are not audited here; do not calculate expected target costs from an assumed uniform pool. Community reports describe a 5-silver scroll; native rand_alchemy_unlock has base_price=5, which alone is not a proved transaction price.

Direct same-hash 1.407 static data audit is promoted to [shared study economy evidence](https://github.com/NikichMods/GraveyardKeeperResearch/blob/main/docs/STUDY_RESOURCE_ECONOMY.md). One clean paper produces 2 station science. One skin's authored processing chain yields 16 clean paper/32 science; five bat wings yield 8 paper/16 science. One paper-press operation yields 30 paper/60 science and requires two paper glop plus white paint; each glop requires one water bucket and 20 wheat. Paper production technology unlocks that chain. These are yields, not income per minute, proof of early access, or measurements of material opportunity costs. Mass paper production can depend on knowledge of an alchemical product, so large early science prices may impede the route that would make them affordable.

Vanilla material study remains an independent expense, but not every possible ingredient source requires a paid Survey; inventory-use/extraction/other routes differ. Shared data audit observes many 1/1, 2/2 and 3/3 faith/science costs and expensive alternatives; goo can require science without faith. Source knowledge is reusable, so total burden depends on already-studied materials and chosen acquisition routes, not the sum of every displayed alternative.

The common work owner consumes ordinary inventory Item needs plus station needs_from_wgo. Faith/science execution is runtime-confirmed. Other ordinary item payments are structurally supported, not installed-tested for this research. Money/technology-point payments have separate semantics and need their own evidence before selection. Native cancellation retains station science while refunding inventory inputs as drops; science-heavy pricing makes the still-open cancellation decision more consequential.

## Relevant design references and interpretation

- Ian Schreiber's [Situational Balance](https://gamebalanceconcepts.wordpress.com/2010/08/11/level-6-situational-balance/) discusses contextual benefit, setup costs and opportunity costs. Application: compare the complete route to useful manufacture; do not charge again as though knowledge eliminated its remaining costs.
- Schreiber's [Economic Systems](https://gamebalanceconcepts.wordpress.com/2010/09/08/level-10-final-boss/) discusses substitution and scarcity. Application: wiki information is an external substitute; too high an optional information fee can drive players out of the intended loop. This is our inference, not a measured mod response.
- Soren Johnson's [in-game economy analysis](https://www.gamedeveloper.com/design/the-trials-and-tribulations-of-balancing-your-in-game-economy) cautions against assuming dynamic prices automatically create enjoyable behavior. Application: avoid adjusting prices to punish popular/fun choices or wealth without a visible gameplay purpose.
- The Factorio developers' [science redesign](https://www.factorio.com/blog/post/fff-275) evaluates prerequisites, chain complexity, materials and crafting time together. Useful method, different game: our mod supplies information rather than making science the main progression system.
- Factorio's [infinite research](https://www.factorio.com/blog/post/fff-161) explicitly supplies optional endgame resource sinks. Our finite 33-topic knowledge feature has a different purpose; keeping all late prices permanently scarce is not automatically desirable.
- Nik Davidson's [Economic Decision Making in Game Design](https://media.gdcvault.com/gdconline10/slides/11760-Economic_Decision_Making.pdf) distinguishes economically equivalent transactions from their emotional experience. Application: a legible single research price can feel different from several additional errands; do not import monetization tactics.
- Pfau/Seif El-Nasr's [player/data-driven balance study](https://arxiv.org/abs/2308.07576) supports combining explicit player goals with measured behavior. It does not provide transferable Graveyard Keeper prices.

## Resource comparison

| Payment | What it asks the player to do | Main risk | Judgment, not adoption |
| --- | --- | --- | --- |
| Science | Use existing study infrastructure; obtain/convert paper or other sources | Easy at mature production; high early prices may impede paper-chain knowledge | Strongest main resource candidate |
| Faith | Allocate a resource competing with vanilla study and other advancement | Additional information fee can delay the very material study needed next | Compare zero versus flat one, not escalating faith |
| Direct paper | Supply physical research material | Less flexible than science; duplicates its conversion role | Credible alternative, weak reason to require both |
| Ink/pen and ink | Add thematic writing preparation | Additional processing/technology dependencies, potentially alchemical | Consider only if those errands serve an agreed goal |
| Commodity/target ingredients | Acquire specific supplies | May require the knowledge being purchased; new search/circularity | Poor default; target product remains prohibited |
| Money | Spend general wealth; compare vendor routes | Strongly different incomes; payment owner not verified | Alternative if player prefers a financial service |
| Work/energy | Perform a visible research action | Long holding/repeated food crafting may become chores | Supporting cost, not automatic main limiter |
| New currency | Separate research budget | New economy, persistence and explanation | No present justification |

## Pricing families

| Model | Strength | Failure or hidden cost | Current assessment |
| --- | --- | --- | --- |
| Low fixed science fee per product | Clear, cheap to maintain; naturally easier as production grows | Late catalog can be inexpensive | Serious baseline; late ease can be earned mastery |
| Fixed two/three product price bands | Cheap optional/decorative themes; larger later commitments | Requires explicit product classification and exceptions | Preferred hypothesis to compare with fixed fee |
| Alchemy station tier alone | Simple native proxy | Advanced station also hosts decorative paints; arity is not utility or story timing | Insufficient on its own |
| Verified source/progression stage of the product | Relates price to when the need becomes available | Multiple sources/routes, early exceptions, subjective stage taxonomy | Useful classification evidence, not a runtime wealth multiplier |
| Current player progress or wealth | Tracks presumed ability to pay | Same old topic grows dearer after upgrades; hoarding/manipulation | Avoid as default |
| Number of completed researches | Predictable mathematical growth | Decorations raise useful-topic prices; sequence penalized; vanilla-known count ambiguity | Weak fit |
| Remaining variant count | Pays for newly granted rows | Row count misrepresents utility | Rejected by user |
| A percentage of total vanilla material Study | Appears data driven | Path/save-dependent, circular and volatile; may penalize harder chains twice | Analyze burden, do not use as live tariff |
| Shared upfront access payment plus tiny per-topic fee | Makes many topics affordable after one investment | New entry barrier and adopted access semantics change | Distinct product option, not needed now |
| Research infrastructure discounts | Tangible investment reduces future costs | New progression/content; paper production already does this naturally | Defer unless natural scaling proves insufficient |

An ascending 1..33 per-topic price totals 561 science, equivalent to 280.5 paper units of value (281 actual sheets if converting all required science from scratch), before material Study. Buying a decoration changes the price of subsequent practical research. It is not equivalent to higher prices for intrinsically later products.

Partial knowledge is a separate design axis. Possibilities: unchanged product fee, a simple fixed partial-topic discount, or cheap completion of the topic. Recommend considering a binary discount (some knowledge versus none) independent of missing-count arithmetic. Benefit of a second production route can still be large, so it is not necessarily worthless; no discount/value is adopted.

## Provisional test anchors, not final balance

Compare a low uniform tariff with a static-band model. Illustrative science anchors: 4 for inexpensive/decorative topics, 8 for ordinary practical topics, 16 for later substantial topics. This equals 2/4/8 paper sheets. Faith alternatives are zero versus one flat faith; one across all 33 paid topics totals 33 additional faith. Work/energy should remain short and tangible until actual action duration is measured; do not invent real-time seconds from native craft units.

These numbers merely make resource burden inspectable. One press operation's 60 science buys fifteen 4-science, seven 8-science (4 left), or three 16-science researches (12 left). This does not prove that a press is accessible or that those prices feel balanced. A completely unknown catalog at a uniform 4/8/16 costs 132/264/528 science respectively; final mixed-band total depends on a reviewed classification.

Meaningful tests compare situations: first practical topic with limited resources and unstudied inputs; cheap decoration versus practical next goal; later topic after paper production; partially-known alternative; old save with many studied sources/known formulas; enough science but no spare faith. Record whether a price causes an interesting allocation, one modest production preparation, passive waiting, or a return to wiki lookup. An entire catalog need not be replayed to assess these boundaries.

## Next decision and unresolved evidence

Preferred next step: classify the 33 products by intended role and plausible availability stage without exposing formulas, then compare uniform versus static-band total burden in paper-production units. Keep low-priced late decorations as explicit exceptions; do not derive price from the currently owned workbench or current wealth. If classification is ambiguous or adds no useful choice, prefer the simpler fixed tariff.

Before committing prices, resolve whether the intended research expense should be a brief preparation or a competing budget decision. Actual early/mid/late resource holdings, acquisition duration, chosen material-study paths and full current-native scroll distribution remain unmeasured. Do not invent a universal balanced price or require the user to replay every source/quest. Subsequent science refunds/save/restart gates remain necessary; no runtime candidate follows this exploratory report.
