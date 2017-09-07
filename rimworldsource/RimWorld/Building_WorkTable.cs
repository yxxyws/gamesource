using System;
using System.Collections.Generic;
using Verse;
namespace RimWorld
{
	public class Building_WorkTable : Building, IBillGiver, IBillGiverWithTickAction
	{
		public BillStack billStack;
		private CompPowerTrader powerComp;
		private CompRefuelable refuelableComp;
		public bool CanWorkWithoutPower
		{
			get
			{
				return this.powerComp == null || this.def.building.unpoweredWorkTableWorkSpeedFactor > 0f;
			}
		}
		public virtual bool UsableNow
		{
			get
			{
				return (this.CanWorkWithoutPower || (this.powerComp != null && this.powerComp.PowerOn)) && (this.refuelableComp == null || this.refuelableComp.HasFuel);
			}
		}
		public BillStack BillStack
		{
			get
			{
				return this.billStack;
			}
		}
		public IntVec3 BillInteractionCell
		{
			get
			{
				return this.InteractionCell;
			}
		}
		public IEnumerable<IntVec3> IngredientStackCells
		{
			get
			{
				return GenAdj.CellsOccupiedBy(this);
			}
		}
		public Building_WorkTable()
		{
			this.billStack = new BillStack(this);
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.LookDeep<BillStack>(ref this.billStack, "billStack", new object[]
			{
				this
			});
		}
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			this.powerComp = base.GetComp<CompPowerTrader>();
			this.refuelableComp = base.GetComp<CompRefuelable>();
		}
		public virtual void BillTick()
		{
			if (this.refuelableComp != null)
			{
				this.refuelableComp.Notify_UsedThisTick();
			}
		}
		public bool CurrentlyUsable()
		{
			return this.UsableNow;
		}
	}
}
