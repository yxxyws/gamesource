using System;
namespace Verse.AI
{
	public static class ExitUtility
	{
		public static bool TryFindRandomExitSpot(Pawn pawn, out IntVec3 spot, TraverseMode mode = TraverseMode.ByPawn)
		{
			Danger maxDanger = Danger.Some;
			int num = 0;
			IntVec3 intVec;
			while (true)
			{
				num++;
				if (num > 40)
				{
					break;
				}
				if (num > 15)
				{
					maxDanger = Danger.Deadly;
				}
				intVec = CellFinder.RandomCell();
				int num2 = Rand.RangeInclusive(0, 3);
				if (num2 == 0)
				{
					intVec.x = 0;
				}
				if (num2 == 1)
				{
					intVec.x = Find.Map.Size.x - 1;
				}
				if (num2 == 2)
				{
					intVec.z = 0;
				}
				if (num2 == 3)
				{
					intVec.z = Find.Map.Size.z - 1;
				}
				if (intVec.Standable())
				{
					if (pawn.CanReach(intVec, PathEndMode.OnCell, maxDanger, false, mode))
					{
						goto IL_C7;
					}
				}
			}
			spot = pawn.Position;
			return false;
			IL_C7:
			spot = intVec;
			return true;
		}
		public static bool TryFindClosestExitSpot(Pawn pawn, out IntVec3 spot, TraverseMode mode = TraverseMode.ByPawn)
		{
			int num = 0;
			int num2 = 0;
			IntVec3 intVec2;
			while (true)
			{
				num2++;
				if (num2 > 30)
				{
					break;
				}
				IntVec3 intVec;
				bool flag = CellFinder.TryFindRandomCellNear(pawn.Position, num, null, out intVec);
				num += 4;
				if (flag)
				{
					int num3 = intVec.x;
					intVec2 = new IntVec3(0, 0, intVec.z);
					if (Find.Map.Size.z - intVec.z < num3)
					{
						num3 = Find.Map.Size.z - intVec.z;
						intVec2 = new IntVec3(intVec.x, 0, Find.Map.Size.z - 1);
					}
					if (Find.Map.Size.x - intVec.x < num3)
					{
						num3 = Find.Map.Size.x - intVec.x;
						intVec2 = new IntVec3(Find.Map.Size.x - 1, 0, intVec.z);
					}
					if (intVec.z < num3)
					{
						intVec2 = new IntVec3(intVec.x, 0, 0);
					}
					if (intVec2.Standable() && pawn.CanReach(intVec2, PathEndMode.OnCell, Danger.Deadly, true, mode))
					{
						goto Block_7;
					}
				}
			}
			spot = pawn.Position;
			return false;
			Block_7:
			spot = intVec2;
			return true;
		}
	}
}
