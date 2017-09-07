using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class IncidentWorker_RaidFriendly : IncidentWorker_Raid
	{
		protected override bool StorytellerCanUseNowSub()
		{
			return (
				from p in Find.AttackTargetsCache.TargetsHostileToColony
				where !p.ThreatDisabled()
				select p).Sum(delegate(IAttackTarget p)
			{
				Pawn pawn = p as Pawn;
				if (pawn != null)
				{
					return pawn.kindDef.combatPower;
				}
				return 0f;
			}) > 120f;
		}
		protected override bool TryResolveRaidFaction(IncidentParms parms)
		{
			if (parms.faction != null)
			{
				return true;
			}
			IEnumerable<Faction> hostileFactions = (
				from p in Find.AttackTargetsCache.TargetsHostileToColony
				select ((Thing)p).Faction).Distinct<Faction>();
			if (!hostileFactions.Any<Faction>())
			{
				hostileFactions = 
					from fac in Find.FactionManager.AllFactions
					where fac.HostileTo(Faction.OfColony)
					select fac;
			}
			IEnumerable<Faction> source = 
				from fac in Find.FactionManager.AllFactions
				where fac.def != FactionDefOf.Colony && !fac.def.hidden && fac.GoodwillWith(Faction.OfColony) > 0f && hostileFactions.Any((Faction hosFac) => hosFac.HostileTo(fac))
				select fac;
			if (!source.Any<Faction>())
			{
				source = 
					from fac in Find.FactionManager.AllFactions
					where fac.def != FactionDefOf.Colony && !fac.def.hidden && !fac.HostileTo(Faction.OfColony) && hostileFactions.Any((Faction hosFac) => hosFac.HostileTo(fac))
					select fac;
			}
			if (!source.Any<Faction>())
			{
				return false;
			}
			parms.faction = source.RandomElementByWeight((Faction fac) => fac.ColonyGoodwill + 120.000008f);
			return true;
		}
		protected override void ResolveRaidStrategy(IncidentParms parms)
		{
			if (parms.raidStrategy != null)
			{
				return;
			}
			parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
		}
		protected override string GetLetterLabel(IncidentParms parms)
		{
			return parms.raidStrategy.letterLabelFriendly;
		}
		protected override string GetLetterText(IncidentParms parms)
		{
			string str = null;
			switch (parms.raidArrivalMode)
			{
			case PawnsArriveMode.EdgeWalkIn:
				str = "FriendlyRaidWalkIn".Translate(new object[]
				{
					parms.faction.def.pawnsPlural,
					parms.faction.name
				});
				break;
			case PawnsArriveMode.EdgeDrop:
				str = "FriendlyRaidEdgeDrop".Translate(new object[]
				{
					parms.faction.def.pawnsPlural,
					parms.faction.name
				});
				break;
			case PawnsArriveMode.CenterDrop:
				str = "FriendlyRaidCenterDrop".Translate(new object[]
				{
					parms.faction.def.pawnsPlural,
					parms.faction.name
				});
				break;
			}
			str += "\n\n";
			return str + parms.raidStrategy.arrivalTextFriendly;
		}
		protected override LetterType GetLetterType()
		{
			return LetterType.BadNonUrgent;
		}
		protected override string GetNewFamilyMembersLetterText(IncidentParms parms)
		{
			return "LetterFamilyMembersRaidFriendly".Translate(new object[]
			{
				parms.faction.def.pawnsPlural
			});
		}
	}
}
