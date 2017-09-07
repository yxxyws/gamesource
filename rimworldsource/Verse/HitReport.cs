using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace Verse
{
	public class HitReport
	{
		private const float DarknessPenaltyMultiplier = 0.85f;
		public const float ProneMultiplier = 0.2f;
		public const float PronePenaltyMinDistance = 5f;
		public TargetInfo target;
		public List<CoverInfo> covers;
		public float coversOverallBlockChance;
		public float shotDistance;
		public float hitChanceThroughPawnStat = 1f;
		public float hitChanceThroughEquipment = 1f;
		public PsychGlow targetLighting = PsychGlow.Lit;
		public float hitChanceThroughTargetSize = 1f;
		public float hitChanceThroughWeather = 1f;
		public float hitChanceThroughSightEfficiency = 1f;
		public float forcedMissRadius = 1f;
		public float HitChanceThroughCover
		{
			get
			{
				return 1f - this.coversOverallBlockChance;
			}
		}
		private float HitChanceThroughDarkness
		{
			get
			{
				if (this.targetLighting == PsychGlow.Dark)
				{
					return 0.85f;
				}
				return 1f;
			}
		}
		public float TotalNonWildShotChance
		{
			get
			{
				return this.hitChanceThroughPawnStat * this.hitChanceThroughEquipment * this.hitChanceThroughWeather * this.HitChanceThroughDarkness * this.hitChanceThroughTargetSize * this.hitChanceThroughSightEfficiency;
			}
		}
		public float TotalHitChance
		{
			get
			{
				float value = this.TotalNonWildShotChance * this.HitChanceThroughCover;
				return Mathf.Clamp(value, 0.01f, 0.99f);
			}
		}
		public string GetTextReadout()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.forcedMissRadius > 0.3f)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("WeaponMissRadius".Translate() + "   " + this.forcedMissRadius.ToString("####0.0"));
			}
			else
			{
				stringBuilder.AppendLine(" " + Mathf.RoundToInt(this.TotalHitChance * 100f) + "%");
				stringBuilder.AppendLine("   " + "ShootReportShooterAbility".Translate() + "\t\t" + GenText.AsPercent(this.hitChanceThroughPawnStat));
				if (this.hitChanceThroughEquipment < 0.99f)
				{
					stringBuilder.AppendLine("   " + "ShootReportWeapon".Translate() + "   " + GenText.AsPercent(this.hitChanceThroughEquipment));
				}
				if (this.targetLighting == PsychGlow.Dark)
				{
					stringBuilder.AppendLine("   " + "Darkness".Translate() + "       " + GenText.AsPercent(0.85f));
				}
				if (this.target.HasThing)
				{
					Pawn pawn = this.target.Thing as Pawn;
					if (pawn != null && pawn.Downed && this.shotDistance > 5f)
					{
						stringBuilder.AppendLine("   " + "TargetProne".Translate() + "      " + GenText.AsPercent(0.2f));
					}
					if (this.hitChanceThroughTargetSize != 1f)
					{
						stringBuilder.AppendLine("   " + "TargetSize".Translate() + "       " + GenText.AsPercent(this.hitChanceThroughTargetSize));
					}
				}
				if (this.hitChanceThroughWeather < 0.99f)
				{
					stringBuilder.AppendLine("   " + "Weather".Translate() + "      " + GenText.AsPercent(this.hitChanceThroughWeather));
				}
				if (this.HitChanceThroughCover < 1f)
				{
					stringBuilder.AppendLine("   " + "ShootingCover".Translate() + "\t\t\t\t" + GenText.AsPercent(this.HitChanceThroughCover));
					for (int i = 0; i < this.covers.Count; i++)
					{
						CoverInfo coverInfo = this.covers[i];
						stringBuilder.AppendLine("     " + "CoverThingBlocksPercentOfShots".Translate(new object[]
						{
							coverInfo.Thing.LabelCap,
							GenText.AsPercent(coverInfo.BlockChance)
						}));
					}
				}
				else
				{
					stringBuilder.AppendLine("   (" + "NoCoverLower".Translate() + ")");
				}
			}
			return stringBuilder.ToString();
		}
	}
}
