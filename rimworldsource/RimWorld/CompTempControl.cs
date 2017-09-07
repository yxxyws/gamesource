using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace RimWorld
{
	public class CompTempControl : ThingComp
	{
		private const float DefaultTargetTemperature = 21f;
		[Unsaved]
		public bool operatingAtHighPower;
		public float targetTemperature = -99999f;
		public CompProperties_TempControl Props
		{
			get
			{
				return (CompProperties_TempControl)this.props;
			}
		}
		public override void PostSpawnSetup()
		{
			base.PostSpawnSetup();
			if (this.targetTemperature < -2000f)
			{
				this.targetTemperature = this.Props.defaultTargetTemperature;
			}
		}
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.LookValue<float>(ref this.targetTemperature, "targetTemperature", 0f, false);
		}
		private float RoundedToCurrentTempModeOffset(float celsiusTemp)
		{
			float num = GenTemperature.CelsiusToOffset(celsiusTemp, Prefs.TemperatureMode);
			num = (float)Mathf.RoundToInt(num);
			return GenTemperature.ConvertTemperatureOffset(num, Prefs.TemperatureMode, TemperatureDisplayMode.Celsius);
		}
		[DebuggerHidden]
		public override IEnumerable<Command> CompGetGizmosExtra()
		{
			CompTempControl.<CompGetGizmosExtra>c__IteratorF3 <CompGetGizmosExtra>c__IteratorF = new CompTempControl.<CompGetGizmosExtra>c__IteratorF3();
			<CompGetGizmosExtra>c__IteratorF.<>f__this = this;
			CompTempControl.<CompGetGizmosExtra>c__IteratorF3 expr_0E = <CompGetGizmosExtra>c__IteratorF;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		private void InterfaceChangeTargetTemperature(float offset)
		{
			if (offset > 0f)
			{
				SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
			}
			else
			{
				SoundDefOf.AmountDecrement.PlayOneShotOnCamera();
			}
			this.targetTemperature += offset;
			this.targetTemperature = Mathf.Clamp(this.targetTemperature, -270f, 2000f);
			this.ThrowCurrentTemperatureText();
		}
		private void ThrowCurrentTemperatureText()
		{
			MoteThrower.ThrowText(this.parent.TrueCenter() + new Vector3(0.5f, 0f, 0.5f), this.targetTemperature.ToStringTemperature("F0"), Color.white, -1);
		}
		public override string CompInspectStringExtra()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("TargetTemperature".Translate() + ": ");
			stringBuilder.AppendLine(this.targetTemperature.ToStringTemperature("F0"));
			stringBuilder.Append("PowerConsumptionMode".Translate() + ": ");
			if (this.operatingAtHighPower)
			{
				stringBuilder.AppendLine("PowerConsumptionHigh".Translate());
			}
			else
			{
				stringBuilder.AppendLine("PowerConsumptionLow".Translate());
			}
			return stringBuilder.ToString();
		}
	}
}
