using System;
namespace Verse.AI.Group
{
	public class LordJob_Travel : LordJob
	{
		private IntVec3 travelDest;
		private int defendTicks;
		public LordJob_Travel()
		{
		}
		public LordJob_Travel(IntVec3 travelDest, int defendTicks = 5000)
		{
			this.travelDest = travelDest;
			this.defendTicks = defendTicks;
		}
		public override StateGraph CreateGraph()
		{
			StateGraph stateGraph = new StateGraph();
			LordToil_Travel lordToil_Travel = new LordToil_Travel(this.travelDest);
			stateGraph.StartingToil = lordToil_Travel;
			LordToil_DefendPoint lordToil_DefendPoint = new LordToil_DefendPoint(false);
			stateGraph.lordToils.Add(lordToil_DefendPoint);
			Transition transition = new Transition(lordToil_Travel, lordToil_DefendPoint);
			transition.triggers.Add(new Trigger_PawnHarmed());
			transition.preActions.Add(new TransitionAction_SetDefendLocalGroup());
			stateGraph.transitions.Add(transition);
			Transition transition2 = new Transition(lordToil_DefendPoint, lordToil_Travel);
			transition2.triggers.Add(new Trigger_TicksPassed(this.defendTicks));
			transition2.preActions.Add(new TransitionAction_EnsureHaveExitDestination());
			stateGraph.transitions.Add(transition2);
			return stateGraph;
		}
		public override void ExposeData()
		{
			Scribe_Values.LookValue<IntVec3>(ref this.travelDest, "travelDest", default(IntVec3), false);
			Scribe_Values.LookValue<int>(ref this.defendTicks, "defendTicks", 5000, false);
		}
	}
}
