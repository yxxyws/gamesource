using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public static class ThingDefGenerator_Buildings
	{
		public static readonly string BlueprintDefNameSuffix = "_Blueprint";
		public static readonly string InstallBlueprintDefNameSuffix = "_Install";
		public static readonly string BuildingFrameDefNameSuffix = "_Frame";
		private static readonly string TerrainBlueprintGraphicPath = "Things/Special/TerrainBlueprint";
		private static Color BlueprintColor = new Color(0.5f, 0.5f, 1f, 0.35f);
		[DebuggerHidden]
		public static IEnumerable<ThingDef> ImpliedBlueprintAndFrameDefs()
		{
			ThingDefGenerator_Buildings.<ImpliedBlueprintAndFrameDefs>c__Iterator5D <ImpliedBlueprintAndFrameDefs>c__Iterator5D = new ThingDefGenerator_Buildings.<ImpliedBlueprintAndFrameDefs>c__Iterator5D();
			ThingDefGenerator_Buildings.<ImpliedBlueprintAndFrameDefs>c__Iterator5D expr_07 = <ImpliedBlueprintAndFrameDefs>c__Iterator5D;
			expr_07.$PC = -2;
			return expr_07;
		}
		private static ThingDef BaseBlueprintDef()
		{
			return new ThingDef
			{
				category = ThingCategory.Ethereal,
				label = "Unspecified blueprint",
				altitudeLayer = AltitudeLayer.Blueprint,
				useHitPoints = false,
				selectable = true,
				seeThroughFog = true,
				comps = 
				{
					CompPropertiesGenerator.Forbiddable()
				},
				drawerType = DrawerType.MapMeshAndRealTime
			};
		}
		private static ThingDef BaseFrameDef()
		{
			return new ThingDef
			{
				isFrame = true,
				category = ThingCategory.Building,
				label = "Unspecified building frame",
				thingClass = typeof(Frame),
				altitudeLayer = AltitudeLayer.Waist,
				useHitPoints = true,
				selectable = true,
				building = new BuildingProperties(),
				comps = 
				{
					CompPropertiesGenerator.Forbiddable()
				}
			};
		}
		private static ThingDef NewBlueprintDef_Thing(ThingDef def, bool isInstallBlueprint, ThingDef normalBlueprint = null)
		{
			ThingDef thingDef = ThingDefGenerator_Buildings.BaseBlueprintDef();
			thingDef.defName = def.defName + ThingDefGenerator_Buildings.BlueprintDefNameSuffix;
			thingDef.label = def.label + "BlueprintLabelExtra".Translate();
			thingDef.size = def.size;
			if (isInstallBlueprint)
			{
				ThingDef expr_4A = thingDef;
				expr_4A.defName += ThingDefGenerator_Buildings.InstallBlueprintDefNameSuffix;
			}
			if (isInstallBlueprint && normalBlueprint != null)
			{
				thingDef.graphicData = normalBlueprint.graphicData;
			}
			else
			{
				thingDef.graphicData = new GraphicData();
				if (def.blueprintGraphicData != null)
				{
					thingDef.graphicData.CopyFrom(def.blueprintGraphicData);
					if (thingDef.graphicData.graphicClass == null)
					{
						thingDef.graphicData.graphicClass = typeof(Graphic_Single);
					}
					if (thingDef.graphicData.shaderType == ShaderType.None)
					{
						thingDef.graphicData.shaderType = ShaderType.MetaOverlay;
					}
					thingDef.graphicData.drawSize = def.graphicData.drawSize;
					thingDef.graphicData.linkFlags = def.graphicData.linkFlags;
					thingDef.graphicData.linkType = def.graphicData.linkType;
				}
				else
				{
					thingDef.graphicData.CopyFrom(def.graphicData);
					thingDef.graphicData.shaderType = ShaderType.Transparent;
					thingDef.graphicData.color = ThingDefGenerator_Buildings.BlueprintColor;
					thingDef.graphicData.colorTwo = Color.white;
					thingDef.graphicData.shadowData = null;
				}
			}
			if (thingDef.graphicData.shadowData != null)
			{
				Log.Error("Blueprint has shadow: " + def);
			}
			if (isInstallBlueprint)
			{
				thingDef.thingClass = typeof(Blueprint_Install);
			}
			else
			{
				thingDef.thingClass = def.blueprintClass;
			}
			if (def.thingClass == typeof(Building_Door))
			{
				thingDef.drawerType = DrawerType.RealtimeOnly;
			}
			else
			{
				thingDef.drawerType = DrawerType.MapMeshAndRealTime;
			}
			thingDef.entityDefToBuild = def;
			if (isInstallBlueprint)
			{
				def.installBlueprintDef = thingDef;
			}
			else
			{
				def.blueprintDef = thingDef;
			}
			return thingDef;
		}
		private static ThingDef NewFrameDef_Thing(ThingDef def)
		{
			ThingDef thingDef = ThingDefGenerator_Buildings.BaseFrameDef();
			thingDef.defName = def.defName + ThingDefGenerator_Buildings.BuildingFrameDefNameSuffix;
			thingDef.label = def.label + "FrameLabelExtra".Translate();
			thingDef.size = def.size;
			thingDef.SetStatBaseValue(StatDefOf.MaxHitPoints, (float)def.BaseMaxHitPoints);
			thingDef.fillPercent = def.fillPercent;
			thingDef.description = def.description;
			thingDef.passability = def.passability;
			thingDef.selectable = def.selectable;
			thingDef.constructEffect = def.constructEffect;
			thingDef.building.isEdifice = def.building.isEdifice;
			if (!def.designationCategory.NullOrEmpty())
			{
				thingDef.stuffCategories = def.stuffCategories;
			}
			thingDef.entityDefToBuild = def;
			def.frameDef = thingDef;
			return thingDef;
		}
		private static ThingDef NewBlueprintDef_Terrain(TerrainDef terrDef)
		{
			ThingDef thingDef = ThingDefGenerator_Buildings.BaseBlueprintDef();
			thingDef.thingClass = typeof(Blueprint_Build);
			thingDef.defName = terrDef.defName + ThingDefGenerator_Buildings.BlueprintDefNameSuffix;
			thingDef.label = terrDef.label + "BlueprintLabelExtra".Translate();
			thingDef.entityDefToBuild = terrDef;
			thingDef.graphicData = new GraphicData();
			thingDef.graphicData.shaderType = ShaderType.MetaOverlay;
			thingDef.graphicData.texPath = ThingDefGenerator_Buildings.TerrainBlueprintGraphicPath;
			thingDef.graphicData.graphicClass = typeof(Graphic_Single);
			thingDef.entityDefToBuild = terrDef;
			terrDef.blueprintDef = thingDef;
			return thingDef;
		}
		private static ThingDef NewFrameDef_Terrain(TerrainDef terrDef)
		{
			ThingDef thingDef = ThingDefGenerator_Buildings.BaseFrameDef();
			thingDef.defName = terrDef.defName + ThingDefGenerator_Buildings.BuildingFrameDefNameSuffix;
			thingDef.label = terrDef.label + "FrameLabelExtra".Translate();
			thingDef.entityDefToBuild = terrDef;
			thingDef.useHitPoints = false;
			thingDef.fillPercent = 0f;
			thingDef.description = "Terrain building in progress.";
			thingDef.passability = Traversability.Standable;
			thingDef.selectable = true;
			thingDef.constructEffect = terrDef.constructEffect;
			thingDef.building.isEdifice = false;
			thingDef.category = ThingCategory.Ethereal;
			thingDef.entityDefToBuild = terrDef;
			terrDef.frameDef = thingDef;
			if (!thingDef.IsFrame)
			{
				Log.Error("Framedef is not frame: " + thingDef);
			}
			return thingDef;
		}
	}
}
