using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
namespace Verse
{
	public sealed class Room
	{
		private const int RegionCountHuge = 60;
		private List<Region> regions = new List<Region>();
		public int ID = -16161616;
		public int lastChangeTick = -1;
		private RoomTempTracker tempTracker;
		private int numRegionsTouchingMapEdge;
		public bool isPrisonCell;
		private int cachedCellCount = -1;
		private int cachedOpenRoofCount = -1;
		private bool statsAndRoleDirty = true;
		private DefMap<RoomStatDef, float> stats = new DefMap<RoomStatDef, float>();
		private RoomRoleDef role;
		private static int nextRoomID = 0;
		private static readonly Color PrisonFieldColor = new Color(1f, 0.7f, 0.2f);
		private static readonly Color NonPrisonFieldColor = new Color(0.3f, 0.3f, 1f);
		private HashSet<Thing> uniqueContainedThingsSet = new HashSet<Thing>();
		private List<Thing> uniqueContainedThings = new List<Thing>();
		private static List<IntVec3> fields = new List<IntVec3>();
		public RoomTempTracker TempTracker
		{
			get
			{
				return this.tempTracker;
			}
		}
		public List<Region> Regions
		{
			get
			{
				return this.regions;
			}
		}
		public int RegionCount
		{
			get
			{
				return this.regions.Count;
			}
		}
		public bool IsHuge
		{
			get
			{
				return this.regions.Count > 60;
			}
		}
		public bool Dereferenced
		{
			get
			{
				return this.regions.Count == 0;
			}
		}
		public bool TouchesMapEdge
		{
			get
			{
				return this.numRegionsTouchingMapEdge > 0;
			}
		}
		public bool HasOpenRoofSection
		{
			get
			{
				return this.OpenRoofCount > 0;
			}
		}
		public bool PsychologicallyOutdoors
		{
			get
			{
				return this.TouchesMapEdge || this.OpenRoofCount > 300;
			}
		}
		public float Temperature
		{
			get
			{
				return this.tempTracker.Temperature;
			}
			set
			{
				this.tempTracker.Temperature = value;
			}
		}
		public bool UsesOutdoorTemperature
		{
			get
			{
				return this.TouchesMapEdge || this.HasOpenRoofSection;
			}
		}
		public int CellCount
		{
			get
			{
				if (this.cachedCellCount == -1)
				{
					this.cachedCellCount = 0;
					for (int i = 0; i < this.regions.Count; i++)
					{
						this.cachedCellCount += this.regions[i].CellCount;
					}
				}
				return this.cachedCellCount;
			}
		}
		public int OpenRoofCount
		{
			get
			{
				if (this.cachedOpenRoofCount == -1)
				{
					this.cachedOpenRoofCount = 0;
					RoofGrid roofGrid = Find.RoofGrid;
					foreach (IntVec3 current in this.Cells)
					{
						if (!roofGrid.Roofed(current))
						{
							this.cachedOpenRoofCount++;
						}
					}
				}
				return this.cachedOpenRoofCount;
			}
		}
		public IEnumerable<IntVec3> Cells
		{
			get
			{
				Room.<>c__Iterator16B <>c__Iterator16B = new Room.<>c__Iterator16B();
				<>c__Iterator16B.<>f__this = this;
				Room.<>c__Iterator16B expr_0E = <>c__Iterator16B;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public IEnumerable<Pawn> Owners
		{
			get
			{
				Room.<>c__Iterator16C <>c__Iterator16C = new Room.<>c__Iterator16C();
				<>c__Iterator16C.<>f__this = this;
				Room.<>c__Iterator16C expr_0E = <>c__Iterator16C;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public IEnumerable<Building_Bed> ContainedBeds
		{
			get
			{
				Room.<>c__Iterator16D <>c__Iterator16D = new Room.<>c__Iterator16D();
				<>c__Iterator16D.<>f__this = this;
				Room.<>c__Iterator16D expr_0E = <>c__Iterator16D;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public List<Thing> AllContainedThings
		{
			get
			{
				this.uniqueContainedThingsSet.Clear();
				this.uniqueContainedThings.Clear();
				for (int i = 0; i < this.regions.Count; i++)
				{
					if (this.regions[i].ListerThings.AllThings != null)
					{
						for (int j = 0; j < this.regions[i].ListerThings.AllThings.Count; j++)
						{
							Thing item = this.regions[i].ListerThings.AllThings[j];
							if (!this.uniqueContainedThingsSet.Contains(item))
							{
								this.uniqueContainedThingsSet.Add(item);
								this.uniqueContainedThings.Add(item);
							}
						}
					}
				}
				return this.uniqueContainedThings;
			}
		}
		public RoomRoleDef Role
		{
			get
			{
				if (this.statsAndRoleDirty)
				{
					this.UpdateRoomStatsAndRole();
				}
				return this.role;
			}
		}
		public bool IsDoor
		{
			get
			{
				if (this.CellCount != 1)
				{
					return false;
				}
				Building edifice = this.Cells.FirstOrDefault<IntVec3>().GetEdifice();
				return edifice != null && edifice is Building_Door;
			}
		}
		[DebuggerHidden]
		public IEnumerator<IntVec3> GetEnumerator()
		{
			Room.<GetEnumerator>c__Iterator16E <GetEnumerator>c__Iterator16E = new Room.<GetEnumerator>c__Iterator16E();
			<GetEnumerator>c__Iterator16E.<>f__this = this;
			return <GetEnumerator>c__Iterator16E;
		}
		public static Room MakeNew()
		{
			Room room = new Room();
			room.ID = Room.nextRoomID;
			room.tempTracker = new RoomTempTracker(room);
			Room.nextRoomID++;
			return room;
		}
		public void AddRegion(Region r)
		{
			this.regions.Add(r);
			if (r.touchesMapEdge)
			{
				this.numRegionsTouchingMapEdge++;
			}
			if (this.regions.Count == 1)
			{
				Find.RegionGrid.allRooms.Add(this);
			}
		}
		public void RemoveRegion(Region r)
		{
			this.regions.Remove(r);
			if (r.touchesMapEdge)
			{
				this.numRegionsTouchingMapEdge--;
			}
			if (this.regions.Count == 0)
			{
				Find.RegionGrid.allRooms.Remove(this);
			}
		}
		public void RoofChanged()
		{
			this.cachedOpenRoofCount = -1;
		}
		public void RoomChanged()
		{
			ProfilerThreadCheck.BeginSample("RoomChanged");
			this.cachedCellCount = -1;
			this.cachedOpenRoofCount = -1;
			if (Game.Mode == GameMode.MapPlaying)
			{
				ProfilerThreadCheck.BeginSample("RoofGenerationRequest");
				RoofMaker.TryGenerateRoofFor(this);
				ProfilerThreadCheck.EndSample();
			}
			this.isPrisonCell = false;
			if (!this.TouchesMapEdge)
			{
				List<Thing> allContainedThings = this.AllContainedThings;
				for (int i = 0; i < allContainedThings.Count; i++)
				{
					Building_Bed building_Bed = allContainedThings[i] as Building_Bed;
					if (building_Bed != null && building_Bed.ForPrisoners)
					{
						this.isPrisonCell = true;
						break;
					}
				}
			}
			if (Game.Mode == GameMode.MapPlaying && this.isPrisonCell)
			{
				foreach (Building_Bed current in this.ContainedBeds)
				{
					current.ForPrisoners = true;
				}
			}
			this.lastChangeTick = Find.TickManager.TicksGame;
			this.tempTracker.RoomChanged();
			this.statsAndRoleDirty = true;
			FacilitiesUtility.NotifyFacilitiesAboutChangedLOSBlockers(this.regions);
			ProfilerThreadCheck.EndSample();
		}
		public void Notify_ContainedThingSpawnedOrDespawned(Thing th)
		{
			if (th.def.category != ThingCategory.Mote && th.def.category != ThingCategory.Projectile && th.def.category != ThingCategory.Skyfaller && th.def.category != ThingCategory.Pawn)
			{
				this.statsAndRoleDirty = true;
			}
		}
		public void Notify_TerrainChanged()
		{
			this.statsAndRoleDirty = true;
		}
		public void Notify_BedTypeChanged()
		{
			this.statsAndRoleDirty = true;
		}
		public float GetStat(RoomStatDef roomStat)
		{
			if (this.statsAndRoleDirty)
			{
				this.UpdateRoomStatsAndRole();
			}
			if (this.stats == null)
			{
				return roomStat.defaultScore;
			}
			return this.stats[roomStat];
		}
		public bool PushHeat(float energy)
		{
			if (this.UsesOutdoorTemperature)
			{
				return false;
			}
			this.Temperature += energy / (float)this.CellCount;
			return true;
		}
		public RoomStatScoreStage GetStatScoreStage(RoomStatDef stat)
		{
			return stat.GetScoreStage(this.GetStat(stat));
		}
		public void DrawFieldEdges()
		{
			if (this.RegionCount >= 20 || this.TouchesMapEdge)
			{
				return;
			}
			foreach (IntVec3 current in this.Cells)
			{
				Room.fields.Add(current);
			}
			Color color = (!this.isPrisonCell) ? Room.NonPrisonFieldColor : Room.PrisonFieldColor;
			color.a = Pulser.PulseBrightness(1f, 0.6f);
			GenDraw.DrawFieldEdges(Room.fields, color);
			Room.fields.Clear();
		}
		private void UpdateRoomStatsAndRole()
		{
			this.statsAndRoleDirty = false;
			if (!this.TouchesMapEdge && !this.IsDoor)
			{
				if (this.stats == null)
				{
					this.stats = new DefMap<RoomStatDef, float>();
				}
				foreach (RoomStatDef current in 
					from x in DefDatabase<RoomStatDef>.AllDefs
					orderby x.updatePriority descending
					select x)
				{
					this.stats[current] = current.Worker.GetScore(this);
				}
				this.role = DefDatabase<RoomRoleDef>.AllDefs.MaxBy((RoomRoleDef x) => x.Worker.GetScore(this));
			}
			else
			{
				this.stats = null;
				this.role = RoomRoleDefOf.None;
			}
		}
		internal void DebugDraw()
		{
			int hashCode = this.GetHashCode();
			foreach (IntVec3 current in this.Cells)
			{
				DebugRender.RenderCell(current, (float)hashCode * 0.01f);
			}
			this.tempTracker.DebugDraw();
		}
		internal string DebugString()
		{
			return string.Concat(new object[]
			{
				"Room ID=",
				this.ID,
				"\n  first=",
				this.Cells.FirstOrDefault<IntVec3>().ToString(),
				"\n  RegionsCount=",
				this.RegionCount.ToString(),
				"\n  CellCount=",
				this.CellCount,
				"\n  OpenRoofCount=",
				this.OpenRoofCount,
				"\n  numRegionsTouchingMapEdge=",
				this.numRegionsTouchingMapEdge,
				"\n  lastChangeTick=",
				this.lastChangeTick,
				"\n  isPrisonCell=",
				this.isPrisonCell,
				"\n  ",
				this.tempTracker.DebugString()
			});
		}
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Room(roomID=",
				this.ID,
				", first=",
				this.Cells.FirstOrDefault<IntVec3>().ToString(),
				", RegionsCount=",
				this.RegionCount.ToString(),
				", lastChangeTick=",
				this.lastChangeTick,
				")"
			});
		}
		public override int GetHashCode()
		{
			return Gen.HashCombineInt(this.ID, 1538478890);
		}
	}
}
