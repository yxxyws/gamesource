using RimWorld;
using System;
namespace Verse.AI
{
	public class MentalStateHandler : IExposable
	{
		private Pawn pawn;
		private MentalState curStateInt;
		public bool neverFleeIndividual;
		public bool InMentalState
		{
			get
			{
				return this.curStateInt != null;
			}
		}
		public MentalStateDef CurStateDef
		{
			get
			{
				if (this.curStateInt == null)
				{
					return null;
				}
				return this.curStateInt.def;
			}
		}
		public MentalState CurState
		{
			get
			{
				return this.curStateInt;
			}
		}
		public MentalStateHandler()
		{
		}
		public MentalStateHandler(Pawn pawn)
		{
			this.pawn = pawn;
		}
		public void ExposeData()
		{
			Scribe_Deep.LookDeep<MentalState>(ref this.curStateInt, "curState", new object[0]);
			Scribe_Values.LookValue<bool>(ref this.neverFleeIndividual, "neverFleeIndividual", false, false);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				if (this.curStateInt != null)
				{
					this.curStateInt.pawn = this.pawn;
				}
				if (Game.Mode != GameMode.Entry)
				{
					Find.AttackTargetsCache.UpdateTarget(this.pawn);
				}
			}
		}
		public void Reset()
		{
			this.ClearMentalStateDirect();
		}
		public void MentalStateHandlerTick()
		{
			if (this.curStateInt != null)
			{
				if (this.pawn.Downed)
				{
					Log.Error("In mental state while downed: " + this.pawn);
					this.CurState.RecoverFromState();
					return;
				}
				this.curStateInt.MentalStateTick();
			}
		}
		public void StartMentalState(MentalStateDef stateDef)
		{
			if (this.pawn.holder != null || this.CurStateDef == stateDef || this.pawn.Downed)
			{
				return;
			}
			if ((this.pawn.IsColonist || this.pawn.HostFaction == Faction.OfColony) && stateDef.tale != null)
			{
				TaleRecorder.RecordTale(stateDef.tale, new object[]
				{
					this.pawn
				});
			}
			if (!stateDef.beginLetter.NullOrEmpty() && PawnUtility.ShouldSendNotificationAbout(this.pawn))
			{
				string label = "MentalStateLetterLabel".Translate() + " (" + stateDef.beginLetterLabel + ")";
				Find.LetterStack.ReceiveLetter(label, string.Format(stateDef.beginLetter, this.pawn.Label).CapitalizeFirst(), stateDef.beginLetterType, this.pawn, null);
			}
			this.pawn.records.Increment(RecordDefOf.TimesInMentalState);
			if (this.pawn.Drafted)
			{
				this.pawn.drafter.Drafted = false;
			}
			this.curStateInt = (MentalState)Activator.CreateInstance(stateDef.stateClass);
			this.curStateInt.pawn = this.pawn;
			this.curStateInt.def = stateDef;
			if (this.pawn.needs.mood != null)
			{
				this.pawn.needs.mood.thoughts.Notify_MentalStateChanged();
			}
			if (stateDef != null && stateDef.isAggro && this.pawn.caller != null)
			{
				this.pawn.caller.Notify_InAggroMentalState();
			}
			if (this.pawn.CurJob != null)
			{
				this.pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
			}
			if (this.CurState != null)
			{
				this.CurState.PostStart();
			}
			Find.AttackTargetsCache.UpdateTarget(this.pawn);
		}
		public void Notify_DamageTaken(DamageInfo dinfo)
		{
			if (!this.neverFleeIndividual && this.pawn.Spawned && this.pawn.MentalStateDef == null && !this.pawn.Downed && dinfo.Def.externalViolence && this.pawn.RaceProps.Humanlike && this.pawn.mindState.canFleeIndividual)
			{
				float lerpPct = (float)(this.pawn.HashOffset() % 100) / 100f;
				float num = this.pawn.kindDef.fleeHealthThresholdRange.LerpThroughRange(lerpPct);
				if (this.pawn.health.summaryHealth.SummaryHealthPercent < num && this.pawn.Faction != Faction.OfColony && this.pawn.HostFaction == null)
				{
					this.StartMentalState(MentalStateDefOf.PanicFlee);
				}
			}
		}
		internal void ClearMentalStateDirect()
		{
			if (this.curStateInt == null)
			{
				return;
			}
			this.curStateInt = null;
			Find.AttackTargetsCache.UpdateTarget(this.pawn);
		}
	}
}
