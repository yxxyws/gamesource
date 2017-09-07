using System;
using Verse;
namespace RimWorld
{
	public class Building_PowerPlant : Building
	{
		protected CompPowerTrader powerComp;
		protected CompRefuelable refuelableComp;
		protected CompBreakdownable breakdownableComp;
		protected CompFlickable flickableComp;
		protected virtual float DesiredPowerOutput
		{
			get
			{
				return -this.powerComp.Props.basePowerConsumption;
			}
		}
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			this.powerComp = base.GetComp<CompPowerTrader>();
			this.refuelableComp = base.GetComp<CompRefuelable>();
			this.breakdownableComp = base.GetComp<CompBreakdownable>();
			this.flickableComp = base.GetComp<CompFlickable>();
			if (!this.IsBrokenDown())
			{
				this.powerComp.PowerOn = true;
			}
		}
		public override void Tick()
		{
			base.Tick();
			if ((this.breakdownableComp != null && this.breakdownableComp.BrokenDown) || (this.refuelableComp != null && !this.refuelableComp.HasFuel) || (this.flickableComp != null && !this.flickableComp.SwitchIsOn) || (this.powerComp != null && !this.powerComp.PowerOn))
			{
				this.powerComp.PowerOutput = 0f;
			}
			else
			{
				this.powerComp.PowerOutput = this.DesiredPowerOutput;
			}
		}
	}
}
