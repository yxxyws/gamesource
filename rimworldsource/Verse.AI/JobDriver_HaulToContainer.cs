using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse.AI
{
	public class JobDriver_HaulToContainer : JobDriver
	{
		private const TargetIndex CarryThingIndex = TargetIndex.A;
		private const TargetIndex DestIndex = TargetIndex.B;
		private float uninstallWorkLeft;
		public override string GetReport()
		{
			Thing thing;
			if (this.pawn.carrier.CarriedThing != null)
			{
				thing = this.pawn.carrier.CarriedThing;
			}
			else
			{
				thing = base.TargetThingA;
			}
			return "ReportHaulingTo".Translate(new object[]
			{
				thing.LabelCap,
				base.CurJob.targetB.Thing.LabelBaseShort
			});
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<float>(ref this.uninstallWorkLeft, "uninstallWorkLeft", 0f, false);
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_HaulToContainer.<MakeNewToils>c__Iterator12E <MakeNewToils>c__Iterator12E = new JobDriver_HaulToContainer.<MakeNewToils>c__Iterator12E();
			<MakeNewToils>c__Iterator12E.<>f__this = this;
			JobDriver_HaulToContainer.<MakeNewToils>c__Iterator12E expr_0E = <MakeNewToils>c__Iterator12E;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		private Toil UninstallIfMinifiableToil()
		{
			Toil toil = new Toil().FailOnDestroyedNullOrForbidden(TargetIndex.A);
			toil.initAction = delegate
			{
				Thing thing = base.CurJob.GetTarget(TargetIndex.A).Thing;
				if (thing.def.Minifiable)
				{
					this.uninstallWorkLeft = 90f;
				}
				else
				{
					base.ReadyForNextToil();
				}
			};
			toil.tickAction = delegate
			{
				this.uninstallWorkLeft -= this.pawn.GetStatValue(StatDefOf.ConstructionSpeed, true);
				if (this.uninstallWorkLeft <= 0f)
				{
					Thing thing = base.CurJob.GetTarget(TargetIndex.A).Thing;
					MinifiedThing minifiedThing = thing.MakeMinified();
					GenSpawn.Spawn(minifiedThing, thing.Position);
					base.CurJob.SetTarget(TargetIndex.A, minifiedThing);
					base.ReadyForNextToil();
					return;
				}
			};
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			toil.WithProgressBar(TargetIndex.A, () => 1f - this.uninstallWorkLeft / 90f, false, -0.5f);
			return toil;
		}
	}
}
