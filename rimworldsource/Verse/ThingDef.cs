using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
namespace Verse
{
	public class ThingDef : BuildableDef
	{
		private const float SmallVolumePerUnit = 0.05f;
		public Type thingClass;
		public ThingCategory category;
		public TickerType tickerType;
		public bool useHitPoints = true;
		public int stackLimit = 1;
		public List<CompProperties> comps = new List<CompProperties>();
		public bool destroyable = true;
		public IntVec2 size = new IntVec2(1, 1);
		public bool rotatable = true;
		public bool smallVolume;
		public List<ThingCount> killedLeavings = new List<ThingCount>();
		public List<ThingCount> butcherProducts;
		public List<ThingCount> smeltProducts;
		public bool randomizeRotationOnSpawn;
		public List<DamageMultiplier> damageMultipliers;
		public bool isBodyPartOrImplant;
		public List<StuffCategoryDef> stuffCategories;
		public RecipeMakerProperties recipeMaker;
		public ThingDef minifiedDef;
		public bool isUnfinishedThing;
		public bool leaveResourcesWhenKilled;
		public float resourcesFractionWhenDeconstructed = 0.75f;
		public ThingDef slagDef;
		public bool isFrame;
		public IntVec3 interactionCellOffset = new IntVec3(0, 0, 0);
		public bool hasInteractionCell;
		public ThingDef filthLeaving;
		public bool forceDebugSpawnable;
		public GraphicData graphicData;
		public DrawerType drawerType = DrawerType.RealtimeOnly;
		public bool drawOffscreen;
		public ColorGenerator colorGenerator;
		public float hideAtSnowDepth = 99999f;
		public bool drawDamagedOverlay = true;
		public bool castEdgeShadows;
		public float staticSunShadowHeight;
		public bool selectable;
		public bool neverMultiSelect;
		public bool isAutoAttackableWorldObject;
		public bool hasTooltip;
		public List<Type> inspectorTabs = new List<Type>();
		[Unsaved]
		public List<ITab> inspectorTabsResolved;
		public bool seeThroughFog;
		public bool drawGUIOverlay;
		public ResourceCountPriority resourceReadoutPriority;
		public bool resourceReadoutAlwaysShow;
		public bool drawPlaceWorkersWhileSelected;
		public bool alwaysHaulable;
		public bool designateHaulable;
		public List<ThingCategoryDef> thingCategories;
		public bool mineable;
		public bool socialPropernessMatters;
		public SoundDef soundDrop;
		public SoundDef soundPickup;
		public SoundDef soundInteract;
		public SoundDef soundImpactDefault;
		public bool saveCompressible;
		public bool isSaveable = true;
		public bool holdsRoof;
		public float fillPercent;
		public bool coversFloor;
		public bool neverOverlapFloors;
		public SurfaceType surfaceType;
		public bool blockPlants;
		public bool blockLight;
		[Unsaved]
		public bool regionBarrier;
		public Tradeability tradeability = Tradeability.Stockable;
		public List<string> tradeTags;
		public bool tradeNeverStack;
		public ColorGenerator colorGeneratorInTraderStock;
		public Type blueprintClass = typeof(Blueprint_Build);
		public GraphicData blueprintGraphicData;
		public TerrainDef naturalTerrain;
		public TerrainDef leaveTerrain;
		public List<RecipeDef> recipes;
		private List<VerbProperties> verbs;
		public float equippedAngleOffset;
		public EquipmentType equipmentType;
		public TechLevel techLevel;
		public List<string> weaponTags;
		public List<string> techHediffsTags;
		public bool canBeSpawningInventory = true;
		public bool destroyOnDrop;
		public List<StatModifier> equippedStatOffsets;
		public BuildableDef entityDefToBuild;
		public ThingDef seed_PlantDefToMake;
		public IngestibleProperties ingestible;
		public FilthProperties filth;
		public BuildingProperties building;
		public RaceProperties race;
		public ApparelProperties apparel;
		public MoteProperties mote;
		public PlantProperties plant;
		public ProjectileProperties projectile;
		public StuffProperties stuffProps;
		public ArtifactProperties artifact;
		private List<RecipeDef> allRecipesCached;
		private static List<VerbProperties> EmptyVerbPropertiesList = new List<VerbProperties>();
		public bool EverHaulable
		{
			get
			{
				return this.alwaysHaulable || this.designateHaulable;
			}
		}
		public bool EverStoreable
		{
			get
			{
				return !this.thingCategories.NullOrEmpty<ThingCategoryDef>();
			}
		}
		public bool IsNutritionSource
		{
			get
			{
				return this.ingestible != null && this.ingestible.nutrition > 0f;
			}
		}
		public bool IsApparel
		{
			get
			{
				return this.apparel != null;
			}
		}
		public float VolumePerUnit
		{
			get
			{
				return this.smallVolume ? 0.05f : 1f;
			}
		}
		public override Color IconDrawColor
		{
			get
			{
				return this.graphicData.color;
			}
		}
		public override IntVec2 Size
		{
			get
			{
				return this.size;
			}
		}
		public int BaseMaxHitPoints
		{
			get
			{
				return Mathf.RoundToInt(this.GetStatValueAbstract(StatDefOf.MaxHitPoints, null));
			}
		}
		public float BaseFlammability
		{
			get
			{
				return this.GetStatValueAbstract(StatDefOf.Flammability, null);
			}
		}
		public float BaseMarketValue
		{
			get
			{
				return this.GetStatValueAbstract(StatDefOf.MarketValue, null);
			}
			set
			{
				this.SetStatBaseValue(StatDefOf.MarketValue, value);
			}
		}
		public bool IsBed
		{
			get
			{
				return typeof(Building_Bed).IsAssignableFrom(this.thingClass);
			}
		}
		public bool IsFrame
		{
			get
			{
				return this.isFrame;
			}
		}
		public bool IsBlueprint
		{
			get
			{
				return this.entityDefToBuild != null && this.category == ThingCategory.Ethereal;
			}
		}
		public bool IsStuff
		{
			get
			{
				return this.stuffProps != null;
			}
		}
		public bool MadeFromStuff
		{
			get
			{
				return !this.stuffCategories.NullOrEmpty<StuffCategoryDef>();
			}
		}
		public bool EverTransmitsPower
		{
			get
			{
				for (int i = 0; i < this.comps.Count; i++)
				{
					CompProperties_Power compProperties_Power = this.comps[i] as CompProperties_Power;
					if (compProperties_Power != null && compProperties_Power.transmitsPower)
					{
						return true;
					}
				}
				return false;
			}
		}
		public bool Minifiable
		{
			get
			{
				return this.minifiedDef != null;
			}
		}
		public bool HasThingIDNumber
		{
			get
			{
				return this.category != ThingCategory.Mote;
			}
		}
		public List<RecipeDef> AllRecipes
		{
			get
			{
				if (this.allRecipesCached == null)
				{
					this.allRecipesCached = new List<RecipeDef>();
					if (this.recipes != null)
					{
						for (int i = 0; i < this.recipes.Count; i++)
						{
							this.allRecipesCached.Add(this.recipes[i]);
						}
					}
					List<RecipeDef> allDefsListForReading = DefDatabase<RecipeDef>.AllDefsListForReading;
					for (int j = 0; j < allDefsListForReading.Count; j++)
					{
						if (allDefsListForReading[j].recipeUsers != null && allDefsListForReading[j].recipeUsers.Contains(this))
						{
							this.allRecipesCached.Add(allDefsListForReading[j]);
						}
					}
				}
				return this.allRecipesCached;
			}
		}
		public bool ConnectToPower
		{
			get
			{
				if (this.EverTransmitsPower)
				{
					return false;
				}
				for (int i = 0; i < this.comps.Count; i++)
				{
					if (this.comps[i].compClass == typeof(CompPowerBattery))
					{
						return true;
					}
					if (this.comps[i].compClass == typeof(CompPowerTrader))
					{
						return true;
					}
				}
				return false;
			}
		}
		public bool CoexistsWithFloors
		{
			get
			{
				return !this.neverOverlapFloors && !this.coversFloor;
			}
		}
		public FillCategory Fillage
		{
			get
			{
				if (this.fillPercent < 0.01f)
				{
					return FillCategory.None;
				}
				if (this.fillPercent > 0.99f)
				{
					return FillCategory.Full;
				}
				return FillCategory.Partial;
			}
		}
		public bool IsWeapon
		{
			get
			{
				return !this.verbs.NullOrEmpty<VerbProperties>() && this.category == ThingCategory.Item;
			}
		}
		public bool IsRangedWeapon
		{
			get
			{
				if (!this.IsWeapon)
				{
					return false;
				}
				for (int i = 0; i < this.verbs.Count; i++)
				{
					if (!this.verbs[i].MeleeRange)
					{
						return true;
					}
				}
				return false;
			}
		}
		public bool IsMeleeWeapon
		{
			get
			{
				if (!this.IsWeapon)
				{
					return false;
				}
				for (int i = 0; i < this.verbs.Count; i++)
				{
					if (this.verbs[i].MeleeRange)
					{
						return true;
					}
				}
				return false;
			}
		}
		public bool MakeFog
		{
			get
			{
				return this.Fillage == FillCategory.Full;
			}
		}
		public bool CanOverlapZones
		{
			get
			{
				if (this.building != null && this.building.SupportsPlants)
				{
					return false;
				}
				if (this.passability == Traversability.Impassable)
				{
					return this.category == ThingCategory.Plant;
				}
				if (this.surfaceType >= SurfaceType.Item)
				{
					return false;
				}
				if (typeof(ISlotGroupParent).IsAssignableFrom(this.thingClass))
				{
					return false;
				}
				if (this.IsBlueprint || this.IsFrame)
				{
					ThingDef thingDef = this.entityDefToBuild as ThingDef;
					if (thingDef != null)
					{
						return thingDef.CanOverlapZones;
					}
				}
				return true;
			}
		}
		public bool CountAsResource
		{
			get
			{
				return this.resourceReadoutPriority != ResourceCountPriority.Uncounted;
			}
		}
		public bool BlockPlanting
		{
			get
			{
				return (this.building == null || !this.building.SupportsPlants) && (this.blockPlants || this.category == ThingCategory.Plant || this.Fillage > FillCategory.None || this.IsEdifice());
			}
		}
		public List<VerbProperties> Verbs
		{
			get
			{
				if (this.verbs != null)
				{
					return this.verbs;
				}
				return ThingDef.EmptyVerbPropertiesList;
			}
		}
		public string LabelAsStuff
		{
			get
			{
				if (!this.stuffProps.stuffAdjective.NullOrEmpty())
				{
					return this.stuffProps.stuffAdjective;
				}
				return this.label;
			}
		}
		public CompProperties CompDefFor<T>() where T : ThingComp
		{
			return this.comps.FirstOrDefault((CompProperties c) => c.compClass == typeof(T));
		}
		public CompProperties CompDefForAssignableFrom<T>() where T : ThingComp
		{
			return this.comps.FirstOrDefault((CompProperties c) => typeof(T).IsAssignableFrom(c.compClass));
		}
		public bool HasComp(Type compType)
		{
			for (int i = 0; i < this.comps.Count; i++)
			{
				if (this.comps[i].compClass == compType)
				{
					return true;
				}
			}
			return false;
		}
		public T GetCompProperties<T>() where T : CompProperties
		{
			for (int i = 0; i < this.comps.Count; i++)
			{
				T t = this.comps[i] as T;
				if (t != null)
				{
					return t;
				}
			}
			return (T)((object)null);
		}
		public override void PostLoad()
		{
			if (this.graphicData != null)
			{
				LongEventHandler.ExecuteWhenFinished(delegate
				{
					if (this.graphicData.shaderType == ShaderType.None)
					{
						this.graphicData.shaderType = ShaderType.Cutout;
					}
					this.graphic = this.graphicData.Graphic;
				});
			}
			if (this.verbs != null && this.verbs.Count == 1)
			{
				this.verbs[0].label = this.label;
			}
			base.PostLoad();
			if (this.category == ThingCategory.Building && this.building == null)
			{
				this.building = new BuildingProperties();
			}
			for (int i = 0; i < this.inspectorTabs.Count; i++)
			{
				if (this.inspectorTabsResolved == null)
				{
					this.inspectorTabsResolved = new List<ITab>();
				}
				this.inspectorTabsResolved.Add(ITabManager.GetSharedInstance(this.inspectorTabs[i]));
			}
			if (this.passability == Traversability.Impassable || this.thingClass == typeof(Building_Door))
			{
				this.regionBarrier = true;
			}
			if (this.building != null)
			{
				this.building.PostLoadSpecial(this);
			}
			if (this.plant != null)
			{
				this.plant.PostLoadSpecial(this);
			}
			if (this.ingestible != null)
			{
				this.ingestible.PostLoadSpecial(this);
			}
		}
		public override void ResolveReferences()
		{
			base.ResolveReferences();
			if (this.building != null)
			{
				this.building.ResolveReferencesSpecial();
			}
			if (this.graphicData != null)
			{
				this.graphicData.ResolveReferencesSpecial();
			}
			if (this.artifact != null)
			{
				this.artifact.ResolveReferencesSpecial(this);
			}
			if (this.soundImpactDefault == null)
			{
				this.soundImpactDefault = SoundDef.Named("BulletImpactGround");
			}
			if (this.soundDrop == null)
			{
				this.soundDrop = SoundDef.Named("Standard_Drop");
			}
			if (this.soundPickup == null)
			{
				this.soundPickup = SoundDef.Named("Standard_Pickup");
			}
			if (this.soundInteract == null)
			{
				this.soundPickup = SoundDef.Named("Standard_Pickup");
			}
			if (this.comps != null)
			{
				for (int i = 0; i < this.comps.Count; i++)
				{
					this.comps[i].ResolveReferences(this);
				}
			}
		}
		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			ThingDef.<ConfigErrors>c__Iterator150 <ConfigErrors>c__Iterator = new ThingDef.<ConfigErrors>c__Iterator150();
			<ConfigErrors>c__Iterator.<>f__this = this;
			ThingDef.<ConfigErrors>c__Iterator150 expr_0E = <ConfigErrors>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public static ThingDef Named(string defName)
		{
			return DefDatabase<ThingDef>.GetNamed(defName, true);
		}
		[DebuggerHidden]
		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			ThingDef.<SpecialDisplayStats>c__Iterator151 <SpecialDisplayStats>c__Iterator = new ThingDef.<SpecialDisplayStats>c__Iterator151();
			<SpecialDisplayStats>c__Iterator.<>f__this = this;
			ThingDef.<SpecialDisplayStats>c__Iterator151 expr_0E = <SpecialDisplayStats>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
