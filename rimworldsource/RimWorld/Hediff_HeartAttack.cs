using System;
using Verse;
namespace RimWorld
{
	public class Hediff_HeartAttack : HediffWithComps
	{
		private const int SeverityChangeInterval = 5000;
		private const float TendReduction = 0.3f;
		private const float TendSuccessChance = 0.4f;
		private float intervalFactor;
		public override void PostMake()
		{
			base.PostMake();
			this.intervalFactor = Rand.Range(0.1f, 2f);
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<float>(ref this.intervalFactor, "intervalFactor", 0f, false);
		}
		public override void Tick()
		{
			base.Tick();
			if (this.pawn.IsHashIntervalTick((int)(5000f * this.intervalFactor)))
			{
				this.Severity += Rand.Range(-0.4f, 0.6f);
			}
		}
		public override void Tended(float quality, int batchPosition = 0)
		{
			base.Tended(quality, 0);
			float num = 0.4f * quality;
			if (Rand.Value < num)
			{
				if (batchPosition == 0)
				{
					MoteThrower.ThrowText(this.pawn.DrawPos, "TextMote_TreatSuccess".Translate(new object[]
					{
						num.ToStringPercent()
					}), 400);
				}
				this.Severity -= 0.3f;
			}
			else
			{
				if (batchPosition == 0)
				{
					MoteThrower.ThrowText(this.pawn.DrawPos, "TextMote_TreatFailed".Translate(new object[]
					{
						num.ToStringPercent()
					}), 400);
				}
			}
		}
	}
}
