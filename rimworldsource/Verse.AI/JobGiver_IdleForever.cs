using RimWorld;
using System;
namespace Verse.AI
{
	public class JobGiver_IdleForever : ThinkNode_JobGiver
	{
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			return new Job(JobDefOf.WaitDowned);
		}
	}
}
