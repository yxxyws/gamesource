using System;
namespace Verse
{
	public class HediffComp_Disappears : HediffComp
	{
		private int ticksToDisappear;
		public override bool CompShouldRemove
		{
			get
			{
				return base.CompShouldRemove || this.ticksToDisappear <= 0;
			}
		}
		public override string CompDebugString
		{
			get
			{
				return "ticksToDisappear: " + this.ticksToDisappear;
			}
		}
		public override void CompPostPostAdd()
		{
			base.CompPostPostAdd();
			this.ticksToDisappear = this.props.disappearsAfterTicks.RandomInRange;
		}
		public override void CompPostTick()
		{
			this.ticksToDisappear--;
		}
		public override void CompExposeData()
		{
			Scribe_Values.LookValue<int>(ref this.ticksToDisappear, "ticksToDisappear", 0, false);
		}
	}
}
