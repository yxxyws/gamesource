using RimWorld;
using System;
namespace Verse.AI
{
	public class JobGiver_Idle : ThinkNode_JobGiver
	{
		public int ticks = 50;
		public override ThinkNode DeepCopy()
		{
			JobGiver_Idle jobGiver_Idle = (JobGiver_Idle)base.DeepCopy();
			jobGiver_Idle.ticks = this.ticks;
			return jobGiver_Idle;
		}
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			return new Job(JobDefOf.Wait)
			{
				expiryInterval = this.ticks
			};
		}
	}
}
