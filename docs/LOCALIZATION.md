# Localization — current research text

## Scope

Current .57 research source has 55 custom keys across the 11 native 1.407 languages: en, ru, de, fr, es, it, pt-br, pl, ja, zh_cn, ko (605 translations). This includes ingredient/source/Study guidance, adopted partial-topic wording, completion/notice/Clotho copy and temporary diagnostic labels/dialogs. Russian partial-topic text is `Часть рецептов известна. Открыть все варианты.`; native descriptions/names remain preserved. Original MPL-2.0 translations are in research/StudyTableWorkProbe/Localization.tsv, embedded as RAR.Localization.tsv; build manifests record the resource hash. No external translation package or native locale/font assets are shipped. TEST_LOG.md owns exact bounded rendering/editorial acceptance. Production extraction in src/ retains 22 keys across the same 11 locales (242 translations), with no diagnostic keys or dictionary injection. The adapted local suite verifies the retained key set, language switching and fallback; production installed rendering remains a separate gate. PRODUCTION_PREPARATION.md owns the extraction boundary. Historical .32-.35 checks below apply to their recorded key sets, not a new claim of exhaustive current-text validation.

Russian retains the exact adopted source wording and "Отмеченные материалы нужно сначала изучить на исследовательском столе." English header/marker/footer are "How to obtain ingredients", "not studied", "Marked materials must first be studied at the study table." Localized study-table terminology was checked against native names (including German Arbeitstisch). Native item/equipment/NPC names remain owned by the game. Two diagnostic item-name templates use the current native pot_heal dictionary value rather than another translated name list.

## Language ownership

Localization selects the current native GJL language at render time; unknown codes use English. AdditionalSources retains semantic method keys at registration, so switching language does not retain old translated text. The historical research implementation uses two custom diagnostic job names with namespaced nikichmods.rar.ui keys injected into the current native dictionary at startup and after the exact GJL.LoadLanguageResource(string) boundary. That native method initializes its dictionary before our postfix. Injection changes only our namespaced keys; failure of injection is logged locally. Cached exact private field access resolves GJL.cur_lng. No polling scan or save-language state is introduced.

The existing guidance bubble is recreated on native language change. Its current marker/footer are captured for width measurement, atomic wrapping and final renderer diagnostics, so these do not depend on a hard-coded Russian marker. Native BubbleWidgetText/GJL remain font owners. Source, Study, research, payment, DLC/quest gates, alpha hierarchy and panel render-group behavior are preserved.

## Checks and limits

- Compiled actual formatter/helper tests cover all keys/locales, native-language changes after source registration, item-use/extraction/cremation/purchase paths, namespaced injection preserving native keys, native diagnostic item-name substitution, English fallback, localized atomic Study markers with dimming/reset, source ordering and save-knowledge isolation. Existing knowledge and installed foreign quantity-checker suites also pass.
- Static glyph availability audit covers all translated strings (including substituted native diagnostic item names) against tiny/small/header fonts for eight Latin/Cyrillic locales, ch_jp_12 for Japanese/Chinese and korean for Korean: 27 locale/font checks, zero missing glyphs. One initially unsupported Chinese full-width semicolon was replaced with supported sentence punctuation. Private native data/inspection stays work/palette.
- These checks do not establish installed glyph rendering, every resolution/input/font-switch sequence or native-speaker editorial acceptance. Minimum combined runtime check: one familiar Russian tooltip, switch to English and inspect the same recipe; optionally switch back. This also covers the user's deferred 0.1.31 text inspection. Do not ask for eleven separate acceptance runs or repeat research/payment/persistence tests for localization alone.

The candidate remains research-only; localization does not select a production toolchain or promote runtime behavior to main.


Returned 0.1.32 feedback: user approves visible RU/EN localization and reports Italian appearance good; JA/ZH/KO screenshots show glyph-complete visible excerpts and preserved hierarchy, without editorial acceptance. Selected recipes jitter horizontally on several languages; this opens geometry acceptance independently of string/glyph checks. Candidate 0.1.33 preserves the table/resource and native font ownership, aligns the owned geometry to NGUI integer pixels/even content widths and allows the shared introductory subtitle to wrap without inflating short source lists. Its installed motion/width acceptance is pending.


Returned 0.1.33 acceptance: user switched all eleven native languages and reports no visual problems. Current font/layout/language-switch presentation is accepted for the observed session; native-speaker editorial validation and exhaustive resolutions/content remain outside that claim. TEST_LOG.md records exact candidate and bounded zero-motion observations.
