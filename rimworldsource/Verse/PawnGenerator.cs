using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Verse
{
	public static class PawnGenerator
	{
		public const int MaxStartMentalStateThreshold = 40;
		private static SimpleCurve DefaultAgeGenerationCurve = new SimpleCurve
		{
			new CurvePoint(0.05f, 0f),
			new CurvePoint(0.1f, 100f),
			new CurvePoint(0.675f, 100f),
			new CurvePoint(0.75f, 30f),
			new CurvePoint(0.875f, 18f),
			new CurvePoint(1f, 10f),
			new CurvePoint(1.125f, 3f),
			new CurvePoint(1.25f, 0f)
		};
		public static Pawn GeneratePawn(PawnKindDef kindDef, Faction faction)
		{
			return PawnGenerator.GeneratePawn(new PawnGenerationRequest
			{
				kindDef = kindDef,
				faction = faction
			});
		}
		public static Pawn GeneratePawn(PawnGenerationRequest request)
		{
			if (request.kindDef.RaceProps.Humanlike && request.faction == null)
			{
				request.faction = FactionUtility.DefaultFactionFrom(request.kindDef.defaultFactionType);
				Log.Error(string.Concat(new object[]
				{
					"Tried to generate pawn of Humanlike race ",
					request.kindDef,
					" with null faction. Setting to ",
					request.faction
				}));
			}
			Pawn pawn = null;
			if (!request.newborn && !request.forceGenerateNewPawn && Rand.Value < PawnGenerator.ChanceToRedressAnyWorldPawn())
			{
				IEnumerable<Pawn> source = 
					from x in Find.WorldPawns.GetPawnsBySituation(WorldPawnSituation.Free)
					where PawnGenerator.IsValidCandidateToRedress(x, request)
					select x;
				if (source.TryRandomElementByWeight((Pawn x) => PawnGenerator.WorldPawnSelectionWeight(x), out pawn))
				{
					PawnGenerator.RedressPawn(pawn, request);
					Find.WorldPawns.RemovePawn(pawn);
				}
			}
			if (pawn == null)
			{
				pawn = PawnGenerator.GenerateNewNakedPawn(request, 0);
				if (pawn == null)
				{
					return null;
				}
				if (!request.newborn)
				{
					PawnGenerator.GenerateGearFor(pawn, request);
				}
			}
			return pawn;
		}
		public static void RedressPawn(Pawn pawn, PawnGenerationRequest request)
		{
			pawn.kindDef = request.kindDef;
			PawnGenerator.GenerateGearFor(pawn, request);
		}
		private static bool IsValidCandidateToRedress(Pawn pawn, PawnGenerationRequest request)
		{
			if (pawn.def != request.kindDef.race)
			{
				return false;
			}
			if (pawn.Faction != request.faction)
			{
				return false;
			}
			if (!request.allowDead && (pawn.Dead || pawn.Destroyed))
			{
				return false;
			}
			if (!request.allowDowned && pawn.Downed)
			{
				return false;
			}
			if (pawn.health.hediffSet.BleedingRate > 0.001f)
			{
				return false;
			}
			if (!request.canGenerateFamilyRelations && pawn.RaceProps.IsFlesh && pawn.relations.RelatedToAnyoneOrAnyoneRelatedToMe)
			{
				return false;
			}
			float? fixedAge = request.fixedAge;
			if (fixedAge.HasValue && pawn.ageTracker.AgeBiologicalYearsFloat != request.fixedAge)
			{
				return false;
			}
			Gender? fixedGender = request.fixedGender;
			if (fixedGender.HasValue && pawn.gender != request.fixedGender)
			{
				return false;
			}
			if (request.fixedLastName != null && ((NameTriple)pawn.Name).Last != request.fixedLastName)
			{
				return false;
			}
			float? fixedSkinWhiteness = request.fixedSkinWhiteness;
			return (!fixedSkinWhiteness.HasValue || pawn.story == null || !(pawn.story.skinWhiteness != request.fixedSkinWhiteness)) && (!request.mustBeCapableOfViolence || pawn.story == null || !pawn.story.WorkTagIsDisabled(WorkTags.Violent));
		}
		private static Pawn GenerateNewNakedPawn(PawnGenerationRequest request, int tries = 0)
		{
			Pawn pawn = (Pawn)ThingMaker.MakeThing(request.kindDef.race, null);
			pawn.kindDef = request.kindDef;
			pawn.SetFactionDirect(request.faction);
			PawnComponentsUtility.CreateInitialComponents(pawn);
			if (pawn.guest != null)
			{
				pawn.guest.Init();
			}
			Gender? fixedGender = request.fixedGender;
			if (fixedGender.HasValue)
			{
				pawn.gender = request.fixedGender.Value;
			}
			else
			{
				if (pawn.RaceProps.hasGenders)
				{
					if (Rand.Value < 0.5f)
					{
						pawn.gender = Gender.Male;
					}
					else
					{
						pawn.gender = Gender.Female;
					}
				}
				else
				{
					pawn.gender = Gender.None;
				}
			}
			PawnGenerator.GenerateRandomAge(pawn, request);
			pawn.needs.SetInitialLevels();
			PawnGenerator.GenerateInitialHediffs(pawn, request);
			if (!request.newborn && request.canGenerateFamilyRelations)
			{
				PawnGenerator.GenerateFamilyRelations(pawn, request);
			}
			if (pawn.RaceProps.Humanlike)
			{
				Pawn_StoryTracker arg_126_0 = pawn.story;
				float? fixedSkinWhiteness = request.fixedSkinWhiteness;
				arg_126_0.skinWhiteness = ((!fixedSkinWhiteness.HasValue) ? PawnSkinColors.RandomSkinWhiteness() : request.fixedSkinWhiteness.Value);
				pawn.story.crownType = ((Rand.Value >= 0.5f) ? CrownType.Narrow : CrownType.Average);
				pawn.story.hairColor = PawnHairColors.RandomHairColor(pawn.story.SkinColor, pawn.ageTracker.AgeBiologicalYears);
				PawnBioGenerator.GiveAppropriateBioTo(pawn, request.fixedLastName);
				pawn.story.hairDef = PawnHairChooser.RandomHairDefFor(pawn, request.faction.def);
				PawnGenerator.GiveRandomTraitsTo(pawn, request);
				pawn.story.GenerateSkillsFromBackstory();
			}
			if (pawn.workSettings != null && request.faction.def == FactionDefOf.Colony)
			{
				pawn.workSettings.EnableAndInitialize();
			}
			if (request.faction != null && pawn.RaceProps.Animal)
			{
				pawn.CheckSetNameFactionChanged();
			}
			if (!request.allowDead && (pawn.Dead || pawn.Destroyed))
			{
				if (pawn.relations != null)
				{
					pawn.relations.ClearAllRelations();
				}
				if (tries < 10)
				{
					return PawnGenerator.GenerateNewNakedPawn(request, tries + 1);
				}
				Log.Error(string.Concat(new object[]
				{
					"Generated dead pawn ",
					pawn.ThingID,
					". Too many tries, returning null. Generation request: ",
					request
				}));
				return null;
			}
			else
			{
				if (!request.allowDowned && pawn.Downed)
				{
					if (pawn.relations != null)
					{
						pawn.relations.ClearAllRelations();
					}
					if (tries < 10)
					{
						return PawnGenerator.GenerateNewNakedPawn(request, tries + 1);
					}
					Log.Error(string.Concat(new object[]
					{
						"Generated downed pawn ",
						pawn.ThingID,
						". Too many tries, returning null. Generation request: ",
						request
					}));
					return null;
				}
				else
				{
					if (!request.mustBeCapableOfViolence || pawn.story == null || !pawn.story.WorkTagIsDisabled(WorkTags.Violent))
					{
						return pawn;
					}
					if (pawn.relations != null)
					{
						pawn.relations.ClearAllRelations();
					}
					if (tries < 10)
					{
						return PawnGenerator.GenerateNewNakedPawn(request, tries + 1);
					}
					Log.Error(string.Concat(new object[]
					{
						"Generated pawn incapable of violence ",
						pawn.ThingID,
						". Too many tries, returning null. Generation request: ",
						request
					}));
					return null;
				}
			}
		}
		private static float ChanceToRedressAnyWorldPawn()
		{
			int pawnsBySituationCount = Find.WorldPawns.GetPawnsBySituationCount(WorldPawnSituation.Free);
			if (pawnsBySituationCount == 0)
			{
				return 0f;
			}
			return Mathf.Min(0.02f + 0.01f * ((float)pawnsBySituationCount / 25f), 0.8f);
		}
		private static float WorldPawnSelectionWeight(Pawn p)
		{
			if (p.RaceProps.IsFlesh && !p.relations.everSeenByPlayer && p.relations.RelatedToAnyoneOrAnyoneRelatedToMe)
			{
				return 0.1f;
			}
			return 1f;
		}
		private static void GenerateGearFor(Pawn pawn, PawnGenerationRequest request)
		{
			PawnApparelGenerator.GenerateStartingApparelFor(pawn, request);
			PawnWeaponGenerator.TryGenerateWeaponFor(pawn);
			PawnInventoryGenerator.GenerateInventoryFor(pawn);
		}
		private static void GenerateInitialHediffs(Pawn pawn, PawnGenerationRequest request)
		{
			int num = 0;
			while (true)
			{
				AgeInjuryUtility.GenerateRandomOldAgeInjuries(pawn, !request.allowDead);
				PawnTechHediffsGenerator.GeneratePartsAndImplantsFor(pawn);
				if (request.allowDead && pawn.Dead)
				{
					break;
				}
				if (request.allowDowned || !pawn.Downed)
				{
					return;
				}
				pawn.health.Reset();
				num++;
				if (num > 80)
				{
					goto Block_4;
				}
			}
			return;
			Block_4:
			Log.Warning(string.Concat(new object[]
			{
				"Could not generate old age injuries for ",
				pawn.ThingID,
				" of age ",
				pawn.ageTracker.AgeBiologicalYears,
				" that allow pawn to move after ",
				80,
				" tries. request=",
				request
			}));
		}
		private static void GenerateRandomAge(Pawn pawn, PawnGenerationRequest request)
		{
			if (request.newborn)
			{
				pawn.ageTracker.AgeBiologicalTicks = 0L;
			}
			else
			{
				float? fixedAge = request.fixedAge;
				float num;
				if (fixedAge.HasValue)
				{
					num = request.fixedAge.Value;
				}
				else
				{
					int num2 = 0;
					while (true)
					{
						if (pawn.RaceProps.ageGenerationCurve != null)
						{
							num = (float)Mathf.RoundToInt(Rand.ByCurve(pawn.RaceProps.ageGenerationCurve, 200));
						}
						else
						{
							if (pawn.RaceProps.IsMechanoid)
							{
								num = (float)Rand.Range(0, 2500);
							}
							else
							{
								num = Rand.ByCurve(PawnGenerator.DefaultAgeGenerationCurve, 200) * pawn.RaceProps.lifeExpectancy;
							}
						}
						num2++;
						if (num2 > 300)
						{
							break;
						}
						if (num <= (float)pawn.kindDef.maxGenerationAge && num >= (float)pawn.kindDef.minGenerationAge)
						{
							goto IL_101;
						}
					}
					Log.Error("Tried 300 times to generate age for " + pawn);
				}
				IL_101:
				pawn.ageTracker.AgeBiologicalTicks = (long)(num * 3600000f) + (long)Rand.Range(0, 3600000);
			}
			int num3;
			if (Rand.Value < pawn.kindDef.backstoryCryptosleepCommonality)
			{
				float value = Rand.Value;
				if (value < 0.7f)
				{
					num3 = Rand.Range(0, 100);
				}
				else
				{
					if (value < 0.95f)
					{
						num3 = Rand.Range(100, 1000);
					}
					else
					{
						int max = GenDate.CurrentYear - 2026 - pawn.ageTracker.AgeBiologicalYears;
						num3 = Rand.Range(1000, max);
					}
				}
			}
			else
			{
				num3 = 0;
			}
			int ticksAbs = GenTicks.TicksAbs;
			long num4 = (long)ticksAbs - pawn.ageTracker.AgeBiologicalTicks;
			num4 -= (long)num3 * 3600000L;
			pawn.ageTracker.BirthAbsTicks = num4;
		}
		public static int RandomTraitDegree(TraitDef traitDef)
		{
			if (traitDef.degreeDatas.Count == 1)
			{
				return traitDef.degreeDatas[0].degree;
			}
			return traitDef.degreeDatas.RandomElementByWeight((TraitDegreeData dd) => dd.Commonality).degree;
		}
		private static void GiveRandomTraitsTo(Pawn pawn, PawnGenerationRequest request)
		{
			if (pawn.story == null)
			{
				return;
			}
			int num = Rand.RangeInclusive(2, 3);
			if (request.allowGay && (LovePartnerRelationUtility.HasAnyLovePartnerOfTheSameGender(pawn) || LovePartnerRelationUtility.HasAnyExLovePartnerOfTheSameGender(pawn)))
			{
				Trait trait = new Trait(TraitDefOf.Gay);
				trait.degree = PawnGenerator.RandomTraitDegree(trait.def);
				pawn.story.traits.GainTrait(trait);
			}
			while (pawn.story.traits.allTraits.Count < num)
			{
				TraitDef newTraitDef = DefDatabase<TraitDef>.AllDefsListForReading.RandomElementByWeight((TraitDef tr) => tr.GetGenderSpecificCommonality(pawn));
				if (!pawn.story.traits.HasTrait(newTraitDef))
				{
					if (newTraitDef == TraitDefOf.Gay)
					{
						if (!request.allowGay)
						{
							continue;
						}
						if (LovePartnerRelationUtility.HasAnyLovePartnerOfTheOppositeGender(pawn) || LovePartnerRelationUtility.HasAnyExLovePartnerOfTheOppositeGender(pawn))
						{
							continue;
						}
					}
					if (!pawn.story.traits.allTraits.Any((Trait tr) => newTraitDef.ConflictsWith(tr)) && (newTraitDef.conflictingTraits == null || !newTraitDef.conflictingTraits.Any((TraitDef tr) => pawn.story.traits.HasTrait(tr))))
					{
						if (newTraitDef.requiredWorkTypes == null || !pawn.story.OneOfWorkTypesIsDisabled(newTraitDef.requiredWorkTypes))
						{
							if (!pawn.story.WorkTagIsDisabled(newTraitDef.requiredWorkTags))
							{
								Trait trait2 = new Trait(newTraitDef);
								trait2.degree = PawnGenerator.RandomTraitDegree(trait2.def);
								if (pawn.mindState == null || pawn.mindState.mentalStateStarter == null || pawn.mindState.mentalStateStarter.StartHardMentalStateThreshold + trait2.OffsetOfStat(StatDefOf.MentalBreakThreshold) <= 40f)
								{
									pawn.story.traits.GainTrait(trait2);
								}
							}
						}
					}
				}
			}
		}
		public static void PostProcessGeneratedGear(Thing gear, Pawn pawn)
		{
			CompQuality compQuality = gear.TryGetComp<CompQuality>();
			if (compQuality != null)
			{
				compQuality.SetQuality(QualityUtility.RandomGeneratedGearQuality(pawn.kindDef), ArtGenerationContext.Outsider);
			}
			if (gear.def.useHitPoints)
			{
				float randomInRange = pawn.kindDef.gearHealthRange.RandomInRange;
				if (randomInRange < 1f)
				{
					int num = Mathf.RoundToInt(randomInRange * (float)gear.MaxHitPoints);
					num = Mathf.Max(1, num);
					gear.HitPoints = num;
				}
			}
		}
		private static void GenerateFamilyRelations(Pawn pawn, PawnGenerationRequest request)
		{
			if (!pawn.RaceProps.Humanlike)
			{
				return;
			}
			List<KeyValuePair<Pawn, PawnRelationDef>> list = new List<KeyValuePair<Pawn, PawnRelationDef>>();
			List<PawnRelationDef> allDefsListForReading = DefDatabase<PawnRelationDef>.AllDefsListForReading;
			IEnumerable<Pawn> enumerable = 
				from x in PawnUtility.AllPawnsMapOrWorldAliveOrDead
				where x.def == pawn.def
				select x;
			foreach (Pawn current in enumerable)
			{
				if (current.ThingState == ThingState.Discarded)
				{
					Log.Warning(string.Concat(new object[]
					{
						"Warning during generating family relations for ",
						pawn,
						": Pawn ",
						current,
						" is discarded, yet he was yielded by PawnUtility. Discarding a pawn means that he is no longer managed by anything."
					}));
				}
				else
				{
					for (int i = 0; i < allDefsListForReading.Count; i++)
					{
						if (allDefsListForReading[i].generationChanceFactor > 0f)
						{
							list.Add(new KeyValuePair<Pawn, PawnRelationDef>(current, allDefsListForReading[i]));
						}
					}
				}
			}
			KeyValuePair<Pawn, PawnRelationDef> keyValuePair = list.RandomElementByWeightWithDefault((KeyValuePair<Pawn, PawnRelationDef> x) => x.Value.generationChanceFactor * x.Value.Worker.GenerationChance(pawn, x.Key, request), 55f);
			if (keyValuePair.Key == null)
			{
				return;
			}
			keyValuePair.Value.Worker.CreateRelation(pawn, keyValuePair.Key, request);
		}
	}
}
