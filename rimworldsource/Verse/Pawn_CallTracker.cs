using RimWorld;
using System;
namespace Verse
{
	public class Pawn_CallTracker
	{
		private const float AngryCallOnMeleeChance = 0.5f;
		public Pawn pawn;
		private int ticksToNextCall = -1;
		private static readonly IntRange CallOnAggroDelayRange = new IntRange(0, 120);
		private static readonly IntRange CallOnMeleeDelayRange = new IntRange(0, 20);
		private bool PawnAggressive
		{
			get
			{
				return this.pawn.InAggroMentalState || this.pawn.mindState.enemyTarget != null || (this.pawn.CurJob != null && this.pawn.CurJob.def == JobDefOf.AttackMelee);
			}
		}
		public Pawn_CallTracker(Pawn pawn)
		{
			this.pawn = pawn;
		}
		public void CallTrackerTick()
		{
			if (this.ticksToNextCall < 0)
			{
				this.ResetTicksToNextCall();
			}
			this.ticksToNextCall--;
			if (this.ticksToNextCall <= 0)
			{
				this.TryDoCall();
				this.ResetTicksToNextCall();
			}
		}
		private void ResetTicksToNextCall()
		{
			this.ticksToNextCall = this.pawn.def.race.soundCallIntervalRange.RandomInRange;
			if (this.PawnAggressive)
			{
				this.ticksToNextCall /= 4;
			}
		}
		private void TryDoCall()
		{
			if (!Find.CameraMap.CurrentViewRect.ExpandedBy(10).Contains(this.pawn.Position))
			{
				return;
			}
			if (this.pawn.Downed || !this.pawn.Awake())
			{
				return;
			}
			if (this.pawn.Position.Fogged())
			{
				return;
			}
			this.DoCall();
		}
		public void DoCall()
		{
			if (!this.pawn.Spawned)
			{
				return;
			}
			if (this.PawnAggressive)
			{
				LifeStageUtility.PlayNearestLifestageSound(this.pawn, (LifeStageAge ls) => ls.soundAngry);
			}
			else
			{
				LifeStageUtility.PlayNearestLifestageSound(this.pawn, (LifeStageAge ls) => ls.soundCall);
			}
		}
		public void Notify_InAggroMentalState()
		{
			this.ticksToNextCall = Pawn_CallTracker.CallOnAggroDelayRange.RandomInRange;
		}
		public void Notify_DidMeleeAttack()
		{
			if (Rand.Value < 0.5f)
			{
				this.ticksToNextCall = Pawn_CallTracker.CallOnMeleeDelayRange.RandomInRange;
			}
		}
		public void Notify_Released()
		{
			if (Rand.Value < 0.75f)
			{
				this.ticksToNextCall = Pawn_CallTracker.CallOnAggroDelayRange.RandomInRange;
			}
		}
	}
}
