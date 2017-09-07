using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_Kidnap : JobDriver
	{
		protected Pawn Takee
		{
			get
			{
				return (Pawn)base.CurJob.targetA.Thing;
			}
		}
		public override string GetReport()
		{
			if (this.pawn.HostileTo(this.Takee))
			{
				return base.GetReport();
			}
			return JobDefOf.Rescue.reportString.Replace("TargetA", this.Takee.LabelBaseShort);
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_Kidnap.<MakeNewToils>c__Iterator2C <MakeNewToils>c__Iterator2C = new JobDriver_Kidnap.<MakeNewToils>c__Iterator2C();
			<MakeNewToils>c__Iterator2C.<>f__this = this;
			JobDriver_Kidnap.<MakeNewToils>c__Iterator2C expr_0E = <MakeNewToils>c__Iterator2C;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
