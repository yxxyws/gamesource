using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse
{
	public static class GenAdj
	{
		public static IntVec3[] CardinalDirections;
		public static IntVec3[] CardinalDirectionsAndInside;
		public static IntVec3[] CardinalDirectionsAround;
		public static IntVec3[] DiagonalDirections;
		public static IntVec3[] DiagonalDirectionsAround;
		public static IntVec3[] AdjacentCells;
		public static IntVec3[] AdjacentCellsAndInside;
		public static IntVec3[] AdjacentCellsAround;
		public static IntVec3[] AdjacentCellsAroundBottom;
		private static List<IntVec3> adjRandomOrderList;
		private static List<IntVec3> validCells;
		static GenAdj()
		{
			GenAdj.CardinalDirections = new IntVec3[4];
			GenAdj.CardinalDirectionsAndInside = new IntVec3[5];
			GenAdj.CardinalDirectionsAround = new IntVec3[4];
			GenAdj.DiagonalDirections = new IntVec3[4];
			GenAdj.DiagonalDirectionsAround = new IntVec3[4];
			GenAdj.AdjacentCells = new IntVec3[8];
			GenAdj.AdjacentCellsAndInside = new IntVec3[9];
			GenAdj.AdjacentCellsAround = new IntVec3[8];
			GenAdj.AdjacentCellsAroundBottom = new IntVec3[9];
			GenAdj.validCells = new List<IntVec3>();
			GenAdj.SetupAdjacencyTables();
		}
		private static void SetupAdjacencyTables()
		{
			GenAdj.CardinalDirections[0] = new IntVec3(0, 0, 1);
			GenAdj.CardinalDirections[1] = new IntVec3(1, 0, 0);
			GenAdj.CardinalDirections[2] = new IntVec3(0, 0, -1);
			GenAdj.CardinalDirections[3] = new IntVec3(-1, 0, 0);
			GenAdj.CardinalDirectionsAndInside[0] = new IntVec3(0, 0, 1);
			GenAdj.CardinalDirectionsAndInside[1] = new IntVec3(1, 0, 0);
			GenAdj.CardinalDirectionsAndInside[2] = new IntVec3(0, 0, -1);
			GenAdj.CardinalDirectionsAndInside[3] = new IntVec3(-1, 0, 0);
			GenAdj.CardinalDirectionsAndInside[4] = new IntVec3(0, 0, 0);
			GenAdj.CardinalDirectionsAround[0] = new IntVec3(0, 0, -1);
			GenAdj.CardinalDirectionsAround[1] = new IntVec3(-1, 0, 0);
			GenAdj.CardinalDirectionsAround[2] = new IntVec3(0, 0, 1);
			GenAdj.CardinalDirectionsAround[3] = new IntVec3(1, 0, 0);
			GenAdj.DiagonalDirections[0] = new IntVec3(-1, 0, -1);
			GenAdj.DiagonalDirections[1] = new IntVec3(-1, 0, 1);
			GenAdj.DiagonalDirections[2] = new IntVec3(1, 0, 1);
			GenAdj.DiagonalDirections[3] = new IntVec3(1, 0, -1);
			GenAdj.DiagonalDirectionsAround[0] = new IntVec3(-1, 0, -1);
			GenAdj.DiagonalDirectionsAround[1] = new IntVec3(-1, 0, 1);
			GenAdj.DiagonalDirectionsAround[2] = new IntVec3(1, 0, 1);
			GenAdj.DiagonalDirectionsAround[3] = new IntVec3(1, 0, -1);
			GenAdj.AdjacentCells[0] = new IntVec3(0, 0, 1);
			GenAdj.AdjacentCells[1] = new IntVec3(1, 0, 0);
			GenAdj.AdjacentCells[2] = new IntVec3(0, 0, -1);
			GenAdj.AdjacentCells[3] = new IntVec3(-1, 0, 0);
			GenAdj.AdjacentCells[4] = new IntVec3(1, 0, -1);
			GenAdj.AdjacentCells[5] = new IntVec3(1, 0, 1);
			GenAdj.AdjacentCells[6] = new IntVec3(-1, 0, 1);
			GenAdj.AdjacentCells[7] = new IntVec3(-1, 0, -1);
			GenAdj.AdjacentCellsAndInside[0] = new IntVec3(0, 0, 1);
			GenAdj.AdjacentCellsAndInside[1] = new IntVec3(1, 0, 0);
			GenAdj.AdjacentCellsAndInside[2] = new IntVec3(0, 0, -1);
			GenAdj.AdjacentCellsAndInside[3] = new IntVec3(-1, 0, 0);
			GenAdj.AdjacentCellsAndInside[4] = new IntVec3(1, 0, -1);
			GenAdj.AdjacentCellsAndInside[5] = new IntVec3(1, 0, 1);
			GenAdj.AdjacentCellsAndInside[6] = new IntVec3(-1, 0, 1);
			GenAdj.AdjacentCellsAndInside[7] = new IntVec3(-1, 0, -1);
			GenAdj.AdjacentCellsAndInside[8] = new IntVec3(0, 0, 0);
			GenAdj.AdjacentCellsAround[0] = new IntVec3(0, 0, 1);
			GenAdj.AdjacentCellsAround[1] = new IntVec3(1, 0, 1);
			GenAdj.AdjacentCellsAround[2] = new IntVec3(1, 0, 0);
			GenAdj.AdjacentCellsAround[3] = new IntVec3(1, 0, -1);
			GenAdj.AdjacentCellsAround[4] = new IntVec3(0, 0, -1);
			GenAdj.AdjacentCellsAround[5] = new IntVec3(-1, 0, -1);
			GenAdj.AdjacentCellsAround[6] = new IntVec3(-1, 0, 0);
			GenAdj.AdjacentCellsAround[7] = new IntVec3(-1, 0, 1);
			GenAdj.AdjacentCellsAroundBottom[0] = new IntVec3(0, 0, -1);
			GenAdj.AdjacentCellsAroundBottom[1] = new IntVec3(-1, 0, -1);
			GenAdj.AdjacentCellsAroundBottom[2] = new IntVec3(-1, 0, 0);
			GenAdj.AdjacentCellsAroundBottom[3] = new IntVec3(-1, 0, 1);
			GenAdj.AdjacentCellsAroundBottom[4] = new IntVec3(0, 0, 1);
			GenAdj.AdjacentCellsAroundBottom[5] = new IntVec3(1, 0, 1);
			GenAdj.AdjacentCellsAroundBottom[6] = new IntVec3(1, 0, 0);
			GenAdj.AdjacentCellsAroundBottom[7] = new IntVec3(1, 0, -1);
			GenAdj.AdjacentCellsAroundBottom[8] = new IntVec3(0, 0, 0);
		}
		public static List<IntVec3> AdjacentCells8WayRandomized()
		{
			if (GenAdj.adjRandomOrderList == null)
			{
				GenAdj.adjRandomOrderList = new List<IntVec3>();
				for (int i = 0; i < 8; i++)
				{
					GenAdj.adjRandomOrderList.Add(GenAdj.AdjacentCells[i]);
				}
			}
			GenAdj.adjRandomOrderList.Shuffle<IntVec3>();
			return GenAdj.adjRandomOrderList;
		}
		[DebuggerHidden]
		public static IEnumerable<IntVec3> CellsOccupiedBy(Thing t)
		{
			GenAdj.<CellsOccupiedBy>c__Iterator19B <CellsOccupiedBy>c__Iterator19B = new GenAdj.<CellsOccupiedBy>c__Iterator19B();
			<CellsOccupiedBy>c__Iterator19B.t = t;
			<CellsOccupiedBy>c__Iterator19B.<$>t = t;
			GenAdj.<CellsOccupiedBy>c__Iterator19B expr_15 = <CellsOccupiedBy>c__Iterator19B;
			expr_15.$PC = -2;
			return expr_15;
		}
		[DebuggerHidden]
		public static IEnumerable<IntVec3> CellsOccupiedBy(IntVec3 center, Rot4 rotation, IntVec2 size)
		{
			GenAdj.<CellsOccupiedBy>c__Iterator19C <CellsOccupiedBy>c__Iterator19C = new GenAdj.<CellsOccupiedBy>c__Iterator19C();
			<CellsOccupiedBy>c__Iterator19C.center = center;
			<CellsOccupiedBy>c__Iterator19C.size = size;
			<CellsOccupiedBy>c__Iterator19C.rotation = rotation;
			<CellsOccupiedBy>c__Iterator19C.<$>center = center;
			<CellsOccupiedBy>c__Iterator19C.<$>size = size;
			<CellsOccupiedBy>c__Iterator19C.<$>rotation = rotation;
			GenAdj.<CellsOccupiedBy>c__Iterator19C expr_31 = <CellsOccupiedBy>c__Iterator19C;
			expr_31.$PC = -2;
			return expr_31;
		}
		[DebuggerHidden]
		public static IEnumerable<IntVec3> CellsAdjacent8Way(TargetInfo pack)
		{
			GenAdj.<CellsAdjacent8Way>c__Iterator19D <CellsAdjacent8Way>c__Iterator19D = new GenAdj.<CellsAdjacent8Way>c__Iterator19D();
			<CellsAdjacent8Way>c__Iterator19D.pack = pack;
			<CellsAdjacent8Way>c__Iterator19D.<$>pack = pack;
			GenAdj.<CellsAdjacent8Way>c__Iterator19D expr_15 = <CellsAdjacent8Way>c__Iterator19D;
			expr_15.$PC = -2;
			return expr_15;
		}
		public static IEnumerable<IntVec3> CellsAdjacent8Way(Thing t)
		{
			return GenAdj.CellsAdjacent8Way(t.Position, t.Rotation, t.def.size);
		}
		[DebuggerHidden]
		public static IEnumerable<IntVec3> CellsAdjacent8Way(IntVec3 thingCenter, Rot4 thingRot, IntVec2 thingSize)
		{
			GenAdj.<CellsAdjacent8Way>c__Iterator19E <CellsAdjacent8Way>c__Iterator19E = new GenAdj.<CellsAdjacent8Way>c__Iterator19E();
			<CellsAdjacent8Way>c__Iterator19E.thingCenter = thingCenter;
			<CellsAdjacent8Way>c__Iterator19E.thingSize = thingSize;
			<CellsAdjacent8Way>c__Iterator19E.thingRot = thingRot;
			<CellsAdjacent8Way>c__Iterator19E.<$>thingCenter = thingCenter;
			<CellsAdjacent8Way>c__Iterator19E.<$>thingSize = thingSize;
			<CellsAdjacent8Way>c__Iterator19E.<$>thingRot = thingRot;
			GenAdj.<CellsAdjacent8Way>c__Iterator19E expr_31 = <CellsAdjacent8Way>c__Iterator19E;
			expr_31.$PC = -2;
			return expr_31;
		}
		public static IEnumerable<IntVec3> CellsAdjacentCardinal(Thing t)
		{
			return GenAdj.CellsAdjacentCardinal(t.Position, t.Rotation, t.def.size);
		}
		[DebuggerHidden]
		public static IEnumerable<IntVec3> CellsAdjacentCardinal(IntVec3 center, Rot4 rot, IntVec2 size)
		{
			GenAdj.<CellsAdjacentCardinal>c__Iterator19F <CellsAdjacentCardinal>c__Iterator19F = new GenAdj.<CellsAdjacentCardinal>c__Iterator19F();
			<CellsAdjacentCardinal>c__Iterator19F.center = center;
			<CellsAdjacentCardinal>c__Iterator19F.size = size;
			<CellsAdjacentCardinal>c__Iterator19F.rot = rot;
			<CellsAdjacentCardinal>c__Iterator19F.<$>center = center;
			<CellsAdjacentCardinal>c__Iterator19F.<$>size = size;
			<CellsAdjacentCardinal>c__Iterator19F.<$>rot = rot;
			GenAdj.<CellsAdjacentCardinal>c__Iterator19F expr_31 = <CellsAdjacentCardinal>c__Iterator19F;
			expr_31.$PC = -2;
			return expr_31;
		}
		[DebuggerHidden]
		public static IEnumerable<IntVec3> CellsAdjacentAlongEdge(IntVec3 thingCent, Rot4 thingRot, IntVec2 thingSize, LinkDirections dir)
		{
			GenAdj.<CellsAdjacentAlongEdge>c__Iterator1A0 <CellsAdjacentAlongEdge>c__Iterator1A = new GenAdj.<CellsAdjacentAlongEdge>c__Iterator1A0();
			<CellsAdjacentAlongEdge>c__Iterator1A.thingCent = thingCent;
			<CellsAdjacentAlongEdge>c__Iterator1A.thingSize = thingSize;
			<CellsAdjacentAlongEdge>c__Iterator1A.thingRot = thingRot;
			<CellsAdjacentAlongEdge>c__Iterator1A.dir = dir;
			<CellsAdjacentAlongEdge>c__Iterator1A.<$>thingCent = thingCent;
			<CellsAdjacentAlongEdge>c__Iterator1A.<$>thingSize = thingSize;
			<CellsAdjacentAlongEdge>c__Iterator1A.<$>thingRot = thingRot;
			<CellsAdjacentAlongEdge>c__Iterator1A.<$>dir = dir;
			GenAdj.<CellsAdjacentAlongEdge>c__Iterator1A0 expr_3F = <CellsAdjacentAlongEdge>c__Iterator1A;
			expr_3F.$PC = -2;
			return expr_3F;
		}
		[DebuggerHidden]
		public static IEnumerable<IntVec3> CellsAdjacent8WayAndInside(this Thing thing)
		{
			GenAdj.<CellsAdjacent8WayAndInside>c__Iterator1A1 <CellsAdjacent8WayAndInside>c__Iterator1A = new GenAdj.<CellsAdjacent8WayAndInside>c__Iterator1A1();
			<CellsAdjacent8WayAndInside>c__Iterator1A.thing = thing;
			<CellsAdjacent8WayAndInside>c__Iterator1A.<$>thing = thing;
			GenAdj.<CellsAdjacent8WayAndInside>c__Iterator1A1 expr_15 = <CellsAdjacent8WayAndInside>c__Iterator1A;
			expr_15.$PC = -2;
			return expr_15;
		}
		public static IntVec3 RandomAdjacentCell8Way(this IntVec3 root)
		{
			return root + GenAdj.AdjacentCells[Rand.RangeInclusive(0, 7)];
		}
		public static IntVec3 RandomAdjacentCellCardinal(this IntVec3 root)
		{
			return root + GenAdj.CardinalDirections[Rand.RangeInclusive(0, 3)];
		}
		public static IntVec3 RandomAdjacentCell8Way(this Thing t)
		{
			CellRect cellRect = t.OccupiedRect();
			CellRect cellRect2 = cellRect.ExpandedBy(1);
			IntVec3 randomCell;
			do
			{
				randomCell = cellRect2.RandomCell;
			}
			while (cellRect.Contains(randomCell));
			return randomCell;
		}
		public static IntVec3 RandomAdjacentCellCardinal(this Thing t)
		{
			CellRect cellRect = t.OccupiedRect();
			IntVec3 randomCell = cellRect.RandomCell;
			if (Rand.Value < 0.5f)
			{
				if (Rand.Value < 0.5f)
				{
					randomCell.x = cellRect.minX - 1;
				}
				else
				{
					randomCell.x = cellRect.maxX + 1;
				}
			}
			else
			{
				if (Rand.Value < 0.5f)
				{
					randomCell.z = cellRect.minZ - 1;
				}
				else
				{
					randomCell.z = cellRect.maxZ + 1;
				}
			}
			return randomCell;
		}
		public static bool TryFindRandomWalkableAdjacentCell8Way(Thing t, out IntVec3 result)
		{
			return GenAdj.TryFindRandomWalkableAdjacentCell8Way(t.Position, t.Rotation, t.def.size, out result);
		}
		public static bool TryFindRandomWalkableAdjacentCell8Way(IntVec3 center, Rot4 rot, IntVec2 size, out IntVec3 result)
		{
			GenAdj.AdjustForRotation(ref center, ref size, rot);
			GenAdj.validCells.Clear();
			foreach (IntVec3 current in GenAdj.CellsAdjacent8Way(center, rot, size))
			{
				if (current.InBounds() && current.Walkable())
				{
					GenAdj.validCells.Add(current);
				}
			}
			return GenAdj.validCells.TryRandomElement(out result);
		}
		public static bool AdjacentTo8WayOrInside(this IntVec3 me, TargetInfo other)
		{
			if (other.HasThing)
			{
				return me.AdjacentTo8WayOrInside(other.Thing);
			}
			return me.AdjacentTo8WayOrInside(other.Cell);
		}
		public static bool AdjacentTo8Way(this IntVec3 me, IntVec3 other)
		{
			int num = me.x - other.x;
			int num2 = me.z - other.z;
			if (num == 0 && num2 == 0)
			{
				return false;
			}
			if (num < 0)
			{
				num *= -1;
			}
			if (num2 < 0)
			{
				num2 *= -1;
			}
			return num <= 1 && num2 <= 1;
		}
		public static bool AdjacentTo8WayOrInside(this IntVec3 me, IntVec3 other)
		{
			int num = me.x - other.x;
			int num2 = me.z - other.z;
			if (num < 0)
			{
				num *= -1;
			}
			if (num2 < 0)
			{
				num2 *= -1;
			}
			return num <= 1 && num2 <= 1;
		}
		public static bool AdjacentTo8WayOrInside(this IntVec3 root, Thing t)
		{
			return root.IsAdjacentTo8WayOrInside(t.Position, t.Rotation, t.def.size);
		}
		public static bool IsAdjacentTo8WayOrInside(this IntVec3 root, IntVec3 center, Rot4 rot, IntVec2 size)
		{
			GenAdj.AdjustForRotation(ref center, ref size, rot);
			int num = center.x - (size.x - 1) / 2 - 1;
			int num2 = center.z - (size.z - 1) / 2 - 1;
			int num3 = num + size.x + 1;
			int num4 = num2 + size.z + 1;
			return root.x >= num && root.x <= num3 && root.z >= num2 && root.z <= num4;
		}
		public static bool IsInside(this IntVec3 root, Thing t)
		{
			return GenAdj.IsInside(root, t.Position, t.Rotation, t.def.size);
		}
		public static bool IsInside(IntVec3 root, IntVec3 center, Rot4 rot, IntVec2 size)
		{
			GenAdj.AdjustForRotation(ref center, ref size, rot);
			int num = center.x - (size.x - 1) / 2;
			int num2 = center.z - (size.z - 1) / 2;
			int num3 = num + size.x;
			int num4 = num2 + size.z;
			return root.x >= num && root.x <= num3 && root.z >= num2 && root.z <= num4;
		}
		public static CellRect OccupiedRect(this Thing t)
		{
			return GenAdj.OccupiedRect(t.Position, t.Rotation, t.def.size);
		}
		public static CellRect OccupiedRect(IntVec3 center, Rot4 rot, IntVec2 size)
		{
			GenAdj.AdjustForRotation(ref center, ref size, rot);
			return new CellRect(center.x - (size.x - 1) / 2, center.z - (size.z - 1) / 2, size.x, size.z);
		}
		public static void AdjustForRotation(ref IntVec3 center, ref IntVec2 size, Rot4 rot)
		{
			if (size.x == 1 && size.z == 1)
			{
				return;
			}
			if (rot.IsHorizontal)
			{
				int x = size.x;
				size.x = size.z;
				size.z = x;
			}
			switch (rot.AsInt)
			{
			case 1:
				if (size.z % 2 == 0)
				{
					center.z--;
				}
				break;
			case 2:
				if (size.x % 2 == 0)
				{
					center.x--;
				}
				if (size.z % 2 == 0)
				{
					center.z--;
				}
				break;
			case 3:
				if (size.x % 2 == 0)
				{
					center.x--;
				}
				break;
			}
		}
	}
}
