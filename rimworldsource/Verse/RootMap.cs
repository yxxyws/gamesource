using RimWorld;
using System;
using System.Linq;
namespace Verse
{
	public class RootMap : Root
	{
		public Map curMap;
		public CameraMap cameraMap;
		public override void Init()
		{
			base.Init();
			this.cameraMap = new CameraMap();
			if (MapInitData.mapToLoad.NullOrEmpty())
			{
				LongEventHandler.QueueLongEvent(delegate
				{
					MapIniter_NewGame.InitNewGeneratedMap();
				}, "GeneratingMap", true, new Action<Exception>(RootMap.ErrorWhileGeneratingMap));
			}
			else
			{
				LongEventHandler.QueueLongEvent(delegate
				{
					MapIniter_LoadFromFile.InitMapFromFile(MapInitData.mapToLoad);
				}, "LoadingLongEvent", true, new Action<Exception>(RootMap.ErrorWhileLoadingMap));
			}
		}
		public override void RootUpdate()
		{
			base.RootUpdate();
			try
			{
				if (!LongEventHandler.ShouldWaitForEvent)
				{
					SkyManager.SkyManagerUpdate();
					ShipCountdown.ShipCountdownUpdate();
					DragSliderManager.DragSlidersUpdate();
					this.realTime.Update();
					this.curMap.MapUpdate();
					DebugViewDrawer.Update();
				}
			}
			catch (Exception e)
			{
				Log.Notify_Exception(e);
				throw;
			}
		}
		private static void ReturnToMainMenu()
		{
			LongEventHandler.ClearQueuedEvents();
			LongEventHandler.QueueLongEvent(null, "Entry", "LoadingLongEvent", true, null);
		}
		private static void ErrorWhileLoadingAssets(Exception e)
		{
			string text = "ErrorWhileLoadingAssets".Translate();
			if (ModsConfig.ActiveMods.Count<InstalledMod>() != 1 || !ModsConfig.ActiveMods.First<InstalledMod>().IsCoreMod)
			{
				text = text + "\n\n" + "ErrorWhileLoadingAssets_ModsInfo".Translate();
			}
			DelayedErrorWindowRequest.Add(text, "ErrorWhileLoadingAssetsTitle".Translate());
			RootMap.ReturnToMainMenu();
		}
		private static void ErrorWhileGeneratingMap(Exception e)
		{
			DelayedErrorWindowRequest.Add("ErrorWhileGeneratingMap".Translate(), "ErrorWhileGeneratingMapTitle".Translate());
			RootMap.ReturnToMainMenu();
		}
		private static void ErrorWhileLoadingMap(Exception e)
		{
			string text = "ErrorWhileLoadingMap".Translate();
			string text2;
			string text3;
			if (!ScribeHeaderUtility.LoadedHeaderModListMatchesModList(out text2, out text3))
			{
				text = text + "\n\n" + "ModsMismatchWarningText".Translate(new object[]
				{
					text2,
					text3
				});
			}
			DelayedErrorWindowRequest.Add(text, "ErrorWhileLoadingMapTitle".Translate());
			CrossRefResolver.Clear();
			PostLoadInitter.Clear();
			RootMap.ReturnToMainMenu();
		}
	}
}
