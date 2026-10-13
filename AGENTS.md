# Researchable Alchemy Recipes — Working Contract

Follow [NikichMods/DevRules/WORKING_RULES.md](https://github.com/NikichMods/DevRules/blob/main/WORKING_RULES.md) and its action-based reading map. Local checkout work also follows CODEX_WORKFLOW.md; engineering, runtime, Git, licensing, CI and bootstrap guidance applies when its subject is entered. This file contains project-specific requirements only.

## Identity and phase

- Project: Researchable Alchemy Recipes.
- Repository: NikichMods/ResearchableAlchemyRecipes (public).
- Target: Graveyard Keeper 1.407, Windows PC, BepInEx 5.
- Phase: adopted product concept; design and mechanism verification. No production source/candidate or production toolchain is selected. Research-only native work/knowledge/single-operation and manual room-source evidence are bounded and accepted. Automatic room-paint research/restart is verified; exact 0.1.18 is accepted for current known-yellow presentation/interaction. Inline candidates 0.1.19/0.1.20 are rejected for broader usability. 0.1.22 proves native tooltip creation but its appearance is rejected. Candidate 0.1.23 fixes anchoring/tail/readiness, uses a parchment palette and grouped equipment sources with native DLC availability; it moves guidance to a separate native tooltip beside unchanged alchemy cards/window; visible exact-known formulas include nonstandard recipes, and source families use native names. Installed placement/input acceptance remains pending. Wider awareness/world source coverage, final economics, localization and compatibility limits remain open. Recover from docs/DESIGN.md; exact candidates and acceptance limits belong to docs/TEST_LOG.md and docs/VERIFIED_RUNTIME_DATA.md.
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
