using System;
using Verse;
using Verse.AI;
using Verse.AI.Group;
namespace RimWorld
{
	public class LordJob_AssaultColony : LordJob
	{
		private Faction assaulterFaction;
		private bool canKidnap = true;
		private bool canTimeoutOrFlee = true;
		private bool sappers;
		private static readonly IntRange AssaultTimeBeforeGiveUp = new IntRange(26000, 38000);
		private static readonly IntRange SapTimeBeforeGiveUp = new IntRange(33000, 38000);
		public LordJob_AssaultColony()
		{
		}
		public LordJob_AssaultColony(Faction assaulterFaction, bool canKidnap = true, bool canTimeoutOrFlee = true, bool sappers = false)
		{
			this.assaulterFaction = assaulterFaction;
			this.canKidnap = canKidnap;
			this.canTimeoutOrFlee = canTimeoutOrFlee;
			this.sappers = sappers;
		}
		public override StateGraph CreateGraph()
		{
			StateGraph stateGraph = new StateGraph();
			LordToil lordToil = null;
			if (this.sappers)
			{
				lordToil = new LordToil_AssaultColonySappers();
				stateGraph.lordToils.Add(lordToil);
			}
			LordToil lordToil2 = new LordToil_AssaultColony();
			stateGraph.lordToils.Add(lordToil2);
			LordToil_ExitMapAnywhere lordToil_ExitMapAnywhere = new LordToil_ExitMapAnywhere(LocomotionUrgency.Jog);
			stateGraph.lordToils.Add(lordToil_ExitMapAnywhere);
			if (this.sappers)
			{
				Transition transition = new Transition(lordToil, lordToil2);
				transition.triggers.Add(new Trigger_NoFightingSappers());
				stateGraph.transitions.Add(transition);
			}
			if (this.assaulterFaction.def.humanlikeFaction)
			{
				if (this.canTimeoutOrFlee)
				{
					Transition transition2 = new Transition(lordToil2, lordToil_ExitMapAnywhere);
					if (lordToil != null)
					{
						transition2.sources.Add(lordToil);
					}
					transition2.triggers.Add(new Trigger_TicksPassed((!this.sappers) ? LordJob_AssaultColony.AssaultTimeBeforeGiveUp.RandomInRange : LordJob_AssaultColony.SapTimeBeforeGiveUp.RandomInRange));
					transition2.preActions.Add(new TransitionAction_Message("MessageRaidersGivenUpLeaving".Translate(new object[]
					{
						this.assaulterFaction.def.pawnsPlural.CapitalizeFirst(),
						this.assaulterFaction.name
					})));
					stateGraph.transitions.Add(transition2);
					Transition transition3 = new Transition(lordToil2, lordToil_ExitMapAnywhere);
					if (lordToil != null)
					{
						transition3.sources.Add(lordToil);
					}
					FloatRange floatRange = new FloatRange(0.25f, 0.35f);
					float randomInRange = floatRange.RandomInRange;
					transition3.triggers.Add(new Trigger_FractionColonyDamageTaken(randomInRange, 900f));
					transition3.preActions.Add(new TransitionAction_Message("MessageRaidersSatisfiedLeaving".Translate(new object[]
					{
						this.assaulterFaction.def.pawnsPlural.CapitalizeFirst(),
						this.assaulterFaction.name
					})));
					stateGraph.transitions.Add(transition3);
				}
				if (this.canKidnap)
				{
					LordToil_Kidnap lordToil_Kidnap = new LordToil_Kidnap();
					stateGraph.lordToils.Add(lordToil_Kidnap);
					Transition transition4 = new Transition(lordToil2, lordToil_Kidnap);
					if (lordToil != null)
					{
						transition4.sources.Add(lordToil);
					}
					transition4.preActions.Add(new TransitionAction_Message("MessageRaidersKidnapping".Translate(new object[]
					{
						this.assaulterFaction.def.pawnsPlural.CapitalizeFirst(),
						this.assaulterFaction.name
					})));
					transition4.triggers.Add(new Trigger_KidnapVictimPresent());
					stateGraph.transitions.Add(transition4);
				}
			}
			Transition transition5 = new Transition(lordToil2, lordToil_ExitMapAnywhere);
			if (lordToil != null)
			{
				transition5.sources.Add(lordToil);
			}
			transition5.triggers.Add(new Trigger_BecameColonyAlly());
			transition5.preActions.Add(new TransitionAction_Message("MessageRaidersLeaving".Translate(new object[]
			{
				this.assaulterFaction.def.pawnsPlural.CapitalizeFirst(),
				this.assaulterFaction.name
			})));
			stateGraph.transitions.Add(transition5);
			return stateGraph;
		}
		public override void ExposeData()
		{
			Scribe_References.LookReference<Faction>(ref this.assaulterFaction, "assaulterFaction", false);
			Scribe_Values.LookValue<bool>(ref this.canKidnap, "canKidnap", true, false);
			Scribe_Values.LookValue<bool>(ref this.canTimeoutOrFlee, "canTimeoutOrFlee", true, false);
			Scribe_Values.LookValue<bool>(ref this.sappers, "sappers", false, false);
		}
	}
}
