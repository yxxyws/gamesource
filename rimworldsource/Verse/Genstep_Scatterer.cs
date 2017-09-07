using System;
using System.Collections.Generic;
using UnityEngine;
namespace Verse
{
	public abstract class Genstep_Scatterer : Genstep
	{
		private const int ScatterNearPlayerRadius = 20;
		public int countAtPlayerStart;
		public FloatRange countPer10kCellsRange = FloatRange.Zero;
		public float minSpacing = 10f;
		public bool spotMustBeStandable;
		public int minDistToPlayerStart;
		public int minEdgeDist;
		public int extraNoBuildEdgeDist;
		public bool buildableAreaOnly;
		public bool warnOnFail = true;
		public List<ScattererValidator> validators = new List<ScattererValidator>();
		[Unsaved]
		protected HashSet<IntVec3> usedSpots = new HashSet<IntVec3>();
		public override void Generate()
		{
			for (int i = 0; i < this.countAtPlayerStart; i++)
			{
				IntVec3 intVec = CellFinder.RandomClosewalkCellNear(MapGenerator.PlayerStartSpot, 20);
				this.ScatterAt(intVec);
				this.usedSpots.Add(intVec);
			}
			int num = Mathf.RoundToInt(10000f / this.countPer10kCellsRange.RandomInRange);
			int num2 = Mathf.RoundToInt((float)Find.Map.Area / (float)num);
			for (int j = 0; j < num2; j++)
			{
				IntVec3 intVec2;
				if (!CellFinderLoose.TryFindRandomNotEdgeCellWith(5, new Predicate<IntVec3>(this.CanScatterAt), out intVec2))
				{
					if (this.warnOnFail)
					{
						Log.Warning("Scatterer " + this.ToString() + " could not find cell to generate at.");
					}
					return;
				}
				this.ScatterAt(intVec2);
				this.usedSpots.Add(intVec2);
			}
		}
		protected abstract void ScatterAt(IntVec3 loc);
		protected virtual bool CanScatterAt(IntVec3 loc)
		{
			if (this.buildableAreaOnly && loc.InNoBuildEdgeArea())
			{
				return false;
			}
			if (this.extraNoBuildEdgeDist > 0 && loc.CloseToEdge(this.extraNoBuildEdgeDist + 10))
			{
				return false;
			}
			if (this.minEdgeDist > 0 && loc.CloseToEdge(this.minEdgeDist))
			{
				return false;
			}
			foreach (IntVec3 current in this.usedSpots)
			{
				if ((current - loc).LengthHorizontal < this.minSpacing)
				{
					bool result = false;
					return result;
				}
			}
			if ((Find.Map.Center - loc).LengthHorizontalSquared < (float)(this.minDistToPlayerStart * this.minDistToPlayerStart))
			{
				return false;
			}
			if (this.spotMustBeStandable && !loc.Standable())
			{
				return false;
			}
			if (this.validators != null)
			{
				for (int i = 0; i < this.validators.Count; i++)
				{
					if (!this.validators[i].Allows(loc))
					{
						return false;
					}
				}
			}
			return true;
		}
		public void DebugForceScatterAt(IntVec3 loc)
		{
			this.ScatterAt(loc);
		}
	}
}
