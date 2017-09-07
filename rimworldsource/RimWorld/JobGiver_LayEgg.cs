using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobGiver_LayEgg : ThinkNode_JobGiver
	{
		private const float LayRadius = 5f;
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			CompEggLayer compEggLayer = pawn.TryGetComp<CompEggLayer>();
			if (compEggLayer == null || !compEggLayer.CanLayNow)
			{
				return null;
			}
			IntVec3 vec = RCellFinder.RandomWanderDestFor(pawn, pawn.Position, 5f, null, Danger.Some);
			return new Job(JobDefOf.LayEgg, vec);
		}
	}
}
