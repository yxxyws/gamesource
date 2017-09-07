using System;
using Verse;
namespace RimWorld
{
	public class StoryWatcher_RampUp : IExposable
	{
		private const int UpdateInterval = 5000;
		private const float UpdateIntervalAsDayFraction = 0.0833333358f;
		private const float ShortFactor_GameStartGraceDays = 22f;
		private const float ShortFactor_DaysToDouble = 160f;
		private const float LongFactor_GameStartGraceDays = 44f;
		private const float LongFactor_DaysToDouble = 320f;
		private float shortTermFactor = 1f;
		private float longTermFactor = 1f;
		public float TotalThreatPointsFactor
		{
			get
			{
				return this.shortTermFactor * this.longTermFactor;
			}
		}
		public float ShortTermFactor
		{
			get
			{
				return this.shortTermFactor;
			}
		}
		public float LongTermFactor
		{
			get
			{
				return this.longTermFactor;
			}
		}
		public void Notify_ColonistIncappedOrKilled(Pawn p)
		{
			float num = this.shortTermFactor - 1f;
			float num2 = this.longTermFactor - 1f;
			switch (Find.MapPawns.FreeColonistsCount)
			{
			case 0:
				num *= 0f;
				num2 *= 0f;
				break;
			case 1:
				num *= 0f;
				num2 *= 0f;
				break;
			case 2:
				num *= 0f;
				num2 *= 0.5f;
				break;
			case 3:
				num *= 0f;
				num2 *= 0.75f;
				break;
			case 4:
				num *= 0.15f;
				num2 *= 0.8f;
				break;
			case 5:
				num *= 0.25f;
				num2 *= 0.9f;
				break;
			case 6:
				num *= 0.5f;
				num2 *= 1f;
				break;
			case 7:
				num *= 0.5f;
				num2 *= 1f;
				break;
			default:
				num *= 0.5f;
				num2 *= 1f;
				break;
			}
			this.shortTermFactor = 1f + num;
			this.longTermFactor = 1f + num2;
		}
		public void RampUpWatcherTick()
		{
			if (Find.TickManager.TicksGame % 5000 == 0)
			{
				if ((float)GenDate.DaysPassed >= 22f)
				{
					this.shortTermFactor += 0.00052083336f;
				}
				if ((float)GenDate.DaysPassed >= 44f)
				{
					this.longTermFactor += 0.00026041668f;
				}
			}
		}
		public void ExposeData()
		{
			Scribe_Values.LookValue<float>(ref this.shortTermFactor, "shortTermFactor", 0f, false);
			Scribe_Values.LookValue<float>(ref this.longTermFactor, "longTermFactor", 0f, false);
		}
	}
}
