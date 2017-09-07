using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public abstract class WorkGiver_Scanner : WorkGiver
	{
		public virtual ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.Undefined);
			}
		}
		public virtual int LocalRegionsToScanFirst
		{
			get
			{
				return -1;
			}
		}
		public virtual bool Prioritized
		{
			get
			{
				return false;
			}
		}
		public virtual PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.Touch;
			}
		}
		[DebuggerHidden]
		public virtual IEnumerable<IntVec3> PotentialWorkCellsGlobal(Pawn pawn)
		{
			WorkGiver_Scanner.<PotentialWorkCellsGlobal>c__Iterator47 <PotentialWorkCellsGlobal>c__Iterator = new WorkGiver_Scanner.<PotentialWorkCellsGlobal>c__Iterator47();
			WorkGiver_Scanner.<PotentialWorkCellsGlobal>c__Iterator47 expr_07 = <PotentialWorkCellsGlobal>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}
		public virtual IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn Pawn)
		{
			return null;
		}
		public virtual bool HasJobOnThing(Pawn pawn, Thing t)
		{
			return this.JobOnThing(pawn, t) != null;
		}
		public virtual bool HasJobOnThingForced(Pawn pawn, Thing t)
		{
			return this.HasJobOnThing(pawn, t);
		}
		public virtual Job JobOnThing(Pawn pawn, Thing t)
		{
			return null;
		}
		public virtual Job JobOnThingForced(Pawn pawn, Thing t)
		{
			return this.JobOnThing(pawn, t);
		}
		public virtual bool HasJobOnCell(Pawn pawn, IntVec3 c)
		{
			return this.JobOnCell(pawn, c) != null;
		}
		public virtual Job JobOnCell(Pawn pawn, IntVec3 cell)
		{
			return null;
		}
		public virtual float GetPriority(Pawn pawn, Thing t)
		{
			return 0f;
		}
	}
}
