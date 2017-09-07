using System;
using Verse;
using Verse.AI;
using Verse.AI.Group;
namespace RimWorld
{
	public class LordJob_TradeWithColony : LordJob
	{
		private Faction faction;
		private IntVec3 chillSpot;
		public LordJob_TradeWithColony()
		{
		}
		public LordJob_TradeWithColony(Faction faction, IntVec3 chillSpot)
		{
			this.faction = faction;
			this.chillSpot = chillSpot;
		}
		public override StateGraph CreateGraph()
		{
			StateGraph stateGraph = new StateGraph();
			LordToil_Travel lordToil_Travel = new LordToil_Travel(this.chillSpot);
			stateGraph.StartingToil = lordToil_Travel;
			LordToil_DefendTraderCaravan lordToil_DefendTraderCaravan = new LordToil_DefendTraderCaravan();
			stateGraph.lordToils.Add(lordToil_DefendTraderCaravan);
			LordToil_DefendTraderCaravan lordToil_DefendTraderCaravan2 = new LordToil_DefendTraderCaravan(this.chillSpot);
			stateGraph.lordToils.Add(lordToil_DefendTraderCaravan2);
			LordToil_ExitMapAndEscortCarriers lordToil_ExitMapAndEscortCarriers = new LordToil_ExitMapAndEscortCarriers();
			stateGraph.lordToils.Add(lordToil_ExitMapAndEscortCarriers);
			LordToil_ExitMapNearest lordToil_ExitMapNearest = new LordToil_ExitMapNearest(LocomotionUrgency.None);
			stateGraph.lordToils.Add(lordToil_ExitMapNearest);
			Transition transition = new Transition(lordToil_Travel, lordToil_DefendTraderCaravan);
			transition.triggers.Add(new Trigger_PawnHarmed());
			transition.preActions.Add(new TransitionAction_WakeAll());
			transition.preActions.Add(new TransitionAction_SetDefendTrader());
			stateGraph.transitions.Add(transition);
			Transition transition2 = new Transition(lordToil_Travel, lordToil_DefendTraderCaravan2);
			transition2.triggers.Add(new Trigger_Memo("TravelArrived"));
			stateGraph.transitions.Add(transition2);
			Transition transition3 = new Transition(lordToil_DefendTraderCaravan, lordToil_Travel);
			transition3.triggers.Add(new Trigger_TicksPassed(5000));
			stateGraph.transitions.Add(transition3);
			Transition transition4 = new Transition(lordToil_DefendTraderCaravan2, lordToil_ExitMapAndEscortCarriers);
			transition4.triggers.Add(new Trigger_TicksPassed(Rand.Range(27000, 45000)));
			transition4.preActions.Add(new TransitionAction_Message("MessageTraderCaravanLeaving".Translate(new object[]
			{
				this.faction.name
			})));
			transition4.preActions.Add(new TransitionAction_WakeAll());
			stateGraph.transitions.Add(transition4);
			Transition transition5 = new Transition(lordToil_ExitMapAndEscortCarriers, lordToil_ExitMapAndEscortCarriers);
			transition5.triggers.Add(new Trigger_PawnLost());
			stateGraph.transitions.Add(transition5);
			Transition transition6 = new Transition(lordToil_ExitMapAndEscortCarriers, lordToil_ExitMapNearest);
			transition6.triggers.Add(new Trigger_TicksPassed(60000));
			transition6.preActions.Add(new TransitionAction_WakeAll());
			stateGraph.transitions.Add(transition6);
			Transition transition7 = new Transition(lordToil_DefendTraderCaravan2, lordToil_ExitMapAndEscortCarriers);
			transition7.sources.Add(lordToil_Travel);
			transition7.sources.Add(lordToil_DefendTraderCaravan);
			transition7.triggers.Add(new Trigger_ImportantCaravanPeopleLost());
			transition7.triggers.Add(new Trigger_BecameColonyEnemy());
			transition7.preActions.Add(new TransitionAction_WakeAll());
			stateGraph.transitions.Add(transition7);
			return stateGraph;
		}
		public override void ExposeData()
		{
			Scribe_References.LookReference<Faction>(ref this.faction, "faction", false);
			Scribe_Values.LookValue<IntVec3>(ref this.chillSpot, "chillSpot", default(IntVec3), false);
		}
	}
}
