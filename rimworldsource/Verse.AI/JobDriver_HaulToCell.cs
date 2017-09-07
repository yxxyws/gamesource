using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse.AI
{
	public class JobDriver_HaulToCell : JobDriver
	{
		private const TargetIndex HaulableInd = TargetIndex.A;
		private const TargetIndex StoreCellInd = TargetIndex.B;
		public override string GetReport()
		{
			IntVec3 cell = this.pawn.jobs.curJob.targetB.Cell;
			Thing thing;
			if (this.pawn.carrier.CarriedThing != null)
			{
				thing = this.pawn.carrier.CarriedThing;
			}
			else
			{
				thing = base.TargetThingA;
			}
			string text = null;
			SlotGroup slotGroup = cell.GetSlotGroup();
			if (slotGroup != null)
			{
				text = slotGroup.parent.SlotYielderLabel();
			}
			string result;
			if (text != null)
			{
				result = "ReportHaulingTo".Translate(new object[]
				{
					thing.LabelCap,
					text
				});
			}
			else
			{
				result = "ReportHauling".Translate(new object[]
				{
					thing.LabelCap
				});
			}
			return result;
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_HaulToCell.<MakeNewToils>c__Iterator12D <MakeNewToils>c__Iterator12D = new JobDriver_HaulToCell.<MakeNewToils>c__Iterator12D();
			<MakeNewToils>c__Iterator12D.<>f__this = this;
			JobDriver_HaulToCell.<MakeNewToils>c__Iterator12D expr_0E = <MakeNewToils>c__Iterator12D;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
