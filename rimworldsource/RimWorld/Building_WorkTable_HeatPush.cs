using System;
using Verse;
namespace RimWorld
{
	public class Building_WorkTable_HeatPush : Building_WorkTable
	{
		private const int HeatPushInterval = 30;
		public override void BillTick()
		{
			base.BillTick();
			if (Find.TickManager.TicksGame % 30 == 4)
			{
				GenTemperature.PushHeat(this, this.def.building.heatPerTickWhileWorking / 30f);
			}
		}
	}
}
