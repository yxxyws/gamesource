using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse.AI;
using Verse.AI.Group;
namespace Verse
{
	public sealed class Map : IExposable
	{
		public MapInfo info = new MapInfo();
		public List<MapComponent> components = new List<MapComponent>();
		public TickManager tickManager;
		public ListerThings listerThings;
		public ListerBuildings listerBuildings;
		public MapPawns mapPawns;
		public GameEnder gameEnder;
		public ThingGrid thingGrid;
		public CoverGrid coverGrid;
		public EdificeGrid buildingGrid;
		public DynamicDrawManager dynamicDrawManager;
		public MapDrawer mapDrawer;
		public FogGrid fogGrid;
		public ResearchManager researchManager;
		public PawnDestinationManager pawnDestinationManager;
		public TooltipGiverList tooltipGiverList;
		public ReservationManager reservationManager;
		public DesignationManager designationManager;
		public LordManager lordManager;
		public PassingShipManager passingShipManager;
		public Storyteller storyteller;
		public SlotGroupManager slotGroupManager;
		public Autosaver autosaver;
		public RegionGrid regionGrid;
		public DebugCellDrawer debugDrawer;
		public ColonyInfo colonyInfo;
		public LetterStack letterStack;
		public GlowGrid glowGrid;
		public TerrainGrid terrainGrid;
		public PathGrid pathGrid;
		public RoofGrid roofGrid;
		public MapConditionManager mapConditionManager;
		public FertilityGrid fertilityGrid;
		public WeatherManager weatherManager;
		public PlaySettings playSettings;
		public ZoneManager zoneManager;
		public ResourceCounter resourceCounter;
		public MapAmbientSound ambientSound;
		public MusicManagerMap musicManagerMap;
		public StoryWatcher storyWatcher;
		public History history;
		public TemperatureCache temperatureCache;
		public SnowGrid snowGrid;
		public TaleManager taleManager;
		public PlayLog playLog;
		public OutfitDatabase outfitDatabase;
		public AreaManager areaManager;
		public AttackTargetsCache attackTargetsCache;
		public VoluntarilyJoinableLordsStarter lordsStarter;
		public World World
		{
			get
			{
				return Current.World;
			}
		}
		public IntVec3 Size
		{
			get
			{
				return this.info.Size;
			}
		}
		public IntVec3 Center
		{
			get
			{
				return new IntVec3(this.Size.x / 2, 0, this.Size.z / 2);
			}
		}
		public int Area
		{
			get
			{
				return this.Size.x * this.Size.z;
			}
		}
		public IEnumerable<IntVec3> AllCells
		{
			get
			{
				Map.<>c__Iterator164 <>c__Iterator = new Map.<>c__Iterator164();
				<>c__Iterator.<>f__this = this;
				Map.<>c__Iterator164 expr_0E = <>c__Iterator;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public WorldSquare WorldSquare
		{
			get
			{
				return Find.World.grid.Get(this.info.worldCoords);
			}
		}
		public BiomeDef Biome
		{
			get
			{
				return this.WorldSquare.biome;
			}
		}
		public IntVec2 WorldCoords
		{
			get
			{
				return this.info.worldCoords;
			}
		}
		[DebuggerHidden]
		public IEnumerator<IntVec3> GetEnumerator()
		{
			Map.<GetEnumerator>c__Iterator165 <GetEnumerator>c__Iterator = new Map.<GetEnumerator>c__Iterator165();
			<GetEnumerator>c__Iterator.<>f__this = this;
			return <GetEnumerator>c__Iterator;
		}
		public void ConstructComponents()
		{
			this.listerThings = new ListerThings(ListerThingsUse.Global);
			this.listerBuildings = new ListerBuildings();
			this.mapPawns = new MapPawns();
			this.tickManager = new TickManager();
			this.gameEnder = new GameEnder();
			this.thingGrid = new ThingGrid();
			this.coverGrid = new CoverGrid();
			this.buildingGrid = new EdificeGrid();
			this.dynamicDrawManager = new DynamicDrawManager();
			this.mapDrawer = new MapDrawer();
			this.tooltipGiverList = new TooltipGiverList();
			this.fogGrid = new FogGrid();
			this.glowGrid = new GlowGrid();
			this.pawnDestinationManager = new PawnDestinationManager();
			this.ambientSound = new MapAmbientSound();
			this.researchManager = new ResearchManager();
			this.reservationManager = new ReservationManager();
			this.designationManager = new DesignationManager();
			this.lordManager = new LordManager();
			this.debugDrawer = new DebugCellDrawer();
			this.colonyInfo = new ColonyInfo();
			this.passingShipManager = new PassingShipManager();
			this.autosaver = new Autosaver();
			this.slotGroupManager = new SlotGroupManager();
			this.regionGrid = new RegionGrid();
			this.letterStack = new LetterStack();
			this.terrainGrid = new TerrainGrid();
			this.pathGrid = new PathGrid();
			this.roofGrid = new RoofGrid();
			this.mapConditionManager = new MapConditionManager();
			this.fertilityGrid = new FertilityGrid();
			this.weatherManager = new WeatherManager();
			this.playSettings = new PlaySettings();
			this.zoneManager = new ZoneManager();
			this.musicManagerMap = new MusicManagerMap();
			this.resourceCounter = new ResourceCounter();
			this.storyWatcher = new StoryWatcher();
			this.history = new History();
			this.temperatureCache = new TemperatureCache();
			this.snowGrid = new SnowGrid();
			this.taleManager = new TaleManager();
			this.playLog = new PlayLog();
			this.outfitDatabase = new OutfitDatabase();
			this.areaManager = new AreaManager();
			this.attackTargetsCache = new AttackTargetsCache();
			this.lordsStarter = new VoluntarilyJoinableLordsStarter();
			foreach (Type current in typeof(MapComponent).AllLeafSubclasses())
			{
				MapComponent item = (MapComponent)Activator.CreateInstance(current);
				this.components.Add(item);
			}
		}
		public void ExposeData()
		{
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				Scribe_Deep.LookDeep<MapInfo>(ref this.info, "mapInfo", new object[0]);
				MapFileCompressor mapFileCompressor = new MapFileCompressor();
				mapFileCompressor.ReadDataFromMap();
				Find.Map.ExposeComponents();
				mapFileCompressor.ExposeData();
				HashSet<string> hashSet = new HashSet<string>();
				Scribe.EnterNode("things");
				foreach (Thing current in this.listerThings.AllThings)
				{
					try
					{
						if (current.def.isSaveable && !current.IsSaveCompressible())
						{
							if (hashSet.Contains(current.ThingID))
							{
								Log.Error("Saving Thing with already-used ID " + current.ThingID);
							}
							else
							{
								hashSet.Add(current.ThingID);
							}
							Thing thing = current;
							Scribe_Deep.LookDeep<Thing>(ref thing, "thing", new object[0]);
						}
					}
					catch (Exception ex)
					{
						Log.Error(string.Concat(new object[]
						{
							"Exception saving ",
							current,
							": ",
							ex
						}));
					}
				}
				Scribe.ExitNode();
			}
			else
			{
				if (Scribe.mode == LoadSaveMode.LoadingVars)
				{
					Log.Error("To load a map, use MapIniter_LoadFromFile, not Map.ExposeData.");
				}
			}
		}
		public void ExposeComponents()
		{
			Scribe_Deep.LookDeep<ColonyInfo>(ref this.colonyInfo, "colonyInfo", new object[0]);
			Scribe_Deep.LookDeep<PlaySettings>(ref this.playSettings, "playSettings", new object[0]);
			Scribe_Deep.LookDeep<RealTime>(ref Find.RootMap.realTime, "realTime", new object[0]);
			Scribe_Deep.LookDeep<StoryWatcher>(ref this.storyWatcher, "storyWatcher", new object[0]);
			Scribe_Deep.LookDeep<GameEnder>(ref this.gameEnder, "gameEnder", new object[0]);
			Scribe_Deep.LookDeep<LetterStack>(ref this.letterStack, "letterStack", new object[0]);
			Scribe_Deep.LookDeep<TickManager>(ref this.tickManager, "tickManager", new object[0]);
			Scribe_Deep.LookDeep<WeatherManager>(ref this.weatherManager, "weatherManager", new object[0]);
			Scribe_Deep.LookDeep<ResearchManager>(ref this.researchManager, "researchManager", new object[0]);
			Scribe_Deep.LookDeep<Storyteller>(ref this.storyteller, "storyteller", new object[0]);
			Scribe_Deep.LookDeep<ReservationManager>(ref this.reservationManager, "reservationManager", new object[0]);
			Scribe_Deep.LookDeep<DesignationManager>(ref this.designationManager, "designationManager", new object[0]);
			Scribe_Deep.LookDeep<LordManager>(ref this.lordManager, "lordManager", new object[0]);
			Scribe_Deep.LookDeep<PassingShipManager>(ref this.passingShipManager, "visitorManager", new object[0]);
			Scribe_Deep.LookDeep<MapConditionManager>(ref this.mapConditionManager, "mapConditionManager", new object[0]);
			Scribe_Deep.LookDeep<FogGrid>(ref this.fogGrid, "fogGrid", new object[0]);
			Scribe_Deep.LookDeep<RoofGrid>(ref this.roofGrid, "roofGrid", new object[0]);
			Scribe_Deep.LookDeep<TerrainGrid>(ref this.terrainGrid, "terrainGrid", new object[0]);
			Scribe_Deep.LookDeep<ZoneManager>(ref this.zoneManager, "zoneManager", new object[0]);
			Scribe_Deep.LookDeep<History>(ref this.history, "history", new object[0]);
			Scribe_Deep.LookDeep<TemperatureCache>(ref this.temperatureCache, "temperatureGrid", new object[0]);
			Scribe_Deep.LookDeep<SnowGrid>(ref this.snowGrid, "snowGrid", new object[0]);
			Scribe_Deep.LookDeep<TaleManager>(ref this.taleManager, "taleManager", new object[0]);
			Scribe_Deep.LookDeep<PlayLog>(ref this.playLog, "playLog", new object[0]);
			Scribe_Deep.LookDeep<OutfitDatabase>(ref this.outfitDatabase, "outfitDatabase", new object[0]);
			Scribe_Deep.LookDeep<AreaManager>(ref this.areaManager, "areaManager", new object[0]);
			Scribe_Deep.LookDeep<VoluntarilyJoinableLordsStarter>(ref this.lordsStarter, "lordsStarter", new object[0]);
			Scribe_Collections.LookList<MapComponent>(ref this.components, "components", LookMode.Deep, new object[0]);
			Find.CameraMap.Expose();
		}
		public void MapUpdate()
		{
			this.tickManager.TickManagerUpdate();
			this.colonyInfo.ColonyInfoUpdate();
			PowerNetManager.UpdatePowerNetsAndConnections_First();
			this.regionGrid.UpdateClean();
			RegionAndRoomUpdater.RebuildDirtyRegionsAndRooms();
			this.glowGrid.GlowGridUpdate_First();
			this.letterStack.LettersUpdate();
			this.lordManager.LordManagerUpdate();
			this.mapDrawer.MapMeshDrawerUpdate_First();
			this.mapDrawer.DrawMapMesh();
			this.dynamicDrawManager.DrawDynamicThings();
			this.mapConditionManager.MapConditionManagerDraw();
			MapEdgeClipDrawer.DrawClippers();
			this.designationManager.DrawDesignations();
			try
			{
				Find.AreaManager.AreaManagerUpdate();
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			if (DebugViewSettings.drawPawnDebug)
			{
				this.pawnDestinationManager.DebugDrawDestinations();
				this.reservationManager.DebugDrawReservations();
			}
			if (DebugViewSettings.drawFoodSearchFromMouse)
			{
				FoodUtility.DebugDrawFoodSearchInnerScan(Gen.MouseCell());
			}
			if (DebugViewSettings.drawPreyInfo)
			{
				FoodUtility.DebugDrawPredatorFoodSource();
			}
			this.debugDrawer.DebugDrawerUpdate();
			this.roofGrid.DebugDrawRoots();
			this.regionGrid.DebugDraw();
			InfestationCellFinder.DebugDraw();
			OverlayDrawer.DrawAllOverlays();
			this.musicManagerMap.MusicUpdate();
			for (int i = 0; i < this.components.Count; i++)
			{
				this.components[i].MapComponentUpdate();
			}
		}
		public void MapCleanup()
		{
			foreach (Thing current in this.listerThings.AllThings.ToList<Thing>())
			{
				current.Destroy(DestroyMode.Vanish);
			}
			AudioSource[] array = Find.CameraMapGameObject.GetComponents<AudioSource>();
			for (int i = 0; i < array.Length; i++)
			{
				AudioSource obj = array[i];
				UnityEngine.Object.Destroy(obj);
			}
		}
		public T GetComponent<T>() where T : MapComponent
		{
			for (int i = 0; i < this.components.Count; i++)
			{
				if (this.components[i] is T)
				{
					return (T)((object)this.components[i]);
				}
			}
			return (T)((object)null);
		}
	}
}
