using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class BuildingProperties
	{
		public bool isEdifice = true;
		public List<string> buildingTags = new List<string>();
		public bool isInert;
		private bool deconstructible = true;
		public bool claimable = true;
		public bool isSittable;
		public SoundDef soundAmbient;
		public ConceptDef spawnedConceptLearnOpportunity;
		public ConceptDef boughtConceptLearnOpportunity;
		public bool expandHomeArea = true;
		public bool wantsHopperAdjacent;
		public bool ignoreNeedsPower;
		public bool allowWireConnection = true;
		public bool shipPart;
		public bool canPlaceOverImpassablePlant = true;
		public float heatPerTickWhileWorking;
		public bool canBuildNonEdificesUnder = true;
		public bool canPlaceOverWall;
		public bool preventDeterioration;
		public bool isMealSource;
		public bool isNaturalRock;
		public bool isResourceRock;
		public bool repairable = true;
		public bool isPlayerEjectable;
		public GraphicData fullGraveGraphicData;
		public int bed_healTickInterval = 5000;
		public bool bed_defaultMedical;
		public bool bed_showSleeperBody;
		public bool bed_humanlike = true;
		public float bed_maxBodySize = 9999f;
		public int foodCostPerDispense;
		public SoundDef soundDispense;
		public ThingDef turretGunDef;
		public ThingDef turretShellDef;
		public int turretBurstWarmupTicks;
		public int turretBurstCooldownTicks = -1;
		public string turretTopGraphicPath;
		[Unsaved]
		public Material turretTopMat;
		public bool ai_combatDangerous;
		public SoundDef soundDoorOpenPowered = SoundDefOf.DoorOpen;
		public SoundDef soundDoorClosePowered = SoundDefOf.DoorClose;
		public SoundDef soundDoorOpenManual = SoundDefOf.DoorOpenManual;
		public SoundDef soundDoorCloseManual = SoundDefOf.DoorCloseManual;
		public string sowTag;
		public ThingDef defaultPlantToGrow;
		public bool plantsDestroyWithMe;
		public SoundDef soundMined;
		public ThingDef mineableThing;
		public int mineableYield = 1;
		public float mineableNonMinedEfficiency = 0.7f;
		public float mineableDropChance = 1f;
		public float mineableScatterCommonality;
		public IntRange mineableScatterBlotchSizeRange = new IntRange(20, 40);
		public StorageSettings fixedStorageSettings;
		public StorageSettings defaultStorageSettings;
		public bool isTrap;
		public DamageArmorCategory trapDamageCategory;
		public GraphicData trapUnarmedGraphicData;
		[Unsaved]
		public Graphic trapUnarmedGraphic;
		public float unpoweredWorkTableWorkSpeedFactor;
		public IntRange watchBuildingStandDistanceRange = IntRange.one;
		public int watchBuildingStandRectWidth = 3;
		public bool SupportsPlants
		{
			get
			{
				return this.sowTag != null;
			}
		}
		public bool IsTurret
		{
			get
			{
				return this.turretGunDef != null;
			}
		}
		public bool IsDeconstructible
		{
			get
			{
				return !this.isNaturalRock && this.deconstructible;
			}
		}
		[DebuggerHidden]
		public IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			BuildingProperties.<ConfigErrors>c__Iterator63 <ConfigErrors>c__Iterator = new BuildingProperties.<ConfigErrors>c__Iterator63();
			<ConfigErrors>c__Iterator.<>f__this = this;
			BuildingProperties.<ConfigErrors>c__Iterator63 expr_0E = <ConfigErrors>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public void PostLoadSpecial(ThingDef parentDef)
		{
		}
		public void ResolveReferencesSpecial()
		{
			if (!this.turretTopGraphicPath.NullOrEmpty())
			{
				LongEventHandler.ExecuteWhenFinished(delegate
				{
					this.turretTopMat = MaterialPool.MatFrom(this.turretTopGraphicPath);
				});
			}
			if (this.fixedStorageSettings != null)
			{
				this.fixedStorageSettings.filter.ResolveReferences();
			}
			if (this.defaultStorageSettings == null && this.fixedStorageSettings != null)
			{
				this.defaultStorageSettings = new StorageSettings();
				this.defaultStorageSettings.CopyFrom(this.fixedStorageSettings);
			}
			if (this.defaultStorageSettings != null)
			{
				this.defaultStorageSettings.filter.ResolveReferences();
			}
		}
	}
}
