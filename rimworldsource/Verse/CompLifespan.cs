using RimWorld;
using System;
namespace Verse
{
	public class CompLifespan : ThingComp
	{
		public int age = -1;
		public CompProperties_Lifespan Props
		{
			get
			{
				return (CompProperties_Lifespan)this.props;
			}
		}
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.LookValue<int>(ref this.age, "age", 0, false);
		}
		public override void CompTick()
		{
			this.age++;
			if (this.age >= this.Props.lifespanTicks)
			{
				this.parent.Destroy(DestroyMode.Vanish);
			}
		}
		public override string CompInspectStringExtra()
		{
			string text = base.CompInspectStringExtra();
			int num = this.Props.lifespanTicks - this.age;
			if (num > 0)
			{
				text = string.Concat(new string[]
				{
					"LifespanExpiry".Translate(),
					" ",
					num.TicksToPeriodString(true),
					"\n",
					text
				});
			}
			return text;
		}
	}
}
