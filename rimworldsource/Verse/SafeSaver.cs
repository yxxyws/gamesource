using System;
using System.IO;
namespace Verse
{
	public static class SafeSaver
	{
		private static readonly string NewFileSuffix = ".new";
		private static readonly string OldFileSuffix = ".old";
		private static string GetFilePath(string path)
		{
			return Path.GetFullPath(path);
		}
		private static string GetNewFilePath(string path)
		{
			return Path.GetFullPath(path + SafeSaver.NewFileSuffix);
		}
		private static string GetOldFilePath(string path)
		{
			return Path.GetFullPath(path + SafeSaver.OldFileSuffix);
		}
		public static void Save(string path, string documentElementName, Action saveAction)
		{
			SafeSaver.RemoveFileIfExists(SafeSaver.GetOldFilePath(path), false);
			SafeSaver.RemoveFileIfExists(SafeSaver.GetNewFilePath(path), false);
			try
			{
				Scribe.InitWriting(SafeSaver.GetNewFilePath(path), documentElementName);
			}
			catch (Exception ex)
			{
				Scribe.FinalizeWriting();
				SafeSaver.RemoveFileIfExists(SafeSaver.GetNewFilePath(path), true);
				GenUI.ErrorDialog("ProblemSavingFile".Translate(new object[]
				{
					ex.ToString()
				}));
				throw;
			}
			try
			{
				saveAction();
			}
			catch
			{
				Scribe.FinalizeWriting();
				SafeSaver.RemoveFileIfExists(SafeSaver.GetNewFilePath(path), true);
				throw;
			}
			Scribe.FinalizeWriting();
			string filePath = SafeSaver.GetFilePath(path);
			if (File.Exists(filePath))
			{
				string oldFilePath = SafeSaver.GetOldFilePath(path);
				SafeSaver.RemoveFileIfExists(oldFilePath, false);
				File.Move(filePath, oldFilePath);
			}
			try
			{
				File.Move(SafeSaver.GetNewFilePath(path), SafeSaver.GetFilePath(path));
			}
			catch
			{
				if (!File.Exists(filePath))
				{
					File.Move(SafeSaver.GetOldFilePath(path), filePath);
					SafeSaver.RemoveFileIfExists(SafeSaver.GetNewFilePath(path), true);
				}
				throw;
			}
			SafeSaver.RemoveFileIfExists(SafeSaver.GetOldFilePath(path), false);
		}
		private static void RemoveFileIfExists(string path, bool silently)
		{
			try
			{
				if (File.Exists(path))
				{
					File.Delete(path);
				}
			}
			catch
			{
				if (!silently)
				{
					throw;
				}
			}
		}
	}
}
