# Reagent heading palette — proposal

## Adopted unoutlined light headings — next candidate

The user accepts the preview's selected light C colours without any outlines and chooses 35% source-row opacity. Other headings retain B. Implementation is pending; installed 0.1.28 remains unchanged. The user proposes dimming the equipment heading and moving the whole section last when it contains only goo, plus always placing the hand mixer last even when it has ordinary sources. The goo-only section rule and a universal hand-mixer ordering rule are being discussed; do not assume either has final acceptance yet.

## Selected outline experiment — 0.1.28

Returned screenshot feedback: outlined oil visually changes the perceived font, and 170/255 goo alpha is not sufficiently secondary. The user requests an offline preview before another DLL: keep C light colours but remove outlines; reduce goo/Study-row opacity further. Private Light-Headings-No-Outline-Preview.png compares current reconstruction against unoutlined C with 40% opacity and an additional 25% source-row sample. Includes all eight selected heading names. Native glyphs/parchment are reused; this is an offline reconstruction, not a modified screenshot or live rendering acceptance. No new palette/opacity value is adopted yet; installed 0.1.28 remains unchanged.

Installed 0.1.27 B is explicitly accepted by the user. The next requested comparison applies C only to alchemy_1_yellow, alchemy_2_yellow, alchemy_3_yellow, powder_white, taste_booster:salt, powder_silver, powder_gold and drop_oil. Other headings retain B. Each selected heading is a separate native text widget; its Outline effect is #4B3422 at (0.5, 0.5) native units. Unsupported/unknown font paths retain B and no outline locally; colour fallback does not disable the mod. This is an adopted experiment, not installed visual acceptance of C.

Direct installed 1.407 inspection finds all default fonts and the GJL Japanese/Chinese ch_jp_12 and Korean korean fonts are atlas-backed, without replacements. UIFont.packedFontShader returns false on this atlas path, so no built-in language is statically excluded. Actual multilingual outline appearance remains untested. A language change rebuilds the bubble through native current-language font selection. Shared UI_INPUT_TIME_AND_ENVIRONMENT.md owns reusable font/layout/alpha evidence.

Failed-experiment goo_ source rows follow ordinary sources within each equipment list and use native [AA] alpha (170/255) through the end of their Study/condition text, followed by [FF]. Material names, Study state and conditions are preserved; monster jelly remains an ordinary source. This subdued presentation still needs user readability acceptance.

## Adopted parchment palette B — 2026-10-13

The user explicitly selects column B for the entire palette: all 36 inputs, without outlines or mixing C into light headings. The B column below is the adopted mapping. Candidate 0.1.27 applies it; installed visual acceptance is separate from the accepted comparison.

READY: this research-only change replaces RGB constants in ReagentHeadingPalette and adds memory_powder's explicit entry. RecipeGuidance consumes Heading; native rich-text UILabel rendering with white label tint is the accepted final colour path. Scope is reagent section headings only. Preserve text, source catalogue, marker/body/title colours, layout, knowledge and native mechanics. Existing compile/formatter checks plus comparison of all mappings prove implementation; a short installed visual inspection establishes perceived colours. No new shader, font effect or runtime hook is involved.

## Complete parchment comparison — 2026-10-13

The comparison was prepared before changing the installed candidate. Four ignored review sheets cover all 36 ingredient names: powders (16), fluids (8), essences (8), and universal ingredients (4). Each compares A: 0.1.26 colours, B: more expressive unoutlined colours, and C: light colours with a thin dark outline. B is now selected; A/C remain comparison history.

The sheets use extracted native Russian tiny_font glyphs/advances, native item icons and back_paper02_rounded parchment, with a reconstructed nine-slice background. Glyphs are enlarged 2x by nearest-neighbour sampling; all 36 names fit their cells and have available glyphs. Private sheets/assets and generation scripts stay ignored under work/palette. This is an offline comparison, not a native rendered screenshot. Colours are keyed by item IDs, independent of localization; English and other native language layouts/font effects still require their own checks.

Outline C simulates native Outline's four diagonal copies using #4B3422 at one review pixel (0.5 native font pixel at the review scale). The live UILabel effect covers the entire label, so heading-only effects require separate heading labels. Native OnFill skips effects for packed bitmap-font shaders. The comparison does not establish live outline availability for the current or all language fonts. Whole-label outlines on source text are not adopted.

| Family / special input | B: unoutlined | C: outlined |
| --- | --- | --- |
| brown | #905322 | #D49B66 |
| d_blue | #3049AE | #819CEB |
| d_green | #477442 | #91C28B |
| d_violet | #7A2AA5 | #CA8AEB |
| green | #507B20 | #B3D75E |
| red | #AA342A | #F17F69 |
| violet | #7729A3 | #C795EF |
| yellow | #827019 | #F4D863 |
| alchemy_1_yellow_electro | #7B751B | #F0ED80 |
| ash | #686375 | #C3BECD |
| blood | #B12A39 | #F27787 |
| drop_alcohol | #486B92 | #AACAE9 |
| drop_oil | #98721C | #F5DA77 |
| powder_gold | #966B1E | #EFC35F |
| powder_graphite | #536C80 | #91ADBF |
| powder_silver | #547890 | #D4E5EE |
| powder_white | #777561 | #FFF9E8 |
| taste_booster:salt | #77704E | #EFE7CF |
| water | #21799E | #85D3F0 |
| memory_powder | #955E36 | #EDB69B |

The ordinary eight family colours apply consistently to all three reagent forms. Memory powder had no explicit palette entry in 0.1.26 and inherits body #4B3422 in A. Body/study-marker text remains #4B3422/#693606 in the comparison. The considered mixed B/C selection was not adopted; the user chose B throughout.

## Parchment adaptation — 0.1.24 candidate

The accepted dark-card colours below are historical after switching to native parchment tooltip. Returned 0.1.23 was readable but the user judged it too dark/almost black. Candidate 0.1.24 raises HLS lightness toward 0.32 and strengthens saturation for coloured families; lightness is bounded by calculated contrast >=3.50 against conservative #C59F69. This is an explicit candidate tradeoff: stronger visible colour, less contrast than 0.1.23's >=4.50; it requires user readability acceptance. All 35 stable IDs remain, with static RGB values and no runtime colour computation. White/ash/silver remain distinguishable dark neutrals. Central returned background samples are approximately #C9A877/#CAA97A. Native font ownership and body #4B3422, title #241A13, study marks/footer #693606 remain unchanged; labels have white tint. Numeric contrast is not live visual acceptance or an all-background guarantee.

| Family | Parchment RGB |
| --- | --- |
| brown | #6B4217 |
| d_blue | #1D3287 |
| d_green | #395235 |
| d_violet | #5F1D87 |
| green | #365412 |
| red | #87271D |
| violet | #551D87 |
| yellow | #564A12 |

| Other ingredient | Parchment RGB |
| --- | --- |
| alchemy_1_yellow_electro | #504C11 |
| ash | #4B4955 |
| blood | #871D25 |
| drop_alcohol | #3B4C61 |
| drop_oil | #5A4813 |
| powder_gold | #5D4814 |
| powder_graphite | #3E4D58 |
| powder_silver | #394E5B |
| powder_white | #4C4C36 |
| taste_booster:salt | #504C35 |
| water | #17516B |

The inspected native tiny_font glyph metadata contains digits 1/2 and comma, but lacks U+2013 (en dash). Combined equipment levels therefore use numeric `1, 2`; U+2014 used in output-count wording is present. This is the inspected font envelope, not proof for all language fonts.

## Historical dark-card palette

Status: the user explicitly adopted the following dark-card palette for ingredient headings. Research candidate 0.1.15 applied it to the then-known-yellow guidance slice. It is retained as historical evidence; it does not imply acceptance of the new parchment colours above or expand guidance to all formulas.

## Scope and comparison method

The accepted ordinary 1.407 corpus contains 35 participating ingredients (15 Powder, 8 Fluid, 8 Essence, 4 Universal). The comparison includes all 35 native Russian names/icons, plus native tiny_font bitmap glyphs/advances at an enlarged nearest-neighbour review scale. A solid #22232D background is sampled from the returned recipe-card screenshot; this is an offline comparison, not an exact live NGUI screenshot/shader/font-layout acceptance. Private extracted assets/font/localization and the generated sheet remain ignored under work/palette; LOCAL_ENVIRONMENT.md locates them. No formula rows or proprietary assets enter this document/repository history.

The palette is manually chosen, not automatic sprite-colour averaging. The eight ordinary triplet families share a heading colour across powder/fluid/essence forms; eleven other participants have explicit entries. Stable native item IDs own the mapping, independent of language. Similar icons may have similar colours: this is visual association, not a unique colour code. Names/section spacing retain structure when colour distinctions are weak.

All 35 candidate foregrounds have calculated sRGB luminance contrast between 6.74 and 12.29 against the sampled solid background. This is a quantitative comparison, not human acceptance, a colour-vision guarantee, or evidence for all runtime backgrounds/language fonts. No outline/shadow is needed in this static proposal. The existing amber #E5B86D pending-source mark is shown separately as a comparison.

## Candidate colours

For native IDs alchemy_1_<family>, alchemy_2_<family>, alchemy_3_<family>:

| Family suffix | Heading RGB |
| --- | --- |
| brown | #DAB792 |
| d_blue | #A8B4E3 |
| d_green | #9FBF9A |
| d_violet | #BDA0CD |
| green | #B2D091 |
| red | #E3A39C |
| violet | #D0B4E8 |
| yellow | #E6D78D |

| Other ingredient ID | Heading RGB |
| --- | --- |
| alchemy_1_yellow_electro | #E8E39B |
| ash | #CECDD4 |
| blood | #E0A0A5 |
| drop_alcohol | #C3CEDB |
| drop_oil | #E0CB8C |
| powder_gold | #E5CA88 |
| powder_graphite | #A8B6C2 |
| powder_silver | #CCD8DF |
| powder_white | #E5E5DA |
| taste_booster:salt | #D8D5C4 |
| water | #A6CFE2 |

## Findings and next decision

Static visual inspection finds a coherent readable full set without using dark text for dark icons. Muted indigo/sage/lilac preserve the dark families' association rather than turning every dark colour into the same grey. Neutral light entries cover white/silver/ash/graphite. Yellow/oil/gold headings are nearest to the amber pending-source mark; compare their semantic roles in the actual selected card before accepting colour styling. The proposal intentionally does not assign unique hues to every ingredient.

Combine the reviewed bounded native-ID heading mapping with the already-authorized common ingredient/energy calibration slider in a later research candidate after the footer wording is settled. Keep native font ownership/localization/layout rules; Russian is only the present prototype language. Broader palette additions require a deliberate rule, not an unverified default colour for an unknown reagent.

## Existing colour mechanism check (2026-10-12)

Question: whether the familiar coloured reagent names already have a native palette worth reusing. Bounded static inspection finds no per-reagent heading palette in the ordinary native item-tooltip path: ItemDefinition.GetItemName returns localization; GetTooltipData gives every item title the same HintTitle style; BubbleWidgetText.Draw applies UITextStyles.Set, which copies the shared style label colour. The 35 participating native Russian localization strings have no inline colour tags. BubbleWidgetAlchemyItem separately uses normal/not-complete study-state colours, not reagent hues. Source pointer: Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9, those four classes plus BubbleWidgetAlchemyItem. This does not establish absence of colour distinctions in every native screen.

The installed Decomp Delight 0.1.9 does have a separate 17-element configurable palette. Exact inspected DLL SHA-256: 8917a356303e31a4053894389761c25e66b6e3d17a0d12a0ae43f2e43fb85a99. Its ItemDefinition_GetTooltipData patch checks SurveyCompleted and derives a decomposition element; Utils.AddToTooltip inserts that localized element name with NGUI colour tags. This is an added element caption, not replacement colouring of the full native item title. The user's current config matches the listed defaults. The mechanism can explain familiar coloured alchemy text, but is not proof of what the user remembers seeing. IL inspection and config remain private under work/palette.

Directly importing that palette would not satisfy this card's readability goal: calculated contrast against #22232D is 1.02 for Death, 1.06 for Oil/Ash and 2.73 for Toxic, versus 6.74–12.29 for the reviewed proposal. Its Electric is cyan rather than the native participating powder icon's yellow; its Oil is dark grey. Thus this is useful prior art for element-based, language-independent styling, not a verified native palette or a drop-in source of our heading colours. No dependency, borrowed implementation or mod change was introduced by this investigation.
