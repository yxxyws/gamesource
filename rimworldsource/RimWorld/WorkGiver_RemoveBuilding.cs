using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public abstract class WorkGiver_RemoveBuilding : WorkGiver_Scanner
	{
		protected abstract DesignationDef Designation
		{
			get;
		}
		protected abstract JobDef RemoveBuildingJob
		{
			get;
		}
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.Touch;
			}
		}
		[DebuggerHidden]
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			WorkGiver_RemoveBuilding.<PotentialWorkThingsGlobal>c__Iterator4D <PotentialWorkThingsGlobal>c__Iterator4D = new WorkGiver_RemoveBuilding.<PotentialWorkThingsGlobal>c__Iterator4D();
			<PotentialWorkThingsGlobal>c__Iterator4D.<>f__this = this;
			WorkGiver_RemoveBuilding.<PotentialWorkThingsGlobal>c__Iterator4D expr_0E = <PotentialWorkThingsGlobal>c__Iterator4D;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override bool HasJobOnThing(Pawn pawn, Thing t)
		{
			return t.Faction == pawn.Faction && pawn.CanReserve(t, 1) && Find.DesignationManager.DesignationOn(t, this.Designation) != null;
		}
		public override Job JobOnThing(Pawn pawn, Thing t)
		{
			return new Job(this.RemoveBuildingJob, t);
		}
	}
}
