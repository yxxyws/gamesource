using System;
using Verse;
namespace RimWorld
{
	public class ThinkNode_ConditionalShouldFollowMaster : ThinkNode_Conditional
	{
		protected override bool Satisfied(Pawn pawn)
		{
			if (pawn.playerSettings == null || pawn.playerSettings.master == null || !pawn.playerSettings.master.Spawned)
			{
				return false;
			}
			Pawn master = pawn.playerSettings.master;
			return master.Drafted || (master.CurJob != null && master.CurJob.def == JobDefOf.Hunt);
		}
	}
}
