using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse.AI
{
	public class JobDriver_AttackMelee : JobDriver
	{
		private int numMeleeAttacksLanded;
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<int>(ref this.numMeleeAttacksLanded, "numMeleeAttacksLanded", 0, false);
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_AttackMelee.<MakeNewToils>c__Iterator128 <MakeNewToils>c__Iterator = new JobDriver_AttackMelee.<MakeNewToils>c__Iterator128();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_AttackMelee.<MakeNewToils>c__Iterator128 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
