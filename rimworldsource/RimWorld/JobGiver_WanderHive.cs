using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobGiver_WanderHive : JobGiver_Wander
	{
		public JobGiver_WanderHive()
		{
			this.wanderRadius = 7.5f;
			this.ticksBetweenWandersRange = new IntRange(125, 200);
		}
		protected override IntVec3 GetWanderRoot(Pawn pawn)
		{
			if (pawn.mindState.hive == null)
			{
				return IntVec3.Invalid;
			}
			return pawn.mindState.hive.Position;
		}
	}
}
