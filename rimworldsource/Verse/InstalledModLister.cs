using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Verse
{
	public static class InstalledModLister
	{
		private static List<InstalledMod> mods;
		public static IEnumerable<InstalledMod> AllInstalledMods
		{
			get
			{
				return InstalledModLister.mods;
			}
		}
		public static IEnumerable<DirectoryInfo> AllActiveModDirs
		{
			get
			{
				return 
					from mod in InstalledModLister.mods
					where mod.Active
					select mod.Directory;
			}
		}
		static InstalledModLister()
		{
			InstalledModLister.mods = new List<InstalledMod>();
			InstalledModLister.RebuildModList();
		}
		internal static void EnsureInit()
		{
		}
		internal static void RebuildModList()
		{
			InstalledModLister.mods.Clear();
			DirectoryInfo[] directories = new DirectoryInfo(GenFilePaths.CoreModsFolderPath).GetDirectories();
			for (int i = 0; i < directories.Length; i++)
			{
				DirectoryInfo directoryInfo = directories[i];
				InstalledModLister.mods.Add(new InstalledMod(directoryInfo.FullName));
			}
			if (InstalledModLister.mods.Count((InstalledMod m) => m.Active) == 0)
			{
				InstalledModLister.mods.First((InstalledMod m) => m.Identifier == LoadedMod.CoreModFolderName).Active = true;
			}
			ModsConfig.DeactivateUninstalledMods();
		}
		internal static InstalledMod GetNamed(string n)
		{
			for (int i = 0; i < InstalledModLister.mods.Count; i++)
			{
				if (InstalledModLister.mods[i].Identifier == n)
				{
					return InstalledModLister.mods[i];
				}
			}
			return null;
		}
	}
}
