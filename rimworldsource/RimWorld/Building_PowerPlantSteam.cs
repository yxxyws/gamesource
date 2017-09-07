using System;
using Verse;
namespace RimWorld
{
	public class Building_PowerPlantSteam : Building_PowerPlant
	{
		private IntermittentSteamSprayer steamSprayer;
		private SteamGeyser geyser;
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			this.steamSprayer = new IntermittentSteamSprayer(this);
		}
		public override void Tick()
		{
			base.Tick();
			if (this.geyser == null)
			{
				this.geyser = (SteamGeyser)Find.ThingGrid.ThingAt(base.Position, ThingDefOf.SteamGeyser);
			}
			if (this.geyser != null)
			{
				this.geyser.harvester = this;
				this.steamSprayer.SteamSprayerTick();
			}
		}
		public override void DeSpawn()
		{
			base.DeSpawn();
			if (this.geyser != null)
			{
				this.geyser.harvester = null;
			}
		}
	}
}
