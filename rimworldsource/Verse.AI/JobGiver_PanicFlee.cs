using System;
namespace Verse.AI
{
	public class JobGiver_PanicFlee : JobGiver_ExitMap
	{
		public JobGiver_PanicFlee()
		{
			this.canBash = true;
		}
		protected override bool TryFindGoodExitDest(Pawn pawn, out IntVec3 spot)
		{
			return ExitUtility.TryFindClosestExitSpot(pawn, out spot, TraverseMode.ByPawn);
		}
	}
}
