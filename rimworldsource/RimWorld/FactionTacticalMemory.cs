using System;
using System.Collections.Generic;
using Verse;
namespace RimWorld
{
	public class FactionTacticalMemory : IExposable
	{
		private const float TrapRememberChance = 0.2f;
		private List<TrapMemory> traps = new List<TrapMemory>();
		public void ExposeData()
		{
			Scribe_Collections.LookList<TrapMemory>(ref this.traps, "traps", LookMode.Undefined, new object[0]);
		}
		public List<TrapMemory> TrapMemories()
		{
			this.traps.RemoveAll((TrapMemory tl) => tl.Expired);
			return this.traps;
		}
		public void TrapRevealed(IntVec3 c)
		{
			if (Rand.Value < 0.2f)
			{
				this.traps.Add(new TrapMemory(c, Find.TickManager.TicksGame));
			}
		}
	}
}
