using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public static class GenConstruct
	{
		private static List<IntVec3> occupiedCellsTemp = new List<IntVec3>();
		public static Blueprint_Build PlaceBlueprintForBuild(BuildableDef sourceDef, IntVec3 center, Rot4 rotation, Faction faction, ThingDef stuff)
		{
			Blueprint_Build blueprint_Build = (Blueprint_Build)ThingMaker.MakeThing(sourceDef.blueprintDef, null);
			blueprint_Build.SetFactionDirect(faction);
			blueprint_Build.stuffToUse = stuff;
			GenSpawn.Spawn(blueprint_Build, center, rotation);
			return blueprint_Build;
		}
		public static Blueprint_Install PlaceBlueprintForInstall(MinifiedThing itemToInstall, IntVec3 center, Rot4 rotation, Faction faction)
		{
			Blueprint_Install blueprint_Install = (Blueprint_Install)ThingMaker.MakeThing(itemToInstall.InnerThing.def.installBlueprintDef, null);
			blueprint_Install.SetThingToInstallFromMinified(itemToInstall);
			blueprint_Install.SetFactionDirect(faction);
			GenSpawn.Spawn(blueprint_Install, center, rotation);
			return blueprint_Install;
		}
		public static Blueprint_Install PlaceBlueprintForReinstall(Building buildingToReinstall, IntVec3 center, Rot4 rotation, Faction faction)
		{
			Blueprint_Install blueprint_Install = (Blueprint_Install)ThingMaker.MakeThing(buildingToReinstall.def.installBlueprintDef, null);
			blueprint_Install.SetBuildingToReinstall(buildingToReinstall);
			blueprint_Install.SetFactionDirect(faction);
			GenSpawn.Spawn(blueprint_Install, center, rotation);
			return blueprint_Install;
		}
		public static bool CanBuildOnTerrain(BuildableDef entDef, IntVec3 c, Rot4 rot)
		{
			TerrainDef terrainDef = entDef as TerrainDef;
			if (terrainDef != null && !c.GetTerrain().changeable)
			{
				return false;
			}
			if (entDef.terrainAffordanceNeeded == TerrainAffordance.Any)
			{
				return true;
			}
			CellRect cellRect = GenAdj.OccupiedRect(c, rot, entDef.Size);
			cellRect.ClipInsideMap();
			CellRect.CellRectIterator iterator = cellRect.GetIterator();
			while (!iterator.Done())
			{
				TerrainDef terrainDef2 = Find.TerrainGrid.TerrainAt(iterator.Current);
				if (!terrainDef2.affordances.Contains(entDef.terrainAffordanceNeeded))
				{
					return false;
				}
				iterator.MoveNext();
			}
			return true;
		}
		public static bool CanConstruct(Thing t, Pawn p)
		{
			Blueprint blueprint = t as Blueprint;
			return (blueprint == null || blueprint.FirstBlockingThing(p, false) == null) && p.CanReserveAndReach(t, PathEndMode.Touch, Danger.Some, 1) && !t.IsBurning();
		}
		public static int AmountNeededByOf(IConstructible c, ThingDef resDef)
		{
			foreach (ThingCount current in c.MaterialsNeeded())
			{
				if (current.thingDef == resDef)
				{
					return current.count;
				}
			}
			return 0;
		}
		public static AcceptanceReport CanPlaceBlueprintAt(BuildableDef entDef, IntVec3 center, Rot4 rot, bool godMode = false)
		{
			GenConstruct.occupiedCellsTemp.Clear();
			foreach (IntVec3 current in GenAdj.CellsOccupiedBy(center, rot, entDef.Size))
			{
				GenConstruct.occupiedCellsTemp.Add(current);
			}
			for (int i = 0; i < GenConstruct.occupiedCellsTemp.Count; i++)
			{
				IntVec3 c = GenConstruct.occupiedCellsTemp[i];
				if (!c.InBounds())
				{
					return new AcceptanceReport("OutOfBounds".Translate());
				}
				if (c.InNoBuildEdgeArea())
				{
					return "TooCloseToMapEdge".Translate();
				}
			}
			if (center.Fogged())
			{
				return "CannotPlaceInUndiscovered".Translate();
			}
			if (entDef.PlaceWorkers != null)
			{
				for (int j = 0; j < entDef.PlaceWorkers.Count; j++)
				{
					AcceptanceReport result = entDef.PlaceWorkers[j].AllowsPlacing(entDef, center, rot);
					if (!result.Accepted)
					{
						return result;
					}
				}
			}
			ThingDef thingDef = entDef as ThingDef;
			if (thingDef != null && thingDef.hasInteractionCell)
			{
				IntVec3 c2 = Thing.InteractionCellWhenAt(thingDef, center, rot);
				if (!c2.InBounds())
				{
					return new AcceptanceReport("InteractionSpotOutOfBounds".Translate());
				}
				List<Thing> list = Find.ThingGrid.ThingsListAtFast(c2);
				for (int k = 0; k < list.Count; k++)
				{
					if (list[k].def.passability == Traversability.Impassable)
					{
						return new AcceptanceReport("InteractionSpotBlocked".Translate(new object[]
						{
							list[k].LabelBase
						}).CapitalizeFirst());
					}
					Blueprint blueprint = list[k] as Blueprint;
					if (blueprint != null && blueprint.def.entityDefToBuild.passability == Traversability.Impassable)
					{
						return new AcceptanceReport("InteractionSpotWillBeBlocked".Translate(new object[]
						{
							blueprint.LabelBase
						}).CapitalizeFirst());
					}
				}
			}
			if (entDef.passability != Traversability.Standable)
			{
				foreach (IntVec3 current2 in GenAdj.CellsAdjacentCardinal(center, rot, entDef.Size))
				{
					if (current2.InBounds())
					{
						foreach (Thing current3 in Find.ThingGrid.ThingsAt(current2))
						{
							Blueprint blueprint2 = current3 as Blueprint;
							ThingDef thingDef3;
							if (blueprint2 != null)
							{
								ThingDef thingDef2 = blueprint2.def.entityDefToBuild as ThingDef;
								if (thingDef2 == null)
								{
									continue;
								}
								thingDef3 = thingDef2;
							}
							else
							{
								thingDef3 = current3.def;
							}
							if (thingDef3.hasInteractionCell && GenConstruct.occupiedCellsTemp.Contains(Thing.InteractionCellWhenAt(thingDef3, current3.Position, current3.Rotation)))
							{
								AcceptanceReport result2 = new AcceptanceReport("WouldBlockInteractionSpot".Translate(new object[]
								{
									entDef.label,
									thingDef3.label
								}).CapitalizeFirst());
								return result2;
							}
						}
					}
				}
			}
			TerrainDef terrainDef = entDef as TerrainDef;
			if (terrainDef != null)
			{
				if (Find.TerrainGrid.TerrainAt(center) == terrainDef)
				{
					return new AcceptanceReport("TerrainIsAlready".Translate(new object[]
					{
						terrainDef.label
					}));
				}
				if (Find.DesignationManager.DesignationAt(center, DesignationDefOf.SmoothFloor) != null)
				{
					return new AcceptanceReport("BeingSmoothed".Translate());
				}
			}
			List<Thing> thingList = center.GetThingList();
			for (int l = 0; l < thingList.Count; l++)
			{
				Thing thing = thingList[l];
				if (thing.Position == center && thing.Rotation == rot)
				{
					if (thing.def == entDef)
					{
						return new AcceptanceReport("IdenticalThingExists".Translate());
					}
					if (thing.def.entityDefToBuild == entDef)
					{
						if (thing is Blueprint)
						{
							return new AcceptanceReport("IdenticalBlueprintExists".Translate());
						}
						return new AcceptanceReport("IdenticalThingExists".Translate());
					}
				}
			}
			if (!GenConstruct.CanBuildOnTerrain(entDef, center, rot))
			{
				return new AcceptanceReport("TerrainCannotSupport".Translate());
			}
			if (!godMode)
			{
				foreach (IntVec3 current4 in GenAdj.CellsOccupiedBy(center, rot, entDef.Size))
				{
					foreach (Thing current5 in Find.ThingGrid.ThingsAt(current4))
					{
						if (!GenSpawn.CanPlaceBlueprintOver(entDef, current5.def))
						{
							AcceptanceReport result2 = new AcceptanceReport("SpaceAlreadyOccupied".Translate());
							return result2;
						}
					}
				}
			}
			return AcceptanceReport.WasAccepted;
		}
	}
}
