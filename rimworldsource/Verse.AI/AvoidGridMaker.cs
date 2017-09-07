using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Verse.AI
{
	public static class AvoidGridMaker
	{
		private static readonly int TrapRadialCells = GenRadial.NumCellsInRadius(2.9f);
		public static ByteGrid MakeAvoidGrid(IncidentParms parms, bool avoidTurrets)
		{
			ByteGrid byteGrid = new ByteGrid();
			if (avoidTurrets)
			{
				List<Building> allBuildingsColonist = Find.ListerBuildings.allBuildingsColonist;
				for (int i = 0; i < allBuildingsColonist.Count; i++)
				{
					if (allBuildingsColonist[i].def.building.ai_combatDangerous)
					{
						Building_TurretGun building_TurretGun = allBuildingsColonist[i] as Building_TurretGun;
						if (building_TurretGun != null)
						{
							AvoidGridMaker.PrintAvoidGridAroundTurret(building_TurretGun, byteGrid);
						}
					}
				}
			}
			List<TrapMemory> list = parms.faction.TacticalMemory.TrapMemories();
			for (int j = 0; j < list.Count; j++)
			{
				AvoidGridMaker.PrintAvoidGridAroundTrapLoc(list[j], byteGrid);
			}
			AvoidGridMaker.ExpandAvoidGridIntoEdifices(byteGrid);
			return byteGrid;
		}
		private static void PrintAvoidGridAroundTrapLoc(TrapMemory mem, ByteGrid avoidGrid)
		{
			Room room = mem.Cell.GetRoom();
			for (int i = 0; i < AvoidGridMaker.TrapRadialCells; i++)
			{
				IntVec3 intVec = mem.Cell + GenRadial.RadialPattern[i];
				if (intVec.InBounds() && intVec.Walkable() && intVec.GetRoom() == room)
				{
					float num = Mathf.Max(1f, intVec.DistanceToSquared(mem.Cell));
					int num2 = Mathf.Max(1, Mathf.RoundToInt(32f * mem.PowerPercent / num));
					AvoidGridMaker.IncrementAvoidGrid(avoidGrid, intVec, num2);
				}
			}
		}
		private static void PrintAvoidGridAroundTurret(Building_TurretGun tur, ByteGrid avoidGrid)
		{
			int num = GenRadial.NumCellsInRadius(tur.GunCompEq.PrimaryVerb.verbProps.range + 4f);
			for (int i = 0; i < num; i++)
			{
				IntVec3 intVec = tur.Position + GenRadial.RadialPattern[i];
				if (intVec.InBounds() && intVec.Walkable() && GenSight.LineOfSight(intVec, tur.Position, true))
				{
					AvoidGridMaker.IncrementAvoidGrid(avoidGrid, intVec, 12);
				}
			}
		}
		private static void IncrementAvoidGrid(ByteGrid avoidGrid, IntVec3 c, int num)
		{
			byte b = avoidGrid[c];
			b = (byte)Mathf.Min(255, (int)b + num);
			avoidGrid[c] = b;
		}
		private static void ExpandAvoidGridIntoEdifices(ByteGrid avoidGrid)
		{
			int numGridCells = CellIndices.NumGridCells;
			for (int i = 0; i < numGridCells; i++)
			{
				if (avoidGrid[i] != 0)
				{
					if (Find.EdificeGrid[i] == null)
					{
						for (int j = 0; j < 8; j++)
						{
							IntVec3 c = CellIndices.IndexToCell(i) + GenAdj.AdjacentCells[j];
							if (c.InBounds())
							{
								if (c.GetEdifice() != null)
								{
									avoidGrid[c] = (byte)Mathf.Min(255, Mathf.Max((int)avoidGrid[c], (int)avoidGrid[i]));
								}
							}
						}
					}
				}
			}
		}
	}
}
