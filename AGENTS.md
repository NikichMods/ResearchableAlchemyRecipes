# Researchable Alchemy Recipes — Working Contract

Follow [NikichMods/DevRules/WORKING_RULES.md](https://github.com/NikichMods/DevRules/blob/main/WORKING_RULES.md) and its action-based reading map. Local checkout work also follows CODEX_WORKFLOW.md; engineering, runtime, Git, licensing, CI and bootstrap guidance applies when its subject is entered. This file contains project-specific requirements only.

## Identity and phase

- Project: Researchable Alchemy Recipes.
- Repository: NikichMods/ResearchableAlchemyRecipes (public).
- Target: Graveyard Keeper 1.407, Windows PC, BepInEx 5.
- Phase: adopted product concept; design and mechanism verification. No production source or production candidate yet. Research-only Study Table Work Probe 0.1.0 on research/study-table-work verified the bounded native work/save-menu-load lifecycle; 0.1.1 demonstrated two-tab usability but its returned log exposes a key-map restoration exception, missing-icon errors and unassigned NGUI layer warnings. The returned 0.1.2 run confirms the key-map correction and native icon rendering/usability; picker-related NGUI layer warnings persist and remain unassigned. Original Material Study and Recipe Research artwork is user-selected; 0.1.3 embeds it and user screenshots approve artwork appearance; 0.1.4 executes alpha-bounds centering but its optical position is not user-accepted. Returned 0.1.5 calibration selects final optical corrections (+1,+1) for Material Study and (+2,+1) for Recipe Research. Research candidate 0.1.6 fixes those values and removes F2/input-capture tooling; exact-binary installed acceptance remains pending. Final UI, formula reward and full process-restart evidence remain open.
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
