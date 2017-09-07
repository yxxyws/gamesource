using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Verse;
namespace RimWorld
{
	public class HistoryUpload
	{
		private static string uploadURL = "http://rimworldgame.com/metrics/upload.php";
		public static void CreateDialog(int uploadFrequency)
		{
			Find.WindowStack.Add(new Dialog_HistoryUpload(uploadFrequency));
		}
		public static bool CanUploadWithNoDialog()
		{
			HistoryUploadPref arg_05_0 = Prefs.HistoryUpload;
			return false;
		}
		public static void TrySaveAndUpload()
		{
		}
		public static void Tick()
		{
			if (Find.TickManager.TicksGame % 1500000 == 0 && Find.TickManager.TicksGame != 0)
			{
				HistoryUpload.TrySaveAndUpload();
			}
		}
		public static void SaveAndUpload()
		{
			try
			{
				MapInfo info = Find.Map.info;
				if (!ExternalHistoryUtility.IsValidGameplayID(info.historyGameplayID))
				{
					info.historyGameplayID = ExternalHistoryUtility.GetRandomGameplayID();
					info.historyFirstUploadDate = ExternalHistoryUtility.GetCurrentUploadDate();
					info.historyFirstUploadTime = ExternalHistoryUtility.GetCurrentUploadTime();
				}
				string tempFileName = Path.GetTempFileName();
				HistoryUpload.SaveToFile(tempFileName);
				HistoryUpload.Upload(tempFileName);
			}
			catch (Exception ex)
			{
				Log.Warning("Could not save or upload history file: " + ex.Message);
			}
		}
		private static void SaveToFile(string path)
		{
			try
			{
				Scribe.InitWriting(path, "history");
				Scribe.EnterNode("map");
				MapInfo info = Find.Map.info;
				ExternalHistory externalHistory = new ExternalHistory();
				externalHistory.gameVersion = VersionControl.CurrentVersionFull;
				externalHistory.gameplayID = info.historyGameplayID;
				externalHistory.userName = Prefs.UserName;
				if (externalHistory.userName == null || externalHistory.userName.Length == 0)
				{
					externalHistory.userName = ExternalHistory.defaultUserName;
				}
				externalHistory.storytellerName = Find.Storyteller.def.label;
				externalHistory.realWorldDate = DateTime.UtcNow.ToString();
				externalHistory.firstUploadDate = info.historyFirstUploadDate;
				externalHistory.firstUploadTime = info.historyFirstUploadTime;
				externalHistory.devMode = Prefs.DevMode;
				externalHistory.history = Find.History;
				Scribe_Deep.LookDeep<ExternalHistory>(ref externalHistory, "externalHistory", new object[0]);
				Scribe.ExitNode();
			}
			finally
			{
				Scribe.FinalizeWriting();
				Scribe.mode = LoadSaveMode.Inactive;
			}
		}
		private static void Pack(string from, string to)
		{
		}
		[DebuggerHidden]
		private static IEnumerator UploadCo(string path)
		{
			HistoryUpload.<UploadCo>c__Iterator7C <UploadCo>c__Iterator7C = new HistoryUpload.<UploadCo>c__Iterator7C();
			<UploadCo>c__Iterator7C.path = path;
			<UploadCo>c__Iterator7C.<$>path = path;
			return <UploadCo>c__Iterator7C;
		}
		private static void Upload(string path)
		{
			Log.Message("Uploading file.");
			Find.RootGameObject.StartCoroutine(HistoryUpload.UploadCo(path));
		}
	}
}
