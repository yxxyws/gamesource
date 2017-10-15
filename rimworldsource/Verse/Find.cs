using RimWorld;
using RimWorld.Planet;
using System;
using UnityEngine;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
namespace Verse
{
	[StaticConstructorOnStartup]
	public static class Find
	{
		private static RootGameObject rootGameObject;
		private static CameraMapGameObject cameraMapGameObject;
		private static Camera cameraMenu;
		private static CameraFade cameraFade;
		private static GameObject audioListenerDummy;
		public static RootGameObject RootGameObject
		{
			get
			{
				return Find.rootGameObject;
			}
		}
		public static Root RootRoot
		{
			get
			{
				return RootManager.root;
			}
		}
		public static RootEntry RootEntry
		{
			get
			{
				return RootManager.rootEntry;
			}
		}
		public static RootMap RootMap
		{
			get
			{
				return RootManager.rootMap;
			}
		}
		public static SoundRoot SoundRoot
		{
			get
			{
				return RootManager.root.soundRoot;
			}
		}
		public static RealTime RealTime
		{
			get
			{
				return RootManager.root.realTime;
			}
		}
		public static UIRoot UIRoot
		{
			get
			{
				return RootManager.root.uiRoot;
			}
		}
		public static SustainerManager SustainerManager
		{
			get
			{
				return Find.SoundRoot.sustainerManager;
			}
		}
		public static SampleOneShotManager SampleOneShotManager
		{
			get
			{
				return Find.SoundRoot.oneShotManager;
			}
		}
		public static UIRoot_Entry UIRoot_Entry
		{
			get
			{
				return Find.RootEntry.uiRoot as UIRoot_Entry;
			}
		}
		public static UIRoot_Map UIRoot_Map
		{
			get
			{
				return Find.RootMap.uiRoot as UIRoot_Map;
			}
		}
		public static WindowStack WindowStack
		{
			get
			{
				return Find.UIRoot.windows;
			}
		}
		public static Page_CharMaker CharMaker
		{
			get
			{
				return Find.WindowStack.WindowOfType<Page_CharMaker>();
			}
		}
		public static MainTabsRoot MainTabsRoot
		{
			get
			{
				return Find.UIRoot_Map.mainTabsRoot;
			}
		}
		public static Selector Selector
		{
			get
			{
				return Find.UIRoot_Map.selector;
			}
		}
		public static Targeter Targeter
		{
			get
			{
				return Find.UIRoot_Map.targeter;
			}
		}
		public static ThingOverlays ThingOverlays
		{
			get
			{
				return Find.UIRoot_Map.thingOverlays;
			}
		}
		public static AlertsReadout Alerts
		{
			get
			{
				return Find.UIRoot_Map.alerts;
			}
		}
		public static CameraMap CameraMap
		{
			get
			{
				return Find.RootMap.cameraMap;
			}
		}
		public static Camera Camera
		{
			get
			{
				return (Game.Mode != GameMode.MapPlaying) ? Find.CameraMenu : Find.CameraMapGameObject.camera;
			}
		}
		public static Camera CameraMenu
		{
			get
			{
				return Find.cameraMenu;
			}
		}
		public static CameraMapGameObject CameraMapGameObject
		{
			get
			{
				return Find.cameraMapGameObject;
			}
		}
		public static CameraFade CameraFade
		{
			get
			{
				return Find.cameraFade;
			}
		}
		public static ColorCorrectionCurves CameraColor
		{
			get
			{
				return Find.CameraMapGameObject.GetComponent<ColorCorrectionCurves>();
			}
		}
		public static GameObject AudioListenerDummy
		{
			get
			{
				return Find.audioListenerDummy;
			}
		}
		public static Map Map
		{
			get
			{
				return (Find.RootMap != null) ? Find.RootMap.curMap : null;
			}
		}
		public static LanguageWorker ActiveLanguageWorker
		{
			get
			{
				return LanguageDatabase.activeLanguage.Worker;
			}
		}
		public static TickManager TickManager
		{
			get
			{
				return Find.Map.tickManager;
			}
		}
		public static ResourceCounter ResourceCounter
		{
			get
			{
				return Find.Map.resourceCounter;
			}
		}
		public static ThingGrid ThingGrid
		{
			get
			{
				return Find.Map.thingGrid;
			}
		}
		public static EdificeGrid EdificeGrid
		{
			get
			{
				return Find.Map.buildingGrid;
			}
		}
		public static CoverGrid CoverGrid
		{
			get
			{
				return Find.Map.coverGrid;
			}
		}
		public static FogGrid FogGrid
		{
			get
			{
				return Find.Map.fogGrid;
			}
		}
		public static GlowGrid GlowGrid
		{
			get
			{
				return Find.Map.glowGrid;
			}
		}
		public static SnowGrid SnowGrid
		{
			get
			{
				return Find.Map.snowGrid;
			}
		}
		public static ListerThings ListerThings
		{
			get
			{
				return Find.Map.listerThings;
			}
		}
		public static ListerBuildings ListerBuildings
		{
			get
			{
				return Find.Map.listerBuildings;
			}
		}
		public static DynamicDrawManager DynamicDrawManager
		{
			get
			{
				return Find.Map.dynamicDrawManager;
			}
		}
		public static PawnDestinationManager PawnDestinationManager
		{
			get
			{
				return Find.Map.pawnDestinationManager;
			}
		}
		public static TooltipGiverList TooltipGiverList
		{
			get
			{
				return Find.Map.tooltipGiverList;
			}
		}
		public static ResearchManager ResearchManager
		{
			get
			{
				return Find.Map.researchManager;
			}
		}
		public static ReservationManager Reservations
		{
			get
			{
				return Find.Map.reservationManager;
			}
		}
		public static DesignationManager DesignationManager
		{
			get
			{
				return Find.Map.designationManager;
			}
		}
		public static LordManager LordManager
		{
			get
			{
				return Find.Map.lordManager;
			}
		}
		public static DebugCellDrawer DebugDrawer
		{
			get
			{
				return Find.Map.debugDrawer;
			}
		}
		public static PassingShipManager PassingShipManager
		{
			get
			{
				return Find.Map.passingShipManager;
			}
		}
		public static Storyteller Storyteller
		{
			get
			{
				return Find.Map.storyteller;
			}
		}
		public static StoryWatcher StoryWatcher
		{
			get
			{
				return Find.Map.storyWatcher;
			}
		}
		public static SlotGroupManager SlotGroupManager
		{
			get
			{
				return Find.Map.slotGroupManager;
			}
		}
		public static RegionGrid RegionGrid
		{
			get
			{
				return Find.Map.regionGrid;
			}
		}
		public static GameEnder GameEnder
		{
			get
			{
				return Find.Map.gameEnder;
			}
		}
		public static ColonyInfo ColonyInfo
		{
			get
			{
				return Find.Map.colonyInfo;
			}
		}
		public static LetterStack LetterStack
		{
			get
			{
				return Find.Map.letterStack;
			}
		}
		public static MapDrawer MapDrawer
		{
			get
			{
				return Find.Map.mapDrawer;
			}
		}
		public static TerrainGrid TerrainGrid
		{
			get
			{
				return Find.Map.terrainGrid;
			}
		}
		public static PathGrid PathGrid
		{
			get
			{
				return Find.Map.pathGrid;
			}
		}
		public static RoofGrid RoofGrid
		{
			get
			{
				return Find.Map.roofGrid;
			}
		}
		public static MapConditionManager MapConditionManager
		{
			get
			{
				return Find.Map.mapConditionManager;
			}
		}
		public static FertilityGrid FertilityGrid
		{
			get
			{
				return Find.Map.fertilityGrid;
			}
		}
		public static WeatherManager WeatherManager
		{
			get
			{
				return Find.Map.weatherManager;
			}
		}
		public static PlaySettings PlaySettings
		{
			get
			{
				return Find.Map.playSettings;
			}
		}
		public static ZoneManager ZoneManager
		{
			get
			{
				return Find.Map.zoneManager;
			}
		}
		public static MusicManagerMap MusicManagerMap
		{
			get
			{
				return Find.Map.musicManagerMap;
			}
		}
		public static History History
		{
			get
			{
				return Find.Map.history;
			}
		}
		public static MapPawns MapPawns
		{
			get
			{
				return Find.Map.mapPawns;
			}
		}
		public static TaleManager TaleManager
		{
			get
			{
				return Find.Map.taleManager;
			}
		}
		public static PlayLog PlayLog
		{
			get
			{
				return Find.Map.playLog;
			}
		}
		public static AreaManager AreaManager
		{
			get
			{
				return Find.Map.areaManager;
			}
		}
		public static AttackTargetsCache AttackTargetsCache
		{
			get
			{
				return Find.Map.attackTargetsCache;
			}
		}
		public static VoluntarilyJoinableLordsStarter VoluntarilyJoinableLordsStarter
		{
			get
			{
				return Find.Map.lordsStarter;
			}
		}
		public static Area_Home AreaHome
		{
			get
			{
				return Find.Map.areaManager.Get<Area_Home>();
			}
		}
		public static Area_NoRoof AreaNoRoof
		{
			get
			{
				return Find.Map.areaManager.Get<Area_NoRoof>();
			}
		}
		public static Area_SnowClear AreaSnowClear
		{
			get
			{
				return Find.Map.areaManager.Get<Area_SnowClear>();
			}
		}
		public static World World
		{
			get
			{
				return Current.World;
			}
		}
		public static WorldSquare MapWorldSquare
		{
			get
			{
				return Find.World.grid.Get(MapInitData.landingCoords);
			}
		}
		public static FactionManager FactionManager
		{
			get
			{
				return Find.World.factionManager;
			}
		}
		public static WorldPawns WorldPawns
		{
			get
			{
				return Find.World.worldPawns;
			}
		}
		public static UniqueIDsManager UniqueIDsManager
		{
			get
			{
				return Find.World.uniqueIDsManager;
			}
		}
		public static void ResetReferences()
		{
			Find.cameraMapGameObject = null;
			Find.cameraMenu = null;
			Find.cameraFade = null;
			Find.audioListenerDummy = GameObject.Find("AudioListenerDummy");
			Find.cameraFade = GameObject.Find("CameraFadeDummy").GetComponent<CameraFade>();
			Find.rootGameObject = GameObject.Find("GameCoreDummy").GetComponent<RootGameObject>();
			if (Application.loadedLevelName == "Entry")
			{
				Find.cameraMenu = GameObject.Find("CameraMenu").GetComponent<Camera>();
			}
			else
			{
				if (Application.loadedLevelName == "Gameplay")
				{
					Find.cameraMapGameObject = GameObject.Find("CameraMap").GetComponent<CameraMapGameObject>();
				}
			}
		}
	}
}
