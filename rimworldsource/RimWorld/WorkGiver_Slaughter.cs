using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class WorkGiver_Slaughter : WorkGiver_Scanner
	{
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.OnCell;
			}
		}
		[DebuggerHidden]
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			WorkGiver_Slaughter.<PotentialWorkThingsGlobal>c__Iterator49 <PotentialWorkThingsGlobal>c__Iterator = new WorkGiver_Slaughter.<PotentialWorkThingsGlobal>c__Iterator49();
			WorkGiver_Slaughter.<PotentialWorkThingsGlobal>c__Iterator49 expr_07 = <PotentialWorkThingsGlobal>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}
		public override bool HasJobOnThing(Pawn pawn, Thing t)
		{
			Pawn pawn2 = t as Pawn;
			if (pawn2 == null || !pawn2.RaceProps.Animal)
			{
				return false;
			}
			if (Find.DesignationManager.DesignationOn(t, DesignationDefOf.Slaughter) == null)
			{
				return false;
			}
			if (pawn2.InAggroMentalState)
			{
				return false;
			}
			if (!pawn.CanReserve(t, 1))
			{
				return false;
			}
			if (pawn.story != null && pawn.story.DisabledWorkTags.Contains(WorkTags.Violent))
			{
				JobFailReason.Is("IsIncapableOfViolenceShort".Translate());
				return false;
			}
			return true;
		}
		public override Job JobOnThing(Pawn pawn, Thing t)
		{
			return new Job(JobDefOf.Slaughter, t);
		}
	}
}
