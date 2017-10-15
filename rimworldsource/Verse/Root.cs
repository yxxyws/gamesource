using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse.Sound;
using Verse.Steam;
namespace Verse
{
	public abstract class Root
	{
		private static bool globalInitDone;
		private static bool prefsApplied;
		public SoundRoot soundRoot;
		public RealTime realTime;
		public UIRoot uiRoot;
		public virtual void Init()
		{
			SteamManager.InitIfNeeded();
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			if (commandLineArgs != null && commandLineArgs.Length > 1)
			{
				Log.Message("Command line arguments: " + GenText.ToSpaceList(commandLineArgs.Skip(1)));
			}
			if (!Root.globalInitDone)
			{
				Game.Init();
				VersionControl.LogVersionNumber();
				Application.targetFrameRate = 60;
				Prefs.Init();
				if (Prefs.DevMode)
				{
					StaticConstructorOnStartupUtility.ReportProbablyMissingAttributes();
				}
				LongEventHandler.QueueLongEvent(new Action(StaticConstructorOnStartupUtility.CallAll), null, false, null);
				Root.globalInitDone = true;
			}
			Find.ResetReferences();
			Action action = delegate
			{
				ActiveTutorNoteManager.CloseAll();
				this.realTime = new RealTime();
				this.soundRoot = new SoundRoot();
				if (Application.loadedLevelName == "Gameplay")
				{
					this.uiRoot = new UIRoot_Map();
				}
				else
				{
					if (Application.loadedLevelName == "Entry")
					{
						this.uiRoot = new UIRoot_Entry();
					}
				}
				this.uiRoot.Init();
				Messages.Notify_LoadedLevelChanged();
			};
			if (!PlayDataLoader.loaded)
			{
				LongEventHandler.QueueLongEvent(delegate
				{
					PlayDataLoader.LoadAllPlayData(false);
				}, null, true, null);
				LongEventHandler.QueueLongEvent(action, "InitializingInterface", false, null);
			}
			else
			{
				action();
			}
		}
		public virtual void RootUpdate()
		{
			try
			{
				LongEventHandler.LongEventsUpdate();
				if (!LongEventHandler.ShouldWaitForEvent)
				{
					Rand.EnsureSeedStackEmpty();
					SteamManager.Update();
					this.uiRoot.UIRootUpdate();
					if (Time.frameCount > 3 && !Root.prefsApplied)
					{
						Root.prefsApplied = true;
						Prefs.Apply();
					}
					this.soundRoot.Update();
				}
			}
			catch (Exception e)
			{
				Log.Notify_Exception(e);
				throw;
			}
		}
		public void RootOnGUI()
		{
			try
			{
				LongEventHandler.LongEventsOnGUI();
				if (!LongEventHandler.ShouldWaitForEvent)
				{
					this.uiRoot.UIRootOnGUI();
				}
			}
			catch (Exception e)
			{
				Log.Notify_Exception(e);
				throw;
			}
		}
		public static void Shutdown()
		{
			SteamManager.ShutdownSteam();
			Application.Quit();
		}
	}
}
