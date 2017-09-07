using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
namespace RimWorld
{
	public static class GridShapeMaker
	{
		public static IEnumerable<IntVec3> IrregularLump(IntVec3 center, int numCells)
		{
			List<IntVec3> lumpCells = new List<IntVec3>();
			for (int i = 0; i < numCells * 2; i++)
			{
				IntVec3 intVec = center + GenRadial.RadialPattern[i];
				if (intVec.InBounds())
				{
					lumpCells.Add(intVec);
				}
			}
			Func<IntVec3, int> NumNeighbors = delegate(IntVec3 sq)
			{
				int num2 = 0;
				for (int j = 0; j < 4; j++)
				{
					IntVec3 item = sq + GenAdj.CardinalDirections[j];
					if (lumpCells.Contains(item))
					{
						num2++;
					}
				}
				return num2;
			};
			while (lumpCells.Count > numCells)
			{
				int fewestNeighbors = 99;
				foreach (IntVec3 current in lumpCells)
				{
					int num = NumNeighbors(current);
					if (num < fewestNeighbors)
					{
						fewestNeighbors = num;
					}
				}
				List<IntVec3> source = (
					from sq in lumpCells
					where NumNeighbors(sq) == fewestNeighbors
					select sq).ToList<IntVec3>();
				lumpCells.Remove(source.RandomElement<IntVec3>());
			}
			return lumpCells;
		}
	}
}
