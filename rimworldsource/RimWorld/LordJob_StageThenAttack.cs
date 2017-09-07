using System;
using Verse;
using Verse.AI.Group;
namespace RimWorld
{
	public class LordJob_StageThenAttack : LordJob
	{
		private Faction faction;
		private IntVec3 stageLoc;
		public LordJob_StageThenAttack()
		{
		}
		public LordJob_StageThenAttack(Faction faction, IntVec3 stageLoc)
		{
			this.faction = faction;
			this.stageLoc = stageLoc;
		}
		public override StateGraph CreateGraph()
		{
			StateGraph stateGraph = new StateGraph();
			LordToil_Stage lordToil_Stage = new LordToil_Stage(this.stageLoc);
			stateGraph.StartingToil = lordToil_Stage;
			LordToil startingToil = stateGraph.AttachSubgraph(new LordJob_AssaultColony(this.faction, true, true, false).CreateGraph()).StartingToil;
			Transition transition = new Transition(lordToil_Stage, startingToil);
			transition.triggers.Add(new Trigger_TicksPassed(Rand.Range(5000, 15000)));
			transition.triggers.Add(new Trigger_FractionPawnsLost(0.3f));
			transition.preActions.Add(new TransitionAction_Message("MessageRaidersBeginningAssault".Translate(new object[]
			{
				this.faction.def.pawnsPlural.CapitalizeFirst(),
				this.faction.name
			}), MessageSound.SeriousAlert));
			transition.preActions.Add(new TransitionAction_WakeAll());
			stateGraph.transitions.Add(transition);
			return stateGraph;
		}
		public override void ExposeData()
		{
			Scribe_References.LookReference<Faction>(ref this.faction, "faction", false);
			Scribe_Values.LookValue<IntVec3>(ref this.stageLoc, "stageLoc", default(IntVec3), false);
		}
	}
}
