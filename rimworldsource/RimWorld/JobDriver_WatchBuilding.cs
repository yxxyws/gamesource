using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_WatchBuilding : JobDriver
	{
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_WatchBuilding.<MakeNewToils>c__Iterator20 <MakeNewToils>c__Iterator = new JobDriver_WatchBuilding.<MakeNewToils>c__Iterator20();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_WatchBuilding.<MakeNewToils>c__Iterator20 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		protected virtual void WatchTickAction()
		{
			this.pawn.Drawer.rotator.FaceCell(base.TargetA.Cell);
			this.pawn.GainComfortFromCellIfPossible();
			float statValue = base.TargetThingA.GetStatValue(StatDefOf.EntertainmentStrengthFactor, true);
			float extraJoyGainFactor = statValue;
			JoyUtility.JoyTickCheckEnd(this.pawn, JoyTickFullJoyAction.EndJob, extraJoyGainFactor);
		}
	}
}
