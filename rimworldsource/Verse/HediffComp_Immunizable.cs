using System;
using System.Text;
using UnityEngine;
namespace Verse
{
	public class HediffComp_Immunizable : HediffComp
	{
		private const int SeverityUpdateInterval = 200;
		private float severityPerDayNotImmuneRandomFactor = 1f;
		public override string CompLabelInBracketsExtra
		{
			get
			{
				if (this.FullyImmune)
				{
					return "DevelopedImmunityLower".Translate();
				}
				return null;
			}
		}
		public override string CompTipStringExtra
		{
			get
			{
				if (base.Def.PossibleToDevelopImmunity())
				{
					return "Immunity".Translate() + ": " + (Mathf.Floor(this.Immunity * 100f) / 100f).ToStringPercent();
				}
				return null;
			}
		}
		public override string CompDebugString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("severity: " + this.parent.Severity.ToString("F3") + ((this.parent.Severity < this.props.maxSeverity) ? string.Empty : " (reached max)"));
				if (this.severityPerDayNotImmuneRandomFactor != 1f)
				{
					stringBuilder.AppendLine("severityPerDayNotImmuneRandomFactor: " + this.severityPerDayNotImmuneRandomFactor.ToString("0.##"));
				}
				if (!base.Pawn.Dead)
				{
					stringBuilder.AppendLine("severity change per day: " + (this.SeverityChangePerInterval() / 200f * 60000f).ToString("F3"));
					ImmunityRecord immunityRecord = base.Pawn.health.immunity.GetImmunityRecord(base.Def);
					if (immunityRecord != null)
					{
						stringBuilder.AppendLine("immunity change per day: " + (immunityRecord.ImmunityChangePerTick(base.Pawn, true, this.parent) * 60000f).ToString("F3"));
					}
				}
				return stringBuilder.ToString();
			}
		}
		public float Immunity
		{
			get
			{
				return base.Pawn.health.immunity.GetImmunity(base.Def);
			}
		}
		public bool FullyImmune
		{
			get
			{
				return this.Immunity >= 1f;
			}
		}
		public override void CompPostPostAdd()
		{
			base.CompPostPostAdd();
			this.severityPerDayNotImmuneRandomFactor = this.props.severityPerDayNotImmuneRandomFactor.RandomInRange;
		}
		public override void CompPostTick()
		{
			base.CompPostTick();
			if (base.Pawn.IsHashIntervalTick(200))
			{
				this.parent.Severity = Mathf.Clamp(this.parent.Severity + this.SeverityChangePerInterval(), this.props.minSeverity, this.props.maxSeverity);
			}
		}
		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Values.LookValue<float>(ref this.severityPerDayNotImmuneRandomFactor, "severityPerDayNotImmuneRandomFactor", 1f, false);
		}
		private float SeverityChangePerInterval()
		{
			float num = (!this.FullyImmune) ? (this.props.severityPerDayNotImmune * this.severityPerDayNotImmuneRandomFactor) : this.props.severityPerDayImmune;
			HediffComp_Tendable hediffComp_Tendable = this.parent.TryGetComp<HediffComp_Tendable>();
			if (hediffComp_Tendable != null && hediffComp_Tendable.IsTended)
			{
				num += this.props.severityPerDayTendedOffset * hediffComp_Tendable.tendQuality;
			}
			return num * 0.00333333341f;
		}
	}
}
