using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
namespace RimWorld
{
	public class IncidentWorker_NeutralGroup : IncidentWorker
	{
		protected IEnumerable<Faction> AllowedFactions
		{
			get
			{
				return 
					from fac in Find.FactionManager.AllFactionsVisible
					where this.IsFactionAllowed(fac)
					select fac;
			}
		}
		protected virtual bool IsFactionAllowed(Faction f)
		{
			return f.def != FactionDefOf.Colony && !f.HostileTo(Faction.OfColony);
		}
		protected virtual bool TryResolveParms(IncidentParms parms)
		{
			if (!parms.spawnCenter.IsValid)
			{
				RCellFinder.TryFindRandomPawnEntryCell(out parms.spawnCenter);
			}
			if (parms.faction == null && !this.AllowedFactions.TryRandomElement(out parms.faction))
			{
				parms.faction = null;
			}
			if (parms.faction == null)
			{
				return false;
			}
			if (parms.points < 0f)
			{
				float value = Rand.Value;
				if (value < 0.4f)
				{
					parms.points = (float)Rand.Range(40, 140);
				}
				else
				{
					if (value < 0.8f)
					{
						parms.points = (float)Rand.Range(140, 200);
					}
					else
					{
						parms.points = (float)Rand.Range(200, 500);
					}
				}
			}
			PawnGroupMakerUtility.AdjustPointsForGroupArrivalParams(parms);
			return true;
		}
		protected List<Pawn> SpawnPawns(IncidentParms parms)
		{
			List<Pawn> list = PawnGroupMakerUtility.GenerateArrivingPawns(parms, false).ToList<Pawn>();
			foreach (Pawn current in list)
			{
				IntVec3 loc = CellFinder.RandomClosewalkCellNear(parms.spawnCenter, 5);
				GenSpawn.Spawn(current, loc);
			}
			PawnRelationUtility.Notify_PawnsSeenByPlayer(list, "LetterFamilyMembersNeutralGroup".Translate(), true);
			return list;
		}
	}
}
