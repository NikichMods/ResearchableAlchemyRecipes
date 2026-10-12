# Reagent heading palette — proposal

Status: the user positively reviewed the full static comparison and supports the readable icon-associated heading direction. Live styling is not installed or accepted. Current implementation/DLL is unchanged.

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
