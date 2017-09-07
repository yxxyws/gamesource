using System;
using System.Collections.Generic;
using Verse;
namespace RimWorld
{
	public class BreakdownManager : MapComponent
	{
		public const int CheckIntervalTicks = 1041;
		private List<CompBreakdownable> comps = new List<CompBreakdownable>();
		public void Register(CompBreakdownable c)
		{
			this.comps.Add(c);
		}
		public void Deregister(CompBreakdownable c)
		{
			this.comps.Remove(c);
		}
		public override void MapComponentTick()
		{
			if (Find.TickManager.TicksGame % 1041 == 0)
			{
				for (int i = 0; i < this.comps.Count; i++)
				{
					this.comps[i].CheckForBreakdown();
				}
			}
		}
	}
}
