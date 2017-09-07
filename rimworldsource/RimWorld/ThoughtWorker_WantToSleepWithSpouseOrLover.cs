using RimWorld.Planet;
using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class ThoughtWorker_WantToSleepWithSpouseOrLover : ThoughtWorker
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
			if (p.ownership.OwnedBed != null && p.ownership.OwnedBed == directPawnRelation.otherPawn.ownership.OwnedBed)
			{
				return false;
			}
			if (p.relations.OpinionOf(directPawnRelation.otherPawn) <= 0)
			{
				return false;
			}
			return true;
		}
		public override float GetMoodOffset(Pawn p, ThoughtStage stage)
		{
			float a = -0.05f * (float)p.relations.OpinionOf(LovePartnerRelationUtility.ExistingMostLikedLovePartnerRel(p, false).otherPawn);
			return Mathf.Min(a, -1f);
		}
		public override string GetLabel(Pawn p, ThoughtStage stage)
		{
			DirectPawnRelation directPawnRelation = LovePartnerRelationUtility.ExistingMostLikedLovePartnerRel(p, false);
			string label = base.GetLabel(p, stage);
			return string.Format(label, directPawnRelation.otherPawn.LabelBaseShort).CapitalizeFirst();
		}
	}
}
