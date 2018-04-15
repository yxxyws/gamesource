using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse.AI;
namespace Verse
{
	public static class MapIniterUtility
	{
		public static void ReinitStaticMapComponents_PreConstruct()
		{
			CellIndices.Reinit();
		}
		public static void ReinitStaticMapComponents_PostConstruct()
		{
			LinkGrid.Reinit();
			GlowFlooder.Reinit();
			PowerNetManager.Reinit();
			PowerNetGrid.Reinit();
			RegionMaker.Reinit();
			PathFinder.Reinit();
			PawnPathPool.Reinit();
			RegionAndRoomUpdater.Reinit();
			GraphicDatabaseHeadRecords.Reinit();
			RegionLinkDatabase.Reinit();
			MoteCounter.Reinit();
			GatherSpotLister.Reinit();
			GenTemperature.Reinit();
			DateReadout.Reinit();
			WindManager.Reinit();
			DesignatorManager.Reinit();
			ListerBuildingsRepairable.Reinit();
			ListerHaulables.Reinit();
			ReverseDesignatorDatabase.Reinit();
		}
		public static void FinalizeMapInit()
		{
			Find.PathGrid.RecalculateAllPerceivedPathCosts();
			RegionAndRoomUpdater.Enabled = true;
			RegionAndRoomUpdater.RebuildAllRegionsAndRooms();
			PowerNetManager.UpdatePowerNetsAndConnections_First();
			TemperatureSaveLoad.ApplyLoadedDataToRegions();
			foreach (Thing current in Find.ListerThings.AllThings.ToList<Thing>())
			{
				try
				{
					current.PostMapInit();
				}
				catch (Exception ex)
				{
					Log.Error(string.Concat(new object[]
					{
						"Exception PostLoadAllSpawned in ",
						current,
						": ",
						ex
					}));
				}
			}
			Find.ResearchManager.ReapplyAllMods();
			ListerFilthInHomeArea.RebuildAll();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				Find.Map.mapDrawer.RegenerateEverythingNow();
			});
			Find.CameraMap.ResetSize();
			Find.CameraFade.SetScreenOverlayColor(Color.black);
			Find.CameraFade.StartFade(Color.clear, 0.65f);
			Find.ResourceCounter.UpdateResourceCounts();
			MapInitData.Notify_MapInited();
			Game.Mode = GameMode.MapPlaying;
			Find.Map.storyWatcher.watcherWealth.ForceRecount();
			LogSimple.FlushToFileAndOpen();
		}
	}
}
