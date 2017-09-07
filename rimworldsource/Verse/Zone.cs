using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse.Sound;
namespace Verse
{
	public abstract class Zone : IExposable, ISelectable
	{
		private const int StaticFireCheckInterval = 1000;
		public string label;
		public List<IntVec3> cells = new List<IntVec3>();
		private bool cellsShuffled;
		public Color color = Color.white;
		private Material materialInt;
		public bool hidden;
		private int lastStaticFireCheckTick = -9999;
		private bool lastStaticFireCheckResult;
		private static BoolGrid extantGrid = new BoolGrid();
		private static BoolGrid foundGrid = new BoolGrid();
		public Material Material
		{
			get
			{
				if (this.materialInt == null)
				{
					this.materialInt = SolidColorMaterials.SimpleSolidColorMaterial(this.color);
					this.materialInt.renderQueue = 3600;
				}
				return this.materialInt;
			}
		}
		public List<IntVec3> Cells
		{
			get
			{
				if (!this.cellsShuffled)
				{
					this.cells.Shuffle<IntVec3>();
					this.cellsShuffled = true;
				}
				return this.cells;
			}
		}
		public IEnumerable<Thing> AllContainedThings
		{
			get
			{
				Zone.<>c__Iterator83 <>c__Iterator = new Zone.<>c__Iterator83();
				<>c__Iterator.<>f__this = this;
				Zone.<>c__Iterator83 expr_0E = <>c__Iterator;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public bool ContainsStaticFire
		{
			get
			{
				if (Find.TickManager.TicksGame > this.lastStaticFireCheckTick + 1000)
				{
					this.lastStaticFireCheckResult = false;
					for (int i = 0; i < this.cells.Count; i++)
					{
						if (this.cells[i].ContainsStaticFire())
						{
							this.lastStaticFireCheckResult = true;
							break;
						}
					}
				}
				return this.lastStaticFireCheckResult;
			}
		}
		public Zone()
		{
		}
		public Zone(string baseName)
		{
			this.label = Find.ZoneManager.NewZoneName(baseName);
			this.color = ZoneColorUtility.NextZoneColor();
			Find.ZoneManager.RegisterZone(this);
		}
		[DebuggerHidden]
		public IEnumerator<IntVec3> GetEnumerator()
		{
			Zone.<GetEnumerator>c__Iterator82 <GetEnumerator>c__Iterator = new Zone.<GetEnumerator>c__Iterator82();
			<GetEnumerator>c__Iterator.<>f__this = this;
			return <GetEnumerator>c__Iterator;
		}
		public virtual void ExposeData()
		{
			Scribe_Values.LookValue<string>(ref this.label, "label", null, false);
			Scribe_Values.LookValue<Color>(ref this.color, "color", default(Color), false);
			Scribe_Values.LookValue<bool>(ref this.hidden, "hidden", false, false);
			Scribe_Collections.LookList<IntVec3>(ref this.cells, "cells", LookMode.Undefined, new object[0]);
		}
		public virtual void AddCell(IntVec3 c)
		{
			if (this.cells.Contains(c))
			{
				Log.Error(string.Concat(new object[]
				{
					"Adding cell to zone which already has it. c=",
					c,
					", zone=",
					this
				}));
				return;
			}
			List<Thing> list = Find.ThingGrid.ThingsListAt(c);
			for (int i = 0; i < list.Count; i++)
			{
				Thing thing = list[i];
				if (!thing.def.CanOverlapZones)
				{
					Log.Error("Added zone over zone-incompatible thing " + thing);
					return;
				}
			}
			this.cells.Add(c);
			Find.ZoneManager.AddZoneGridCell(this, c);
			Find.MapDrawer.MapMeshDirty(c, MapMeshFlag.Zone);
			AutoHomeAreaMaker.Notify_ZoneCellAdded(c);
			this.cellsShuffled = false;
		}
		public virtual void RemoveCell(IntVec3 c)
		{
			if (!this.cells.Contains(c))
			{
				Log.Error(string.Concat(new object[]
				{
					"Cannot remove cell from zone which doesn't have it. c=",
					c,
					", zone=",
					this
				}));
				return;
			}
			this.cells.Remove(c);
			Find.ZoneManager.ClearZoneGridCell(c);
			Find.MapDrawer.MapMeshDirty(c, MapMeshFlag.Zone);
			this.cellsShuffled = false;
			if (this.cells.Count == 0)
			{
				this.Deregister();
			}
		}
		public virtual void Delete()
		{
			if (this.cells.Count == 0)
			{
				this.Deregister();
			}
			else
			{
				while (this.cells.Count > 0)
				{
					this.RemoveCell(this.cells[this.cells.Count - 1]);
				}
			}
			Find.Selector.Deselect(this);
			SoundDefOf.DesignateZoneDelete.PlayOneShotOnCamera();
		}
		public virtual void Deregister()
		{
			Find.ZoneManager.DeregisterZone(this);
		}
		public bool ContainsCell(IntVec3 c)
		{
			for (int i = 0; i < this.cells.Count; i++)
			{
				if (this.cells[i] == c)
				{
					return true;
				}
			}
			return false;
		}
		public virtual string GetInspectString()
		{
			return string.Empty;
		}
		public abstract IEnumerable<ITab> GetInspectionTabs();
		[DebuggerHidden]
		public virtual IEnumerable<Gizmo> GetGizmos()
		{
			Zone.<GetGizmos>c__Iterator84 <GetGizmos>c__Iterator = new Zone.<GetGizmos>c__Iterator84();
			<GetGizmos>c__Iterator.<>f__this = this;
			Zone.<GetGizmos>c__Iterator84 expr_0E = <GetGizmos>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public void CheckContiguous()
		{
			if (this.cells.Count == 0)
			{
				return;
			}
			if (Zone.extantGrid.InnerArray.Length != Find.Map.Area)
			{
				Zone.extantGrid = new BoolGrid();
			}
			Zone.extantGrid.Clear();
			if (Zone.foundGrid.InnerArray.Length != Find.Map.Area)
			{
				Zone.foundGrid = new BoolGrid();
			}
			Zone.foundGrid.Clear();
			for (int i = 0; i < this.cells.Count; i++)
			{
				Zone.extantGrid.Set(this.cells[i], true);
			}
			Predicate<IntVec3> passCheck = (IntVec3 c) => Zone.extantGrid[c] && !Zone.foundGrid[c];
			int numFound = 0;
			Action<IntVec3> processor = delegate(IntVec3 c)
			{
				Zone.foundGrid.Set(c, true);
				numFound++;
			};
			FloodFiller.FloodFill(this.cells[0], passCheck, processor);
			if (numFound < this.cells.Count)
			{
				foreach (IntVec3 current in Find.Map.AllCells)
				{
					if (Zone.extantGrid[current] && !Zone.foundGrid[current])
					{
						this.RemoveCell(current);
					}
				}
			}
		}
		public override string ToString()
		{
			return this.label;
		}
	}
}
