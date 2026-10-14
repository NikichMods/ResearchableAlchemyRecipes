# Quest-request awareness — 2026-10-14

Status: adopted behavior, statically verified native owners, research-only 0.1.41 implementation awaiting installed acceptance. PRODUCT_REQUIREMENTS.md owns the source rule; this document records evidence and its bounds. No final economics are bundled.

## Native request census

Read-only same-hash game_data/FlowScript audit inspects 764 serialized graphs using the verified FlowScript script owner. Item SmartRes nodes referencing ordinary successful alchemical products or the extra memory-potion product are traced to their consumers. The found NPC price requests are healing (Clotho), acid (Astrologer) and spices (Merchant). This is a bounded authored Item/Flow_Answer census, not a proof about every possible custom script, modded quest or future content.

| Product | Assignment owner | Completion owner | Consequence |
| --- | --- | --- | --- |
| pot_heal | npc_witch graph path_id 150562, node 256 sets npc_clotho_task_1 Visible after the introductory dialogue asking for help; potion price node 299 -> answer 302 | Both restoration branches set the same task Complete, nodes 44/91 | Assigned or fulfilled native task proves mention; no trade window/stock/physical vendor required |
| acid | npc_astrologer graph, node 226 explicitly names acid; node 228 sets astrologer_acid Visible; resource 473 -> answer 476 | Node 268 sets astrologer_acid Complete | Exact acid task, not the earlier diary task, establishes awareness without church-workbench presence |
| Native spices | Previously verified witch node 140 enables the return phrase and node 142 reveals the sole formula | Native recipe knowledge owns exclusion | No redundant paid topic |
| memory_potion | Found reference is witch graph Flow_DropItem node 892, part of the separate native quest supply route | No Item SmartRes hand-in price asking the player to brew it found in this census | Do not invent a new paid topic from the existence of a native recipe alone; guidance's 44-formula scope remains separate |

Native localized task identifiers corroborate the acid request and Clotho memory-help task. No formula rows/dialogue text dump is published. Both NPC ObjectDefinitions have empty npc_alias, so the exact persisted NPC IDs above are valid.

## Readiness and chosen seam

READY for the bounded healing/acid research-source correction: the observable change is immediate remaining-only availability after actual assignment; Flow_SetTaskState -> GameSave.SetTaskState -> KnownNPC tasks owns assignment, and SourceResearch.Refresh / Available at menu/payment owns final topic availability. GameSave.known_npcs is the existing persisted owner. Native GetNPC and GetQuestState scan stored lists without writes/singleton initialization; absent NPC yields null, absent task yields a non-qualifying state. GetOrCreateNPC is rejected because it invokes GameBalance.me, can create NPC/achievement state, and is unnecessary.

QuestResearchSources.Known uses only the two exact audited mappings and Visible/Complete states. Complete remains evidence of a previous product request even if the player fulfilled it with a bought/found item and still lacks formulas. The authoritative journal state is reused: no new encounter flags, copied task store, polling or lifecycle hook. The foundational The Beginning Of Alchemy and remaining-formula gates stay. Existing source families and non-quest vendor discovery retain their gates. Both quest cases share the same verified read/visibility/payment seam and can be batched in one candidate.

Coverage of all naturally reachable quest assignment/persistence UI and negative runtime cases remains pending. The static census does not grant universal quest support. A new demand outside these audited owners must reopen its own evidence gate; do not use a visible unrelated journal task as a product mention or hand-in actionability proof.

## Automated and installed boundaries

Actual host-assembly tests pass absent NPC/no creation, exact task IDs, unrelated product/task isolation, Visible and Complete, native KnownNPCList binary roundtrip, save separation, no-trade/no-source-station eligibility, pre-alchemy rejection, partial knowledge visibility and full-knowledge paid-topic suppression. Existing knowledge/quantity/localization suites pass. These tests do not run actual assignment dialogue, native scene snapshots, installed menu/payment or a full save reload.

Minimal installed observation: on a naturally assigned healing task with unknown formulas, open research after the vanilla alchemy introduction and before Clotho trade; expect healing but no other unseen potion topics. Check partial/full native knowledge behavior only if naturally available. An assigned unknown acid task should be offered even without the church workbench supplying its source. Read current logs for quest_visible and final cards; no forced quest/reset/new game or replay of successful prior research is required. If no such save exists now, retain these installed gates as pending rather than fabricating assignment evidence.
