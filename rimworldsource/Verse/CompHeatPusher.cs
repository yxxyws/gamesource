using System;
namespace Verse
{
	public class CompHeatPusher : ThingComp
	{
		private const int HeatPushInterval = 60;
		public CompProperties_HeatPusher Props
		{
			get
			{
				return (CompProperties_HeatPusher)this.props;
			}
		}
		protected virtual bool ShouldPushHeatNow
		{
			get
			{
				return true;
			}
		}
		public override void CompTick()
		{
			base.CompTick();
			if (this.parent.IsHashIntervalTick(60) && this.ShouldPushHeatNow)
			{
				CompProperties_HeatPusher props = this.Props;
				if (this.parent.Position.GetTemperature() < props.heatPushMaxTemperature)
				{
					GenTemperature.PushHeat(this.parent.Position, props.heatPerSecond);
				}
			}
		}
	}
}
