using RimWorld;
using System;
namespace Verse.AI
{
	public class MentalState : IExposable
	{
		private const int TickInterval = 150;
		public Pawn pawn;
		public MentalStateDef def;
		private int age;
		public int Age
		{
			get
			{
				return this.age;
			}
		}
		public virtual void ExposeData()
		{
			Scribe_Defs.LookDef<MentalStateDef>(ref this.def, "def");
			Scribe_Values.LookValue<int>(ref this.age, "age", 0, false);
		}
		public virtual void PostStart()
		{
		}
		public virtual void PostEnd()
		{
			if (PawnUtility.ShouldSendNotificationAbout(this.pawn))
			{
				Messages.Message(string.Format(this.def.recoveryMessage, this.pawn.NameStringShort), this.pawn, MessageSound.Silent);
			}
		}
		public virtual void MentalStateTick()
		{
			if (this.pawn.IsHashIntervalTick(150))
			{
				this.age += 150;
				if (this.age >= this.def.minTicksBeforeRecovery && Rand.Value < this.def.recoveryChancePerInterval)
				{
					this.RecoverFromState();
					return;
				}
				if (this.def.recoverFromSleep && !this.pawn.Awake())
				{
					this.RecoverFromState();
					return;
				}
				if (this.def.recoverFromDowned && this.pawn.Downed)
				{
					this.RecoverFromState();
					return;
				}
			}
		}
		public void RecoverFromState()
		{
			if (this.pawn.MentalState != this)
			{
				Log.Error(string.Concat(new object[]
				{
					"Recovered from ",
					this.def,
					" but pawn's mental state is not this, it is ",
					this.pawn.MentalState
				}));
			}
			this.pawn.mindState.mentalStateHandler.ClearMentalStateDirect();
			if (this.def.recoveryThought != null && this.pawn.needs.mood != null)
			{
				this.pawn.needs.mood.thoughts.TryGainThought(this.def.recoveryThought);
			}
			this.PostEnd();
		}
		public virtual bool ForceHostileTo(Thing t)
		{
			return false;
		}
		public virtual bool ForceHostileTo(Faction f)
		{
			return false;
		}
		public EffecterDef CurrentStateEffecter()
		{
			return this.def.stateEffecter;
		}
		public virtual RandomSocialMode SocialModeMax()
		{
			return RandomSocialMode.SuperActive;
		}
	}
}
