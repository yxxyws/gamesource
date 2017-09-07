using System;
using Verse;
using Verse.AI.Group;
namespace RimWorld
{
	public class LordJob_MechanoidsDefendShip : LordJob
	{
		private Thing shipPart;
		private Faction faction;
		private float defendRadius;
		private IntVec3 defSpot;
		public LordJob_MechanoidsDefendShip()
		{
		}
		public LordJob_MechanoidsDefendShip(Thing shipPart, Faction faction, float defendRadius, IntVec3 defSpot)
		{
			this.shipPart = shipPart;
			this.faction = faction;
			this.defendRadius = defendRadius;
			this.defSpot = defSpot;
		}
		public override StateGraph CreateGraph()
		{
			StateGraph stateGraph = new StateGraph();
			if (!this.defSpot.IsValid)
			{
				stateGraph.AttachSubgraph(new LordJob_AssaultColony(this.faction, true, true, false).CreateGraph());
				return stateGraph;
			}
			LordToil_DefendPoint lordToil_DefendPoint = new LordToil_DefendPoint(this.defSpot, this.defendRadius);
			stateGraph.StartingToil = lordToil_DefendPoint;
			LordToil startingToil = stateGraph.AttachSubgraph(new LordJob_AssaultColony(this.faction, true, true, false).CreateGraph()).StartingToil;
			Transition transition = new Transition(lordToil_DefendPoint, startingToil);
			transition.triggers.Add(new Trigger_ThingDamageTaken(this.shipPart, 0.5f));
			transition.triggers.Add(new Trigger_Memo("AssaultColony"));
			stateGraph.transitions.Add(transition);
			return stateGraph;
		}
		public override void ExposeData()
		{
			Scribe_References.LookReference<Thing>(ref this.shipPart, "shipPart", false);
			Scribe_References.LookReference<Faction>(ref this.faction, "faction", false);
			Scribe_Values.LookValue<float>(ref this.defendRadius, "defendRadius", 0f, false);
			Scribe_Values.LookValue<IntVec3>(ref this.defSpot, "defSpot", default(IntVec3), false);
		}
	}
}
