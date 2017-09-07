using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_FoodFeedPatient : JobDriver
	{
		private const float FeedDurationMultiplier = 1.5f;
		protected Thing Food
		{
			get
			{
				return base.CurJob.targetA.Thing;
			}
		}
		protected Pawn Deliveree
		{
			get
			{
				return (Pawn)base.CurJob.targetB.Thing;
			}
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_FoodFeedPatient.<MakeNewToils>c__Iterator3E <MakeNewToils>c__Iterator3E = new JobDriver_FoodFeedPatient.<MakeNewToils>c__Iterator3E();
			<MakeNewToils>c__Iterator3E.<>f__this = this;
			JobDriver_FoodFeedPatient.<MakeNewToils>c__Iterator3E expr_0E = <MakeNewToils>c__Iterator3E;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
