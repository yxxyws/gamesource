using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Verse
{
	internal static class TableMaker
	{
		public static void DoTables_HealingQualityPerMedicine()
		{
			List<float> list = new List<float>();
			list.Add(0.5f);
			list.AddRange(
				from d in DefDatabase<ThingDef>.AllDefs
				where typeof(Medicine).IsAssignableFrom(d.thingClass)
				select d.GetStatValueAbstract(StatDefOf.MedicalPotency, null));
			SkillNeed_Direct skillNeed_Direct = (SkillNeed_Direct)StatDefOf.BaseHealingQuality.skillNeedFactors[0];
			TableDataGetter<float>[] array = new TableDataGetter<float>[21];
			array[0] = new TableDataGetter<float>("potency", (float p) => p.ToStringPercent());
			for (int i = 0; i < 20; i++)
			{
				float factor = skillNeed_Direct.factorsPerLevel[i];
				array[i + 1] = new TableDataGetter<float>((i + 1).ToString(), (float p) => (p * factor).ToStringPercent());
			}
			DebugTables.MakeTablesDialog<float>(list, array);
		}
		public static void DoTables_MeleeBalanceStuffless()
		{
			TableMaker.DoTables_MeleeBalance(null);
		}
		public static void DoTables_MeleeBalancePlasteel()
		{
			TableMaker.DoTables_MeleeBalance(ThingDefOf.Plasteel);
		}
		public static void DoTables_BuildingFillpercents()
		{
			TableMaker.DoTables_FillPercents(ThingCategory.Building);
		}
		public static void DoTables_ItemFillpercents()
		{
			TableMaker.DoTables_FillPercents(ThingCategory.Item);
		}
		public static void DoTables_RangedWeaponBalance()
		{
			Func<ThingDef, int> damageGetter = (ThingDef d) => (d.Verbs[0].projectileDef == null) ? 0 : d.Verbs[0].projectileDef.projectile.damageAmountBase;
			Func<ThingDef, float> warmupGetter = (ThingDef d) => (float)d.Verbs[0].warmupTicks / 60f;
			Func<ThingDef, float> cooldownGetter = (ThingDef d) => d.GetStatValueAbstract(StatDefOf.RangedWeapon_Cooldown, null);
			Func<ThingDef, int> burstShotsGetter = (ThingDef d) => d.Verbs[0].burstShotCount;
			Func<ThingDef, float> dpsRawGetter = delegate(ThingDef d)
			{
				int num = burstShotsGetter(d);
				float num2 = warmupGetter(d) + cooldownGetter(d);
				num2 += (float)(num - 1) * ((float)d.Verbs[0].ticksBetweenBurstShots / 60f);
				return (float)(damageGetter(d) * num) / num2;
			};
			Func<ThingDef, float> accTouchGetter = (ThingDef d) => d.GetStatValueAbstract(StatDefOf.AccuracyTouch, null);
			Func<ThingDef, float> accShortGetter = (ThingDef d) => d.GetStatValueAbstract(StatDefOf.AccuracyShort, null);
			Func<ThingDef, float> accMedGetter = (ThingDef d) => d.GetStatValueAbstract(StatDefOf.AccuracyMedium, null);
			Func<ThingDef, float> accLongGetter = (ThingDef d) => d.GetStatValueAbstract(StatDefOf.AccuracyLong, null);
			IEnumerable<ThingDef> arg_2FD_0 = 
				from d in DefDatabase<ThingDef>.AllDefs
				where d.IsRangedWeapon
				select d;
			TableDataGetter<ThingDef>[] expr_15E = new TableDataGetter<ThingDef>[15];
			expr_15E[0] = new TableDataGetter<ThingDef>("defName", (ThingDef d) => d.defName);
			expr_15E[1] = new TableDataGetter<ThingDef>("damage", (ThingDef d) => damageGetter(d).ToString());
			expr_15E[2] = new TableDataGetter<ThingDef>("warmup", (ThingDef d) => warmupGetter(d).ToString("F2"));
			expr_15E[3] = new TableDataGetter<ThingDef>("burst", (ThingDef d) => burstShotsGetter(d).ToString());
			expr_15E[4] = new TableDataGetter<ThingDef>("cooldown", (ThingDef d) => cooldownGetter(d).ToString("F2"));
			expr_15E[5] = new TableDataGetter<ThingDef>("dpsRaw", (ThingDef d) => dpsRawGetter(d).ToString("F3"));
			expr_15E[6] = new TableDataGetter<ThingDef>("accTouch", (ThingDef d) => accTouchGetter(d).ToStringPercent());
			expr_15E[7] = new TableDataGetter<ThingDef>("accShort", (ThingDef d) => accShortGetter(d).ToStringPercent());
			expr_15E[8] = new TableDataGetter<ThingDef>("accMed", (ThingDef d) => accMedGetter(d).ToStringPercent());
			expr_15E[9] = new TableDataGetter<ThingDef>("accLong", (ThingDef d) => accLongGetter(d).ToStringPercent());
			expr_15E[10] = new TableDataGetter<ThingDef>("dpsTouch", (ThingDef d) => (dpsRawGetter(d) * accTouchGetter(d)).ToString("F2"));
			expr_15E[11] = new TableDataGetter<ThingDef>("dpsShort", (ThingDef d) => (dpsRawGetter(d) * accShortGetter(d)).ToString("F2"));
			expr_15E[12] = new TableDataGetter<ThingDef>("dpsMed", (ThingDef d) => (dpsRawGetter(d) * accMedGetter(d)).ToString("F2"));
			expr_15E[13] = new TableDataGetter<ThingDef>("dpsLong", (ThingDef d) => (dpsRawGetter(d) * accLongGetter(d)).ToString("F2"));
			expr_15E[14] = new TableDataGetter<ThingDef>("mktval", (ThingDef d) => d.GetStatValueAbstract(StatDefOf.MarketValue, null).ToString("F0"));
			DebugTables.MakeTablesDialog<ThingDef>(arg_2FD_0, expr_15E);
		}
		public static void DoTables_Nutritions()
		{
			IEnumerable<ThingDef> arg_81_0 = 
				from d in DefDatabase<ThingDef>.AllDefs
				where d.ingestible != null
				select d;
			TableDataGetter<ThingDef>[] expr_2D = new TableDataGetter<ThingDef>[2];
			expr_2D[0] = new TableDataGetter<ThingDef>("defName", (ThingDef d) => d.defName);
			expr_2D[1] = new TableDataGetter<ThingDef>("nutrition", (ThingDef d) => d.ingestible.nutrition.ToString("F2"));
			DebugTables.MakeTablesDialog<ThingDef>(arg_81_0, expr_2D);
		}
		public static void DoTables_RacesBasics()
		{
			Func<PawnKindDef, float> dps = delegate(PawnKindDef d)
			{
				if (d.race.Verbs.NullOrEmpty<VerbProperties>())
				{
					return 0f;
				}
				IEnumerable<VerbProperties> source = 
					from v in d.race.Verbs
					where (float)v.meleeDamageBaseAmount > 0.001f
					select v;
				float num = source.Sum((VerbProperties v) => (float)v.meleeDamageBaseAmount / (float)v.defaultCooldownTicks * 60f);
				return num / (float)source.Count<VerbProperties>();
			};
			Func<PawnKindDef, float> pointsGuess = delegate(PawnKindDef d)
			{
				float num = 15f;
				num += dps(d) * 10f;
				num *= Mathf.Lerp(1f, d.race.GetStatValueAbstract(StatDefOf.MoveSpeed, null) / 3f, 0.25f);
				num *= d.RaceProps.baseHealthScale;
				num *= GenMath.LerpDouble(0.25f, 1f, 1.65f, 1f, Mathf.Clamp(d.RaceProps.baseBodySize, 0.25f, 1f));
				return num * 0.76f;
			};
			Func<PawnKindDef, float> mktValGuess = delegate(PawnKindDef d)
			{
				float num = 18f;
				num += pointsGuess(d) * 2.7f;
				switch (d.RaceProps.trainableIntelligence)
				{
				case TrainableIntelligence.None:
					num *= 0.5f;
					break;
				case TrainableIntelligence.Simple:
					num *= 0.8f;
					break;
				case TrainableIntelligence.Intermediate:
					break;
				case TrainableIntelligence.Advanced:
					num += 250f;
					break;
				default:
					throw new InvalidOperationException();
				}
				num += d.RaceProps.baseBodySize * 80f;
				if (d.race.HasComp(typeof(CompMilkable)))
				{
					num += 125f;
				}
				if (d.race.HasComp(typeof(CompShearable)))
				{
					num += 90f;
				}
				if (d.race.HasComp(typeof(CompEggLayer)))
				{
					num += 90f;
				}
				num *= Mathf.Lerp(0.8f, 1.2f, d.RaceProps.wildness);
				return num * 0.75f;
			};
			IEnumerable<PawnKindDef> arg_2C4_0 = 
				from d in DefDatabase<PawnKindDef>.AllDefs
				where d.race != null && !d.RaceProps.Humanlike
				select d;
			TableDataGetter<PawnKindDef>[] expr_7B = new TableDataGetter<PawnKindDef>[15];
			expr_7B[0] = new TableDataGetter<PawnKindDef>("defName", (PawnKindDef d) => d.defName);
			expr_7B[1] = new TableDataGetter<PawnKindDef>("points", (PawnKindDef d) => d.combatPower.ToString("F0"));
			expr_7B[2] = new TableDataGetter<PawnKindDef>("points guess", (PawnKindDef d) => pointsGuess(d).ToString("F0"));
			expr_7B[3] = new TableDataGetter<PawnKindDef>("mktval", (PawnKindDef d) => d.race.GetStatValueAbstract(StatDefOf.MarketValue, null).ToString("F0"));
			expr_7B[4] = new TableDataGetter<PawnKindDef>("mktval guess", (PawnKindDef d) => mktValGuess(d).ToString("F0"));
			expr_7B[5] = new TableDataGetter<PawnKindDef>("healthScale", (PawnKindDef d) => d.RaceProps.baseHealthScale.ToString("F2"));
			expr_7B[6] = new TableDataGetter<PawnKindDef>("bodySize", (PawnKindDef d) => d.RaceProps.baseBodySize.ToString("F2"));
			expr_7B[7] = new TableDataGetter<PawnKindDef>("hunger rate", (PawnKindDef d) => d.RaceProps.baseHungerRate.ToString("F2"));
			expr_7B[8] = new TableDataGetter<PawnKindDef>("speed", (PawnKindDef d) => d.race.GetStatValueAbstract(StatDefOf.MoveSpeed, null).ToString("F2"));
			expr_7B[9] = new TableDataGetter<PawnKindDef>("melee dps", (PawnKindDef d) => dps(d).ToString("F2"));
			expr_7B[10] = new TableDataGetter<PawnKindDef>("wildness", (PawnKindDef d) => d.RaceProps.wildness.ToStringPercent());
			expr_7B[11] = new TableDataGetter<PawnKindDef>("life expec.", (PawnKindDef d) => d.RaceProps.lifeExpectancy.ToString("F1"));
			expr_7B[12] = new TableDataGetter<PawnKindDef>("train-int", (PawnKindDef d) => d.RaceProps.trainableIntelligence.GetLabel());
			expr_7B[13] = new TableDataGetter<PawnKindDef>("temps", (PawnKindDef d) => d.race.GetStatValueAbstract(StatDefOf.ComfyTemperatureMin, null).ToString("F0") + ".." + d.race.GetStatValueAbstract(StatDefOf.ComfyTemperatureMax, null).ToString("F0"));
			expr_7B[14] = new TableDataGetter<PawnKindDef>("mateCPH", (PawnKindDef d) => d.RaceProps.mateChancePerHour.ToStringPercent("F2"));
			DebugTables.MakeTablesDialog<PawnKindDef>(arg_2C4_0, expr_7B);
		}
		public static void DoTables_PlantsBasics()
		{
			IEnumerable<ThingDef> arg_D5_0 = 
				from d in DefDatabase<ThingDef>.AllDefs
				where d.category == ThingCategory.Plant
				select d;
			TableDataGetter<ThingDef>[] expr_2D = new TableDataGetter<ThingDef>[4];
			expr_2D[0] = new TableDataGetter<ThingDef>("defName", (ThingDef d) => d.defName);
			expr_2D[1] = new TableDataGetter<ThingDef>("growDays", (ThingDef d) => d.plant.growDays.ToString("F2"));
			expr_2D[2] = new TableDataGetter<ThingDef>("nutrition", (ThingDef d) => (d.ingestible == null) ? "-" : d.ingestible.nutrition.ToString("F2"));
			expr_2D[3] = new TableDataGetter<ThingDef>("nut/day", (ThingDef d) => (d.ingestible == null) ? "-" : (d.ingestible.nutrition / d.plant.growDays).ToString("F4"));
			DebugTables.MakeTablesDialog<ThingDef>(arg_D5_0, expr_2D);
		}
		private static void DoTables_FillPercents(ThingCategory cat)
		{
			IEnumerable<ThingDef> arg_7D_0 = 
				from d in DefDatabase<ThingDef>.AllDefs
				where d.category == cat && !d.IsFrame && d.passability != Traversability.Impassable
				select d;
			TableDataGetter<ThingDef>[] expr_29 = new TableDataGetter<ThingDef>[2];
			expr_29[0] = new TableDataGetter<ThingDef>("defName", (ThingDef d) => d.defName);
			expr_29[1] = new TableDataGetter<ThingDef>("fillPercent", (ThingDef d) => d.fillPercent.ToStringPercent());
			DebugTables.MakeTablesDialog<ThingDef>(arg_7D_0, expr_29);
		}
		private static void DoTables_MeleeBalance(ThingDef stuff)
		{
			Func<Def, float> damageGetter = delegate(Def d)
			{
				ThingDef thingDef = d as ThingDef;
				if (thingDef != null)
				{
					return thingDef.GetStatValueAbstract(StatDefOf.MeleeWeapon_DamageAmount, stuff);
				}
				HediffDef hediffDef = d as HediffDef;
				if (hediffDef != null)
				{
					return (float)hediffDef.CompPropsFor(typeof(HediffComp_VerbGiver)).verbs[0].meleeDamageBaseAmount;
				}
				return -1f;
			};
			Func<Def, float> warmupGetter = delegate(Def d)
			{
				ThingDef thingDef = d as ThingDef;
				if (thingDef != null)
				{
					return (float)thingDef.Verbs[0].warmupTicks / 60f;
				}
				HediffDef hediffDef = d as HediffDef;
				if (hediffDef != null)
				{
					return (float)hediffDef.CompPropsFor(typeof(HediffComp_VerbGiver)).verbs[0].warmupTicks / 60f;
				}
				return -1f;
			};
			Func<Def, float> cooldownGetter = delegate(Def d)
			{
				ThingDef thingDef = d as ThingDef;
				if (thingDef != null)
				{
					return thingDef.GetStatValueAbstract(StatDefOf.MeleeWeapon_Cooldown, stuff);
				}
				HediffDef hediffDef = d as HediffDef;
				if (hediffDef != null)
				{
					return (float)hediffDef.CompPropsFor(typeof(HediffComp_VerbGiver)).verbs[0].defaultCooldownTicks / 60f;
				}
				return -1f;
			};
			Func<Def, float> dpsGetter = (Def d) => damageGetter(d) / (warmupGetter(d) + cooldownGetter(d));
			Func<Def, float> marketValueGetter = delegate(Def d)
			{
				ThingDef thingDef = d as ThingDef;
				if (thingDef != null)
				{
					return thingDef.GetStatValueAbstract(StatDefOf.MarketValue, stuff);
				}
				HediffDef hediffDef = d as HediffDef;
				if (hediffDef == null)
				{
					return -1f;
				}
				if (hediffDef.spawnThingOnRemoved == null)
				{
					return 0f;
				}
				return hediffDef.spawnThingOnRemoved.GetStatValueAbstract(StatDefOf.MarketValue, null);
			};
			IEnumerable<Def> enumerable = (
				from d in DefDatabase<ThingDef>.AllDefs
				where d.IsMeleeWeapon
				select d).Cast<Def>().Concat((
				from h in DefDatabase<HediffDef>.AllDefs
				where h.CompPropsFor(typeof(HediffComp_VerbGiver)) != null
				select h).Cast<Def>());
			IEnumerable<Def> arg_184_0 = enumerable;
			TableDataGetter<Def>[] expr_DD = new TableDataGetter<Def>[6];
			expr_DD[0] = new TableDataGetter<Def>("defName", (Def d) => d.defName);
			expr_DD[1] = new TableDataGetter<Def>("damage", (Def d) => damageGetter(d).ToString());
			expr_DD[2] = new TableDataGetter<Def>("warmup", (Def d) => warmupGetter(d).ToString("F2"));
			expr_DD[3] = new TableDataGetter<Def>("cooldown", (Def d) => cooldownGetter(d).ToString("F2"));
			expr_DD[4] = new TableDataGetter<Def>("dps", (Def d) => dpsGetter(d).ToString("F2"));
			expr_DD[5] = new TableDataGetter<Def>("mktval", (Def d) => marketValueGetter(d).ToString("F0"));
			DebugTables.MakeTablesDialog<Def>(arg_184_0, expr_DD);
		}
	}
}
