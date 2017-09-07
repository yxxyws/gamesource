using RimWorld;
using System;
namespace Verse.AI
{
	public class JobGiver_IdleError : ThinkNode_JobGiver
	{
		private const int WaitTime = 100;
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			Log.ErrorOnce(pawn + " issued IdleError wait job. The behavior tree should never get here.", 532983);
			return new Job(JobDefOf.Wait)
			{
				expiryInterval = 100
			};
		}
	}
}
