using System;
namespace Verse.AI.Group
{
	public class LordJob_TravelAndExit : LordJob
	{
		private IntVec3 travelDest;
		public LordJob_TravelAndExit()
		{
		}
		public LordJob_TravelAndExit(IntVec3 travelDest)
		{
			this.travelDest = travelDest;
		}
		public override StateGraph CreateGraph()
		{
			StateGraph stateGraph = new StateGraph();
			LordToil startingToil = stateGraph.AttachSubgraph(new LordJob_Travel(this.travelDest, 5000).CreateGraph()).StartingToil;
			stateGraph.StartingToil = startingToil;
			LordToil_ExitMapNearest lordToil_ExitMapNearest = new LordToil_ExitMapNearest(LocomotionUrgency.None);
			stateGraph.lordToils.Add(lordToil_ExitMapNearest);
			Transition transition = new Transition(startingToil, lordToil_ExitMapNearest);
			transition.triggers.Add(new Trigger_Memo("TravelArrived"));
			stateGraph.transitions.Add(transition);
			return stateGraph;
		}
		public override void ExposeData()
		{
			Scribe_Values.LookValue<IntVec3>(ref this.travelDest, "travelDest", default(IntVec3), false);
		}
	}
}
