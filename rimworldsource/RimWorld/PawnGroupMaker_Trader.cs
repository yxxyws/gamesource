using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;
namespace RimWorld
{
	public class PawnGroupMaker_Trader : PawnGroupMaker
	{
		private const float GuardsPointsPerMarketValue = 0.015f;
		private const float MinGuardsPoints = 130f;
		private const float MaxGuardsPoints = 1700f;
		public List<PawnGenOption> traders = new List<PawnGenOption>();
		public List<PawnGenOption> carriers = new List<PawnGenOption>();
		public List<PawnGenOption> guards = new List<PawnGenOption>();
		public override float MinPointsToGenerateAnything
		{
			get
			{
				return 0f;
			}
		}
		public override bool CanGenerateFrom(IncidentParms parms)
		{
			return parms.traderCaravan && base.CanGenerateFrom(parms) && this.traders.Any<PawnGenOption>() && this.carriers.Any<PawnGenOption>();
		}
		[DebuggerHidden]
		public override IEnumerable<Pawn> GenerateArrivingPawns(IncidentParms parms, bool errorOnZeroResults = true)
		{
			PawnGroupMaker_Trader.<GenerateArrivingPawns>c__IteratorB0 <GenerateArrivingPawns>c__IteratorB = new PawnGroupMaker_Trader.<GenerateArrivingPawns>c__IteratorB0();
			<GenerateArrivingPawns>c__IteratorB.parms = parms;
			<GenerateArrivingPawns>c__IteratorB.errorOnZeroResults = errorOnZeroResults;
			<GenerateArrivingPawns>c__IteratorB.<$>parms = parms;
			<GenerateArrivingPawns>c__IteratorB.<$>errorOnZeroResults = errorOnZeroResults;
			<GenerateArrivingPawns>c__IteratorB.<>f__this = this;
			PawnGroupMaker_Trader.<GenerateArrivingPawns>c__IteratorB0 expr_2A = <GenerateArrivingPawns>c__IteratorB;
			expr_2A.$PC = -2;
			return expr_2A;
		}
		private Pawn GenerateTrader(IncidentParms parms, TraderKindDef traderKind)
		{
			Pawn pawn = PawnGenerator.GeneratePawn(this.traders.RandomElementByWeight((PawnGenOption x) => (float)x.selectionWeight).kind, parms.faction);
			pawn.mindState.wantsToTradeWithColony = true;
			PawnComponentsUtility.AddAndRemoveDynamicComponents(pawn, true);
			pawn.trader.traderKind = traderKind;
			this.PostGenerate(pawn);
			return pawn;
		}
		[DebuggerHidden]
		private IEnumerable<Pawn> GenerateCarriers(IncidentParms parms, Pawn trader, List<Thing> wares)
		{
			PawnGroupMaker_Trader.<GenerateCarriers>c__IteratorB1 <GenerateCarriers>c__IteratorB = new PawnGroupMaker_Trader.<GenerateCarriers>c__IteratorB1();
			<GenerateCarriers>c__IteratorB.wares = wares;
			<GenerateCarriers>c__IteratorB.parms = parms;
			<GenerateCarriers>c__IteratorB.<$>wares = wares;
			<GenerateCarriers>c__IteratorB.<$>parms = parms;
			<GenerateCarriers>c__IteratorB.<>f__this = this;
			PawnGroupMaker_Trader.<GenerateCarriers>c__IteratorB1 expr_2A = <GenerateCarriers>c__IteratorB;
			expr_2A.$PC = -2;
			return expr_2A;
		}
		[DebuggerHidden]
		private IEnumerable<Pawn> GetSlavesFromWares(IncidentParms parms, Pawn trader, List<Thing> wares)
		{
			PawnGroupMaker_Trader.<GetSlavesFromWares>c__IteratorB2 <GetSlavesFromWares>c__IteratorB = new PawnGroupMaker_Trader.<GetSlavesFromWares>c__IteratorB2();
			<GetSlavesFromWares>c__IteratorB.wares = wares;
			<GetSlavesFromWares>c__IteratorB.parms = parms;
			<GetSlavesFromWares>c__IteratorB.<$>wares = wares;
			<GetSlavesFromWares>c__IteratorB.<$>parms = parms;
			PawnGroupMaker_Trader.<GetSlavesFromWares>c__IteratorB2 expr_23 = <GetSlavesFromWares>c__IteratorB;
			expr_23.$PC = -2;
			return expr_23;
		}
		[DebuggerHidden]
		private IEnumerable<Pawn> GenerateGuards(IncidentParms parms, Pawn trader, List<Thing> wares)
		{
			PawnGroupMaker_Trader.<GenerateGuards>c__IteratorB3 <GenerateGuards>c__IteratorB = new PawnGroupMaker_Trader.<GenerateGuards>c__IteratorB3();
			<GenerateGuards>c__IteratorB.wares = wares;
			<GenerateGuards>c__IteratorB.parms = parms;
			<GenerateGuards>c__IteratorB.<$>wares = wares;
			<GenerateGuards>c__IteratorB.<$>parms = parms;
			<GenerateGuards>c__IteratorB.<>f__this = this;
			PawnGroupMaker_Trader.<GenerateGuards>c__IteratorB3 expr_2A = <GenerateGuards>c__IteratorB;
			expr_2A.$PC = -2;
			return expr_2A;
		}
		private bool ValidateTradersAndCarriers(Faction faction)
		{
			PawnGenOption pawnGenOption = this.traders.FirstOrDefault((PawnGenOption x) => !x.kind.trader);
			if (pawnGenOption != null)
			{
				Log.Error(string.Concat(new object[]
				{
					"Cannot generate arriving trader caravan for ",
					faction,
					" because there is a pawn kind (",
					pawnGenOption.kind.LabelCap,
					") who is not a trader but is in a traders list."
				}));
				return false;
			}
			PawnGenOption pawnGenOption2 = this.carriers.FirstOrDefault((PawnGenOption x) => !x.kind.carrier);
			if (pawnGenOption2 != null)
			{
				Log.Error(string.Concat(new object[]
				{
					"Cannot generate arriving trader caravan for ",
					faction,
					" because there is a pawn kind (",
					pawnGenOption2.kind.LabelCap,
					") who is not a carrier but is in a carriers list."
				}));
				return false;
			}
			return true;
		}
	}
}
