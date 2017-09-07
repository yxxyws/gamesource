using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class PawnRelationWorker_Fiance : PawnRelationWorker
	{
		public override float GenerationChance(Pawn generated, Pawn other, PawnGenerationRequest request)
		{
			float num = 1f;
			num *= this.GetOldAgeFactor(generated);
			num *= this.GetOldAgeFactor(other);
			return LovePartnerRelationUtility.LovePartnerRelationGenerationChance(generated, other, request, false) * base.BaseGenerationChanceFactor(generated, other, request) * num;
		}
		public override void CreateRelation(Pawn generated, Pawn other, PawnGenerationRequest request)
		{
			generated.relations.AddDirectRelation(PawnRelationDefOf.Fiance, other);
			LovePartnerRelationUtility.TryToShareChildrenForGeneratedLovePartner(generated, other, request, 0.7f);
			PawnRelationWorker_Fiance.ResolveMySkinColor(request, generated, other);
		}
		private float GetOldAgeFactor(Pawn pawn)
		{
			return Mathf.Clamp(GenMath.LerpDouble(50f, 80f, 1f, 0.01f, (float)pawn.ageTracker.AgeBiologicalYears), 0.01f, 1f);
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
