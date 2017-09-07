using System;
using UnityEngine;
namespace Verse
{
	public class HediffGiver_Heatstroke : HediffGiver
	{
		public override bool CheckGiveEverySecond(Pawn pawn)
		{
			if (!pawn.Spawned)
			{
				return false;
			}
			FloatRange floatRange = pawn.ComfortableTemperatureRange();
			FloatRange floatRange2 = pawn.SafeTemperatureRange();
			float temperatureForCell = GenTemperature.GetTemperatureForCell(pawn.Position);
			HediffSet hediffSet = pawn.health.hediffSet;
			Hediff firstHediffOfDef = hediffSet.GetFirstHediffOfDef(this.hediff);
			if (temperatureForCell > floatRange2.max)
			{
				float num = Mathf.Abs(temperatureForCell - floatRange2.max);
				float num2 = num * 6.45E-05f;
				num2 = Mathf.Max(num2, 0.000375f);
				HealthUtility.AdjustSeverity(pawn, this.hediff, num2);
			}
			else
			{
				if (firstHediffOfDef != null && temperatureForCell < floatRange.max)
				{
					float num3 = firstHediffOfDef.Severity * 0.027f;
					num3 = Mathf.Clamp(num3, 0.0015f, 0.015f);
					firstHediffOfDef.Severity -= num3;
				}
			}
			return false;
		}
	}
}
