using RimWorld;
using System;
namespace Verse.AI
{
	public class JobGiver_Orders : ThinkNode_JobGiver
	{
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			if (pawn.Drafted)
			{
				return new Job(JobDefOf.WaitCombat, pawn.Position);
			}
			return null;
		}
	}
}
