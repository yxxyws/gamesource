using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_MarryAdjacentPawn : JobDriver
	{
		private const TargetIndex OtherFianceInd = TargetIndex.A;
		private const int Duration = 2500;
		private int ticksLeftToMarry = 2500;
		private Pawn OtherFiance
		{
			get
			{
				return (Pawn)base.CurJob.GetTarget(TargetIndex.A).Thing;
			}
		}
		public int TicksLeftToMarry
		{
			get
			{
				return this.ticksLeftToMarry;
			}
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_MarryAdjacentPawn.<MakeNewToils>c__IteratorF <MakeNewToils>c__IteratorF = new JobDriver_MarryAdjacentPawn.<MakeNewToils>c__IteratorF();
			<MakeNewToils>c__IteratorF.<>f__this = this;
			JobDriver_MarryAdjacentPawn.<MakeNewToils>c__IteratorF expr_0E = <MakeNewToils>c__IteratorF;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<int>(ref this.ticksLeftToMarry, "ticksLeftToMarry", 0, false);
		}
	}
}
