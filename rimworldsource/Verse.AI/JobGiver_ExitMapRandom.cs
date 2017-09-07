using System;
namespace Verse.AI
{
	public class JobGiver_ExitMapRandom : JobGiver_ExitMap
	{
		protected override bool TryFindGoodExitDest(Pawn pawn, out IntVec3 spot)
		{
			return ExitUtility.TryFindRandomExitSpot(pawn, out spot, TraverseMode.ByPawn);
		}
	}
}
