using RimWorld;
using System;
using System.Collections.Generic;
namespace Verse.AI
{
	public static class PawnPathUtility
	{
		public static Thing FirstBlockingBuilding(this PawnPath path, out IntVec3 cellBefore)
		{
			List<IntVec3> nodesReversed = path.NodesReversed;
			if (nodesReversed.Count == 1)
			{
				cellBefore = nodesReversed[0];
				return null;
			}
			for (int i = nodesReversed.Count - 2; i >= 1; i--)
			{
				Building edifice = nodesReversed[i].GetEdifice();
				if (edifice != null && (edifice.def.passability == Traversability.Impassable || edifice is Building_Door))
				{
					cellBefore = nodesReversed[i + 1];
					return edifice;
				}
			}
			cellBefore = nodesReversed[0];
			return null;
		}
		public static IntVec3 FinalWalkableNonDoorCell(this PawnPath path)
		{
			if (path.NodesReversed.Count == 1)
			{
				return path.NodesReversed[0];
			}
			List<IntVec3> nodesReversed = path.NodesReversed;
			for (int i = 0; i < nodesReversed.Count; i++)
			{
				Building edifice = nodesReversed[i].GetEdifice();
				if (edifice == null || edifice.def.passability != Traversability.Impassable)
				{
					Building_Door building_Door = edifice as Building_Door;
					if (building_Door == null || building_Door.FreePassage)
					{
						return nodesReversed[i];
					}
				}
			}
			return nodesReversed[0];
		}
		public static IntVec3 LastCellBeforeBlockerOrFinalCell(this PawnPath path)
		{
			if (path.NodesReversed.Count == 1)
			{
				return path.NodesReversed[0];
			}
			List<IntVec3> nodesReversed = path.NodesReversed;
			for (int i = nodesReversed.Count - 2; i >= 1; i--)
			{
				Building edifice = nodesReversed[i].GetEdifice();
				if (edifice != null)
				{
					if (edifice.def.passability == Traversability.Impassable)
					{
						return nodesReversed[i + 1];
					}
					Building_Door building_Door = edifice as Building_Door;
					if (building_Door != null && !building_Door.FreePassage)
					{
						return nodesReversed[i + 1];
					}
				}
			}
			return nodesReversed[0];
		}
		public static bool TryFindLastCellBeforeBlockingDoor(this PawnPath path, out IntVec3 result)
		{
			if (path.NodesReversed.Count == 1)
			{
				result = path.NodesReversed[0];
				return false;
			}
			List<IntVec3> nodesReversed = path.NodesReversed;
			for (int i = nodesReversed.Count - 2; i >= 1; i--)
			{
				Building_Door building_Door = nodesReversed[i].GetEdifice() as Building_Door;
				if (building_Door != null && !building_Door.FreePassage)
				{
					result = nodesReversed[i + 1];
					return true;
				}
			}
			result = nodesReversed[0];
			return false;
		}
		public static bool TryFindCellAtIndex(PawnPath path, int index, out IntVec3 result)
		{
			if (path.NodesReversed.Count < index)
			{
				result = IntVec3.Invalid;
				return false;
			}
			result = path.NodesReversed[path.NodesReversed.Count - 1 - index];
			return true;
		}
	}
}
