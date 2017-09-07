using System;
using Verse;
namespace RimWorld
{
	public class PawnRelationWorker_Spouse : PawnRelationWorker
	{
		public override float GenerationChance(Pawn generated, Pawn other, PawnGenerationRequest request)
		{
			return LovePartnerRelationUtility.LovePartnerRelationGenerationChance(generated, other, request, false) * base.BaseGenerationChanceFactor(generated, other, request);
		}
		public override void CreateRelation(Pawn generated, Pawn other, PawnGenerationRequest request)
		{
			generated.relations.AddDirectRelation(PawnRelationDefOf.Spouse, other);
			LovePartnerRelationUtility.TryToShareChildrenForGeneratedLovePartner(generated, other, request, 1f);
			PawnRelationWorker_Spouse.ResolveMyName(request, generated);
			PawnRelationWorker_Spouse.ResolveMySkinColor(request, generated, other);
		}
		private static void ResolveMyName(PawnGenerationRequest request, Pawn generated)
		{
			if (request.fixedLastName != null)
			{
				return;
			}
			if (Rand.Value < 0.8f)
			{
				request.fixedLastName = ((NameTriple)generated.GetSpouse().Name).Last;
			}
		}
		private static void ResolveMySkinColor(PawnGenerationRequest request, Pawn generated, Pawn other)
		{
			float? fixedSkinWhiteness = request.fixedSkinWhiteness;
			if (fixedSkinWhiteness.HasValue)
			{
				return;
			}
			request.fixedSkinWhiteness = new float?(PawnSkinColors.GetRandomSkinColorSimilarTo(other.story.skinWhiteness, 0f, 1f));
		}
	}
}
