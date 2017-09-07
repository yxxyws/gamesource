using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_ExtinguishSelf : JobDriver
	{
		protected const int NumSpeechesToSay = 5;
		protected Fire TargetFire
		{
			get
			{
				return (Fire)base.CurJob.targetA.Thing;
			}
		}
		public override string GetReport()
		{
			string result = "Extinguishing fire.";
			if (this.TargetFire.parent != null)
			{
				result = "Extinguishing fire on " + this.TargetFire.parent.LabelCap + ".";
			}
			return result;
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_ExtinguishSelf.<MakeNewToils>c__IteratorE <MakeNewToils>c__IteratorE = new JobDriver_ExtinguishSelf.<MakeNewToils>c__IteratorE();
			<MakeNewToils>c__IteratorE.<>f__this = this;
			JobDriver_ExtinguishSelf.<MakeNewToils>c__IteratorE expr_0E = <MakeNewToils>c__IteratorE;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
