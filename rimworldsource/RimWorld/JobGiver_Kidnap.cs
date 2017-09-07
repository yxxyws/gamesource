using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobGiver_Kidnap : ThinkNode_JobGiver
	{
		public const float LordStateChangeSearchRadius = 8f;
		private const float VictimSearchRadius = 20f;
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			IntVec3 vec;
			if (!ExitUtility.TryFindClosestExitSpot(pawn, out vec, TraverseMode.ByPawn))
			{
				return null;
			}
			Pawn pawn2 = KidnapAIUtility.ClosestKidnapVictim(pawn, 20f, false);
			if (pawn2 == null)
			{
				return null;
			}
			return new Job(JobDefOf.Kidnap)
			{
				targetA = pawn2,
				targetB = vec,
				maxNumToCarry = 1
			};
		}
	}
}
