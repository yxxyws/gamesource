using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse.AI
{
	public class JobDriver_AttackStatic : JobDriver
	{
		private bool startedIncapacitated;
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<bool>(ref this.startedIncapacitated, "startedIncapacitated", false, false);
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_AttackStatic.<MakeNewToils>c__Iterator129 <MakeNewToils>c__Iterator = new JobDriver_AttackStatic.<MakeNewToils>c__Iterator129();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_AttackStatic.<MakeNewToils>c__Iterator129 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
