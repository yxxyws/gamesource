using System;
using System.Collections.Generic;
using System.Linq;
namespace Verse
{
	public sealed class ZoneManager : IExposable
	{
		private List<Zone> allZones = new List<Zone>();
		private Zone[] zoneGrid;
		public List<Zone> AllZones
		{
			get
			{
				return this.allZones;
			}
		}
		public ZoneManager()
		{
			if (this.zoneGrid == null)
			{
				this.zoneGrid = new Zone[CellIndices.NumGridCells];
			}
		}
		public void ExposeData()
		{
			Scribe_Collections.LookList<Zone>(ref this.allZones, "allZones", LookMode.Deep, new object[0]);
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				this.RebuildZoneGrid();
			}
		}
		private void RebuildZoneGrid()
		{
			this.zoneGrid = new Zone[CellIndices.NumGridCells];
			foreach (Zone current in this.allZones)
			{
				foreach (IntVec3 current2 in current)
				{
					this.zoneGrid[CellIndices.CellToIndex(current2)] = current;
				}
			}
		}
		public void RegisterZone(Zone newZone)
		{
			this.allZones.Add(newZone);
		}
		public void DeregisterZone(Zone oldZone)
		{
			this.allZones.Remove(oldZone);
		}
		internal void AddZoneGridCell(Zone zone, IntVec3 c)
		{
			this.zoneGrid[CellIndices.CellToIndex(c)] = zone;
		}
		internal void ClearZoneGridCell(IntVec3 c)
		{
			this.zoneGrid[CellIndices.CellToIndex(c)] = null;
		}
		public Zone ZoneAt(IntVec3 c)
		{
			return this.zoneGrid[CellIndices.CellToIndex(c)];
		}
		public string NewZoneName(string nameBase)
		{
			for (int i = 1; i <= 1000; i++)
			{
				string cand = nameBase + " " + i;
				if (!this.allZones.Any((Zone z) => z.label == cand))
				{
					return cand;
				}
			}
			Log.Error("Ran out of zone names.");
			return "Zone X";
		}
		internal void Notify_NoZoneOverlapThingSpawned(Thing thing)
		{
			CellRect cellRect = thing.OccupiedRect();
			for (int i = cellRect.minZ; i <= cellRect.maxZ; i++)
			{
				for (int j = cellRect.minX; j <= cellRect.maxX; j++)
				{
					IntVec3 c = new IntVec3(j, 0, i);
					Zone zone = this.ZoneAt(c);
					if (zone != null)
					{
						zone.RemoveCell(c);
						zone.CheckContiguous();
					}
				}
			}
		}
	}
}
