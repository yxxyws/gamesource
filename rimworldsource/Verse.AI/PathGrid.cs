using RimWorld;
using System;
using System.Collections.Generic;
namespace Verse.AI
{
	public sealed class PathGrid
	{
		private const int ImpassableCost = 10000;
		private const int PathCostPerPawnBodySize = 15;
		public int[] pathGrid;
		public PathGrid()
		{
			this.ResetPathGrid();
		}
		public void ResetPathGrid()
		{
			this.pathGrid = new int[CellIndices.NumGridCells];
		}
		public bool Walkable(IntVec3 loc)
		{
			return loc.InBounds() && this.pathGrid[CellIndices.CellToIndex(loc)] < 10000;
		}
		public bool WalkableFast(IntVec3 loc)
		{
			return this.pathGrid[CellIndices.CellToIndex(loc)] < 10000;
		}
		public bool WalkableFast(int x, int z)
		{
			return this.pathGrid[CellIndices.CellToIndex(x, z)] < 10000;
		}
		public int PerceivedPathCostAt(IntVec3 loc)
		{
			return this.pathGrid[CellIndices.CellToIndex(loc)];
		}
		public void RecalculatePerceivedPathCostUnderThing(Thing t)
		{
			if (t.def.size == IntVec2.One)
			{
				this.RecalculatePerceivedPathCostAt(t.Position);
			}
			else
			{
				CellRect cellRect = t.OccupiedRect();
				for (int i = cellRect.minZ; i <= cellRect.maxZ; i++)
				{
					for (int j = cellRect.minX; j <= cellRect.maxX; j++)
					{
						IntVec3 c = new IntVec3(j, 0, i);
						this.RecalculatePerceivedPathCostAt(c);
					}
				}
			}
		}
		public void RecalculatePerceivedPathCostAt(IntVec3 c)
		{
			if (!c.InBounds())
			{
				return;
			}
			bool flag = this.WalkableFast(c);
			this.pathGrid[CellIndices.CellToIndex(c)] = PathGrid.CalculatedCostAt(c, true);
			if (this.WalkableFast(c) != flag)
			{
				RegionDirtyer.Notify_WalkabilityChanged(c);
			}
		}
		public void RecalculateAllPerceivedPathCosts()
		{
			foreach (IntVec3 current in Find.Map.AllCells)
			{
				this.RecalculatePerceivedPathCostAt(current);
			}
		}
		public static int CalculatedCostAt(IntVec3 c, bool perceivedStatic)
		{
			int num = 0;
			TerrainDef terrainDef = Find.TerrainGrid.TerrainAt(c);
			if (terrainDef == null || terrainDef.passability == Traversability.Impassable)
			{
				num = 10000;
			}
			else
			{
				num += terrainDef.pathCost;
			}
			int num2 = SnowUtility.MovementTicksAddOn(Find.SnowGrid.GetCategory(c));
			num += num2;
			List<Thing> list = Find.ThingGrid.ThingsListAt(c);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].def.passability == Traversability.Impassable)
				{
					return 10000;
				}
				num += list[i].def.pathCost;
			}
			if (perceivedStatic)
			{
				for (int j = 0; j < 9; j++)
				{
					IntVec3 b = GenAdj.AdjacentCellsAndInside[j];
					IntVec3 c2 = c + b;
					if (c2.InBounds())
					{
						Fire fire = null;
						list = Find.ThingGrid.ThingsListAtFast(c2);
						for (int k = 0; k < list.Count; k++)
						{
							fire = (list[k] as Fire);
							if (fire != null)
							{
								break;
							}
						}
						if (fire != null && fire.parent == null)
						{
							if (b.x == 0 && b.z == 0)
							{
								num += 1000;
							}
							else
							{
								num += 150;
							}
						}
					}
				}
			}
			return num;
		}
	}
}
