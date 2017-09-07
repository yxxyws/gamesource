using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse
{
	public class PawnKindDef : Def
	{
		public ThingDef race;
		public FactionDef defaultFactionType;
		public string backstoryCategory;
		public float backstoryCryptosleepCommonality;
		public bool forceNormalGearQuality;
		public int minGenerationAge;
		public int maxGenerationAge = 999999;
		public FloatRange gearHealthRange = FloatRange.One;
		public bool factionLeader;
		public List<PawnKindLifeStage> lifeStages = new List<PawnKindLifeStage>();
		public bool isFighter = true;
		public float combatPower = -1f;
		public float baseRecruitDifficulty = 50f;
		public bool aiAvoidCover;
		public FloatRange fleeHealthThresholdRange = new FloatRange(-0.4f, 0.4f);
		public FloatRange apparelMoney = FloatRange.Zero;
		public List<ThingDef> apparelRequired;
		public List<string> apparelTags;
		public float apparelAllowHeadwearChance = 1f;
		public bool apparelIgnoreSeasons;
		public FloatRange weaponMoney = FloatRange.Zero;
		public List<string> weaponTags;
		public FloatRange techHediffsMoney = FloatRange.Zero;
		public List<string> techHediffsTags;
		public float techHediffsChance = 0.1f;
		public QualityCategory itemQuality = QualityCategory.Normal;
		public List<ThingCount> fixedInventory = new List<ThingCount>();
		public PawnInventoryOption inventoryOptions;
		public int invMealCount;
		public bool carrier;
		public bool trader;
		public string labelMale;
		public string labelFemale;
		public bool wildSpawn_spawnWild;
		public float wildSpawn_EcoSystemWeight = 1f;
		public IntRange wildSpawn_GroupSizeRange = IntRange.one;
		public RaceProperties RaceProps
		{
			get
			{
				return this.race.race;
			}
		}
		public override void ResolveReferences()
		{
			base.ResolveReferences();
			for (int i = 0; i < this.lifeStages.Count; i++)
			{
				this.lifeStages[i].ResolveReferences();
			}
		}
		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			PawnKindDef.<ConfigErrors>c__Iterator141 <ConfigErrors>c__Iterator = new PawnKindDef.<ConfigErrors>c__Iterator141();
			<ConfigErrors>c__Iterator.<>f__this = this;
			PawnKindDef.<ConfigErrors>c__Iterator141 expr_0E = <ConfigErrors>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public static PawnKindDef Named(string defName)
		{
			return DefDatabase<PawnKindDef>.GetNamed(defName, true);
		}
	}
}
