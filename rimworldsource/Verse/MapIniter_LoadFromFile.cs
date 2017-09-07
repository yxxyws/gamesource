using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Verse
{
	public class MapIniter_LoadFromFile
	{
		public static void InitMapFromFile(string fileName)
		{
			string str = GenText.ToCommaList(
				from mod in LoadedModManager.LoadedMods
				select mod.name);
			Log.Message("Initializing map from file " + fileName + " with mods " + str);
			DeepProfiler.Start("Loading map from file " + fileName);
			DeepProfiler.Start("InitLoading (read file)");
			RegionAndRoomUpdater.Enabled = false;
			Game.Mode = GameMode.MapInitializing;
			Scribe.InitLoading(GenFilePaths.FilePathForSavedGame(fileName));
			DeepProfiler.End();
			ScribeHeaderUtility.LoadGameDataHeader(ScribeHeaderUtility.ScribeHeaderMode.Map);
			LongEventHandler.SetCurrentEventText("LoadingWorld".Translate());
			Scribe.EnterNode("world");
			Current.World = new World();
			Current.World.ExposeData();
			Scribe.ExitNode();
			LongEventHandler.SetCurrentEventText("LoadingMap".Translate());
			Find.RootMap.curMap = new Map();
			Scribe.EnterNode("map");
			Scribe_Deep.LookDeep<MapInfo>(ref Find.Map.info, "mapInfo", new object[0]);
			MapIniterUtility.ReinitStaticMapComponents_PreConstruct();
			Find.Map.ConstructComponents();
			MapIniterUtility.ReinitStaticMapComponents_PostConstruct();
			Find.Map.ExposeComponents();
			DeepProfiler.Start("Load compressed things");
			MapFileCompressor mapFileCompressor = new MapFileCompressor();
			mapFileCompressor.ExposeData();
			DeepProfiler.End();
			DeepProfiler.Start("Load non-compressed things");
			List<Thing> list = null;
			Scribe_Collections.LookList<Thing>(ref list, "things", LookMode.Deep, new object[0]);
			DeepProfiler.End();
			Scribe.ExitNode();
			Scribe.FinalizeLoading();
			LongEventHandler.SetCurrentEventText("InitializingMap".Translate());
			DeepProfiler.Start("ResolveAllCrossReferences");
			CrossRefResolver.ResolveAllCrossReferences();
			DeepProfiler.End();
			DeepProfiler.Start("DoAllPostLoadInits");
			PostLoadInitter.DoAllPostLoadInits();
			DeepProfiler.End();
			LongEventHandler.SetCurrentEventText("SpawningAllThings".Translate());
			List<Thing> list2 = mapFileCompressor.ThingsToSpawnAfterLoad().ToList<Thing>();
			DeepProfiler.Start("Merge compressed and non-compressed thing lists");
			List<Thing> list3 = new List<Thing>(list.Count + list2.Count);
			foreach (Thing current in list.Concat(list2))
			{
				list3.Add(current);
			}
			DeepProfiler.End();
			DeepProfiler.Start("Spawn everything into the map");
			foreach (Thing current2 in list3)
			{
				try
				{
					GenSpawn.Spawn(current2, current2.Position, current2.Rotation);
				}
				catch (Exception ex)
				{
					Log.Error(string.Concat(new object[]
					{
						"Exception spawning loaded thing ",
						current2,
						": ",
						ex
					}));
				}
			}
			DeepProfiler.End();
			MapIniterUtility.FinalizeMapInit();
			if (Prefs.PauseOnLoad)
			{
				LongEventHandler.ExecuteWhenFinished(delegate
				{
					Find.TickManager.DoSingleTick();
					Find.TickManager.CurTimeSpeed = TimeSpeed.Paused;
				});
			}
			PermadeathModeUtility.CheckUpdatePermadeathModeUniqueNameOnGameLoad(fileName);
			ScribeHeaderUtility.CreateDialogsForVersionMismatchWarnings();
			DeepProfiler.End();
		}
	}
}
