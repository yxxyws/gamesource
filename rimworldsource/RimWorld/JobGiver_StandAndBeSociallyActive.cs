using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobGiver_StandAndBeSociallyActive : ThinkNode_JobGiver
	{
		public IntRange ticksRange = new IntRange(300, 600);
		public override ThinkNode DeepCopy()
		{
			JobGiver_StandAndBeSociallyActive jobGiver_StandAndBeSociallyActive = (JobGiver_StandAndBeSociallyActive)base.DeepCopy();
			jobGiver_StandAndBeSociallyActive.ticksRange = this.ticksRange;
			return jobGiver_StandAndBeSociallyActive;
		}
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			return new Job(JobDefOf.StandAndBeSociallyActive)
			{
				expiryInterval = this.ticksRange.RandomInRange
			};
		}
	}
}
