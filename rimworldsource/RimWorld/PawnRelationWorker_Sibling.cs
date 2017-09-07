using RimWorld.Planet;
using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class PawnRelationWorker_Sibling : PawnRelationWorker
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			return me != other && (me.GetMother() != null && me.GetFather() != null && me.GetMother() == other.GetMother() && me.GetFather() == other.GetFather());
		}
		public override float GenerationChance(Pawn generated, Pawn other, PawnGenerationRequest request)
		{
			float num = 1f;
			float num2 = 1f;
			if (other.GetFather() != null || other.GetMother() != null)
			{
				num = ChildRelationUtility.ChanceOfBecomingChildOf(generated, other.GetFather(), other.GetMother(), request, null, null);
			}
			else
			{
				float? fixedSkinWhiteness = request.fixedSkinWhiteness;
				if (fixedSkinWhiteness.HasValue)
				{
					num2 = ChildRelationUtility.GetSkinSimilarityFactor(request.fixedSkinWhiteness.Value, other.story.skinWhiteness);
				}
				else
				{
					num2 = PawnSkinColors.GetWhitenessCommonalityFactor(other.story.skinWhiteness);
				}
			}
			float num3 = Mathf.Abs(generated.ageTracker.AgeBiologicalYearsFloat - other.ageTracker.AgeBiologicalYearsFloat);
			if (num3 >= 31f)
			{
				return 0f;
			}
			if (num3 >= 36f)
			{
				return 0f;
			}
			float num4 = GenMath.LerpDouble(0f, 15f, 1f, 0.05f, num3);
			num4 = Mathf.Clamp(num4, 0.05f, 1f);
			return num * num2 * num4 * base.BaseGenerationChanceFactor(generated, other, request);
		}
		public override void CreateRelation(Pawn generated, Pawn other, PawnGenerationRequest request)
		{
			bool flag = other.GetMother() != null;
			bool flag2 = other.GetFather() != null;
			bool flag3 = Rand.Value < 0.85f;
			if (flag && LovePartnerRelationUtility.HasAnyLovePartner(other.GetMother()))
			{
				flag3 = false;
			}
			if (flag2 && LovePartnerRelationUtility.HasAnyLovePartner(other.GetFather()))
			{
				flag3 = false;
			}
			if (!flag)
			{
				Pawn newMother = PawnRelationWorker_Sibling.GenerateParent(generated, other, Gender.Female, request, flag3);
				other.SetMother(newMother);
			}
			generated.SetMother(other.GetMother());
			if (!flag2)
			{
				Pawn newFather = PawnRelationWorker_Sibling.GenerateParent(generated, other, Gender.Male, request, flag3);
				other.SetFather(newFather);
			}
			generated.SetFather(other.GetFather());
			if (!flag || !flag2)
			{
				bool flag4 = other.GetMother().story.traits.HasTrait(TraitDefOf.Gay) || other.GetFather().story.traits.HasTrait(TraitDefOf.Gay);
				if (flag4)
				{
					other.GetFather().relations.AddDirectRelation(PawnRelationDefOf.ExLover, other.GetMother());
				}
				else
				{
					if (flag3)
					{
						other.GetFather().relations.AddDirectRelation(PawnRelationDefOf.Spouse, other.GetMother());
					}
					else
					{
						LovePartnerRelationUtility.GiveRandomExLoverOrExSpouseRelation(other.GetFather(), other.GetMother());
					}
				}
			}
			PawnRelationWorker_Sibling.ResolveMyName(request, generated);
			PawnRelationWorker_Sibling.ResolveMySkinColor(request, generated);
		}
		private static Pawn GenerateParent(Pawn generatedChild, Pawn existingChild, Gender genderToGenerate, PawnGenerationRequest childRequest, bool newlyGeneratedParentsWillBeSpousesIfNotGay)
		{
			float ageBiologicalYearsFloat = generatedChild.ageTracker.AgeBiologicalYearsFloat;
			float ageBiologicalYearsFloat2 = existingChild.ageTracker.AgeBiologicalYearsFloat;
			float num = (genderToGenerate != Gender.Male) ? 14f : 14f;
			float num2 = (genderToGenerate != Gender.Male) ? 45f : 50f;
			float value = (genderToGenerate != Gender.Male) ? 27f : 30f;
			float num3 = Mathf.Max(ageBiologicalYearsFloat, ageBiologicalYearsFloat2) + num;
			float num4 = Mathf.Min(ageBiologicalYearsFloat, ageBiologicalYearsFloat2) + num2;
			float midAge = Mathf.Clamp(value, num3, num4);
			float value2;
			float value3;
			string last;
			PawnRelationWorker_Sibling.GenerateParentParams(num3, num4, midAge, generatedChild, existingChild, childRequest, out value2, out value3, out last);
			PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest();
			if (newlyGeneratedParentsWillBeSpousesIfNotGay && last.NullOrEmpty() && Rand.Value < 0.8f)
			{
				if (genderToGenerate == Gender.Male && existingChild.GetMother() != null && !existingChild.GetMother().story.traits.HasTrait(TraitDefOf.Gay))
				{
					last = ((NameTriple)existingChild.GetMother().Name).Last;
					pawnGenerationRequest.allowGay = false;
				}
				else
				{
					if (genderToGenerate == Gender.Female && existingChild.GetFather() != null && !existingChild.GetFather().story.traits.HasTrait(TraitDefOf.Gay))
					{
						last = ((NameTriple)existingChild.GetFather().Name).Last;
						pawnGenerationRequest.allowGay = false;
					}
				}
			}
			pawnGenerationRequest.forceGenerateNewPawn = true;
			pawnGenerationRequest.kindDef = existingChild.kindDef;
			pawnGenerationRequest.faction = existingChild.Faction;
			pawnGenerationRequest.fixedAge = new float?(value2);
			pawnGenerationRequest.fixedSkinWhiteness = new float?(value3);
			pawnGenerationRequest.fixedGender = new Gender?(genderToGenerate);
			pawnGenerationRequest.fixedLastName = last;
			pawnGenerationRequest.allowDead = true;
			pawnGenerationRequest.allowDowned = true;
			pawnGenerationRequest.canGenerateFamilyRelations = false;
			if (pawnGenerationRequest.faction == null || pawnGenerationRequest.faction.def == FactionDefOf.Colony)
			{
				bool tryMedievalOrBetter = pawnGenerationRequest.faction != null && pawnGenerationRequest.faction.def.techLevel >= TechLevel.Medieval;
				Find.FactionManager.TryGetRandomNonColonyHumanlikeFaction(out pawnGenerationRequest.faction, tryMedievalOrBetter);
			}
			Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
			if (!Find.WorldPawns.Contains(pawn))
			{
				Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Keep);
			}
			return pawn;
		}
		private static void GenerateParentParams(float minAge, float maxAge, float midAge, Pawn generatedChild, Pawn existingChild, PawnGenerationRequest childRequest, out float age, out float skinWhiteness, out string lastName)
		{
			age = Rand.GaussianAsymmetric(midAge, (midAge - minAge) / 2f, (maxAge - midAge) / 2f);
			age = Mathf.Clamp(age, minAge, maxAge);
			if (existingChild.GetFather() != null)
			{
				skinWhiteness = ParentRelationUtility.GetRandomSecondParentSkinColor(existingChild.GetFather().story.skinWhiteness, existingChild.story.skinWhiteness, childRequest.fixedSkinWhiteness);
			}
			else
			{
				if (existingChild.GetMother() != null)
				{
					skinWhiteness = ParentRelationUtility.GetRandomSecondParentSkinColor(existingChild.GetMother().story.skinWhiteness, existingChild.story.skinWhiteness, childRequest.fixedSkinWhiteness);
				}
				else
				{
					float? fixedSkinWhiteness = childRequest.fixedSkinWhiteness;
					if (!fixedSkinWhiteness.HasValue)
					{
						skinWhiteness = PawnSkinColors.GetRandomSkinColorSimilarTo(existingChild.story.skinWhiteness, 0f, 1f);
					}
					else
					{
						float num = Mathf.Min(childRequest.fixedSkinWhiteness.Value, existingChild.story.skinWhiteness);
						float num2 = Mathf.Max(childRequest.fixedSkinWhiteness.Value, existingChild.story.skinWhiteness);
						if (Rand.Value < 0.5f)
						{
							skinWhiteness = PawnSkinColors.GetRandomSkinColorSimilarTo(num, 0f, num);
						}
						else
						{
							skinWhiteness = PawnSkinColors.GetRandomSkinColorSimilarTo(num2, num2, 1f);
						}
					}
				}
			}
			lastName = null;
			if (!ChildRelationUtility.DefinitelyHasNotBirthName(existingChild) && ChildRelationUtility.ChildWantsNameOfAnyParent(existingChild))
			{
				if (existingChild.GetMother() == null && existingChild.GetFather() == null)
				{
					if (Rand.Value < 0.5f)
					{
						lastName = ((NameTriple)existingChild.Name).Last;
					}
				}
				else
				{
					string last = ((NameTriple)existingChild.Name).Last;
					string b = null;
					if (existingChild.GetMother() != null)
					{
						b = ((NameTriple)existingChild.GetMother().Name).Last;
					}
					else
					{
						if (existingChild.GetFather() != null)
						{
							b = ((NameTriple)existingChild.GetFather().Name).Last;
						}
					}
					if (last != b)
					{
						lastName = last;
					}
				}
			}
		}
		private static void ResolveMyName(PawnGenerationRequest request, Pawn generated)
		{
			if (request.fixedLastName != null)
			{
				return;
			}
			if (ChildRelationUtility.ChildWantsNameOfAnyParent(generated))
			{
				if (Rand.Value < 0.5f)
				{
					request.fixedLastName = ((NameTriple)generated.GetFather().Name).Last;
				}
				else
				{
					request.fixedLastName = ((NameTriple)generated.GetMother().Name).Last;
				}
			}
		}
		private static void ResolveMySkinColor(PawnGenerationRequest request, Pawn generated)
		{
			float? fixedSkinWhiteness = request.fixedSkinWhiteness;
			if (fixedSkinWhiteness.HasValue)
			{
				return;
			}
			request.fixedSkinWhiteness = new float?(ChildRelationUtility.GetRandomChildSkinColor(generated.GetFather().story.skinWhiteness, generated.GetMother().story.skinWhiteness));
		}
	}
}
