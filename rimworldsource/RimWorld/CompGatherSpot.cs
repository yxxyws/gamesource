using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class CompGatherSpot : ThingComp
	{
		private bool active = true;
		public bool Active
		{
			get
			{
				return this.active;
			}
			set
			{
				if (value == this.active)
				{
					return;
				}
				this.active = value;
				if (this.active)
				{
					GatherSpotLister.RegisterActivated(this);
				}
				else
				{
					GatherSpotLister.RegisterDeactivated(this);
				}
			}
		}
		public override void PostExposeData()
		{
			Scribe_Values.LookValue<bool>(ref this.active, "active", false, false);
		}
		public override void PostSpawnSetup()
		{
			base.PostSpawnSetup();
			if (this.active)
			{
				GatherSpotLister.RegisterActivated(this);
			}
		}
		public override void PostDeSpawn()
		{
			base.PostDeSpawn();
			if (this.Active)
			{
				this.Active = false;
			}
		}
		[DebuggerHidden]
		public override IEnumerable<Command> CompGetGizmosExtra()
		{
			CompGatherSpot.<CompGetGizmosExtra>c__IteratorEF <CompGetGizmosExtra>c__IteratorEF = new CompGatherSpot.<CompGetGizmosExtra>c__IteratorEF();
			<CompGetGizmosExtra>c__IteratorEF.<>f__this = this;
			CompGatherSpot.<CompGetGizmosExtra>c__IteratorEF expr_0E = <CompGetGizmosExtra>c__IteratorEF;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
