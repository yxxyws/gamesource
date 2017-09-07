using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_FoodDeliver : JobDriver
	{
		private Pawn Deliveree
		{
			get
			{
				return (Pawn)base.CurJob.targetB.Thing;
			}
		}
		public override string GetReport()
		{
			return "DeliveringFood".Translate(new object[]
			{
				this.Deliveree.NameStringShort
			});
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_FoodDeliver.<MakeNewToils>c__Iterator3D <MakeNewToils>c__Iterator3D = new JobDriver_FoodDeliver.<MakeNewToils>c__Iterator3D();
			<MakeNewToils>c__Iterator3D.<>f__this = this;
			JobDriver_FoodDeliver.<MakeNewToils>c__Iterator3D expr_0E = <MakeNewToils>c__Iterator3D;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
