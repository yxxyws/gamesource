using System;
namespace Verse.AI
{
	public class JobGiver_WanderCurrentRoom : JobGiver_Wander
	{
		public JobGiver_WanderCurrentRoom()
		{
			this.wanderRadius = 7f;
			this.ticksBetweenWandersRange = new IntRange(125, 200);
			this.locomotionUrgency = LocomotionUrgency.Amble;
			this.wanderDestValidator = delegate(Pawn pawn, IntVec3 loc)
			{
				Room room = pawn.Position.GetRoom();
				return room == null || room.CellCount == 1 || WanderUtility.InSameRoom(pawn.Position, loc);
			};
		}
		protected override IntVec3 GetWanderRoot(Pawn pawn)
		{
			return pawn.Position;
		}
	}
}
