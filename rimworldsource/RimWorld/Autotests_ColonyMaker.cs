using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public static class Autotests_ColonyMaker
	{
		private const int OverRectSize = 80;
		private static CellRect overRect;
		private static BoolGrid usedCells = new BoolGrid();
		public static void MakeColony_Full()
		{
			Autotests_ColonyMaker.MakeColony(new ColonyMakerFlag[]
			{
				ColonyMakerFlag.ConduitGrid,
				ColonyMakerFlag.PowerPlants,
				ColonyMakerFlag.Batteries,
				ColonyMakerFlag.WorkTables,
				ColonyMakerFlag.AllBuildings,
				ColonyMakerFlag.AllItems,
				ColonyMakerFlag.Filth,
				ColonyMakerFlag.ColonistsMany,
				ColonyMakerFlag.ColonistsHungry,
				ColonyMakerFlag.ColonistsTired,
				ColonyMakerFlag.ColonistsInjured,
				ColonyMakerFlag.ColonistsDiseased,
				ColonyMakerFlag.Beds,
				ColonyMakerFlag.Stockpiles,
				ColonyMakerFlag.GrowingZones
			});
		}
		public static void MakeColony_Bills()
		{
			Autotests_ColonyMaker.MakeColony(new ColonyMakerFlag[]
			{
				ColonyMakerFlag.ConduitGrid,
				ColonyMakerFlag.PowerPlants,
				ColonyMakerFlag.Batteries,
				ColonyMakerFlag.WorkTables,
				ColonyMakerFlag.ItemsRawFood,
				ColonyMakerFlag.ColonistsMany
			});
		}
		public static void MakeColony_Filth()
		{
			Autotests_ColonyMaker.MakeColony(new ColonyMakerFlag[]
			{
				ColonyMakerFlag.Filth,
				ColonyMakerFlag.ColonistsMany
			});
		}
		public static void MakeColony_Hauling()
		{
			Autotests_ColonyMaker.MakeColony(new ColonyMakerFlag[]
			{
				ColonyMakerFlag.AllItems,
				ColonyMakerFlag.ColonistsMany
			});
		}
		public static void MakeColony_Fire()
		{
			Autotests_ColonyMaker.MakeColony(new ColonyMakerFlag[]
			{
				ColonyMakerFlag.Fire
			});
		}
		public static void MakeColony(params ColonyMakerFlag[] flags)
		{
			bool godMode = Game.GodMode;
			Game.GodMode = true;
			Thing.allowDestroyNonDestroyable = true;
			Autotests_ColonyMaker.usedCells.Clear();
			Autotests_ColonyMaker.overRect = new CellRect(Find.Map.Center.x - 40, Find.Map.Center.z - 40, 80, 80);
			Autotests_ColonyMaker.DeleteAllSpawnedPawns();
			GenDebug.ClearArea(Autotests_ColonyMaker.overRect);
			if (flags.Contains(ColonyMakerFlag.ConduitGrid))
			{
				Designator_Build designator_Build = new Designator_Build(ThingDefOf.PowerConduit);
				for (int i = Autotests_ColonyMaker.overRect.minX; i < Autotests_ColonyMaker.overRect.maxX; i++)
				{
					for (int j = Autotests_ColonyMaker.overRect.minZ; j < Autotests_ColonyMaker.overRect.maxZ; j += 7)
					{
						designator_Build.DesignateSingleCell(new IntVec3(i, 0, j));
					}
				}
				for (int k = Autotests_ColonyMaker.overRect.minZ; k < Autotests_ColonyMaker.overRect.maxZ; k++)
				{
					for (int l = Autotests_ColonyMaker.overRect.minX; l < Autotests_ColonyMaker.overRect.maxX; l += 7)
					{
						designator_Build.DesignateSingleCell(new IntVec3(l, 0, k));
					}
				}
			}
			if (flags.Contains(ColonyMakerFlag.PowerPlants))
			{
				List<ThingDef> list = new List<ThingDef>
				{
					ThingDefOf.SolarGenerator,
					ThingDef.Named("WindTurbine")
				};
				for (int m = 0; m < 8; m++)
				{
					if (Autotests_ColonyMaker.TryMakeBuilding(list[m % list.Count]) == null)
					{
						Log.Message("Could not make solar generator.");
						break;
					}
				}
			}
			if (flags.Contains(ColonyMakerFlag.Batteries))
			{
				for (int n = 0; n < 6; n++)
				{
					Thing thing = Autotests_ColonyMaker.TryMakeBuilding(ThingDefOf.Battery);
					if (thing == null)
					{
						Log.Message("Could not make battery.");
						break;
					}
					((Building_Battery)thing).GetComp<CompPowerBattery>().AddEnergy(999999f);
				}
			}
			if (flags.Contains(ColonyMakerFlag.WorkTables))
			{
				IEnumerable<ThingDef> enumerable = 
					from def in DefDatabase<ThingDef>.AllDefs
					where typeof(Building_WorkTable).IsAssignableFrom(def.thingClass)
					select def;
				foreach (ThingDef current in enumerable)
				{
					Thing thing2 = Autotests_ColonyMaker.TryMakeBuilding(current);
					if (thing2 == null)
					{
						Log.Message("Could not make worktable: " + current.defName);
						break;
					}
					Building_WorkTable building_WorkTable = thing2 as Building_WorkTable;
					if (building_WorkTable != null)
					{
						foreach (RecipeDef current2 in building_WorkTable.def.AllRecipes)
						{
							building_WorkTable.billStack.AddBill(current2.MakeNewBill());
						}
					}
				}
			}
			if (flags.Contains(ColonyMakerFlag.AllBuildings))
			{
				IEnumerable<ThingDef> enumerable2 = 
					from def in DefDatabase<ThingDef>.AllDefs
					where def.category == ThingCategory.Building && def.designationCategory != null
					select def;
				foreach (ThingDef current3 in enumerable2)
				{
					if (current3 != ThingDefOf.PowerConduit)
					{
						if (Autotests_ColonyMaker.TryMakeBuilding(current3) == null)
						{
							Log.Message("Could not make building: " + current3.defName);
							break;
						}
					}
				}
			}
			CellRect rect;
			if (!Autotests_ColonyMaker.TryGetFreeRect(33, 33, out rect))
			{
				Log.Error("Could not get wallable rect");
			}
			rect = rect.ContractedBy(1);
			if (flags.Contains(ColonyMakerFlag.AllItems))
			{
				List<ThingDef> itemDefs = (
					from def in DefDatabase<ThingDef>.AllDefs
					where DebugThingPlaceHelper.IsDebugSpawnable(def) && def.category == ThingCategory.Item
					select def).ToList<ThingDef>();
				Autotests_ColonyMaker.FillWithItems(rect, itemDefs);
			}
			else
			{
				if (flags.Contains(ColonyMakerFlag.ItemsRawFood))
				{
					List<ThingDef> list2 = new List<ThingDef>();
					list2.Add(ThingDefOf.RawPotatoes);
					Autotests_ColonyMaker.FillWithItems(rect, list2);
				}
			}
			if (flags.Contains(ColonyMakerFlag.Filth))
			{
				foreach (IntVec3 current4 in rect)
				{
					GenSpawn.Spawn(ThingDefOf.FilthDirt, current4);
				}
			}
			if (flags.Contains(ColonyMakerFlag.ItemsWall))
			{
				CellRect cellRect = rect.ExpandedBy(1);
				Designator_Build designator_Build2 = new Designator_Build(ThingDefOf.Wall);
				designator_Build2.DebugSetStuffDef(ThingDefOf.WoodLog);
				foreach (IntVec3 current5 in cellRect.EdgeCells)
				{
					designator_Build2.DesignateSingleCell(current5);
				}
			}
			if (flags.Contains(ColonyMakerFlag.ColonistsMany))
			{
				Autotests_ColonyMaker.MakeColonists(15, Autotests_ColonyMaker.overRect.Center);
			}
			else
			{
				if (flags.Contains(ColonyMakerFlag.ColonistOne))
				{
					Autotests_ColonyMaker.MakeColonists(1, Autotests_ColonyMaker.overRect.Center);
				}
			}
			if (flags.Contains(ColonyMakerFlag.Fire))
			{
				CellRect cellRect2;
				if (!Autotests_ColonyMaker.TryGetFreeRect(30, 30, out cellRect2))
				{
					Log.Error("Could not get free rect for fire.");
				}
				ThingDef def2 = ThingDef.Named("PlantTreeOak");
				foreach (IntVec3 current6 in cellRect2)
				{
					GenSpawn.Spawn(def2, current6);
				}
				foreach (IntVec3 current7 in cellRect2)
				{
					if (current7.x % 7 == 0 && current7.z % 7 == 0)
					{
						GenExplosion.DoExplosion(current7, 3.9f, DamageDefOf.Flame, null, null, null, null, null, 0f, false, null, 0f);
					}
				}
			}
			if (flags.Contains(ColonyMakerFlag.ColonistsHungry))
			{
				Autotests_ColonyMaker.DoToColonists(0.4f, delegate(Pawn col)
				{
					col.needs.food.CurLevel = Mathf.Max(0f, Rand.Range(-0.05f, 0.05f));
				});
			}
			if (flags.Contains(ColonyMakerFlag.ColonistsTired))
			{
				Autotests_ColonyMaker.DoToColonists(0.4f, delegate(Pawn col)
				{
					col.needs.rest.CurLevel = Mathf.Max(0f, Rand.Range(-0.05f, 0.05f));
				});
			}
			if (flags.Contains(ColonyMakerFlag.ColonistsInjured))
			{
				Autotests_ColonyMaker.DoToColonists(0.4f, delegate(Pawn col)
				{
					DamageDef def3 = (
						from d in DefDatabase<DamageDef>.AllDefs
						where d.externalViolence
						select d).RandomElement<DamageDef>();
					col.TakeDamage(new DamageInfo(def3, 10, null, null, null));
				});
			}
			if (flags.Contains(ColonyMakerFlag.ColonistsDiseased))
			{
				foreach (HediffDef current8 in 
					from d in DefDatabase<HediffDef>.AllDefs
					where d.hediffClass != typeof(Hediff_AddedPart) && (d.HasComp(typeof(HediffComp_Immunizable)) || d.HasComp(typeof(HediffComp_GrowthMode)))
					select d)
				{
					Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfColony);
					CellRect cellRect3;
					Autotests_ColonyMaker.TryGetFreeRect(1, 1, out cellRect3);
					GenSpawn.Spawn(pawn, cellRect3.Center);
					pawn.health.AddHediff(current8, null, null);
				}
			}
			if (flags.Contains(ColonyMakerFlag.Beds))
			{
				IEnumerable<ThingDef> source = 
					from def in DefDatabase<ThingDef>.AllDefs
					where def.thingClass == typeof(Building_Bed)
					select def;
				int freeColonistsCount = Find.MapPawns.FreeColonistsCount;
				for (int num = 0; num < freeColonistsCount; num++)
				{
					if (Autotests_ColonyMaker.TryMakeBuilding(source.RandomElement<ThingDef>()) == null)
					{
						Log.Message("Could not make beds.");
						break;
					}
				}
			}
			if (flags.Contains(ColonyMakerFlag.Stockpiles))
			{
				Designator_ZoneAddStockpile_Resources designator_ZoneAddStockpile_Resources = new Designator_ZoneAddStockpile_Resources();
				IEnumerator enumerator9 = Enum.GetValues(typeof(StoragePriority)).GetEnumerator();
				try
				{
					while (enumerator9.MoveNext())
					{
						StoragePriority priority = (StoragePriority)((byte)enumerator9.Current);
						CellRect cellRect4;
						Autotests_ColonyMaker.TryGetFreeRect(7, 7, out cellRect4);
						cellRect4 = cellRect4.ContractedBy(1);
						designator_ZoneAddStockpile_Resources.DesignateMultiCell(cellRect4.Cells);
						Zone_Stockpile zone_Stockpile = (Zone_Stockpile)Find.ZoneManager.ZoneAt(cellRect4.Center);
						zone_Stockpile.settings.Priority = priority;
					}
				}
				finally
				{
					IDisposable disposable = enumerator9 as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			if (flags.Contains(ColonyMakerFlag.GrowingZones))
			{
				Zone_Growing dummyZone = new Zone_Growing();
				foreach (ThingDef current9 in 
					from d in DefDatabase<ThingDef>.AllDefs
					where d.plant != null && GenPlant.CanSowOnGrower(d, dummyZone)
					select d)
				{
					CellRect cellRect5;
					if (!Autotests_ColonyMaker.TryGetFreeRect(6, 6, out cellRect5))
					{
						Log.Error("Could not get growing zone rect.");
					}
					cellRect5 = cellRect5.ContractedBy(1);
					foreach (IntVec3 current10 in cellRect5)
					{
						Find.TerrainGrid.SetTerrain(current10, TerrainDefOf.Soil);
					}
					Designator_ZoneAdd_Growing designator_ZoneAdd_Growing = new Designator_ZoneAdd_Growing();
					designator_ZoneAdd_Growing.DesignateMultiCell(cellRect5.Cells);
					Zone_Growing zone_Growing = (Zone_Growing)Find.ZoneManager.ZoneAt(cellRect5.Center);
					zone_Growing.SetPlantDefToGrow(current9);
				}
				dummyZone.Delete();
			}
			Autotests_ColonyMaker.ClearAllHomeArea();
			Autotests_ColonyMaker.FillWithHomeArea(Autotests_ColonyMaker.overRect);
			Game.GodMode = godMode;
			Thing.allowDestroyNonDestroyable = false;
		}
		private static void FillWithItems(CellRect rect, List<ThingDef> itemDefs)
		{
			int num = 0;
			foreach (IntVec3 current in rect)
			{
				if (current.x % 6 != 0 && current.z % 6 != 0)
				{
					ThingDef def = itemDefs[num];
					DebugThingPlaceHelper.DebugSpawn(def, current, -1, true);
					num++;
					if (num >= itemDefs.Count)
					{
						num = 0;
					}
				}
			}
		}
		private static Thing TryMakeBuilding(ThingDef def)
		{
			CellRect cellRect;
			if (!Autotests_ColonyMaker.TryGetFreeRect(def.size.x + 2, def.size.z + 2, out cellRect))
			{
				return null;
			}
			foreach (IntVec3 current in cellRect)
			{
				Find.TerrainGrid.SetTerrain(current, TerrainDefOf.Concrete);
			}
			Designator_Build designator_Build = new Designator_Build(def);
			IntVec3 c = new IntVec3(cellRect.minX + 1, 0, cellRect.minZ + 1);
			designator_Build.DesignateSingleCell(c);
			return c.GetEdifice();
		}
		private static bool TryGetFreeRect(int width, int height, out CellRect result)
		{
			int minX = Autotests_ColonyMaker.overRect.minX;
			int maxX = Autotests_ColonyMaker.overRect.maxX;
			int minZ = Autotests_ColonyMaker.overRect.minZ;
			int maxZ = Autotests_ColonyMaker.overRect.maxZ;
			for (int i = minZ; i <= maxZ; i++)
			{
				for (int j = minX; j <= maxX; j++)
				{
					if (!Autotests_ColonyMaker.usedCells[j, i])
					{
						CellRect cellRect = new CellRect(j, i, width, height);
						bool flag = true;
						for (int k = cellRect.minZ; k < cellRect.maxZ; k++)
						{
							for (int l = cellRect.minX; l < cellRect.maxX; l++)
							{
								if (Autotests_ColonyMaker.usedCells[l, k] || !Autotests_ColonyMaker.overRect.Contains(new IntVec3(l, 0, k)))
								{
									flag = false;
									break;
								}
							}
							if (!flag)
							{
								break;
							}
						}
						if (flag)
						{
							result = cellRect;
							foreach (IntVec3 current in cellRect)
							{
								Autotests_ColonyMaker.usedCells.Set(current, true);
								if (current.GetTerrain().passability == Traversability.Impassable)
								{
									Find.TerrainGrid.SetTerrain(current, TerrainDefOf.Concrete);
								}
							}
							return true;
						}
					}
				}
			}
			result = new CellRect(0, 0, width, height);
			return false;
		}
		private static void DoToColonists(float fraction, Action<Pawn> funcToDo)
		{
			int num = Rand.RangeInclusive(1, Mathf.RoundToInt((float)Find.MapPawns.FreeColonistsCount * fraction));
			int num2 = 0;
			foreach (Pawn current in Find.MapPawns.FreeColonists.InRandomOrder(null))
			{
				funcToDo(current);
				num2++;
				if (num2 >= num)
				{
					break;
				}
			}
		}
		private static void MakeColonists(int count, IntVec3 center)
		{
			for (int i = 0; i < count; i++)
			{
				CellRect cellRect;
				Autotests_ColonyMaker.TryGetFreeRect(1, 1, out cellRect);
				Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfColony);
				foreach (WorkTypeDef current in DefDatabase<WorkTypeDef>.AllDefs)
				{
					if (!pawn.story.WorkTypeIsDisabled(current))
					{
						pawn.workSettings.SetPriority(current, 1);
					}
				}
				GenSpawn.Spawn(pawn, cellRect.Center);
			}
		}
		private static void DeleteAllSpawnedPawns()
		{
			foreach (Pawn current in Find.MapPawns.AllPawnsSpawned.ToList<Pawn>())
			{
				current.Destroy(DestroyMode.Vanish);
				current.relations.ClearAllRelations();
			}
			Find.GameEnder.gameEnding = false;
		}
		private static void ClearAllHomeArea()
		{
			foreach (IntVec3 current in Find.Map.AllCells)
			{
				Find.AreaHome.Clear(current);
			}
		}
		private static void FillWithHomeArea(CellRect r)
		{
			Designator_AreaHomeExpand designator_AreaHomeExpand = new Designator_AreaHomeExpand();
			designator_AreaHomeExpand.DesignateMultiCell(r.Cells);
		}
	}
}
