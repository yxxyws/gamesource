using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Verse
{
	public static class MapFilesUtility
	{
		public static IEnumerable<FileInfo> AllMapFiles
		{
			get
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(GenFilePaths.SavedMapsFolderPath);
				if (!directoryInfo.Exists)
				{
					directoryInfo.Create();
				}
				return 
					from f in directoryInfo.GetFiles()
					where f.Extension == ".rws"
					orderby f.LastWriteTime descending
					select f;
			}
		}
		public static bool IsAutoSave(string mapName)
		{
			return mapName.Length >= 8 && mapName.Substring(0, 8) == "Autosave";
		}
		public static bool HaveMapNamed(string mapName)
		{
			foreach (string current in 
				from f in MapFilesUtility.AllMapFiles
				select Path.GetFileNameWithoutExtension(f.Name))
			{
				if (current == mapName)
				{
					return true;
				}
			}
			return false;
		}
		public static string UnusedDefaultName()
		{
			string text = string.Empty;
			int num = 1;
			do
			{
				text = "Colony" + num.ToString();
				num++;
			}
			while (MapFilesUtility.HaveMapNamed(text));
			return text;
		}
	}
}
