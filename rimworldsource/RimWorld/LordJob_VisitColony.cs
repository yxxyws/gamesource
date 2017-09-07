using System;
using Verse;
using Verse.AI.Group;
namespace RimWorld
{
	public class LordJob_VisitColony : LordJob
	{
		private Faction faction;
		private IntVec3 chillSpot;
		public LordJob_VisitColony()
		{
		}
		public LordJob_VisitColony(Faction faction, IntVec3 chillSpot)
		{
			this.faction = faction;
			this.chillSpot = chillSpot;
		}
		public override StateGraph CreateGraph()
		{
			StateGraph stateGraph = new StateGraph();
			LordToil startingToil = stateGraph.AttachSubgraph(new LordJob_Travel(this.chillSpot, 5000).CreateGraph()).StartingToil;
			LordToil_DefendPoint lordToil_DefendPoint = new LordToil_DefendPoint(this.chillSpot, 28f);
			stateGraph.lordToils.Add(lordToil_DefendPoint);
			LordToil_TakeWoundedGuest lordToil_TakeWoundedGuest = new LordToil_TakeWoundedGuest();
			stateGraph.lordToils.Add(lordToil_TakeWoundedGuest);
			StateGraph stateGraph2 = new LordJob_TravelAndExit(IntVec3.Invalid).CreateGraph();
			LordToil startingToil2 = stateGraph.AttachSubgraph(stateGraph2).StartingToil;
			LordToil target = stateGraph2.lordToils[1];
			Transition transition = new Transition(startingToil, lordToil_DefendPoint);
			transition.triggers.Add(new Trigger_Memo("TravelArrived"));
			stateGraph.transitions.Add(transition);
			Transition transition2 = new Transition(lordToil_DefendPoint, lordToil_TakeWoundedGuest);
			transition2.triggers.Add(new Trigger_WoundedGuestPresent());
			transition2.preActions.Add(new TransitionAction_Message("MessageVisitorsTakingWounded".Translate(new object[]
			{
				this.faction.def.pawnsPlural.CapitalizeFirst(),
				this.faction.name
			})));
			stateGraph.transitions.Add(transition2);
			Transition transition3 = new Transition(lordToil_DefendPoint, target);
			transition3.triggers.Add(new Trigger_BecameColonyEnemy());
			transition3.preActions.Add(new TransitionAction_WakeAll());
			transition3.preActions.Add(new TransitionAction_SetDefendLocalGroup());
			stateGraph.transitions.Add(transition3);
			Transition transition4 = new Transition(startingToil, startingToil2);
			transition4.triggers.Add(new Trigger_BecameColonyEnemy());
			transition4.preActions.Add(new TransitionAction_WakeAll());
			transition4.preActions.Add(new TransitionAction_EnsureHaveExitDestination());
			stateGraph.transitions.Add(transition4);
			Transition transition5 = new Transition(lordToil_DefendPoint, startingToil2);
			transition5.triggers.Add(new Trigger_TicksPassed(Rand.Range(8000, 22000)));
			transition5.preActions.Add(new TransitionAction_Message("VisitorsLeaving".Translate(new object[]
			{
				this.faction.name
			})));
			transition5.preActions.Add(new TransitionAction_WakeAll());
			transition5.preActions.Add(new TransitionAction_EnsureHaveExitDestination());
			stateGraph.transitions.Add(transition5);
			return stateGraph;
		}
		public override void ExposeData()
		{
			Scribe_References.LookReference<Faction>(ref this.faction, "faction", false);
			Scribe_Values.LookValue<IntVec3>(ref this.chillSpot, "chillSpot", default(IntVec3), false);
		}
	}
}
