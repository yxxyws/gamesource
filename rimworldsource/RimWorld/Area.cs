using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public abstract class Area : ILoadReferenceable, IExposable
	{
		private BoolGrid innerGrid = new BoolGrid();
		private AreaDrawer drawer;
		private Texture2D colorTextureInt;
		public int TrueCount
		{
			get
			{
				return this.innerGrid.TrueCount;
			}
		}
		public abstract string Label
		{
			get;
		}
		public abstract Color Color
		{
			get;
		}
		public abstract int ListPriority
		{
			get;
		}
		public Texture2D ColorTexture
		{
			get
			{
				if (this.colorTextureInt == null)
				{
					this.colorTextureInt = SolidColorMaterials.NewSolidColorTexture(this.Color);
				}
				return this.colorTextureInt;
			}
		}
		public bool this[int index]
		{
			get
			{
				return this.innerGrid[index];
			}
		}
		public bool this[IntVec3 c]
		{
			get
			{
				return this.innerGrid[CellIndices.CellToIndex(c)];
			}
		}
		private AreaDrawer Drawer
		{
			get
			{
				if (this.drawer == null)
				{
					this.drawer = new AreaDrawer(this);
				}
				return this.drawer;
			}
		}
		public IEnumerable<IntVec3> ActiveCells
		{
			get
			{
				return this.innerGrid.ActiveCells;
			}
		}
		public virtual bool Mutable
		{
			get
			{
				return false;
			}
		}
		public virtual void ExposeData()
		{
			Scribe_Deep.LookDeep<BoolGrid>(ref this.innerGrid, "innerGrid", new object[0]);
		}
		public virtual bool AssignableAsAllowed(AllowedAreaMode mode)
		{
			return false;
		}
		public virtual void SetLabel(string label)
		{
			throw new NotImplementedException();
		}
		public void Set(IntVec3 c)
		{
			if (this.innerGrid[c])
			{
				return;
			}
			this.innerGrid[c] = true;
			this.SetDirty(c);
		}
		public void Clear(IntVec3 c)
		{
			if (!this.innerGrid[c])
			{
				return;
			}
			this.innerGrid[c] = false;
			this.SetDirty(c);
		}
		private void SetDirty(IntVec3 c)
		{
			this.Drawer.SetDirty();
			Region region = c.GetRegion();
			if (region != null)
			{
				region.Notify_AreaChanged(this);
			}
		}
		public void Delete()
		{
			Find.AreaManager.Remove(this);
		}
		public void MarkForDraw()
		{
			this.Drawer.MarkForDraw();
		}
		public void AreaUpdate()
		{
			this.Drawer.AreaDrawerUpdate();
		}
		public void Invert()
		{
			this.innerGrid.Invert();
			this.Drawer.SetDirty();
		}
		public abstract string GetUniqueLoadID();
	}
}
