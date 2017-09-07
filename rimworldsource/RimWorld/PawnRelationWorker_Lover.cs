using System;
using Verse;
namespace RimWorld
{
	public class PawnRelationWorker_Lover : PawnRelationWorker
	{
		public override float GenerationChance(Pawn generated, Pawn other, PawnGenerationRequest request)
		{
			return LovePartnerRelationUtility.LovePartnerRelationGenerationChance(generated, other, request, false) * base.BaseGenerationChanceFactor(generated, other, request);
		}
		public override void CreateRelation(Pawn generated, Pawn other, PawnGenerationRequest request)
		{
			generated.relations.AddDirectRelation(PawnRelationDefOf.Lover, other);
			LovePartnerRelationUtility.TryToShareChildrenForGeneratedLovePartner(generated, other, request, 0.35f);
			PawnRelationWorker_Lover.ResolveMySkinColor(request, generated, other);
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
