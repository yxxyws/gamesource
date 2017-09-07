using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;
namespace RimWorld
{
	public class IncidentWorker_TraderCaravanArrival : IncidentWorker_NeutralGroup
	{
		public override float AdjustedChance
		{
			get
			{
				List<Faction> allFactionsListForReading = Find.FactionManager.AllFactionsListForReading;
				int num = 0;
				for (int i = 0; i < allFactionsListForReading.Count; i++)
				{
					if (allFactionsListForReading[i].def.caravanTraderKinds.Any<TraderKindDef>() && !allFactionsListForReading[i].HostileTo(Faction.OfColony))
					{
						num++;
					}
				}
				if (num == 0)
				{
					return 0f;
				}
				return this.def.baseChance;
			}
		}
		protected override bool StorytellerCanUseNowSub()
		{
			return base.StorytellerCanUseNowSub() && base.AllowedFactions.Any<Faction>();
		}
		public override bool TryExecute(IncidentParms parms)
		{
			if (!this.TryResolveParms(parms))
			{
				return false;
			}
			List<Pawn> list = base.SpawnPawns(parms);
			if (list.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].needs != null && list[i].needs.food != null)
				{
					list[i].needs.food.CurLevel = list[i].needs.food.MaxLevel;
				}
			}
			Find.LetterStack.ReceiveLetter("LetterLabelTraderCaravanArrival".Translate(new object[]
			{
				parms.faction.name
			}), "LetterTraderCaravanArrival".Translate(new object[]
			{
				parms.faction.name
			}), LetterType.Good, list[0], null);
			IntVec3 chillSpot;
			RCellFinder.TryFindRandomSpotJustOutsideColony(list[0], out chillSpot);
			LordJob_TradeWithColony lordJob = new LordJob_TradeWithColony(parms.faction, chillSpot);
			LordMaker.MakeNewLord(parms.faction, lordJob, list);
			return true;
		}
		protected override bool IsFactionAllowed(Faction f)
		{
			return base.IsFactionAllowed(f) && f.def.caravanTraderKinds.Any<TraderKindDef>();
		}
		protected override bool TryResolveParms(IncidentParms parms)
		{
			if (!base.TryResolveParms(parms))
			{
				return false;
			}
			parms.traderCaravan = true;
			return true;
		}
	}
}
