using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public static class ShipUtility
	{
		private static List<Building> closedSet = new List<Building>();
		private static List<Building> openSet = new List<Building>();
		[DebuggerHidden]
		public static IEnumerable<string> LaunchFailReasons(Building rootBuilding)
		{
			ShipUtility.<LaunchFailReasons>c__IteratorF5 <LaunchFailReasons>c__IteratorF = new ShipUtility.<LaunchFailReasons>c__IteratorF5();
			<LaunchFailReasons>c__IteratorF.rootBuilding = rootBuilding;
			<LaunchFailReasons>c__IteratorF.<$>rootBuilding = rootBuilding;
			ShipUtility.<LaunchFailReasons>c__IteratorF5 expr_15 = <LaunchFailReasons>c__IteratorF;
			expr_15.$PC = -2;
			return expr_15;
		}
		public static List<Building> ShipBuildingsAttachedTo(Building root)
		{
			if (root == null || root.Destroyed)
			{
				return new List<Building>();
			}
			ShipUtility.closedSet.Clear();
			ShipUtility.openSet.Clear();
			ShipUtility.openSet.Add(root);
			while (ShipUtility.openSet.Count > 0)
			{
				Building building = ShipUtility.openSet[ShipUtility.openSet.Count - 1];
				ShipUtility.openSet.Remove(building);
				ShipUtility.closedSet.Add(building);
				foreach (IntVec3 current in GenAdj.CellsAdjacentCardinal(building))
				{
					Building edifice = current.GetEdifice();
					if (edifice != null && edifice.def.building.shipPart && !ShipUtility.closedSet.Contains(edifice) && !ShipUtility.openSet.Contains(edifice))
					{
						ShipUtility.openSet.Add(edifice);
					}
				}
			}
			return ShipUtility.closedSet;
		}
	}
}
