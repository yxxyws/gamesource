using RimWorld;
using System;
using System.Collections.Generic;
namespace Verse
{
	public static class RoofMaker
	{
		private class RunningRoofEffect
		{
			private const int RoofGenDelay = 75;
			private const int TicksBetweenSteps = 3;
			private List<IntVec3> cells;
			public Room room;
			public int startTick;
			private readonly int startingZIntercept;
			private bool completed;
			public bool ShouldRemove
			{
				get
				{
					return this.completed || this.room.Dereferenced;
				}
			}
			public RunningRoofEffect(Room room, List<IntVec3> cells)
			{
				this.room = room;
				this.cells = cells;
				this.startTick = Find.TickManager.TicksGame;
				int num = 99999;
				IntVec3 intVec = default(IntVec3);
				for (int i = 0; i < cells.Count; i++)
				{
					IntVec3 intVec2 = cells[i];
					int num2 = intVec2.x - intVec2.z;
					if (num2 < num)
					{
						intVec = intVec2;
						num = num2;
					}
				}
				this.startingZIntercept = intVec.z - intVec.x;
			}
			public void RoofRequestTick()
			{
				if (!this.room.Dereferenced)
				{
					int num = (Find.TickManager.TicksGame - (this.startTick + 75)) / 3;
					if (num >= 0 && !this.DoRoofEffectAlongSlice(this.startingZIntercept - num))
					{
						this.Complete();
					}
				}
			}
			public void ForceInstantFinish()
			{
				this.Complete();
			}
			private void Complete()
			{
				this.completed = true;
			}
			private bool DoRoofEffectAlongSlice(int sliceZIntercept)
			{
				if (this.cells.Count == 0)
				{
					return false;
				}
				bool result = false;
				for (int i = 0; i < this.cells.Count; i++)
				{
					IntVec3 cell = this.cells[i];
					if (cell.x == cell.z - sliceZIntercept)
					{
						result = true;
						MoteThrower.PlaceTempRoof(cell);
					}
				}
				return result;
			}
		}
		private const int MaxCellsToDoRoofMote = 500;
		private static List<RoofMaker.RunningRoofEffect> runningEffects = new List<RoofMaker.RunningRoofEffect>();
		private static List<Room> queuedGenerateRooms = new List<Room>();
		private static HashSet<IntVec3> cellsToRoof = new HashSet<IntVec3>();
		private static HashSet<IntVec3> innerCells = new HashSet<IntVec3>();
		private static List<IntVec3> justRoofedCells = new List<IntVec3>();
		public static void TryGenerateRoofOnImpassable(IntVec3 c)
		{
			if (!c.Roofed() && c.Impassable() && RoofCollapser.IsSupported(c))
			{
				bool flag = false;
				for (int i = 0; i < 9; i++)
				{
					IntVec3 loc = c + GenRadial.RadialPattern[i];
					Room room = loc.GetRoom();
					if (room != null && !room.TouchesMapEdge)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					Find.RoofGrid.SetRoof(c, RoofDefOf.RoofConstructed);
					MoteThrower.PlaceTempRoof(c);
				}
			}
		}
		public static void TryGenerateRoofFor(Room room)
		{
			RoofMaker.queuedGenerateRooms.Add(room);
		}
		public static void RoofMakerTick_First()
		{
			for (int i = 0; i < RoofMaker.runningEffects.Count; i++)
			{
				RoofMaker.runningEffects[i].RoofRequestTick();
			}
			RoofMaker.runningEffects.RemoveAll((RoofMaker.RunningRoofEffect req) => req.ShouldRemove);
			RoofMaker.ResolveQueuedGenerateRoofs();
		}
		public static void ResolveQueuedGenerateRoofs()
		{
			for (int i = 0; i < RoofMaker.queuedGenerateRooms.Count; i++)
			{
				RoofMaker.TryGenerateRoofNow(RoofMaker.queuedGenerateRooms[i]);
			}
			RoofMaker.queuedGenerateRooms.Clear();
		}
		private static void TryGenerateRoofNow(Room room)
		{
			if (room.Dereferenced || room.TouchesMapEdge)
			{
				return;
			}
			if (room.RegionCount > 80)
			{
				return;
			}
			RoofMaker.RunningRoofEffect runningRoofEffect = RoofMaker.ExistingRunningEffectFor(room);
			if (runningRoofEffect != null)
			{
				runningRoofEffect.ForceInstantFinish();
			}
			RoofMaker.innerCells.Clear();
			foreach (IntVec3 current in room.Cells)
			{
				if (!RoofMaker.innerCells.Contains(current))
				{
					RoofMaker.innerCells.Add(current);
				}
				for (int i = 0; i < 8; i++)
				{
					IntVec3 c = current + GenAdj.AdjacentCells[i];
					if (c.InBounds())
					{
						Thing edifice = c.GetEdifice();
						if (edifice != null && edifice.def.regionBarrier && (edifice.def.size.x > 1 || edifice.def.size.z > 1))
						{
							CellRect cellRect = edifice.OccupiedRect();
							cellRect.ClipInsideMap();
							for (int j = cellRect.minZ; j <= cellRect.maxZ; j++)
							{
								for (int k = cellRect.minX; k <= cellRect.maxX; k++)
								{
									IntVec3 item = new IntVec3(k, 0, j);
									if (!RoofMaker.innerCells.Contains(item))
									{
										RoofMaker.innerCells.Add(item);
									}
								}
							}
						}
					}
				}
			}
			RoofMaker.cellsToRoof.Clear();
			foreach (IntVec3 current2 in RoofMaker.innerCells)
			{
				for (int l = 0; l < 9; l++)
				{
					IntVec3 intVec = current2 + GenAdj.AdjacentCellsAndInside[l];
					if (intVec.InBounds() && (l == 8 || intVec.GetRegionBarrier() != null) && !RoofMaker.cellsToRoof.Contains(intVec))
					{
						RoofMaker.cellsToRoof.Add(intVec);
					}
				}
			}
			RoofMaker.justRoofedCells.Clear();
			RoofDef named = DefDatabase<RoofDef>.GetNamed("RoofConstructed", true);
			foreach (IntVec3 current3 in RoofMaker.cellsToRoof)
			{
				if (Find.RoofGrid.RoofAt(current3) == null && !RoofMaker.justRoofedCells.Contains(current3))
				{
					if (!Find.AreaNoRoof[current3])
					{
						if (RoofCollapser.IsSupported(current3))
						{
							Find.RoofGrid.SetRoof(current3, named);
							RoofMaker.justRoofedCells.Add(current3);
						}
					}
				}
			}
			if (RoofMaker.justRoofedCells.Count > 0 && RoofMaker.justRoofedCells.Count < 500)
			{
				RoofMaker.RunningRoofEffect item2 = new RoofMaker.RunningRoofEffect(room, RoofMaker.justRoofedCells);
				RoofMaker.runningEffects.Add(item2);
			}
		}
		private static RoofMaker.RunningRoofEffect ExistingRunningEffectFor(Room room)
		{
			for (int i = 0; i < RoofMaker.runningEffects.Count; i++)
			{
				if (RoofMaker.runningEffects[i].room == room)
				{
					return RoofMaker.runningEffects[i];
				}
			}
			return null;
		}
		public static void InstantFinishAll()
		{
			foreach (RoofMaker.RunningRoofEffect current in RoofMaker.runningEffects)
			{
				current.ForceInstantFinish();
			}
			RoofMaker.runningEffects.Clear();
		}
	}
}
