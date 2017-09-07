using System;
using Verse;
using Verse.AI;
using Verse.AI.Group;
namespace RimWorld
{
	internal class JobGiver_FightFiresNearPoint : ThinkNode_JobGiver
	{
		public float maxDistFromPoint = -1f;
		public override ThinkNode DeepCopy()
		{
			JobGiver_FightFiresNearPoint jobGiver_FightFiresNearPoint = (JobGiver_FightFiresNearPoint)base.DeepCopy();
			jobGiver_FightFiresNearPoint.maxDistFromPoint = this.maxDistFromPoint;
			return jobGiver_FightFiresNearPoint;
		}
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			Predicate<Thing> validator = delegate(Thing t)
			{
				Pawn pawn2 = ((AttachableThing)t).parent as Pawn;
				return pawn2 == null && pawn.CanReserve(t, 1);
			};
			Thing thing = GenClosest.ClosestThingReachable(pawn.GetLord().CurLordToil.FlagLoc, ThingRequest.ForDef(ThingDefOf.Fire), PathEndMode.Touch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), this.maxDistFromPoint, validator, null, -1, false);
			if (thing != null)
			{
				return new Job(JobDefOf.BeatFire, thing);
			}
			return null;
		}
	}
}
