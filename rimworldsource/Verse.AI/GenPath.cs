using RimWorld;
using System;
namespace Verse.AI
{
	public static class GenPath
	{
		public static bool IsGoodWorkSpotFor(this IntVec3 c, Pawn pawn)
		{
			return c.IsUsableWorkSpotFor(pawn) && c.Standable();
		}
		public static bool IsUsableWorkSpotFor(this IntVec3 c, Pawn pawn)
		{
			return c.Walkable() && !Find.PawnDestinationManager.DestinationIsReserved(c, pawn) && !c.ContainsStaticFire();
		}
		public static void ResolvePathMode(ref TargetInfo dest, ref PathEndMode peMode)
		{
			if (peMode == 4)
			{
				dest = dest.Thing.InteractionCell;
				peMode = PathEndMode.OnCell;
				return;
			}
			if (peMode == 3)
			{
				if (Find.PathGrid.PerceivedPathCostAt(dest.Cell) > 30 || !dest.Cell.Walkable())
				{
					peMode = PathEndMode.Touch;
				}
				else
				{
					peMode = PathEndMode.OnCell;
				}
			}
		}
	}
}
