using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_RelaxAlone : JobDriver
	{
		private Rot4 faceDir;
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_RelaxAlone.<MakeNewToils>c__Iterator21 <MakeNewToils>c__Iterator = new JobDriver_RelaxAlone.<MakeNewToils>c__Iterator21();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_RelaxAlone.<MakeNewToils>c__Iterator21 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<Rot4>(ref this.faceDir, "faceDir", default(Rot4), false);
		}
	}
}
