using System;
using System.Text;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class CompRottable : ThingComp
	{
		public float rotProgress;
		private CompProperties_Rottable PropsRot
		{
			get
			{
				return (CompProperties_Rottable)this.props;
			}
		}
		public RotStage Stage
		{
			get
			{
				if (this.rotProgress < (float)this.PropsRot.TicksToRotStart)
				{
					return RotStage.Fresh;
				}
				if (this.rotProgress < (float)this.PropsRot.TicksToDessicated)
				{
					return RotStage.Rotting;
				}
				return RotStage.Dessicated;
			}
		}
		public int TicksUntilRotAtCurrentTemp
		{
			get
			{
				float num = (float)this.PropsRot.TicksToRotStart - this.rotProgress;
				if (num <= 0f)
				{
					return 0;
				}
				float num2 = GenTemperature.GetTemperatureForCell(this.parent.PositionHeld);
				num2 = (float)Mathf.RoundToInt(num2);
				float num3 = GenTemperature.RotRateAtTemperature(num2);
				return Mathf.RoundToInt(num / num3);
			}
		}
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.LookValue<float>(ref this.rotProgress, "rotProg", 0f, false);
		}
		public override void CompTickRare()
		{
			float num = this.rotProgress;
			float num2 = 1f;
			float temperatureForCell = GenTemperature.GetTemperatureForCell(this.parent.PositionHeld);
			num2 *= GenTemperature.RotRateAtTemperature(temperatureForCell);
			this.rotProgress += (float)Mathf.RoundToInt(num2 * 250f);
			if (this.Stage == RotStage.Rotting && this.PropsRot.rotDestroys)
			{
				this.parent.Destroy(DestroyMode.Vanish);
				return;
			}
			bool flag = Mathf.FloorToInt(num / 60000f) != Mathf.FloorToInt(this.rotProgress / 60000f);
			if (flag)
			{
				if (this.Stage == RotStage.Rotting && this.PropsRot.rotDamagePerDay > 0f)
				{
					this.parent.TakeDamage(new DamageInfo(DamageDefOf.Rotting, GenMath.RoundRandom(this.PropsRot.rotDamagePerDay), null, null, null));
				}
				else
				{
					if (this.Stage == RotStage.Dessicated && this.PropsRot.dessicatedDamagePerDay > 0f && this.ShouldTakeDessicateDamage())
					{
						this.parent.TakeDamage(new DamageInfo(DamageDefOf.Rotting, GenMath.RoundRandom(this.PropsRot.dessicatedDamagePerDay), null, null, null));
					}
				}
			}
		}
		private bool ShouldTakeDessicateDamage()
		{
			if (this.parent.holder != null)
			{
				Thing thing = this.parent.holder.owner as Thing;
				if (thing != null && thing.def.category == ThingCategory.Building && thing.def.building.preventDeterioration)
				{
					return false;
				}
			}
			return true;
		}
		public override void PreAbsorbStack(Thing otherStack, int count)
		{
			float t = (float)count / (float)(this.parent.stackCount + count);
			float to = ((ThingWithComps)otherStack).GetComp<CompRottable>().rotProgress;
			this.rotProgress = Mathf.Lerp(this.rotProgress, to, t);
		}
		public override void PostSplitOff(Thing piece)
		{
			((ThingWithComps)piece).GetComp<CompRottable>().rotProgress = this.rotProgress;
		}
		public override string CompInspectStringExtra()
		{
			StringBuilder stringBuilder = new StringBuilder();
			switch (this.Stage)
			{
			case RotStage.Fresh:
				stringBuilder.AppendLine("RotStateFresh".Translate());
				break;
			case RotStage.Rotting:
				stringBuilder.AppendLine("RotStateRotting".Translate());
				break;
			case RotStage.Dessicated:
				stringBuilder.AppendLine("RotStateDessicated".Translate());
				break;
			}
			float num = (float)this.PropsRot.TicksToRotStart - this.rotProgress;
			if (num > 0f)
			{
				float num2 = GenTemperature.GetTemperatureForCell(this.parent.PositionHeld);
				num2 = (float)Mathf.RoundToInt(num2);
				float num3 = GenTemperature.RotRateAtTemperature(num2);
				int ticksUntilRotAtCurrentTemp = this.TicksUntilRotAtCurrentTemp;
				if (num3 < 0.001f)
				{
					stringBuilder.AppendLine("CurrentlyFrozen".Translate());
				}
				else
				{
					if (num3 < 0.999f)
					{
						stringBuilder.AppendLine("CurrentlyRefrigerated".Translate(new object[]
						{
							ticksUntilRotAtCurrentTemp.TicksToPeriodExtendedString()
						}));
					}
					else
					{
						stringBuilder.AppendLine("NotRefrigerated".Translate(new object[]
						{
							ticksUntilRotAtCurrentTemp.TicksToPeriodExtendedString()
						}));
					}
				}
			}
			return stringBuilder.ToString();
		}
	}
}
