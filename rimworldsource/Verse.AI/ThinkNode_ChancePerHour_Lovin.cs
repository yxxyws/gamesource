using RimWorld;
using System;
namespace Verse.AI
{
	public class ThinkNode_ChancePerHour_Lovin : ThinkNode_ChancePerHour
	{
		protected override float ChancePerHour(Pawn pawn)
		{
			if (pawn.CurJob == null || !pawn.jobs.curDriver.layingDown || pawn.jobs.curDriver.layingDownBed == null)
			{
				return 0f;
			}
			Pawn partnerInMyBed = LovePartnerRelationUtility.GetPartnerInMyBed(pawn);
			if (partnerInMyBed == null)
			{
				return 0f;
			}
			return LovePartnerRelationUtility.GetLovinChancePerHour(pawn, partnerInMyBed);
		}
	}
}
