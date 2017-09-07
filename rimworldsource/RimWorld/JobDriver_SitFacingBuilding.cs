using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_SitFacingBuilding : JobDriver
	{
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_SitFacingBuilding.<MakeNewToils>c__Iterator22 <MakeNewToils>c__Iterator = new JobDriver_SitFacingBuilding.<MakeNewToils>c__Iterator22();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_SitFacingBuilding.<MakeNewToils>c__Iterator22 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
