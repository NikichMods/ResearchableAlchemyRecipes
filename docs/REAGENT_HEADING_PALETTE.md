# Reagent heading palette — proposal

Status: static comparison prepared, not adopted or installed. The user authorizes inspection of the whole supported ingredient palette before selecting coloured headings. Current implementation/DLL is unchanged.

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

The user should evaluate the full sheet before choosing coloured headings. If approved as a candidate direction, combine its bounded native-ID heading mapping with the already-authorized common ingredient/energy calibration slider in a later research candidate after the footer wording is settled. Keep native font ownership/localization/layout rules; Russian is only the present prototype language. Broader palette additions require a deliberate rule, not an unverified default colour for an unknown reagent.
