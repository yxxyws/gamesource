using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobGiver_FleeExplosion : ThinkNode_JobGiver
	{
		public const float FleeDist = 9f;
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			if (pawn.RaceProps.intelligence < Intelligence.Humanlike)
			{
				return null;
			}
			if (pawn.mindState.knownWickBurner == null)
			{
				return null;
			}
			if (!pawn.mindState.knownWickBurner.Spawned)
			{
				pawn.mindState.knownWickBurner = null;
				return null;
			}
			if (PawnUtility.PlayerForcedJobNowOrSoon(pawn))
			{
				return null;
			}
			Thing knownWickBurner = pawn.mindState.knownWickBurner;
			if ((pawn.Position - knownWickBurner.Position).LengthHorizontalSquared > 81f)
			{
				return null;
			}
			IntVec3 vec;
			if (!RCellFinder.TryFindDirectFleeDestination(knownWickBurner.Position, 9f, pawn, out vec))
			{
				return null;
			}
			return new Job(JobDefOf.Goto, vec)
			{
				locomotionUrgency = LocomotionUrgency.Sprint
			};
		}
	}
}
