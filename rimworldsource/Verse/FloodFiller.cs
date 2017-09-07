using System;
using System.Collections.Generic;
namespace Verse
{
	public static class FloodFiller
	{
		private static Queue<IntVec3> openSet = new Queue<IntVec3>();
		private static BoolGrid queuedGrid = new BoolGrid();
		public static void FloodFill(IntVec3 root, Predicate<IntVec3> passCheck, Action<IntVec3> processor)
		{
			ProfilerThreadCheck.BeginSample("FloodFill");
			if (!passCheck(root))
			{
				return;
			}
			int area = Find.Map.Area;
			if (FloodFiller.queuedGrid.InnerArray.Length != Find.Map.Area)
			{
				FloodFiller.queuedGrid = new BoolGrid();
			}
			FloodFiller.queuedGrid.Clear();
			IntVec3[] cardinalDirectionsAround = GenAdj.CardinalDirectionsAround;
			int num = cardinalDirectionsAround.Length;
			FloodFiller.queuedGrid.Set(root, true);
			FloodFiller.openSet.Clear();
			FloodFiller.openSet.Enqueue(root);
			while (FloodFiller.openSet.Count > 0)
			{
				IntVec3 intVec = FloodFiller.openSet.Dequeue();
				processor(intVec);
				for (int i = 0; i < num; i++)
				{
					IntVec3 intVec2 = intVec + cardinalDirectionsAround[i];
					if (intVec2.InBounds() && !FloodFiller.queuedGrid[intVec2] && passCheck(intVec2))
					{
						FloodFiller.openSet.Enqueue(intVec2);
						FloodFiller.queuedGrid.Set(intVec2, true);
					}
				}
				if (FloodFiller.openSet.Count > area)
				{
					Log.Error("Overflow on flood fill (>" + area + " cells). Make sure we're not flooding over the same area after we check it.");
					ProfilerThreadCheck.EndSample();
					return;
				}
			}
			ProfilerThreadCheck.EndSample();
		}
	}
}
