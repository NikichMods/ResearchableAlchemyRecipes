# Source Eligibility Probe

## Completed observation 0.1.3 — manual Clotho trade snapshot

Zero patches and no host world/save/balance access before explicit F9. Open Clotho's ordinary trade and press F9 once with the window open. No purchase, crafting, tier unlock or knowledge reset required. Codex installs/reads the local log directly. StudyTableWorkProbe 0.1.37 remains unchanged.

Read only the existing native VendorGUI.trading.trader, actual stock, drawing tier pages, active item cells and lock-overlay state for eight potions. Read current source instance and native @clotho_trade state separately. Do not construct Vendor/Trading, access lazy wgo.vendor or fill inventories. Active cells under an overlay do not prove visible item exposure; note whether the locked-tier goods are recognizable. Expected READY version=0.1.3, TRIGGER, eight RAR_SOURCE_VENDOR_PRODUCT rows, RESULT shown_indexed=True, DONE; no ERROR. Field binding/stock/grouped-card tests pass. Returned READY/F9/eight product rows/RESULT/DONE pass. The user excludes locked tiers after visual observation. TEST_LOG.md owns exact identities and limits; this does not register potion topics. Remove the finished observer with game closed.

## Historical church observation 0.1.2 — completed and removed

Current research candidate: 0.1.2, manual read-only church-workbench observer for Graveyard Keeper 1.407 / BepInEx 5. The accepted StudyTableWorkProbe 0.1.35 stays unchanged. This separate diagnostic installs zero patches, reads no native world/save/balance until an explicit F9 press and creates no save state.

Codex installs the exact verified observer while the game is closed, preserving the current local log. Load the existing save, open an idle church workbench's ordinary crafting menu and press F9 once while the menu remains open. No spending, crafting, synthetic unlock, new game or cleanup is needed. Codex reads the local log directly. This observation does not yet add research topics.

The church branch compares save-visible ordinary source definitions with the final native GUI list and all definitions represented by grouped quality cards. It requires exact source-ID set agreement for each of sixteen products, rather than merely matching counts. Existing WorldMap instances and their physical/tutorial/busy/native-interaction inputs are logged independently. The actual shown instance must be present in the world list. No formula ingredients or exact mixed-formula IDs are logged.

Expected evidence: READY version=0.1.2/patches=0, TRIGGER, all sixteen RAR_SOURCE_CHURCH_PRODUCT rows with exact_ids_agree=True, RAR_SOURCE_CHURCH_RESULT with exact_ids_agree=True/shown_indexed=True, then DONE and no RAR_SOURCE_ERROR. Grouped-card/binding tests pass through test.ps1; actual startup/F9 acceptance remains pending. Once this bounded observation is complete, remove only this diagnostic DLL; no save cleanup is required. Keep the main study probe and other mods.

## Accepted 0.1.1 room baseline and historical procedure

0.1.1 was accepted for the positive room-builder source/card agreement and removed after that test. The church branch is new in 0.1.2; F9 in the room builder still runs the older baseline path, which need not be repeated. Candidate 0.1.0 is withdrawn after a user-reported load freeze; removing that DLL restores loading. Root cause is not established. The original log shows READY but no census marker and stops during world restoration. Do not reinstall 0.1.0 or overwrite its frozen bytes.

Method reassessment: automatic loaded-world polling and three Harmony observers add integration surfaces before the actual source check. They are unnecessary for this question. 0.1.1 installs zero patches and only checks F9 in Update; no native save/world/balance accessor is evaluated until an explicit key press. It reads the already-loaded GameBalance._instance through a verified FieldInfo rather than invoking the lazy GameBalance.me loader. Startup only validates host identity and the field. This change removes loading participation; it is containment, not a proved diagnosis/fix of the original freeze.

Question: which native keeper-room blueprints and Merchant/Spices states exist in this loaded save, and does native visibility agree with the actual rendered source cards? Prior accepted work/formula probes do not observe these owners. Static source inspection establishes the seams; a single explicit snapshot of an already-open source is enough. No synthetic unlock/encounter setup is needed.

Install one SourceEligibilityProbe-0.1.1.dll into BepInEx/plugins after confirming the old candidate is removed. Study Table Work Probe 0.1.7 can remain as in the successful baseline. Load the existing save, open the room-decoration builder in the keeper's house and press F9 once while its window stays open. Return LogOutput.log. No new game, quest replay, crafting, resource spending or save cleanup is required. No dialogue-render observer remains; Spices native task/phrase/formula state is still measured.

Reads: native technology state, WorldMap builder lookup and removed/disabled/tutorial flags, native IsCraftVisible for builder definitions, native known-formula counts, noncreating KnownNPCList.GetNPC/GetQuestState and phrase lists. It inspects the shown native CraftGUI.list_items when its current desk is keeper_room_builddesk. Presence/visibility alone is not labeled proven reachability; the shown source window is the final card boundary. Existing GameBalance input identity and static references are owned by docs/VERIFIED_RUNTIME_DATA.md.

Each substantive native query stage has before/after markers. Expected: READY patches=0 trigger=F9, TRIGGER, completed query pairs, paint/Spices rows, actual native builder cards, DONE. An absent after/DONE marker or ERROR blocks acceptance. F9 with the room builder closed still measures state but logs a skipped final card check. Current loaded save proves only exercised positive availability; locked/early states and the rest of the 34-product corpus remain open.

No craft registration, formula grants, progression mutations, persistent diagnostic state, UI changes or automatic snapshots. Removal leaves no probe save state. This observer does not yet create playable research topics. Strict verified-host identity bounds the validity of diagnostic claims to the inspected 1.407 envelope.

Build: build.ps1, deterministic SDK 6.0.406 / C# 7.3 / warnings as errors, exact committed-source clean-tree handoff, plugin/assembly/file version and SHA-256 checks. Assets/references/executables remain ignored. Compile passes; actual corrected-candidate loading/F9 execution is still pending.
