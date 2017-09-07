using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class StatWorker
	{
		protected StatDef stat;
		public void InitSetStat(StatDef newStat)
		{
			this.stat = newStat;
		}
		public float GetValue(Thing thing, bool applyPostProcess = true)
		{
			return this.GetValue(StatRequest.For(thing), true);
		}
		public float GetValue(StatRequest req, bool applyPostProcess = true)
		{
			float valueUnfinalized = this.GetValueUnfinalized(req, applyPostProcess);
			this.FinalizeValue(req, ref valueUnfinalized, applyPostProcess);
			return valueUnfinalized;
		}
		public float GetValueAbstract(BuildableDef def, ThingDef stuffDef = null)
		{
			return this.GetValue(StatRequest.For(def, stuffDef), true);
		}
		public virtual float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
		{
			float num = this.stat.defaultBaseValue;
			if (req.Def.statBases != null)
			{
				for (int i = 0; i < req.Def.statBases.Count; i++)
				{
					if (req.Def.statBases[i].stat == this.stat)
					{
						num = req.Def.statBases[i].value;
						break;
					}
				}
			}
			Pawn pawn = req.Thing as Pawn;
			if (pawn != null)
			{
				if (pawn.story != null)
				{
					for (int j = 0; j < pawn.story.traits.allTraits.Count; j++)
					{
						num += pawn.story.traits.allTraits[j].OffsetOfStat(this.stat);
					}
				}
				if (pawn.apparel != null)
				{
					for (int k = 0; k < pawn.apparel.WornApparel.Count; k++)
					{
						num += StatWorker.StatOffsetFromGear(pawn.apparel.WornApparel[k], this.stat);
					}
				}
				if (pawn.equipment != null && pawn.equipment.Primary != null)
				{
					num += StatWorker.StatOffsetFromGear(pawn.equipment.Primary, this.stat);
				}
				num *= pawn.ageTracker.CurLifeStage.statFactors.GetStatFactorFromList(this.stat);
			}
			if (req.StuffDef != null && (num > 0f || this.stat.applyFactorsIfNegative))
			{
				num += req.StuffDef.stuffProps.statOffsets.GetStatOffsetFromList(this.stat);
				num *= req.StuffDef.stuffProps.statFactors.GetStatFactorFromList(this.stat);
			}
			if (req.HasThing)
			{
				CompAffectedByFacilities compAffectedByFacilities = req.Thing.TryGetComp<CompAffectedByFacilities>();
				if (compAffectedByFacilities != null)
				{
					num += compAffectedByFacilities.GetStatOffset(this.stat);
				}
				if (this.stat.statFactors != null)
				{
					for (int l = 0; l < this.stat.statFactors.Count; l++)
					{
						num *= req.Thing.GetStatValue(this.stat.statFactors[l], true);
					}
				}
				if (pawn != null)
				{
					if (this.stat.skillNeedFactors != null && pawn.skills != null)
					{
						for (int m = 0; m < this.stat.skillNeedFactors.Count; m++)
						{
							num *= this.stat.skillNeedFactors[m].FactorFor(pawn);
						}
					}
					if (this.stat.capacityFactors != null)
					{
						for (int n = 0; n < this.stat.capacityFactors.Count; n++)
						{
							PawnCapacityFactor pawnCapacityFactor = this.stat.capacityFactors[n];
							float factor = pawnCapacityFactor.GetFactor(pawn.health.capacities.GetEfficiency(pawnCapacityFactor.capacity));
							num = Mathf.Lerp(num, num * factor, pawnCapacityFactor.weight);
						}
					}
				}
			}
			return num;
		}
		public virtual string GetExplanation(StatRequest req, ToStringNumberSense numberSense)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("StatsReport_BaseValue".Translate());
			float statValueAbstract = req.Thing.def.GetStatValueAbstract(this.stat, null);
			stringBuilder.AppendLine("    " + this.stat.ValueToString(statValueAbstract, numberSense));
			Pawn pawn = req.Thing as Pawn;
			if (pawn != null)
			{
				if (pawn.RaceProps.intelligence >= Intelligence.ToolUser)
				{
					if (pawn.story != null && pawn.story.traits != null)
					{
						List<Trait> list = (
							from tr in pawn.story.traits.allTraits
							where tr.CurrentData.statOffsets != null && tr.CurrentData.statOffsets.Any((StatModifier se) => se.stat == this.stat)
							select tr).ToList<Trait>();
						List<Trait> list2 = (
							from tr in pawn.story.traits.allTraits
							where tr.CurrentData.statFactors != null && tr.CurrentData.statFactors.Any((StatModifier se) => se.stat == this.stat)
							select tr).ToList<Trait>();
						if (list.Count > 0 || list2.Count > 0)
						{
							stringBuilder.AppendLine();
							stringBuilder.AppendLine("StatsReport_RelevantTraits".Translate());
							for (int i = 0; i < list.Count; i++)
							{
								Trait trait = list[i];
								string toStringAsOffset = trait.CurrentData.statOffsets.First((StatModifier se) => se.stat == this.stat).ToStringAsOffset;
								stringBuilder.AppendLine("    " + trait.LabelCap + ": " + toStringAsOffset);
							}
							for (int j = 0; j < list2.Count; j++)
							{
								Trait trait2 = list2[j];
								string toStringAsFactor = trait2.CurrentData.statFactors.First((StatModifier se) => se.stat == this.stat).ToStringAsFactor;
								stringBuilder.AppendLine("    " + trait2.LabelCap + ": " + toStringAsFactor);
							}
						}
					}
					if (StatWorker.RelevantGear(pawn, this.stat).Any<Thing>())
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine("StatsReport_RelevantGear".Translate());
						if (pawn.apparel != null)
						{
							for (int k = 0; k < pawn.apparel.WornApparel.Count; k++)
							{
								Apparel gear = pawn.apparel.WornApparel[k];
								stringBuilder.AppendLine(StatWorker.InfoTextLineFromGear(gear, this.stat));
							}
						}
						if (pawn.equipment != null && pawn.equipment.Primary != null)
						{
							stringBuilder.AppendLine(StatWorker.InfoTextLineFromGear(pawn.equipment.Primary, this.stat));
						}
					}
				}
				float statFactorFromList = pawn.ageTracker.CurLifeStage.statFactors.GetStatFactorFromList(this.stat);
				if (statFactorFromList != 1f)
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"StatsReport_LifeStage".Translate(),
						" (",
						pawn.ageTracker.CurLifeStage.label,
						"): ",
						statFactorFromList.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor)
					}));
				}
			}
			if (req.StuffDef != null && (statValueAbstract > 0f || this.stat.applyFactorsIfNegative))
			{
				float statOffsetFromList = req.StuffDef.stuffProps.statOffsets.GetStatOffsetFromList(this.stat);
				if (statOffsetFromList != 0f)
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"StatsReport_Material".Translate(),
						" (",
						req.StuffDef.LabelCap,
						"): ",
						statOffsetFromList.ToStringByStyle(this.stat.toStringStyle, ToStringNumberSense.Offset)
					}));
				}
				float statFactorFromList2 = req.StuffDef.stuffProps.statFactors.GetStatFactorFromList(this.stat);
				if (statFactorFromList2 != 1f)
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"StatsReport_Material".Translate(),
						" (",
						req.StuffDef.LabelCap,
						"): ",
						statFactorFromList2.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor)
					}));
				}
			}
			CompAffectedByFacilities compAffectedByFacilities = req.Thing.TryGetComp<CompAffectedByFacilities>();
			if (compAffectedByFacilities != null)
			{
				compAffectedByFacilities.GetStatsExplanation(this.stat, stringBuilder);
			}
			if (this.stat.statFactors != null)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("StatsReport_OtherStats".Translate());
				for (int l = 0; l < this.stat.statFactors.Count; l++)
				{
					StatDef statDef = this.stat.statFactors[l];
					stringBuilder.AppendLine("    " + statDef.LabelCap + ": x" + statDef.Worker.GetValue(req, true).ToStringPercent());
				}
			}
			if (pawn != null)
			{
				if (this.stat.skillNeedFactors != null && !pawn.RaceProps.Animal && pawn.skills != null)
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendLine("StatsReport_Skills".Translate());
					for (int m = 0; m < this.stat.skillNeedFactors.Count; m++)
					{
						SkillNeed skillNeed = this.stat.skillNeedFactors[m];
						int level = pawn.skills.GetSkill(skillNeed.skill).level;
						stringBuilder.AppendLine(string.Concat(new object[]
						{
							"    ",
							skillNeed.skill.LabelCap,
							" (",
							level,
							"): x",
							skillNeed.FactorFor(pawn).ToStringPercent()
						}));
					}
				}
				if (this.stat.capacityFactors != null)
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendLine("StatsReport_HealthFactors".Translate());
					if (this.stat.capacityFactors != null)
					{
						foreach (PawnCapacityFactor current in 
							from hfa in this.stat.capacityFactors
							orderby hfa.capacity.listOrder
							select hfa)
						{
							string text = current.capacity.GetLabelFor(pawn).CapitalizeFirst();
							float factor = current.GetFactor(pawn.health.capacities.GetEfficiency(current.capacity));
							string text2 = factor.ToStringPercent();
							stringBuilder.AppendLine(string.Concat(new string[]
							{
								"    ",
								text,
								": x",
								text2,
								" (",
								"HealthFactorPercentImpact".Translate(new object[]
								{
									current.weight.ToStringPercent()
								}),
								")"
							}));
						}
					}
				}
			}
			return stringBuilder.ToString();
		}
		private void FinalizeValue(StatRequest req, ref float val, bool applyPostProcess)
		{
			if (this.stat.parts != null)
			{
				for (int i = 0; i < this.stat.parts.Count; i++)
				{
					this.stat.parts[i].TransformValue(req, ref val);
				}
			}
			if (applyPostProcess && this.stat.postProcessCurve != null)
			{
				val = this.stat.postProcessCurve.Evaluate(val);
			}
			if (Mathf.Abs(val) > this.stat.roundToFiveOver)
			{
				val = Mathf.Round(val / 5f) * 5f;
			}
			val = Mathf.Clamp(val, this.stat.minValue, this.stat.maxValue);
			if (this.stat.roundValue)
			{
				val = (float)Mathf.RoundToInt(val);
			}
		}
		public void FinalizeExplanation(StringBuilder sb, StatRequest req, ToStringNumberSense numberSense, float val)
		{
			if (this.stat.parts != null)
			{
				for (int i = 0; i < this.stat.parts.Count; i++)
				{
					string text = this.stat.parts[i].ExplanationPart(req);
					if (!text.NullOrEmpty())
					{
						sb.AppendLine(text);
						sb.AppendLine();
					}
				}
			}
			if (this.stat.postProcessCurve != null)
			{
				string text2 = this.stat.ValueToString(this.GetValue(req, false), numberSense);
				string text3 = this.stat.ValueToString(this.GetValue(req, true), numberSense);
				sb.AppendLine(string.Concat(new string[]
				{
					"StatsReport_PostProcessed".Translate(),
					": ",
					text2,
					" -> ",
					text3
				}));
				sb.AppendLine();
			}
			sb.AppendLine("StatsReport_FinalValue".Translate() + ": " + this.stat.ValueToString(val, this.stat.toStringNumberSense));
		}
		public virtual bool ShouldShowFor(BuildableDef eDef)
		{
			if (!this.stat.showIfUndefined && !eDef.statBases.StatListContains(this.stat))
			{
				return false;
			}
			ThingDef thingDef = eDef as ThingDef;
			if (thingDef != null && thingDef.category == ThingCategory.Pawn)
			{
				if (!this.stat.showOnPawns)
				{
					return false;
				}
				if (!this.stat.showOnHumanlikes && thingDef.race.Humanlike)
				{
					return false;
				}
				if (!this.stat.showOnAnimals && thingDef.race.Animal)
				{
					return false;
				}
				if (!this.stat.showOnMechanoids && thingDef.race.IsMechanoid)
				{
					return false;
				}
			}
			if (this.stat.category == StatCategoryDefOf.BasicsPawn || this.stat.category == StatCategoryDefOf.PawnCombat)
			{
				return thingDef != null && thingDef.category == ThingCategory.Pawn;
			}
			if (this.stat.category == StatCategoryDefOf.PawnMisc || this.stat.category == StatCategoryDefOf.PawnSocial || this.stat.category == StatCategoryDefOf.PawnWork)
			{
				return thingDef != null && thingDef.category == ThingCategory.Pawn && thingDef.race.Humanlike;
			}
			if (this.stat.category == StatCategoryDefOf.Building)
			{
				if (thingDef == null)
				{
					return false;
				}
				if (this.stat == StatDefOf.DoorOpenSpeed)
				{
					return thingDef.thingClass == typeof(Building_Door);
				}
				return thingDef.category == ThingCategory.Building;
			}
			else
			{
				if (this.stat.category == StatCategoryDefOf.Apparel)
				{
					return thingDef != null && (thingDef.IsApparel || thingDef.category == ThingCategory.Pawn);
				}
				if (this.stat.category == StatCategoryDefOf.Weapon)
				{
					return thingDef != null && (thingDef.IsMeleeWeapon || thingDef.IsRangedWeapon);
				}
				if (this.stat.category == StatCategoryDefOf.BasicsNonPawn)
				{
					return thingDef == null || thingDef.category != ThingCategory.Pawn;
				}
				if (this.stat.category.displayAllByDefault)
				{
					return true;
				}
				Log.Error(string.Concat(new object[]
				{
					"Unhandled case: ",
					this.stat,
					", ",
					eDef
				}));
				return false;
			}
		}
		public virtual string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq)
		{
			return stat.ValueToString(value, numberSense);
		}
		private static string InfoTextLineFromGear(Thing gear, StatDef stat)
		{
			float f = StatWorker.StatOffsetFromGear(gear, stat);
			return "    " + gear.LabelCap + ": " + f.ToStringByStyle(stat.toStringStyle, ToStringNumberSense.Offset);
		}
		private static float StatOffsetFromGear(Thing gear, StatDef stat)
		{
			return gear.def.equippedStatOffsets.GetStatOffsetFromList(stat);
		}
		[DebuggerHidden]
		private static IEnumerable<Thing> RelevantGear(Pawn pawn, StatDef stat)
		{
			StatWorker.<RelevantGear>c__Iterator122 <RelevantGear>c__Iterator = new StatWorker.<RelevantGear>c__Iterator122();
			<RelevantGear>c__Iterator.pawn = pawn;
			<RelevantGear>c__Iterator.stat = stat;
			<RelevantGear>c__Iterator.<$>pawn = pawn;
			<RelevantGear>c__Iterator.<$>stat = stat;
			StatWorker.<RelevantGear>c__Iterator122 expr_23 = <RelevantGear>c__Iterator;
			expr_23.$PC = -2;
			return expr_23;
		}
		private static bool GearAffectsStat(ThingDef gearDef, StatDef stat)
		{
			if (gearDef.equippedStatOffsets != null)
			{
				for (int i = 0; i < gearDef.equippedStatOffsets.Count; i++)
				{
					if (gearDef.equippedStatOffsets[i].stat == stat && gearDef.equippedStatOffsets[i].value != 0f)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
