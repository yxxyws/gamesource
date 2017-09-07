using System;
using Verse;
using Verse.AI.Group;
namespace RimWorld
{
	public class LordJob_AssistColony : LordJob
	{
		private Faction faction;
		private IntVec3 fallbackLocation;
		public LordJob_AssistColony()
		{
		}
		public LordJob_AssistColony(Faction faction, IntVec3 fallbackLocation)
		{
			this.faction = faction;
			this.fallbackLocation = fallbackLocation;
		}
		public override StateGraph CreateGraph()
		{
			StateGraph stateGraph = new StateGraph();
			LordToil startingToil = stateGraph.AttachSubgraph(new LordJob_Travel(this.fallbackLocation, 2000).CreateGraph()).StartingToil;
			LordToil_HuntEnemies lordToil_HuntEnemies = new LordToil_HuntEnemies(this.fallbackLocation);
			stateGraph.lordToils.Add(lordToil_HuntEnemies);
			LordToil startingToil2 = stateGraph.AttachSubgraph(new LordJob_Travel(IntVec3.Invalid, 5000).CreateGraph()).StartingToil;
			Transition transition = new Transition(startingToil, lordToil_HuntEnemies);
			transition.triggers.Add(new Trigger_Memo("TravelArrived"));
			stateGraph.transitions.Add(transition);
			Transition transition2 = new Transition(lordToil_HuntEnemies, startingToil2);
			transition2.preActions.Add(new TransitionAction_Message("MessageFriendlyFightersLeaving".Translate(new object[]
			{
				this.faction.def.pawnsPlural.CapitalizeFirst(),
				this.faction.name
			})));
			transition2.triggers.Add(new Trigger_TicksPassed(16000));
			stateGraph.transitions.Add(transition2);
			return stateGraph;
		}
		public override void ExposeData()
		{
			Scribe_References.LookReference<Faction>(ref this.faction, "faction", false);
			Scribe_Values.LookValue<IntVec3>(ref this.fallbackLocation, "fallbackLocation", default(IntVec3), false);
		}
	}
}
