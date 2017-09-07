using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class WorkGiver_Strip : WorkGiver_Scanner
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
			WorkGiver_Strip.<PotentialWorkThingsGlobal>c__Iterator54 <PotentialWorkThingsGlobal>c__Iterator = new WorkGiver_Strip.<PotentialWorkThingsGlobal>c__Iterator54();
			WorkGiver_Strip.<PotentialWorkThingsGlobal>c__Iterator54 expr_07 = <PotentialWorkThingsGlobal>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}
		public override bool HasJobOnThing(Pawn pawn, Thing t)
		{
			return Find.DesignationManager.DesignationOn(t, DesignationDefOf.Strip) != null && pawn.CanReserve(t, 1) && StrippableUtility.CanBeStrippedByColony(t);
		}
		public override Job JobOnThing(Pawn pawn, Thing t)
		{
			return new Job(JobDefOf.Strip, t);
		}
	}
}
