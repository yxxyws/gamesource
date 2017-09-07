using RimWorld;
using System;
namespace Verse.AI
{
	public abstract class JobGiver_ExitMap : ThinkNode_JobGiver
	{
		protected LocomotionUrgency locomotion;
		protected int jobMaxDuration = 999999;
		protected bool canBash;
		public override ThinkNode DeepCopy()
		{
			JobGiver_ExitMap jobGiver_ExitMap = (JobGiver_ExitMap)base.DeepCopy();
			jobGiver_ExitMap.locomotion = this.locomotion;
			jobGiver_ExitMap.jobMaxDuration = this.jobMaxDuration;
			jobGiver_ExitMap.canBash = this.canBash;
			return jobGiver_ExitMap;
		}
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			IntVec3 vec;
			if (!this.TryFindGoodExitDest(pawn, out vec))
			{
				return null;
			}
			return new Job(JobDefOf.Goto, vec)
			{
				exitMapOnArrival = true,
				locomotionUrgency = PawnUtility.ResolveLocomotion(pawn, this.locomotion, LocomotionUrgency.Jog),
				expiryInterval = this.jobMaxDuration,
				canBash = this.canBash
			};
		}
		protected abstract bool TryFindGoodExitDest(Pawn pawn, out IntVec3 dest);
	}
}
