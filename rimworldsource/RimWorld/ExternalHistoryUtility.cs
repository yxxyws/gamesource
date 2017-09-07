using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Verse;
namespace RimWorld
{
	public static class ExternalHistoryUtility
	{
		private static List<FileInfo> cachedFiles;
		private static int gameplayIDLength;
		private static string gameplayIDAvailableChars;
		public static IEnumerable<FileInfo> Files
		{
			get
			{
				ExternalHistoryUtility.<>c__Iterator7B <>c__Iterator7B = new ExternalHistoryUtility.<>c__Iterator7B();
				ExternalHistoryUtility.<>c__Iterator7B expr_07 = <>c__Iterator7B;
				expr_07.$PC = -2;
				return expr_07;
			}
		}
		static ExternalHistoryUtility()
		{
			ExternalHistoryUtility.gameplayIDLength = 20;
			ExternalHistoryUtility.gameplayIDAvailableChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(GenFilePaths.ExternalHistoryFolderPath);
				ExternalHistoryUtility.cachedFiles = (
					from f in directoryInfo.GetFiles()
					where f.Extension == ".rwh"
					orderby f.LastWriteTime descending
					select f).ToList<FileInfo>();
			}
			catch (Exception ex)
			{
				Log.Error("Could not get external history files: " + ex.Message);
			}
		}
		public static ExternalHistory Load(string path)
		{
			ExternalHistory result = null;
			try
			{
				result = new ExternalHistory();
				Scribe.InitLoading(path);
				Scribe_Deep.LookDeep<ExternalHistory>(ref result, "externalHistory", new object[0]);
				Scribe.ExitNode();
				Scribe.mode = LoadSaveMode.Inactive;
			}
			catch (Exception ex)
			{
				Log.Error("Could not load external history (" + path + "): " + ex.Message);
				return null;
			}
			return result;
		}
		public static string GetRandomGameplayID()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < ExternalHistoryUtility.gameplayIDLength; i++)
			{
				int index = Rand.Range(0, ExternalHistoryUtility.gameplayIDAvailableChars.Length);
				stringBuilder.Append(ExternalHistoryUtility.gameplayIDAvailableChars[index]);
			}
			return stringBuilder.ToString();
		}
		public static bool IsValidGameplayID(string ID)
		{
			if (ID.NullOrEmpty() || ID.Length != ExternalHistoryUtility.gameplayIDLength)
			{
				return false;
			}
			for (int i = 0; i < ID.Length; i++)
			{
				bool flag = false;
				for (int j = 0; j < ExternalHistoryUtility.gameplayIDAvailableChars.Length; j++)
				{
					if (ID[i] == ExternalHistoryUtility.gameplayIDAvailableChars[j])
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}
		public static string GetCurrentUploadDate()
		{
			return DateTime.UtcNow.ToString("yyMMdd");
		}
		public static int GetCurrentUploadTime()
		{
			return (int)(DateTime.UtcNow.TimeOfDay.TotalSeconds / 2.0);
		}
	}
}
