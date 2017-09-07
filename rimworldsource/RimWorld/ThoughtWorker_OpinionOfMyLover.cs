using RimWorld.Planet;
using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class ThoughtWorker_OpinionOfMyLover : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			DirectPawnRelation directPawnRelation = LovePartnerRelationUtility.ExistingMostLikedLovePartnerRel(p, false);
			if (directPawnRelation == null)
			{
				return false;
			}
			if (!directPawnRelation.otherPawn.IsColonist || directPawnRelation.otherPawn.IsWorldPawn())
			{
				return false;
			}
			return p.relations.OpinionOf(directPawnRelation.otherPawn) != 0;
		}
		public override float GetMoodOffset(Pawn p, ThoughtStage stage)
		{
			float num = 0.1f * (float)p.relations.OpinionOf(LovePartnerRelationUtility.ExistingMostLikedLovePartnerRel(p, false).otherPawn);
			if (num < 0f)
			{
				return Mathf.Min(num, -1f);
			}
			return Mathf.Max(num, 1f);
		}
		public override string GetLabel(Pawn p, ThoughtStage stage)
		{
			DirectPawnRelation directPawnRelation = LovePartnerRelationUtility.ExistingMostLikedLovePartnerRel(p, false);
			string label = base.GetLabel(p, stage);
			return string.Format(label, directPawnRelation.def.GetGenderSpecificLabel(directPawnRelation.otherPawn), directPawnRelation.otherPawn.LabelBaseShort).CapitalizeFirst();
		}
	}
}
