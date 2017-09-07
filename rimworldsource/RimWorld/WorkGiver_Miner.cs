using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class WorkGiver_Miner : WorkGiver_Scanner
	{
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
			WorkGiver_Miner.<PotentialWorkThingsGlobal>c__Iterator50 <PotentialWorkThingsGlobal>c__Iterator = new WorkGiver_Miner.<PotentialWorkThingsGlobal>c__Iterator50();
			WorkGiver_Miner.<PotentialWorkThingsGlobal>c__Iterator50 expr_07 = <PotentialWorkThingsGlobal>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}
		public override Job JobOnThing(Pawn pawn, Thing t)
		{
			if (!t.def.mineable)
			{
				return null;
			}
			if (Find.DesignationManager.DesignationAt(t.Position, DesignationDefOf.Mine) == null)
			{
				return null;
			}
			if (!pawn.CanReserve(t, 1))
			{
				return null;
			}
			bool flag = false;
			for (int i = 0; i < 8; i++)
			{
				IntVec3 c = t.Position + GenAdj.AdjacentCells[i];
				if (c.InBounds() && c.Standable())
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				for (int j = 0; j < 8; j++)
				{
					IntVec3 c2 = t.Position + GenAdj.AdjacentCells[j];
					if (c2.InBounds())
					{
						if (c2.Walkable() && !c2.Standable())
						{
							Thing firstHaulable = c2.GetFirstHaulable();
							if (firstHaulable != null && firstHaulable.def.passability == Traversability.PassThroughOnly)
							{
								return HaulAIUtility.HaulAsideJobFor(pawn, firstHaulable);
							}
						}
					}
				}
				return null;
			}
			return new Job(JobDefOf.Mine, t, 1500, true);
		}
	}
}
