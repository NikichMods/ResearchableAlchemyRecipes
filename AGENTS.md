# Researchable Alchemy Recipes — Working Contract

Follow [NikichMods/DevRules/WORKING_RULES.md](https://github.com/NikichMods/DevRules/blob/main/WORKING_RULES.md) and its action-based reading map. Local checkout work also follows CODEX_WORKFLOW.md; engineering, runtime, Git, licensing, CI and bootstrap guidance applies when its subject is entered. This file contains project-specific requirements only.

## Identity and phase

- Project: Researchable Alchemy Recipes.
- Repository: NikichMods/ResearchableAlchemyRecipes (public).
- Target: Graveyard Keeper 1.407, Windows PC, BepInEx 5.
- Phase: adopted product concept and accepted bounded research mechanism; no production source/candidate/toolchain selected. Accepted immutable StudyTableWorkProbe 0.1.43 shortens work to 2/2/3/5/8 and corrects science return when StartRemovalCraft interrupts research; detailed censuses are Debug. Local checks and bounded installed removal-refund/unmark/shorter-work return are accepted; TEST_LOG.md records exact limits. 0.1.42 has bounded accepted payment/full restart/F9 refund/completion and user-reported post-cancel movement; quest/source negatives retain their limits. Vanilla demolition comparison is captured; actual destruction still follows native inventory loss, with no new-table bank/prohibition. Latest accepted baseline 0.1.40 is accepted for Clotho trade discovery, save/reload persistence and mana learning/native alchemy card, preserving prior room/church/resurrection and guidance acceptance within docs/TEST_LOG.md limits. Ordinary source accounting covers 33 mod-research products/42 formulas plus native-disclosed Spices/one formula (34 products/43 formulas). Ingredient guidance separately covers the 44-formula/36-input ordinary/native quest audit with non-slime sources. All 43 custom strings cover 11 languages; adopted partial text Часть рецептов известна. Открыть все варианты. has passed locale/font checks but lacks the user's visual inspection. Exhaustive source/lifecycle/compatibility and production acceptance remain open. Science tariff 1/2/4/8/10 and zero recipe-research faith are adopted; native comparison is verified and the final explicit work 2/2/3/5/8 and unchanged energy 2/3/6/12/15 are adopted in docs/RESEARCH_PRICE_REVIEW.md. Next: remaining source/compatibility/reward/notifications/production gates; do not replay accepted 0.1.43 removal or earlier lifecycle checks. Native-style availability notifications and compact result presentation are implemented in unaccepted research 0.1.44; docs/RESEARCH_NOTIFICATIONS.md owns local checks and the installed procedure. Production gates remain open. Recover from docs/DESIGN.md current checkpoint; historical candidate detail belongs to docs/TEST_LOG.md and docs/VERIFIED_RUNTIME_DATA.md.
- Current installed unaccepted research: immutable StudyTableWorkProbe 0.1.46 adds visible native science fees and in-place native notice-page updates; local suites/33 registered-price assertions/native-row metadata checks pass. Await only science-price/icon placement and F7 continuous Next/final OK/early Close. 0.1.45 retains bounded accepted button/preview-heading and natural Clotho quest-only offering; user observes notices wait for another mod's weekday speech. Acid quest-only isolation lacks a suitable natural save. 0.1.44 returned real paging/blocked-preview wait and bounded reload suppression. Exhaustive source/compatibility and production acceptance remain open. Follow docs/RESEARCH_NOTIFICATIONS.md and TEST_LOG.md; preserve immutable previous artifacts and do not replay accepted quest/UI/work/removal/source matrices.
- Purpose: target-directed paid recipe learning with ingredient-source guidance, followed by ordinary vanilla material study and production.

## Canonical documents

- docs/PRODUCT_REQUIREMENTS.md owns adopted player-facing requirements.
- docs/DESIGN.md owns proposed interface, open choices and next research questions.
- docs/VERIFIED_RUNTIME_DATA.md owns evidence pointers and their limits.

For a new chat, start with the Current recovery entry point in docs/DESIGN.md, then read the owning requirements/evidence as needed. LOCAL_ENVIRONMENT.md is an ignored machine-local map to the checkouts, game inputs and prior inspection artifacts; it is not a second product specification.

Recover actual files and Git state before checkout work. Read these documents and relevant accepted evidence before implementation. Capture adopted decisions in their owning document within the same working segment; do not treat tentative designs as accepted.

## Shared evidence and separation

Shared host facts belong in NikichMods/GraveyardKeeperResearch: start with its AGENTS.md and docs/RESEARCH_INDEX.md, then docs/ALCHEMY_SYSTEM.md and other relevant entries. Search accepted prior research before adding a probe.

AlchemyRiddle is an independent minigame. Its accepted game research may be consulted as evidence, but its deduction mechanics, generator, UI and former recipe-disclosure restrictions are not this project's requirements. Do not modify AlchemyRiddle as part of this project.

Do not disclose concrete unknown vanilla formulas to the user without an explicit request. Internal bounded inspection may use the real corpus; publish derived facts, identifiers, hashes and aggregate evidence, not copied game binaries, bulk decompiled source, extracted assets or recipe dumps. Keep private context and machine-specific configuration outside tracked files.

## Implementation boundary

Verify each independent host-mechanics dependency under DevRules before production mutation. Prefer native save knowledge and vanilla study/production paths over duplicate state or reimplemented mechanics. Adding research must not require ownership of the unknown target product. Preserve ordinary recipe definitions and material-study behavior unless a separate change is agreed.

## Git, acceptance and licensing

- main is the stable/documentation baseline; adopted documentation may be committed directly.
- Use research/<topic> for substantial diagnostic work and dev/<version> or a semantic feature branch for build-bearing work.
- Runtime behavior reaches main only after its exact candidate is tested and explicitly accepted by the user. Numbered handed artifacts are immutable; stable binary distribution uses GitHub Releases under GIT_WORKFLOW.md.
- No production version, build toolchain or CI gate is selected yet. The research probe has its own local compile/identity gate; this does not select a production setup. Add infrastructure only when an actual executable check requires it.
- Original software and its software-supporting documents use MPL-2.0. See LICENSE and LICENSING.md; host and third-party rights are separate.
