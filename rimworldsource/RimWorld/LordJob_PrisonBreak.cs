using System;
using Verse;
using Verse.AI;
using Verse.AI.Group;
namespace RimWorld
{
	public class LordJob_PrisonBreak : LordJob
	{
		private IntVec3 groupUpLoc;
		private IntVec3 exitPoint;
		private int sapperThingID = -1;
		public LordJob_PrisonBreak()
		{
		}
		public LordJob_PrisonBreak(IntVec3 groupUpLoc, IntVec3 exitPoint, int sapperThingID)
		{
			this.groupUpLoc = groupUpLoc;
			this.exitPoint = exitPoint;
			this.sapperThingID = sapperThingID;
		}
		public override StateGraph CreateGraph()
		{
			StateGraph stateGraph = new StateGraph();
			LordToil_Travel lordToil_Travel = new LordToil_Travel(this.groupUpLoc);
			lordToil_Travel.maxDanger = Danger.Deadly;
			stateGraph.StartingToil = lordToil_Travel;
			LordToil_PrisonerEscape lordToil_PrisonerEscape = new LordToil_PrisonerEscape(this.exitPoint, this.sapperThingID);
			stateGraph.lordToils.Add(lordToil_PrisonerEscape);
			LordToil_ExitMapNearest lordToil_ExitMapNearest = new LordToil_ExitMapNearest(LocomotionUrgency.Jog);
			stateGraph.lordToils.Add(lordToil_ExitMapNearest);
			Transition transition = new Transition(lordToil_Travel, lordToil_PrisonerEscape);
			transition.triggers.Add(new Trigger_Memo("TravelArrived"));
			stateGraph.transitions.Add(transition);
			Transition transition2 = new Transition(lordToil_Travel, lordToil_PrisonerEscape);
			transition2.triggers.Add(new Trigger_PawnLost());
			stateGraph.transitions.Add(transition2);
			Transition transition3 = new Transition(lordToil_PrisonerEscape, lordToil_PrisonerEscape);
			transition3.triggers.Add(new Trigger_PawnLost());
			transition3.triggers.Add(new Trigger_PawnHarmed());
			stateGraph.transitions.Add(transition3);
			Transition transition4 = new Transition(lordToil_PrisonerEscape, lordToil_ExitMapNearest);
			transition4.triggers.Add(new Trigger_Memo("TravelArrived"));
			stateGraph.transitions.Add(transition4);
			return stateGraph;
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<IntVec3>(ref this.groupUpLoc, "groupUpLoc", default(IntVec3), false);
			Scribe_Values.LookValue<IntVec3>(ref this.exitPoint, "exitPoint", default(IntVec3), false);
			Scribe_Values.LookValue<int>(ref this.sapperThingID, "sapperThingID", -1, false);
		}
		public override void Notify_PawnAdded(Pawn p)
		{
			Reachability.ClearCache();
		}
		public override void Notify_PawnLost(Pawn p, PawnLostCondition condition)
		{
			Reachability.ClearCache();
		}
	}
}
