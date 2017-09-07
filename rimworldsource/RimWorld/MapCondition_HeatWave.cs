using System;
namespace RimWorld
{
	public class MapCondition_HeatWave : MapCondition
	{
		private int LerpTicks = 12000;
		private float MaxTempOffset = 20f;
		public override float TemperatureOffset()
		{
			return MapConditionUtility.LerpInOutValue((float)base.TicksPassed, (float)base.TicksLeft, (float)this.LerpTicks, this.MaxTempOffset);
		}
	}
}
