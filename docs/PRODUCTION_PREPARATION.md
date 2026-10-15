# Production preparation — source boundary map

## Status and evidence

Preparation started at the user's request on 2026-10-15. This is a source/evidence map and a proposed extraction plan, not a selected production source, build system, version, candidate or acceptance record. PRODUCT_REQUIREMENTS.md owns product semantics; DESIGN.md owns the next action.

Inspected checkout: `research/study-table-work`, HEAD `0b9047d70dd8a69090dcbefddba2c9d843deabcb`, clean at entry. Its tracked StudyTableWorkProbe source/build/tests/assets have no diff from frozen compiled source `8b3faebe9f65e095aa8a6217fe8d6b23390ec4f2` (.57). There are 21 top-level C# files and four existing CLR test programs. Exact immutable build/install identity and bounded accepted runtime slices remain in TEST_LOG.md. This pass changes documentation only; it does not rerun accepted gameplay matrices.

Question: can the accepted implementation be extracted by removing research responsibilities while preserving native work, knowledge, UI ownership and save data? Source inspection supports minimal extraction, with two boundaries requiring further preparation: old research-save resolution and activation/failure containment. A gameplay rewrite is not justified by this map.

Shared native owners: GraveyardKeeperResearch/docs/CRAFTING_INVENTORY_AND_TRADING.md (Study-table sample boundary, native work/registration/refund ownership), docs/ALCHEMY_SYSTEM.md (native learned-list/writer/save boundary), and docs/UI_INPUT_TIME_AND_ENVIRONMENT.md (tabs, tooltip final geometry, fonts and picker separators). Project acceptance and implementation limits stay here.

## Retain / remove / change

All filenames below are under `research/StudyTableWorkProbe/`. Retain means preserve the established mechanism and its limits in the eventual production source, not declare the entire file production-ready.

| Current owner | Retain | Remove or change for extraction | Reason / boundary |
| --- | --- | --- | --- |
| StudyTableWorkProbe.cs | Loader, pre-cache registration, source cache verification, study-table dispatch, actual source-job completion, Update guidance/notice calls, LateUpdate shown-page commit, teardown | Replace research identity/facade; remove F7/F8/F9, Cleanup, DumpActive, Resources captures and diagnostic start/cancel/destruction/restore/load observers. Separate synthetic registration/completion; replace sample JobId readiness checks with real registry/cache readiness | Entry mixes essential integration with observation. Keep SourceResearch.Complete after native OnFinishedCraft; logging observers are not refund owners |
| SourceResearch.cs | Fixed 33-product/42-formula registry, native-source predicates, partial description, remaining-only GrantRemaining, completion, first-trade/higher-tier discovery, cache/readiness checks | Remove Menu/MenuActions, PreviewProducts, Status and card/native-list census patches; replace result-close Status callback. No interactive Cleanup in ordinary runtime | Registry and guidance corpus are distinct. Debug-level censuses still enumerate cards and construct strings |
| QuestResearchSources.cs | Read-only exact native task predicates | No speculative source expansion | Healing quest-only slice accepted; acid quest-only natural isolation remains pending |
| ResearchEconomics.cs | Adopted tariff, faith0, explicit work and energy mapping, native job inputs | No rebalance or formula-count pricing | RESEARCH_PRICE_REVIEW.md remains owner; no new wallet/payment owner |
| ResearchScienceReserve.cs | Native table Item prepaid metadata; successful-start record; Cancel/StartRemovalCraft refund; completion clear; legacy fee fallback | Review activation/failure separately; reduce success traces | These patches implement refunds. Removing CancelObserver or F9 must not remove Cancel refund. Demolition still loses native inventory |
| ResearchRepetitionGuard.cs | One operation, prepayment recheck, amount/queue restrictions, QueueEverything ordering, old queued/prepaid-batch handling where supported | Remove FormulaKnowledge readiness branch only with resolved legacy-save boundary; scope IsJob/IsJobId to retained definitions | Quantity safety and old paid-work accounting are gameplay behavior |
| StudyTabs.cs + TabArtwork.cs | Native composition, header forwarding, snapshot/key-map restoration, accepted original icons/offsets, scoped quantity compatibility | Remove successful open/switch/art/restore traces or retain only justified sparse support events | Preserve ordinary Survey/input/geometry; no calibration or .48 global card hook/custom component |
| QuantityInputCompatibility.cs | Verified MaxButtonsRedux soft dependency/contract, scoped invalid-card guard, actual native-focus cache projection, owned-frame close notification | Remove routine bind/skip/synchronization/close chatter; review unknown-contract failure ownership with activation gate | It preserves the foreign quantity algorithm for ready cards. Do not turn it into a global quantity disable or presume another foreign version is verified |
| ResearchCostDisplay.cs + ResearchSciencePriceCell.cs | Ordered native spend extension, actual table shortage/fee, plain owned views on tab switch, header tiny font/tint, fixed geometry, disposal | Remove begin/ready price census | No new global redraw hook, inventory frame poll or component |
| RecipeGuidance.cs + AdditionalSources.cs | Native known-card/station/alchemy gate, producer selection, family names, additional verified routes, DLC/quest/study annotations, focus refresh | Remove PreviewMenu, previewUnstudied and formatter's synthetic sulfur override; equivalent fixtures stay in tests. Review registration aggregate logs | Paid registry stays 33/42; guidance acceptance separately covers 44 formulas/36 inputs. Broad metadata selection does not expand paid topics |
| GuidanceTooltip.cs + GuidanceTextLayout.cs + ReagentHeadingPalette.cs | Owned native bubble/panel, atomic wrapping/indentation, native font measurements, accepted colours/opacity, integer/even geometry, final UIPanel prefix, settled alpha, amount clearance/destruction | Remove ObserveGeometry, 120-frame counters and Ready.Postfix; remove log-only marker-count/first-reveal observation after caller audit | Measurement/settling is functional anti-jump behavior. Post-geometry motion/marker census is research. Keep WidgetsBubbleGUI/UpdateBubble filters and final geometry prefix |
| ResearchCompletion.cs | Newly learned count, one native row, scoped subheader restoration, native unlock sound once | Remove routine result trace; replace SourceResearch.Status callback with null or a necessary lifecycle callback | Knowledge is committed before presentation; presentation failure must not duplicate grant/payment |
| AvailabilityNotifications.cs + AvailabilityNoticeState.cs | Eight separately owned optional hooks, native window events, dirty requests, safe delivery, in-place pages, generation/save ownership, late displayed-only flags, local failure containment | Remove Preview/PreviewMenu, previewData/previewProducts, preview branches/labels, blocker-mask observation and routine shown-page traces | Detached fixtures never become production notice state. Update/LateUpdate remain functional; no zero-overhead claim |
| FormulaKnowledge.cs | Historical evidence/tests; legacy resolution only if transition investigation proves necessary | No synthetic preparation/restoration/reward/menu/status/census in ordinary production flow | PrepareInput removes native knowledge; Restore can restore the snapshot baseline. Current gameplay reward is SourceResearch.GrantRemaining, not FormulaKnowledge.Grant |
| Localization.cs + Localization.tsv | Selected native language, English fallback, native names/fonts, retained UI text across 11 locales | Remove research keys and FormulaKnowledge.Product substitution. Inject/LanguageLoaded serve diagnostic job labels; remove after checking retained/legacy definitions for Id keys | Research has 55 keys including diagnostics. Reduced production keys need their own completeness/reference check |

F6/result preview, F2 calibration and the picker observer are already absent in .57. SourceEligibilityProbe is separate research tooling, not production payload. Preserve numbered artifacts and frozen source; extraction uses a separate source location/branch instead of rewriting the handed baseline.

## Persistent data is not diagnostic debris

| Data / current owner | Preservation boundary |
| --- | --- |
| Native formula IDs in GameSave.completed_one_time_crafts | Earned native/vanilla/scripted knowledge stays intact. No synthetic reset, removal by prefix or duplicate knowledge ledger |
| `rar_probe_source_paint_0110_<product>` | Real paid job IDs despite their name. Native saved current/queue/completed work references use them. Recommended extraction preserves IDs; renaming needs a proved transition and offers no player benefit |
| `rar_research_reserved_science` in table Item | Actual prepaid fee metadata; retain until successful native refund or normal completion settles it. Not an extra science balance |
| `rar_seen_clotho_<product>`, `rar_clotho_trade_tier`, `rar_clotho_expanded_<product>` in player Item | Preserve acquaintance/discovery/cause state and keys; no forced repeat visit through cosmetic renaming |
| `rar_notice_seen_<product>`, `rar_notice_initial` in player Item | Preserve shown-page deduplication and commit timing; empty/pre-alchemy queries do not consume the overview |
| `rar_probe_work_010`, `rar_probe_knowledge_heal_017`, prepared/original knowledge markers | Old experiments. Current hidden definitions support resolution/cleanup. Definition removal is separate from control removal. Determine relevant saved references/snapshots before choosing a minimal transition; never automatically call FormulaKnowledge.Restore on load |

Definitions must exist before native saved work resolves, independently of topic visibility. Last-craft/private cleanup fields are only used by F8 cleanup; they should not remain mandatory startup dependencies solely for that removed path. This does not prove every synthetic reference can safely be deleted now.

## Extraction evidence checkpoints

Planning checkpoints only: no production-source mutation has occurred. READY does not substitute for exact-candidate runtime acceptance.

| Planned property | Owner / final consumer | Scope and preserved invariants | Readiness / cheapest sufficient check |
| --- | --- | --- | --- |
| Remove diagnostic keys/previews | Plugin.Update; source/guidance menus; notice preview branches | Research controls/synthetic display only; retain Tick, Refresh, ObserveShown and real source/player state | READY for mapped removal. Compile/reference/reachability scan, adapted real policy tests, ordinary changed-path UI sanity |
| Remove work/card/save observers | Entry observers, source censuses, FormulaKnowledge save/list status patches | Keep mixed completion's real dispatch, refund/queue patches and native Survey | READY for observer-only removal. Patch/caller inventory; no new gameplay matrix |
| Remove tooltip observation | Ready.Postfix -> ObserveGeometry; log-only reveal/count branch | Keep Ready.Prefix -> PrepareGeometry, lastLayout/settled alpha, native wrapping/panel | READY for bounded diagnostic removal. Compile/patch/caller checks; ordinary final-candidate settling/focus sanity, no new 120-frame matrix |
| Delete/remap synthetic definitions | Native balance cache before save/world/craft restoration; hidden snapshot markers | Preserve earned knowledge, paid work/reserve, queues and unrelated saves | BLOCKED for deletion/remap until missing-ID behavior and actual transition requirements are established. Existing evidence first, then bounded reference inspection if needed |
| Change startup guards/rollback/containment | Awake/PatchAll/Fail/OnDestroy, optional notices, settlement and other guards | Vanilla fallback, no partial payment/reward path, owned UI restored, unrelated patches untouched | BLOCKED for new containment. Fail sets enabledProbe=false/restores tabs; that alone does not prove every patch inert. Audit consumers and use narrow deterministic fault injection only where needed |
| Replace research logging | Write/call sites; native shared BepInEx log | Keep component/version/host identity and actionable failure/containment; routine paths quiet | Boundary mapped; implementation checkpoint still needed. Do not globally suppress failure lines or claim production support acceptance from source alone |

Distinguish optional notices (local disabled state/separate patch owner) from mandatory activation and settlement of already-paid work. ResearchScienceReserve patches do not all consult enabledProbe; blindly adding guards could strand a paid fee. This is a preparation question, not an observed refund defect or authorization to alter economics.

## Smallest proposed structure and checks

Recommendation, not selected setup: one BepInEx assembly, `src/` with retained existing modules and a thin production entry class, owned icons/localization beside it, `tests/` for executable fixtures. Keep module boundaries unless extraction needs a small adjustment. No framework, DI, compatibility package, new save system or gameplay hook is justified. Copying/renaming the whole probe would ship synthetic mutation/observers; rewriting would reopen accepted mechanics without benefit. Research remains evidence; production needs one canonical executable source set rather than two independently maintained gameplay copies.

Existing build.ps1: SDK 6.0.406 Roslyn, language 7.3, explicit local host/BepInEx references/resources, deterministic optimized warnings-as-errors compile, clean exact-HEAD handoff gate, immutable output and hash/version manifest. These are research capabilities, not a selected production compiler/target. Input hashes do not provision a hosted runner.

Proposed build gate preserves exact source/reference/resource identity, version agreement and immutable handoff guarantees. Select compiler/target/runner after establishing authorized reference acquisition and executable checks. Do not upload game binaries/assets to the public repo or distribute them. Original icons/translations retain MPL-2.0; native sprites/fonts remain runtime-owned. New dependency/reference provenance and distribution notices need license review. No CI is created for this documentation step.

| Existing test | Production use / limitation |
| --- | --- |
| KnowledgeStateTests | Synthetic FormulaKnowledge prepare/grant/restore only. Keep with research/legacy tooling; not production reward coverage. SourceTopicTests already tests real GrantRemaining |
| SourceTopicTests | Reuse registry/economics/knowledge/quest/tier/reserve/price/formatter assertions. Remove diagnostic Cleanup/preview/injection assumptions; update reflection names/paths. Actual QueueEverything DLL check needs private inputs |
| QuantityCompatibilityTests | Reuse installed-checker policy/cache/focus/unpatch and soft dependency identity after rename; requires verified foreign DLL, not proof of arbitrary stacks/controller lifecycle |
| NotificationStateTests | Reuse native Item roundtrip, displayed-only flags, blocker/cause/page policy and optional-hook metadata; adapt names and keep fixtures outside runtime |

Tests need proprietary host and some foreign DLLs; separate portable policy from private integration inputs, without presuming hosted availability. Private 44-formula/36-input coverage and 27 locale/font-route audits remain reusable only for unchanged relevant inputs. Removing diagnostic strings requires retained-locale completeness checks, not eleven new visual sessions.

Final candidate checks cover extraction: loader/identity/sparse normal log; ordinary Study/research close/reopen and price/header; a naturally available unknown/partial topic through one charge/native completion/known-formula guidance/save reload; absent research hotkeys; preserved discovery/notice flags. Add checks only for newly changed save/activation boundaries. Accepted .40/.42/.43 and visuals remain reference evidence, not a full replay plan. Later real Clotho tier-up, acid quest-only isolation and physical-source negatives stay separate; no forced resets.

## Next action

Before writing the production entry/build, close (1) minimum safe handling of old synthetic job/snapshot references and (2) mandatory activation/failure/settlement ownership after cleanup-only dependencies disappear. Inspect native/research evidence and current callers first. Outcomes determine hidden legacy definitions versus a one-off research transition, and retained enable/restore versus a separately verified containment change.

Then record selected source structure, plugin identity/version owner, compiler/reference provisioning and executable gate; begin build-bearing work on a semantic development branch. This preparation record does not accept a production artifact or stable promotion.
