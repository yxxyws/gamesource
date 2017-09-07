using System;
using Verse;
namespace RimWorld
{
	public class PawnRecentMemory : IExposable
	{
		private Pawn pawn;
		private int lastLightTick = 999999;
		private int lastOutdoorTick = 999999;
		public int TicksSinceLastLight
		{
			get
			{
				return Find.TickManager.TicksGame - this.lastLightTick;
			}
		}
		public int TicksSinceOutdoors
		{
			get
			{
				return Find.TickManager.TicksGame - this.lastOutdoorTick;
			}
		}
		public PawnRecentMemory(Pawn pawn)
		{
			this.pawn = pawn;
		}
		public void ExposeData()
		{
			Scribe_Values.LookValue<int>(ref this.lastLightTick, "lastLightTick", 0, false);
			Scribe_Values.LookValue<int>(ref this.lastOutdoorTick, "lastOutdoorTick", 0, false);
		}
		public void RecentMemoryInterval()
		{
			if (Find.GlowGrid.PsychGlowAt(this.pawn.Position) != PsychGlow.Dark)
			{
				this.lastLightTick = Find.TickManager.TicksGame;
			}
			Room room = this.pawn.Position.GetRoom();
			if (room != null && room.PsychologicallyOutdoors)
			{
				this.lastOutdoorTick = Find.TickManager.TicksGame;
			}
		}
	}
}
