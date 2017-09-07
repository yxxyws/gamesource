using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_PlantCut : JobDriver_PlantWork
	{
		protected override void Init()
		{
			if (base.Plant.def.plant.harvestedThingDef != null && base.Plant.YieldNow() > 0)
			{
				this.xpPerTick = 0.154f;
			}
			else
			{
				this.xpPerTick = 0f;
			}
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_PlantCut.<MakeNewToils>c__Iterator3A <MakeNewToils>c__Iterator3A = new JobDriver_PlantCut.<MakeNewToils>c__Iterator3A();
			<MakeNewToils>c__Iterator3A.<>f__this = this;
			JobDriver_PlantCut.<MakeNewToils>c__Iterator3A expr_0E = <MakeNewToils>c__Iterator3A;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
