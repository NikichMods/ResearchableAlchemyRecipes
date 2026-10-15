# Production reliability and simplicity audit — 2026-10-15

Current correction status: A1/A2 are implemented in development 0.1.2, with local fault checks described below. Original findings refer to frozen 0.1.0 and remain historical evidence. Exact numbered candidate/install/ordinary return belongs to TEST_LOG.md; local tests do not establish rendered Unity failure cleanup. The 0.1.1 background-bubble fix is preserved and its focused installed return remains pending.

## Scope and conclusion

Reviewed candidate 0.1.0 source frozen at 33b906567cb46502a8aaf255aeab2645769a282b, against documentation HEAD 7add5ad6c06d43b4fe4b4caeca08c9c24cd2457e. All 20 production C# files, build/test scripts, failure/state fixtures and current owning evidence were inspected. Source/build/tests have no diff from the frozen candidate source. The immutable artifact and exact-test copy both retain SHA-256 22e2d95cec9198af174cc47aa9a5171e4fb75dd46d2f061d984abc594c718ea1.

The native-first architecture remains justified; no rewrite, framework or new persistence mechanism is warranted. Two source-established gaps prevent treating the current failure-containment claim as complete. They do not revoke the accepted bounded normal player flow, demonstrate an installed failure, or establish lost science/knowledge. Resolve the gaps before relying on full containment for publication readiness. This audit changes documentation only; candidate/source/installed DLL are unchanged.

## Criteria

Canonical rules live in NikichMods/DevRules, rather than a duplicate project audit policy:

- ENGINEERING_RULES.md §2: choose the least-complex sufficient mechanism and establish each independent behavior's owner, final consumer, scope, invariants and evidence.
- ENGINEERING_RULES.md §§4–5: narrow reversible changes, native ownership, canonical state, bounded hooks, no speculative generality; its **Audit and hardening triage** separates current defects/contract gaps from maintenance, watch items and non-goals.
- ENGINEERING_RULES.md §§6,8–9: assess actual recurring cost, retain exact artifact identity and stop affected behavior safely.
- RUNTIME_INTEGRATION.md: verify failure boundaries at the cheapest sufficient layer, contain recurring failures, preserve owned lifecycle/rollback and sparse actionable support logs. Partial hooks may remain only where their residual behavior is safe.
- GIT_WORKFLOW.md §§6,10,12: exact source/artifact, support-log audit, no ceremonial release or requirement to eliminate every deferred observation.

Product semantics remain owned by PRODUCT_REQUIREMENTS.md. TEST_LOG.md owns exact runtime acceptance. NATIVE_INTEGRATION_REVIEW.md supplies earlier mechanism comparisons; its research-era observations alone are not a full audit of production 0.1.0.

## Must fix before claiming complete containment

### A1 — Cleanup can bypass the circuit breaker

**Priority P2; current contract gap, established by control flow.**

Plugin.Update catches a guidance refresh exception, but calls RecipeGuidance.Restore before Plugin.Fail (Plugin.cs:65). GuidanceTooltip.Position and Ready likewise call Hide before Fail (GuidanceTooltip.cs:212,237). Restore/Hide can reach native DestroyBubble. If that cleanup throws, Fail is never reached: Enabled remains true and the original failure never receives the RAR_DISABLED containment diagnostic. Hide clears local bubble references before native destruction, so this finding does **not** prove that the same bubble retries every frame. The gap is skipped containment and an escaped secondary exception.

Plugin.OnDestroy (Plugin.cs:50–57) has the same sequential cleanup weakness: an exception in notice disposal skips guidance, tab/key restoration, own unpatching and artwork disposal. Enabled=false makes entry inactive, but does not perform those remaining cleanup obligations. This is a lifecycle code gap, not evidence that ordinary scene teardown currently fails or that runtime plugin unloading is supported.

Smallest correction: enter the existing guarded Fail directly for runtime failures; it already attempts owned cleanup independently. Isolate teardown cleanup attempts so one failure cannot skip the others; preserve original/secondary diagnostics. Do not globally unpatch on a gameplay failure, auto-cancel native work, or discard paid-settlement hooks.

Evidence boundary: FailureContainmentTests calls Fail directly and injects faults *inside* its cleanup dispatch; it does not exercise these pre-Fail callers or the sequential OnDestroy entry. A private standalone attempt to detour the actual Update path could not complete under CLR because of Unity ECall/security restrictions; it produced no valid runtime result. Existing four-suite results remain valid within their documented bounds, not proof of these callers.

Verification for a correction: exercise failure dispatch from the actual managed caller where feasible, plus cleanup continuation under one injected secondary failure. Keep Unity native destruction/rendered rollback explicitly bounded; do not create an in-game fault harness merely to fill a checklist.

### A2 — Optional notice disable leaves an owned page with a dead transition

**Priority P2; current contract gap, established by state/control flow.**

AvailabilityNotifications.Fail (AvailabilityNotifications.cs:269–274) sets disabled and clears dirty, restores the count container, then logs. It does not clear pageOpen/nextPage/displayed state, invalidate the generation or close the owned native page. If an error occurs while paging or committing displayed flags, Tick and ObserveShown thereafter return immediately because disabled=true. The existing Next callback still calls ClosePage(true), which only requests a later transition; Tick will never perform it. The Close button/native dismissal can still work: this is not an established permanently locked game.

RestoreContainer also runs before the failure diagnostic and only clears its reference after native SetActive. If that restoration throws, the exception can escape Fail and suppress its diagnostic. Dispose has a related ordering boundary: Reset runs before optional unpatching, so Reset failure can skip unpatching; this matters where residual callbacks retain owned UI state, rather than requiring physical removal of every inert hook.

Smallest correction: set the local disabled state first, relinquish page/callback ownership and independently attempt closing the captured owned page/restoring its container. Ensure restoration failure cannot suppress the semantic diagnostic or other cleanup. Preserve ordinary research, native shared-window ownership and pending shown-only semantics. Do not hide a foreign replacement page or mark undisplayed topics as seen.

Evidence boundary: NotificationStateTests proves policy, native Item roundtrip and hook metadata. FailureContainmentTests intercepts notice Dispose to prove general cleanup dispatch; it intentionally does not execute this engine-bound notice failure lifecycle. No installed notice failure has been reported. Add only representative disable-with-open-page and cleanup-fault checks needed for the correction; preserve accepted continuous paging in normal operation.

## Useful at the next relevant change

| Observation | Consequence / proportional action |
| --- | --- |
| GuidanceTooltip.Show hides and clears selected/formula when side/marker space is insufficient (lines 83–94); Refresh can then try the same unchanged card again next frame | Repeated formatting/create/destroy work follows if that fallback is reached continuously. No such installed case or cost measurement is recorded. At a relevant layout change, retain a bounded suppressed-selection/geometry decision or revisit the trigger; do not invent a global layout cache now. |
| SourceResearch.AcquaintanceTier returns tier in both branches; RecordExpansion's re-store check compares the same direct value | Redundant residue after the research-compatibility waiver. Remove when touching this code; no migration or behavior change is required. |
| SourceResearch.Register initializes fee/work/energy placeholders immediately overwritten by ResearchEconomics.Apply; Plugin.Write retains a removed localization-injection event special case | Reduce misleading obsolete defaults/event handling at a natural cleanup boundary. The applied economics and current error events remain correct. |
| Recovery documents still describe pre-production phase / completed tasks in several current-status sentences | Update current status together with the next implementation segment, preserving historical evidence. Do not treat older statements as new authorization or unresolved accepted tests. |

These maintenance observations alone do not require a numbered build or a new user test matrix.

## What remains justified

| Area | Audit result and limits |
| --- | --- |
| Native economy/work/save | Jobs supply native definitions and tariffs; CraftComponent and Item own payment/work/reservation storage, native formula IDs own knowledge. No duplicate wallet, work scheduler or sidecar saves. Existing reserve/queue/disabled grant evidence remains applicable. No new loss/duplication defect was established in this review. |
| Disabled paid settlement | Verified completion remains independent of Enabled, new registered starts/queues reject while disabled, and native interruption retains reserve handling. Broad unpatching on runtime failure would break this deliberate separation. |
| UI composition and guidance | Native windows/widgets with scoped ownership; snapshot keys/layout, source grouping, locale formatting and final geometry hooks have concrete purposes/evidence. Engine-level UIPanel/bubble hooks remain sensitive, but a replacement renderer or generalized UI layer has no demonstrated benefit. |
| Runtime cost | Source reads are on demand; notice eligibility is evaluated after safe delivery, not as an unconditional frame world census. Active guidance and pending-notice checks still use frame callbacks. This establishes structure, not CPU/GC/frame-time performance. |
| Compatibility | Exact native identity gating is justified by persistent economy/progression mutation. The scoped MaxButtonsRedux/QueueEverything boundaries use verified actual dependencies. No arbitrary-host/mod-stack compatibility claim follows. |
| Build and support log | Explicit local references, warnings-as-errors, immutable clean-source candidate and manifest provide a concrete gate. Accepted normal support log contains one activation line and no RAR failures; it does not prove negative-path containment. Proprietary references remain local. No hosted CI provisioning is needed solely for this audit. |

## Watch only and closed/non-goals

- Watch only: genuine later Clotho tier-up, acid quest-only natural isolation, broader physical-source negatives, foreign popup interactions and measured performance retain their existing acceptance limits. Reopen for the appropriate natural evidence, actual support report, changed mechanism or newly claimed support target; do not force state resets or replay accepted matrices.
- Watch only: the native Survey null-return message retains its attribution and limits in TEST_LOG.md. No product regression/cause was established; no shared patch is justified.
- Closed: specific native picker separator-layer warnings; do not restore the observer or globally patch layers.
- Non-goals: old unpublished research-save aliases/migrations/cleanup; a custom wallet/save system; speculative future-host classifier/framework; standalone ingredient journal; general UI rewrite.

## Next action

### A1/A2 correction gate — 2026-10-15, before source mutation

User authorizes the bounded audit corrections after the separate 0.1.1 paging fix. Both are READY from the current production source, existing failure fixtures and pinned native BaseGUI Open/Hide ordering. A1 owner/final consumer: Plugin.Update and GuidanceTooltip Position/Ready dispatch to Plugin.Fail before cleanup; OnDestroy owns final teardown attempts. Scope is cleanup dispatch only. Keep verified paid completion/refund hooks during gameplay failure; only final teardown removes the plugin's own Harmony owner. Check one actual guidance failure entry with a primary error plus secondary cleanup faults, and OnDestroy continuation through independently failed cleanups; actual engine Update remains a separate boundary if CLR cannot execute it.

A2 owner/final consumer: AvailabilityNotifications.Fail/Dispose/Reset own page flags, generation/callbacks, count-container snapshot, native Hide(false), window subscriptions and optional Harmony owner. Native BaseGUI.Hide clears the window/stack and then invokes its stored on-hide callback; generation must invalidate that callback before Hide. Native Open emits on_window_opened for shared-instance replacement; relinquish ownership before any restoration can throw, so failure cleanup cannot hide a foreign replacement. Reset transient state before native cleanup, independently attempt container restore/own-page close/unpatch and retain a diagnostic for each failed attempt. ClosePage must recheck state if ObserveShown itself disables notices. Keep Plugin.Enabled/research/knowledge/notice persistence and 0.1.1 initial-only overlay waiting unchanged.

Selected mechanism is direct use of existing failure/ownership paths with ordered state release and independent cleanup attempts; no generic lifecycle framework, new hook, persistence field, world scan or automatic work cancellation. Cheapest checks extend the existing failure suite with deterministic managed-boundary faults and real optional-owner unpatching. Where Unity ECalls prevent standalone execution, test at a narrower managed boundary and explicitly retain native limits; do not reconstruct the engine or claim native rendering/destruction from intercepted calls. A bounded ordinary installed load/window-close return remains distinct from the local fault evidence; no in-game fault harness or replay of accepted matrices is required. New candidate identity will be 0.1.2; 0.1.0/0.1.1 remain immutable.

### Local correction result

A1: three pre-cleanup catch blocks now call Fail directly. Final OnDestroy attempts each cleanup independently, including own unpatch and artwork. The extended failure suite executes the real owned-bubble Position handler with an absent widget plus a secondary guidance cleanup fault: the original NullReferenceException reaches one RAR_DISABLED record, entry disables and remaining cleanup attempts continue. Actual OnDestroy runs through three injected failed view cleanups and still removes a separately identified test Harmony owner. The original Update/body-replacement approach could not be prepared under CLR because of Unity ECalls and was discarded, with no shipped seam or altered test assembly. Update/Ready's identical direct dispatch is source-reviewed rather than claimed as executed by those fixtures.

A2: Fail logs once, disables notices and disposes their own events/patches. Reset invalidates generation/page/save/displayed ownership before native cleanup; container references clear before native SetActive. Container restoration and captured-page close are independent. ClosePage rechecks state after ObserveShown; button/on-hide callbacks reject invalid generations. Shared-instance replacement relinquishes ownership before restoration, so cleanup cannot hide that replacement. Native Open's successful return establishes page ownership before later heading/button/layout work, permitting cleanup if those later steps fail.

The extended suite runs actual notice Fail/Dispose/Reset, with the engine-bound close intercepted only in the test process. It verifies released state before the close attempt, unchanged unseen-topic flags, local rather than global disable, one root diagnostic, independent container/page errors, repeat suppression and actual removal of only the optional test patch owner. The container error is the known standalone CLR SecurityException at the real Unity-dependent RestoreContainer method entry, not a rendered SetActive failure; native container clearing/rendering itself is source-reviewed. Foreign WindowOpened relinquishment is checked through its cleanup/disable state; the subsequent Request cannot JIT Time.frameCount under CLR, and only that identified engine limitation is accepted by the fixture. Native Hide, live on-hide invocation and full Update/Input are not claimed as locally executed.

Compile/version and four suites pass after correction. These checks close the source/control-flow findings within the stated limits; ordinary installed return and the inherited later-background-thought/Next check remain distinct pending evidence. Maintenance/watch items are not bundled into this candidate.

Priority clarification after the audit: A1/A2 are worthwhile bounded hardening, not observed installed failures or proof that 0.1.0 is unsafe to release. The original must-fix/publication wording above is stronger than the demonstrated urgency; do not treat it as an automatic release blocker or permission for a large harness/rewrite. Address them at a proportionate nearby reliability boundary with per-property READY gates; keep accepted economics, knowledge, sources and normal layout invariant. The fixes may share one later candidate after sufficient local checks; do not rebuild 0.1.0. The audit itself requests no new gameplay check from the user.

The user's later actual background-thought/Next delay is a different normal-operation defect, now owned by RESEARCH_NOTIFICATIONS.md. Its 0.1.1 continuation correction takes precedence; it neither demonstrates A2's exception-disable scenario nor closes A1/A2.
