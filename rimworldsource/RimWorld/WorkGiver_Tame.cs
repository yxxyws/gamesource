using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class WorkGiver_Tame : WorkGiver_InteractAnimal
	{
		public const int MinTameInterval = 30000;
		[DebuggerHidden]
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			WorkGiver_Tame.<PotentialWorkThingsGlobal>c__Iterator4A <PotentialWorkThingsGlobal>c__Iterator4A = new WorkGiver_Tame.<PotentialWorkThingsGlobal>c__Iterator4A();
			WorkGiver_Tame.<PotentialWorkThingsGlobal>c__Iterator4A expr_07 = <PotentialWorkThingsGlobal>c__Iterator4A;
			expr_07.$PC = -2;
			return expr_07;
		}
		public override Job JobOnThing(Pawn pawn, Thing t)
		{
			Pawn pawn2 = t as Pawn;
			if (pawn2 == null || pawn2.RaceProps.Humanlike)
			{
				return null;
			}
			if (Find.DesignationManager.DesignationOn(t, DesignationDefOf.Tame) == null)
			{
				return null;
			}
			if (Find.TickManager.TicksGame < pawn2.mindState.lastAssignedInteractTime + 30000)
			{
				JobFailReason.Is(WorkGiver_InteractAnimal.AnimalInteractedTooRecentlyTrans);
				return null;
			}
			if (!this.CanInteractWithAnimal(pawn, pawn2))
			{
				return null;
			}
			if (pawn2.RaceProps.EatsFood && !base.HasFoodToInteractAnimal(pawn, pawn2))
			{
				return base.TakeFoodForAnimalInteractJob(pawn, pawn2);
			}
			return new Job(JobDefOf.Tame, t);
		}
	}
}
