using System;
using Verse;
namespace RimWorld
{
	public class PawnRelationWorker_Parent : PawnRelationWorker
	{
		public override float GenerationChance(Pawn generated, Pawn other, PawnGenerationRequest request)
		{
			float num = 0f;
			if (other.gender == Gender.Male)
			{
				num = ChildRelationUtility.ChanceOfBecomingChildOf(generated, other, other.GetSpouseOppositeGender(), request, null, null);
			}
			else
			{
				if (other.gender == Gender.Female)
				{
					num = ChildRelationUtility.ChanceOfBecomingChildOf(generated, other.GetSpouseOppositeGender(), other, request, null, null);
				}
			}
			return num * base.BaseGenerationChanceFactor(generated, other, request);
		}
		public override void CreateRelation(Pawn generated, Pawn other, PawnGenerationRequest request)
		{
			if (other.gender == Gender.Male)
			{
				generated.SetFather(other);
				Pawn spouseOppositeGender = other.GetSpouseOppositeGender();
				if (spouseOppositeGender != null)
				{
					generated.SetMother(spouseOppositeGender);
				}
				PawnRelationWorker_Parent.ResolveMyName(request, generated);
				PawnRelationWorker_Parent.ResolveMySkinColor(request, generated);
			}
			else
			{
				if (other.gender == Gender.Female)
				{
					generated.SetMother(other);
					Pawn spouseOppositeGender2 = other.GetSpouseOppositeGender();
					if (spouseOppositeGender2 != null)
					{
						generated.SetFather(spouseOppositeGender2);
					}
					PawnRelationWorker_Parent.ResolveMyName(request, generated);
					PawnRelationWorker_Parent.ResolveMySkinColor(request, generated);
				}
			}
		}
		private static void ResolveMyName(PawnGenerationRequest request, Pawn generatedChild)
		{
			if (request.fixedLastName != null)
			{
				return;
			}
			if (ChildRelationUtility.ChildWantsNameOfAnyParent(generatedChild))
			{
				bool flag = Rand.Value < 0.5f || generatedChild.GetMother() == null;
				if (generatedChild.GetFather() == null)
				{
					flag = false;
				}
				if (flag)
				{
					request.fixedLastName = ((NameTriple)generatedChild.GetFather().Name).Last;
				}
				else
				{
					request.fixedLastName = ((NameTriple)generatedChild.GetMother().Name).Last;
				}
			}
		}
		private static void ResolveMySkinColor(PawnGenerationRequest request, Pawn generatedChild)
		{
			float? fixedSkinWhiteness = request.fixedSkinWhiteness;
			if (fixedSkinWhiteness.HasValue)
			{
				return;
			}
			if (generatedChild.GetFather() != null && generatedChild.GetMother() != null)
			{
				request.fixedSkinWhiteness = new float?(ChildRelationUtility.GetRandomChildSkinColor(generatedChild.GetFather().story.skinWhiteness, generatedChild.GetMother().story.skinWhiteness));
			}
			else
			{
				if (generatedChild.GetFather() != null)
				{
					request.fixedSkinWhiteness = new float?(PawnSkinColors.GetRandomSkinColorSimilarTo(generatedChild.GetFather().story.skinWhiteness, 0f, 1f));
				}
				else
				{
					request.fixedSkinWhiteness = new float?(PawnSkinColors.GetRandomSkinColorSimilarTo(generatedChild.GetMother().story.skinWhiteness, 0f, 1f));
				}
			}
		}
	}
}
