using System;
using Verse;
namespace RimWorld
{
	public class LiquidFuel : Thing
	{
		private const int DryOutTime = 1500;
		private int spawnTick;
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<int>(ref this.spawnTick, "spawnTick", 0, false);
		}
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			this.spawnTick = Find.TickManager.TicksGame;
		}
		public void Refill()
		{
			this.spawnTick = Find.TickManager.TicksGame;
		}
		public override void Tick()
		{
			if (this.spawnTick + 1500 < Find.TickManager.TicksGame)
			{
				this.Destroy(DestroyMode.Vanish);
			}
		}
	}
}
