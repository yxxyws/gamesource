using System;
namespace Verse.AI
{
	public class JobGiver_ExitMapNearest : JobGiver_ExitMap
	{
		protected override bool TryFindGoodExitDest(Pawn pawn, out IntVec3 spot)
		{
			return ExitUtility.TryFindClosestExitSpot(pawn, out spot, TraverseMode.ByPawn);
		}
	}
}
