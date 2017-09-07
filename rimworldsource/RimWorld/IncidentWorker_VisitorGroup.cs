using System;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;
namespace RimWorld
{
	public class IncidentWorker_VisitorGroup : IncidentWorker_NeutralGroup
	{
		private const float TraderChance = 0.8f;
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
			IntVec3 chillSpot;
			RCellFinder.TryFindRandomSpotJustOutsideColony(list[0], out chillSpot);
			LordJob_VisitColony lordJob = new LordJob_VisitColony(parms.faction, chillSpot);
			LordMaker.MakeNewLord(parms.faction, lordJob, list);
			bool flag = false;
			if (Rand.Value < 0.8f)
			{
				flag = this.TryConvertOnePawnToSmallTrader(list, parms.faction);
			}
			string label;
			string text2;
			if (list.Count == 1)
			{
				string text = (!flag) ? string.Empty : "SingleVisitorArrivesTraderInfo".Translate();
				label = "LetterLabelSingleVisitorArrives".Translate();
				text2 = "SingleVisitorArrives".Translate(new object[]
				{
					list[0].story.adulthood.title.ToLower(),
					parms.faction.name,
					list[0].Name,
					text
				});
				text2 = text2.AdjustedFor(list[0]);
			}
			else
			{
				string text3 = (!flag) ? string.Empty : "GroupVisitorsArriveTraderInfo".Translate();
				label = "LetterLabelGroupVisitorsArrive".Translate();
				text2 = "GroupVisitorsArrive".Translate(new object[]
				{
					parms.faction.name,
					text3
				});
			}
			Find.LetterStack.ReceiveLetter(label, text2, LetterType.Good, list[0], null);
			return true;
		}
		private bool TryConvertOnePawnToSmallTrader(List<Pawn> pawns, Faction faction)
		{
			if (faction.def.visitorTraderKinds.NullOrEmpty<TraderKindDef>())
			{
				return false;
			}
			Pawn pawn = pawns.RandomElement<Pawn>();
			Lord lord = pawn.GetLord();
			pawn.mindState.wantsToTradeWithColony = true;
			PawnComponentsUtility.AddAndRemoveDynamicComponents(pawn, true);
			TraderKindDef traderKindDef = faction.def.visitorTraderKinds.RandomElement<TraderKindDef>();
			pawn.trader.traderKind = traderKindDef;
			pawn.inventory.DestroyAll(DestroyMode.Vanish);
			foreach (Thing current in TraderStockGenerator.GenerateTraderThings(traderKindDef))
			{
				Pawn pawn2 = current as Pawn;
				if (pawn2 != null)
				{
					if (pawn2.Faction != pawn.Faction)
					{
						pawn2.SetFaction(pawn.Faction, null);
					}
					IntVec3 loc = CellFinder.RandomClosewalkCellNear(pawn.Position, 5);
					GenSpawn.Spawn(pawn2, loc);
					lord.AddPawn(pawn2);
				}
				else
				{
					if (!pawn.inventory.container.TryAdd(current))
					{
						current.Destroy(DestroyMode.Vanish);
					}
				}
			}
			return true;
		}
	}
}
