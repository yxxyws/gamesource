using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobGiver_Berserk : ThinkNode_JobGiver
	{
		private const float MaxAttackDistance = 30f;
		private const float WaitChance = 0.5f;
		private const int WaitTicks = 90;
		private const int MinMeleeChaseTicks = 420;
		private const int MaxMeleeChaseTicks = 900;
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			if (Rand.Value < 0.5f)
			{
				return new Job(JobDefOf.WaitCombat)
				{
					expiryInterval = 90
				};
			}
			Predicate<Thing> validator = delegate(Thing t)
			{
				Pawn pawn3 = t as Pawn;
				return !pawn3.Downed && t != pawn;
			};
			Pawn pawn2 = (Pawn)GenClosest.ClosestThingReachable(pawn.Position, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, true), 30f, validator, null, -1, false);
			if (pawn2 != null)
			{
				return new Job(JobDefOf.AttackMelee, pawn2)
				{
					maxNumMeleeAttacks = 1,
					expiryInterval = Rand.Range(420, 900),
					canBash = true
				};
			}
			return null;
		}
	}
}
