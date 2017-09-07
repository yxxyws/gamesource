using System;
using System.Collections.Generic;
namespace Verse
{
	public static class ZoneMaker
	{
		public static Zone MakeZoneWithCells(Zone z, IEnumerable<IntVec3> cells)
		{
			if (cells != null)
			{
				foreach (IntVec3 current in cells)
				{
					z.AddCell(current);
				}
			}
			return z;
		}
	}
}
