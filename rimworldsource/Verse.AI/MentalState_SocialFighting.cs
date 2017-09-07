using RimWorld;
using System;
namespace Verse.AI
{
	public class MentalState_SocialFighting : MentalState
	{
		public Pawn otherPawn;
		private bool ShouldStop
		{
			get
			{
				return !this.otherPawn.Spawned || this.otherPawn.Dead || this.otherPawn.Downed || !this.IsOtherPawnSocialFightingWithMe;
			}
		}
		private bool HasAttackJob
		{
			get
			{
				Job curJob = this.pawn.CurJob;
				return curJob != null && curJob.def == JobDefOf.AttackMelee && curJob.targetA.Thing == this.otherPawn;
			}
		}
		private bool IsOtherPawnSocialFightingWithMe
		{
			get
			{
				if (!this.otherPawn.InMentalState)
				{
					return false;
				}
				MentalState_SocialFighting mentalState_SocialFighting = this.otherPawn.MentalState as MentalState_SocialFighting;
				return mentalState_SocialFighting != null && mentalState_SocialFighting.otherPawn == this.pawn;
			}
		}
		public override void MentalStateTick()
		{
			if (this.ShouldStop)
			{
				base.RecoverFromState();
			}
			else
			{
				if (!this.HasAttackJob)
				{
					this.StartAttackJob();
				}
				base.MentalStateTick();
			}
		}
		public override void PostEnd()
		{
			if (this.HasAttackJob)
			{
				this.pawn.jobs.StopAll(false);
				this.pawn.mindState.meleeThreat = null;
			}
			if (this.IsOtherPawnSocialFightingWithMe)
			{
				this.otherPawn.MentalState.RecoverFromState();
			}
			if ((PawnUtility.ShouldSendNotificationAbout(this.pawn) || PawnUtility.ShouldSendNotificationAbout(this.otherPawn)) && this.pawn.thingIDNumber < this.otherPawn.thingIDNumber)
			{
				Messages.Message(string.Format(this.def.recoveryMessage, this.pawn.NameStringShort, this.otherPawn.LabelBaseShort), this.pawn, MessageSound.Silent);
			}
			if (!this.pawn.Dead && this.pawn.needs.mood != null && !this.otherPawn.Dead)
			{
				ThoughtDef def;
				if (Rand.Value < 0.5f)
				{
					def = ThoughtDefOf.HadAngeringFight;
				}
				else
				{
					def = ThoughtDefOf.HadCatharticFight;
				}
				Thought_SocialMemory thought_SocialMemory = (Thought_SocialMemory)ThoughtMaker.MakeThought(def);
				thought_SocialMemory.SetOtherPawn(this.otherPawn);
				this.pawn.needs.mood.thoughts.TryGainThought(thought_SocialMemory);
			}
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.LookReference<Pawn>(ref this.otherPawn, "otherPawn", false);
		}
		private void StartAttackJob()
		{
			Verb verbToUse;
			if (!InteractionUtility.TryGetRandomVerbForSocialFight(this.pawn, out verbToUse))
			{
				return;
			}
			Job job = new Job(JobDefOf.AttackMelee, this.otherPawn);
			job.maxNumMeleeAttacks = 1;
			job.verbToUse = verbToUse;
			this.pawn.jobs.StopAll(false);
			this.pawn.jobs.StartJob(job, JobCondition.None, null, false, false, null);
		}
		public override RandomSocialMode SocialModeMax()
		{
			return RandomSocialMode.Off;
		}
	}
}
