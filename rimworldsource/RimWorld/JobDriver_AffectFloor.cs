using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public abstract class JobDriver_AffectFloor : JobDriver
	{
		private float workLeft = -1000f;
		protected abstract int BaseWorkAmount
		{
			get;
		}
		protected abstract DesignationDef DesDef
		{
			get;
		}
		protected virtual StatDef SpeedStat
		{
			get
			{
				return null;
			}
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_AffectFloor.<MakeNewToils>c__Iterator7 <MakeNewToils>c__Iterator = new JobDriver_AffectFloor.<MakeNewToils>c__Iterator7();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_AffectFloor.<MakeNewToils>c__Iterator7 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		protected abstract void DoEffect(IntVec3 c);
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<float>(ref this.workLeft, "workLeft", 0f, false);
		}
	}
}
