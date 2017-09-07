using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class WorkGiver_Flick : WorkGiver_Scanner
	{
		[DebuggerHidden]
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn Pawn)
		{
			WorkGiver_Flick.<PotentialWorkThingsGlobal>c__Iterator55 <PotentialWorkThingsGlobal>c__Iterator = new WorkGiver_Flick.<PotentialWorkThingsGlobal>c__Iterator55();
			WorkGiver_Flick.<PotentialWorkThingsGlobal>c__Iterator55 expr_07 = <PotentialWorkThingsGlobal>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}
		public override bool HasJobOnThing(Pawn pawn, Thing t)
		{
			return Find.DesignationManager.DesignationOn(t, DesignationDefOf.Flick) != null && pawn.CanReserveAndReach(t, PathEndMode.Touch, pawn.NormalMaxDanger(), 1);
		}
		public override Job JobOnThing(Pawn pawn, Thing t)
		{
			return new Job(JobDefOf.Flick, t);
		}
	}
}
