using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public static class ChildRelationUtility
	{
		public const float MinFemaleAgeToHaveChildren = 14f;
		public const float MaxFemaleAgeToHaveChildren = 45f;
		public const float UsualFemaleAgeToHaveChildren = 27f;
		public const float MinMaleAgeToHaveChildren = 14f;
		public const float MaxMaleAgeToHaveChildren = 50f;
		public const float UsualMaleAgeToHaveChildren = 30f;
		public const float ChanceForChildToHaveNameOfAnyParent = 0.99f;
		public static float ChanceOfBecomingChildOf(Pawn child, Pawn father, Pawn mother, PawnGenerationRequest childGenerationRequest, PawnGenerationRequest fatherGenerationRequest, PawnGenerationRequest motherGenerationRequest)
		{
			if (father != null && father.gender != Gender.Male)
			{
				Log.Warning("Tried to calculate chance for father with gender \"" + father.gender + "\".");
				return 0f;
			}
			if (mother != null && mother.gender != Gender.Female)
			{
				Log.Warning("Tried to calculate chance for mother with gender \"" + mother.gender + "\".");
				return 0f;
			}
			if (father != null && child.GetFather() != null && child.GetFather() != father)
			{
				return 0f;
			}
			if (mother != null && child.GetMother() != null && child.GetMother() != mother)
			{
				return 0f;
			}
			if (mother != null && father != null && !LovePartnerRelationUtility.LovePartnerRelationExists(mother, father) && !LovePartnerRelationUtility.ExLovePartnerRelationExists(mother, father))
			{
				return 0f;
			}
			float? skinWhiteness = ChildRelationUtility.GetSkinWhiteness(child, childGenerationRequest);
			float? skinWhiteness2 = ChildRelationUtility.GetSkinWhiteness(father, fatherGenerationRequest);
			float? skinWhiteness3 = ChildRelationUtility.GetSkinWhiteness(mother, motherGenerationRequest);
			bool fatherIsNew = father != null && child.GetFather() != father;
			bool motherIsNew = mother != null && child.GetMother() != mother;
			float skinColorFactor = ChildRelationUtility.GetSkinColorFactor(skinWhiteness, skinWhiteness2, skinWhiteness3, fatherIsNew, motherIsNew);
			if (skinColorFactor <= 0f)
			{
				return 0f;
			}
			float num = 1f;
			float num2 = 1f;
			float num3 = 1f;
			float num4 = 1f;
			if (father != null && child.GetFather() == null)
			{
				float num5 = father.ageTracker.AgeBiologicalYearsFloat - child.ageTracker.AgeBiologicalYearsFloat;
				if (num5 <= 0f)
				{
					return 0f;
				}
				num = ChildRelationUtility.GetAgeFactor(num5, 14f, 50f, 30f);
				if (father.story.traits.HasTrait(TraitDefOf.Gay))
				{
					num4 = 0.1f;
				}
			}
			if (mother != null && child.GetMother() == null)
			{
				float num6 = mother.ageTracker.AgeBiologicalYearsFloat - child.ageTracker.AgeBiologicalYearsFloat;
				if (num6 <= 0f)
				{
					return 0f;
				}
				int num7 = ChildRelationUtility.NumberOfChildrenFemaleWantsEver(mother);
				if (mother.relations.ChildrenCount >= num7)
				{
					return 0f;
				}
				num2 = ChildRelationUtility.GetAgeFactor(num6, 14f, 45f, 27f);
				num3 = 1f - (float)mother.relations.ChildrenCount / (float)num7;
				if (mother.story.traits.HasTrait(TraitDefOf.Gay))
				{
					num4 = 0.1f;
				}
			}
			float num8 = 1f;
			if (mother != null)
			{
				Pawn firstDirectRelationPawn = mother.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Spouse, null);
				if (firstDirectRelationPawn != null && firstDirectRelationPawn != father)
				{
					num8 *= 0.15f;
				}
			}
			if (father != null)
			{
				Pawn firstDirectRelationPawn2 = father.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Spouse, null);
				if (firstDirectRelationPawn2 != null && firstDirectRelationPawn2 != mother)
				{
					num8 *= 0.15f;
				}
			}
			return skinColorFactor * num * num2 * num3 * num8 * num4;
		}
		public static bool ChildWantsNameOfAnyParent(Pawn child)
		{
			Rand.PushSeed();
			Rand.Seed = child.thingIDNumber * 7;
			bool result = Rand.Value < 0.99f;
			Rand.PopSeed();
			return result;
		}
		private static int NumberOfChildrenFemaleWantsEver(Pawn female)
		{
			Rand.PushSeed();
			Rand.Seed = female.thingIDNumber * 3;
			int result = Rand.RangeInclusive(0, 3);
			Rand.PopSeed();
			return result;
		}
		private static float? GetSkinWhiteness(Pawn pawn, PawnGenerationRequest request)
		{
			if (request != null)
			{
				return request.fixedSkinWhiteness;
			}
			if (pawn != null)
			{
				return new float?(pawn.story.skinWhiteness);
			}
			return null;
		}
		private static float GetAgeFactor(float ageAtBirth, float min, float max, float mid)
		{
			return GenMath.GetFactorInInterval(min, mid, max, 1.6f, ageAtBirth);
		}
		private static float GetSkinColorFactor(float? childSkinWhiteness, float? fatherSkinWhiteness, float? motherSkinWhiteness, bool fatherIsNew, bool motherIsNew)
		{
			if (childSkinWhiteness.HasValue && fatherSkinWhiteness.HasValue && motherSkinWhiteness.HasValue)
			{
				float num = Mathf.Min(fatherSkinWhiteness.Value, motherSkinWhiteness.Value);
				float num2 = Mathf.Max(fatherSkinWhiteness.Value, motherSkinWhiteness.Value);
				if (childSkinWhiteness.HasValue && childSkinWhiteness.Value < num - 0.05f)
				{
					return 0f;
				}
				if (childSkinWhiteness.HasValue && childSkinWhiteness.Value > num2 + 0.05f)
				{
					return 0f;
				}
			}
			float num3 = 1f;
			if (fatherIsNew)
			{
				num3 *= ChildRelationUtility.GetNewParentSkinColorFactor(fatherSkinWhiteness, motherSkinWhiteness, childSkinWhiteness);
			}
			if (motherIsNew)
			{
				num3 *= ChildRelationUtility.GetNewParentSkinColorFactor(motherSkinWhiteness, fatherSkinWhiteness, childSkinWhiteness);
			}
			return num3;
		}
		private static float GetNewParentSkinColorFactor(float? newParentSkinWhiteness, float? otherParentSkinWhiteness, float? childSkinWhiteness)
		{
			if (newParentSkinWhiteness.HasValue)
			{
				if (!otherParentSkinWhiteness.HasValue)
				{
					if (childSkinWhiteness.HasValue)
					{
						return ChildRelationUtility.GetSkinSimilarityFactor(newParentSkinWhiteness.Value, childSkinWhiteness.Value);
					}
					return PawnSkinColors.GetWhitenessCommonalityFactor(newParentSkinWhiteness.Value);
				}
				else
				{
					if (childSkinWhiteness.HasValue)
					{
						float reflectedSkin = ChildRelationUtility.GetReflectedSkin(otherParentSkinWhiteness.Value, childSkinWhiteness.Value);
						return ChildRelationUtility.GetSkinSimilarityFactor(newParentSkinWhiteness.Value, reflectedSkin);
					}
					float skinWhiteness = (newParentSkinWhiteness.Value + otherParentSkinWhiteness.Value) / 2f;
					return PawnSkinColors.GetWhitenessCommonalityFactor(skinWhiteness);
				}
			}
			else
			{
				if (!otherParentSkinWhiteness.HasValue)
				{
					if (childSkinWhiteness.HasValue)
					{
						return PawnSkinColors.GetWhitenessCommonalityFactor(childSkinWhiteness.Value);
					}
					return 1f;
				}
				else
				{
					if (childSkinWhiteness.HasValue)
					{
						float reflectedSkin2 = ChildRelationUtility.GetReflectedSkin(otherParentSkinWhiteness.Value, childSkinWhiteness.Value);
						return PawnSkinColors.GetWhitenessCommonalityFactor(reflectedSkin2);
					}
					return PawnSkinColors.GetWhitenessCommonalityFactor(otherParentSkinWhiteness.Value);
				}
			}
		}
		public static float GetReflectedSkin(float value, float mirror)
		{
			return Mathf.Clamp01(GenMath.Reflection(value, mirror));
		}
		public static float GetSkinSimilarityFactor(float skinWhiteness1, float skinWhiteness2)
		{
			float min = Mathf.Clamp01(skinWhiteness1 - 0.15f);
			float max = Mathf.Clamp01(skinWhiteness1 + 0.15f);
			return GenMath.GetFactorInInterval(min, skinWhiteness1, max, 2.5f, skinWhiteness2);
		}
		public static float GetRandomChildSkinColor(float fatherSkinWhiteness, float motherSkinWhiteness)
		{
			float clampMin = Mathf.Min(fatherSkinWhiteness, motherSkinWhiteness);
			float clampMax = Mathf.Max(fatherSkinWhiteness, motherSkinWhiteness);
			float value = (fatherSkinWhiteness + motherSkinWhiteness) / 2f;
			return PawnSkinColors.GetRandomSkinColorSimilarTo(value, clampMin, clampMax);
		}
		public static bool DefinitelyHasNotBirthName(Pawn pawn)
		{
			Pawn spouse = pawn.GetSpouse();
			if (spouse == null)
			{
				return false;
			}
			string last = ((NameTriple)spouse.Name).Last;
			return !(((NameTriple)pawn.Name).Last != last) && ((spouse.GetMother() != null && ((NameTriple)spouse.GetMother().Name).Last == last) || (spouse.GetFather() != null && ((NameTriple)spouse.GetFather().Name).Last == last));
		}
	}
}
