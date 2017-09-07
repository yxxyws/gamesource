using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class WorkGiver_Open : WorkGiver_Scanner
	{
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.ClosestTouch;
			}
		}
		[DebuggerHidden]
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			WorkGiver_Open.<PotentialWorkThingsGlobal>c__Iterator51 <PotentialWorkThingsGlobal>c__Iterator = new WorkGiver_Open.<PotentialWorkThingsGlobal>c__Iterator51();
			WorkGiver_Open.<PotentialWorkThingsGlobal>c__Iterator51 expr_07 = <PotentialWorkThingsGlobal>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}
		public override bool HasJobOnThing(Pawn pawn, Thing t)
		{
			return Find.DesignationManager.DesignationOn(t, DesignationDefOf.Open) != null && pawn.CanReserve(t, 1);
		}
		public override Job JobOnThing(Pawn pawn, Thing t)
		{
			return new Job(JobDefOf.Open, t);
		}
	}
}
