using System;
using Verse;
namespace RimWorld
{
	public static class PowerNetGrid
	{
		private static PowerNet[] netGrid;
		public static void Reinit()
		{
			PowerNetGrid.netGrid = new PowerNet[CellIndices.NumGridCells];
		}
		public static PowerNet TransmittedPowerNetAt(IntVec3 c)
		{
			return PowerNetGrid.netGrid[CellIndices.CellToIndex(c)];
		}
		public static void Notify_PowerNetCreated(PowerNet newNet)
		{
			for (int i = 0; i < newNet.transmitters.Count; i++)
			{
				CellRect cellRect = newNet.transmitters[i].parent.OccupiedRect();
				for (int j = cellRect.minZ; j <= cellRect.maxZ; j++)
				{
					for (int k = cellRect.minX; k <= cellRect.maxX; k++)
					{
						int num = CellIndices.CellToIndex(k, j);
						if (PowerNetGrid.netGrid[num] != null)
						{
							Log.Warning(string.Concat(new object[]
							{
								"Two power nets on the same cell (",
								k,
								", ",
								j,
								"). First transmitters: ",
								newNet.transmitters[0].parent.LabelCap,
								" and ",
								(!PowerNetGrid.netGrid[num].transmitters.NullOrEmpty<CompPower>()) ? PowerNetGrid.netGrid[num].transmitters[0].parent.LabelCap : "[none]",
								"."
							}));
						}
						PowerNetGrid.netGrid[num] = newNet;
					}
				}
			}
		}
		public static void Notify_PowerNetDeleted(PowerNet deadNet)
		{
			for (int i = 0; i < deadNet.transmitters.Count; i++)
			{
				CellRect cellRect = deadNet.transmitters[i].parent.OccupiedRect();
				for (int j = cellRect.minZ; j <= cellRect.maxZ; j++)
				{
					for (int k = cellRect.minX; k <= cellRect.maxX; k++)
					{
						PowerNetGrid.netGrid[CellIndices.CellToIndex(k, j)] = null;
					}
				}
			}
		}
	}
}
