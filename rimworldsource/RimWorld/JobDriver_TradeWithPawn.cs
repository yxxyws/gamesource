using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_TradeWithPawn : JobDriver
	{
		private Pawn Trader
		{
			get
			{
				return (Pawn)base.TargetThingA;
			}
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_TradeWithPawn.<MakeNewToils>c__Iterator33 <MakeNewToils>c__Iterator = new JobDriver_TradeWithPawn.<MakeNewToils>c__Iterator33();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_TradeWithPawn.<MakeNewToils>c__Iterator33 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
