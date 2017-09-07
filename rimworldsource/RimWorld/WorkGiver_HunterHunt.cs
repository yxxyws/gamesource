using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class WorkGiver_HunterHunt : WorkGiver_Scanner
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
			WorkGiver_HunterHunt.<PotentialWorkThingsGlobal>c__Iterator4F <PotentialWorkThingsGlobal>c__Iterator4F = new WorkGiver_HunterHunt.<PotentialWorkThingsGlobal>c__Iterator4F();
			WorkGiver_HunterHunt.<PotentialWorkThingsGlobal>c__Iterator4F expr_07 = <PotentialWorkThingsGlobal>c__Iterator4F;
			expr_07.$PC = -2;
			return expr_07;
		}
		public override bool ShouldSkip(Pawn pawn)
		{
			return !WorkGiver_HunterHunt.HasHuntingWeapon(pawn) || WorkGiver_HunterHunt.HasShieldAndRangedWeapon(pawn);
		}
		public override bool HasJobOnThing(Pawn pawn, Thing t)
		{
			Pawn pawn2 = t as Pawn;
			return pawn2 != null && pawn2.RaceProps.Animal && pawn.CanReserve(t, 1) && Find.DesignationManager.DesignationOn(t, DesignationDefOf.Hunt) != null;
		}
		public override Job JobOnThing(Pawn pawn, Thing t)
		{
			return new Job(JobDefOf.Hunt, t);
		}
		public static bool HasHuntingWeapon(Pawn p)
		{
			if (p.equipment.Primary != null)
			{
				return true;
			}
			List<Hediff> hediffs = p.health.hediffSet.hediffs;
			for (int i = 0; i < hediffs.Count; i++)
			{
				if (hediffs[i].def.addedPartProps != null && hediffs[i].def.addedPartProps.isGoodWeapon)
				{
					return true;
				}
			}
			return false;
		}
		public static bool HasShieldAndRangedWeapon(Pawn p)
		{
			if (p.equipment.Primary != null && !p.equipment.Primary.def.Verbs[0].MeleeRange)
			{
				List<Apparel> wornApparel = p.apparel.WornApparel;
				for (int i = 0; i < wornApparel.Count; i++)
				{
					if (wornApparel[i] is PersonalShield)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
