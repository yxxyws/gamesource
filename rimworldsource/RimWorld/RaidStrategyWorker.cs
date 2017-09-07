using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;
namespace RimWorld
{
	public abstract class RaidStrategyWorker
	{
		public RaidStrategyDef def;
		public virtual bool CanUseWith(IncidentParms parms)
		{
			return (float)GenDate.DaysPassed >= this.def.minDaysPassed && parms.points >= this.MinimumPoints(parms.faction);
		}
		public virtual float MinimumPoints(Faction faction)
		{
			return faction.def.MinPointsToGenerateNormalPawnGroup();
		}
		public virtual float MinMaxAllowedPawnGenOptionCost(Faction faction)
		{
			return 0f;
		}
		public virtual Lord MakeLord(IncidentParms parms, List<Pawn> pawns)
		{
			Lord lord = LordMaker.MakeNewLord(parms.faction, this.MakeLordJob(ref parms), pawns);
			lord.avoidGrid = AvoidGridMaker.MakeAvoidGrid(parms, false);
			return lord;
		}
		public abstract LordJob MakeLordJob(ref IncidentParms parms);
		public virtual bool CanUsePawnGenOption(PawnGenOption g, List<PawnGenOption> chosenGroups)
		{
			return true;
		}
	}
}
