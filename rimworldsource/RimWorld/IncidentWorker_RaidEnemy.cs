using System;
using System.Linq;
using Verse;
namespace RimWorld
{
	public class IncidentWorker_RaidEnemy : IncidentWorker_Raid
	{
		public override bool TryExecute(IncidentParms parms)
		{
			if (base.TryExecute(parms))
			{
				Find.TickManager.slower.SignalForceNormalSpeedShort();
				Find.StoryWatcher.statsRecord.numRaidsEnemy++;
				return true;
			}
			return false;
		}
		protected override bool TryResolveRaidFaction(IncidentParms parms)
		{
			if (parms.faction != null)
			{
				return true;
			}
			float maxPoints = parms.points;
			if (maxPoints <= 0f)
			{
				maxPoints = 999999f;
			}
			return (
				from fac in Find.FactionManager.AllFactions
				where fac.HostileTo(Faction.OfColony) && maxPoints >= fac.def.MinPointsToGenerateNormalPawnGroup() && (float)GenDate.DaysPassed >= fac.def.earliestRaidDays
				select fac).TryRandomElementByWeight((Faction fac) => fac.def.raidCommonality, out parms.faction);
		}
		protected override void ResolveRaidStrategy(IncidentParms parms)
		{
			if (parms.raidStrategy != null)
			{
				return;
			}
			parms.raidStrategy = (
				from d in DefDatabase<RaidStrategyDef>.AllDefs
				where d.Worker.CanUseWith(parms)
				select d).RandomElementByWeight((RaidStrategyDef d) => d.selectionChance);
		}
		protected override string GetLetterLabel(IncidentParms parms)
		{
			return parms.raidStrategy.letterLabelEnemy;
		}
		protected override string GetLetterText(IncidentParms parms)
		{
			string str = null;
			switch (parms.raidArrivalMode)
			{
			case PawnsArriveMode.EdgeWalkIn:
				str = "EnemyRaidWalkIn".Translate(new object[]
				{
					parms.faction.def.pawnsPlural,
					parms.faction.name
				});
				break;
			case PawnsArriveMode.EdgeDrop:
				str = "EnemyRaidEdgeDrop".Translate(new object[]
				{
					parms.faction.def.pawnsPlural,
					parms.faction.name
				});
				break;
			case PawnsArriveMode.CenterDrop:
				str = "EnemyRaidCenterDrop".Translate(new object[]
				{
					parms.faction.def.pawnsPlural,
					parms.faction.name
				});
				break;
			}
			str += "\n\n";
			return str + parms.raidStrategy.arrivalTextEnemy;
		}
		protected override LetterType GetLetterType()
		{
			return LetterType.BadUrgent;
		}
		protected override string GetNewFamilyMembersLetterText(IncidentParms parms)
		{
			return "LetterFamilyMembersRaidEnemy".Translate(new object[]
			{
				parms.faction.def.pawnsPlural
			});
		}
	}
}
