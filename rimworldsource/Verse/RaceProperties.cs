using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
namespace Verse
{
	public class RaceProperties
	{
		public Intelligence intelligence;
		public FleshType fleshType;
		private ThingDef bloodDef;
		public bool hasGenders = true;
		public bool needsRest = true;
		public ThinkTreeDef thinkTreeMain;
		public ThinkTreeDef thinkTreeConstant;
		public PawnNameCategory nameCategory;
		public FoodTypeFlags foodType;
		public FoodPreferability minFoodPreferability;
		public BodyDef body;
		public string meatLabel;
		public Color meatColor;
		public Type deathActionWorkerClass;
		public List<BiomeAnimalRecord> wildBiomes;
		public SimpleCurve ageGenerationCurve;
		public bool makesFootprints;
		public int executionRange;
		public float lifeExpectancy;
		public List<HediffGiverSetDef> hediffGiverSets;
		public float petness;
		public bool herdAnimal;
		public bool predator;
		public float maxPreyBodySize;
		public float wildness;
		public float nuzzleChancePerHour;
		public float nuzzlePower;
		public float manhunterOnDamageChance;
		public float manhunterOnTameFailChance;
		public float gestationPeriodDays;
		public SimpleCurve litterSizeCurve;
		public float mateChancePerHour;
		public List<string> untrainableTags;
		public List<string> trainableTags;
		public TrainableIntelligence trainableIntelligence;
		public RulePackDef nameGenerator;
		public float nameOnTameChance;
		public float nameOnNuzzleChance;
		public float baseBodySize;
		public float baseHealthScale;
		public float baseHungerRate;
		public List<LifeStageAge> lifeStageAges;
		public Color leatherColor;
		public string leatherLabel;
		public float leatherCommonalityFactor;
		public float leatherInsulation;
		public List<StatModifier> leatherStatFactors;
		public float leatherMarketValue;
		public ShadowData specialShadowData;
		public IntRange soundCallIntervalRange;
		public SoundDef soundMeleeHitPawn;
		public SoundDef soundMeleeHitBuilding;
		public SoundDef soundMeleeMiss;
		[Unsaved]
		private DeathActionWorker deathActionWorkerInt;
		[Unsaved]
		public ThingDef meatDef;
		[Unsaved]
		public ThingDef leatherDef;
		[Unsaved]
		public ThingDef corpseDef;
		public bool Humanlike
		{
			get
			{
				return this.intelligence >= Intelligence.Humanlike;
			}
		}
		public bool ToolUser
		{
			get
			{
				return this.intelligence >= Intelligence.ToolUser;
			}
		}
		public bool Animal
		{
			get
			{
				return !this.ToolUser && this.IsFlesh;
			}
		}
		public bool EatsFood
		{
			get
			{
				return this.foodType != FoodTypeFlags.None;
			}
		}
		public float FoodLevelPercentageWantEat
		{
			get
			{
				switch (this.ResolvedDietCategory)
				{
				case DietCategory.NeverEats:
					return 0.5f;
				case DietCategory.Herbivorous:
					return 0.65f;
				case DietCategory.Dendrovorous:
					return 0.65f;
				case DietCategory.Ovivorous:
					return 0.5f;
				case DietCategory.Omnivorous:
					return 0.3f;
				case DietCategory.Carnivorous:
					return 0.3f;
				default:
					throw new InvalidOperationException();
				}
			}
		}
		public DietCategory ResolvedDietCategory
		{
			get
			{
				if (!this.EatsFood)
				{
					return DietCategory.NeverEats;
				}
				if (this.Eats(FoodTypeFlags.Tree))
				{
					return DietCategory.Dendrovorous;
				}
				if (this.Eats(FoodTypeFlags.Meat))
				{
					if (this.Eats(FoodTypeFlags.VegetableOrFruit) || this.Eats(FoodTypeFlags.Plant))
					{
						return DietCategory.Omnivorous;
					}
					return DietCategory.Carnivorous;
				}
				else
				{
					if (this.Eats(FoodTypeFlags.AnimalProduct))
					{
						return DietCategory.Ovivorous;
					}
					return DietCategory.Herbivorous;
				}
			}
		}
		public DeathActionWorker DeathActionWorker
		{
			get
			{
				if (this.deathActionWorkerInt == null && this.deathActionWorkerClass != null)
				{
					this.deathActionWorkerInt = (DeathActionWorker)Activator.CreateInstance(this.deathActionWorkerClass);
				}
				return this.deathActionWorkerInt;
			}
		}
		public bool IsMechanoid
		{
			get
			{
				return this.fleshType == FleshType.Mechanoid;
			}
		}
		public bool IsFlesh
		{
			get
			{
				return this.fleshType != FleshType.Mechanoid;
			}
		}
		public ThingDef BloodDef
		{
			get
			{
				if (this.bloodDef != null)
				{
					return this.bloodDef;
				}
				if (this.IsFlesh)
				{
					return ThingDefOf.FilthBlood;
				}
				return null;
			}
		}
		public RaceProperties()
		{
			ColorInt colorInt = new ColorInt(141, 56, 52);
			this.meatColor = colorInt.ToColor;
			this.executionRange = 2;
			this.lifeExpectancy = 10f;
			this.maxPreyBodySize = 99999f;
			this.wildness = -1f;
			this.nuzzleChancePerHour = 0.2f;
			this.nuzzlePower = 1f;
			this.gestationPeriodDays = 10f;
			this.mateChancePerHour = 0.1f;
			this.trainableIntelligence = TrainableIntelligence.Intermediate;
			this.baseBodySize = 1f;
			this.baseHealthScale = 1f;
			this.baseHungerRate = 1f;
			this.lifeStageAges = new List<LifeStageAge>();
			this.leatherColor = ColorLibrary.Leather;
			this.leatherCommonalityFactor = 1f;
			this.leatherInsulation = 1.1f;
			this.leatherMarketValue = 3.5f;
			this.soundCallIntervalRange = new IntRange(2000, 4000);
			base..ctor();
		}
		public bool WillAutomaticallyEat(Thing t)
		{
			return t.def.ingestible != null && t.def.ingestible.preferability >= this.minFoodPreferability && this.CanEverEat(t);
		}
		public bool CanEverEat(Thing t)
		{
			return this.CanEverEat(t.def);
		}
		public bool CanEverEat(ThingDef t)
		{
			return this.EatsFood && t.ingestible != null && t.ingestible.preferability != FoodPreferability.Undefined && this.Eats(t.ingestible.foodType);
		}
		public bool Eats(FoodTypeFlags food)
		{
			return this.EatsFood && (this.foodType & food) != FoodTypeFlags.None;
		}
		[DebuggerHidden]
		public IEnumerable<string> ConfigErrors()
		{
			RaceProperties.<ConfigErrors>c__Iterator138 <ConfigErrors>c__Iterator = new RaceProperties.<ConfigErrors>c__Iterator138();
			<ConfigErrors>c__Iterator.<>f__this = this;
			RaceProperties.<ConfigErrors>c__Iterator138 expr_0E = <ConfigErrors>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		[DebuggerHidden]
		internal IEnumerable<StatDrawEntry> SpecialDisplayStats(ThingDef parentDef)
		{
			RaceProperties.<SpecialDisplayStats>c__Iterator139 <SpecialDisplayStats>c__Iterator = new RaceProperties.<SpecialDisplayStats>c__Iterator139();
			<SpecialDisplayStats>c__Iterator.parentDef = parentDef;
			<SpecialDisplayStats>c__Iterator.<$>parentDef = parentDef;
			<SpecialDisplayStats>c__Iterator.<>f__this = this;
			RaceProperties.<SpecialDisplayStats>c__Iterator139 expr_1C = <SpecialDisplayStats>c__Iterator;
			expr_1C.$PC = -2;
			return expr_1C;
		}
	}
}
