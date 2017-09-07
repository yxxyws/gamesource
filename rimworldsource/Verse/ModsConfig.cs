using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Verse
{
	public static class ModsConfig
	{
		private class ModsConfigData
		{
			public int buildNumber = -1;
			public List<string> activeMods = new List<string>();
		}
		private static ModsConfig.ModsConfigData data;
		public static IEnumerable<InstalledMod> ActiveMods
		{
			get
			{
				ModsConfig.<>c__Iterator174 <>c__Iterator = new ModsConfig.<>c__Iterator174();
				ModsConfig.<>c__Iterator174 expr_07 = <>c__Iterator;
				expr_07.$PC = -2;
				return expr_07;
			}
		}
		static ModsConfig()
		{
			bool flag = File.Exists(GenFilePaths.ConfigFilePath);
			bool flag2 = false;
			ModsConfig.data = XmlLoader.ItemFromXmlFile<ModsConfig.ModsConfigData>(GenFilePaths.ConfigFilePath, true);
			if (ModsConfig.data.buildNumber < VersionControl.CurrentBuild)
			{
				Log.Message(string.Concat(new object[]
				{
					"Mods config data is from build ",
					ModsConfig.data.buildNumber,
					" while we are at build ",
					VersionControl.CurrentBuild,
					". Resetting."
				}));
				ModsConfig.data = new ModsConfig.ModsConfigData();
				flag2 = true;
			}
			ModsConfig.data.buildNumber = VersionControl.CurrentBuild;
			if (!flag || flag2)
			{
				ModsConfig.data.activeMods.Add(LoadedMod.CoreModFolderName);
				ModsConfig.Save();
			}
		}
		public static void DeactivateUninstalledMods()
		{
			int i;
			for (i = ModsConfig.data.activeMods.Count - 1; i >= 0; i--)
			{
				if (!InstalledModLister.AllInstalledMods.Any((InstalledMod m) => m.Identifier == ModsConfig.data.activeMods[i]))
				{
					ModsConfig.data.activeMods.RemoveAt(i);
				}
			}
		}
		public static void Reset()
		{
			ModsConfig.data.activeMods.Clear();
			ModsConfig.data.activeMods.Add(LoadedMod.CoreModFolderName);
			ModsConfig.Save();
		}
		public static bool IsActive(string modIdentifier)
		{
			return ModsConfig.data.activeMods.Contains(modIdentifier);
		}
		public static void SetActive(string modIdentifier, bool active)
		{
			if (active)
			{
				if (!ModsConfig.data.activeMods.Contains(modIdentifier))
				{
					ModsConfig.data.activeMods.Add(modIdentifier);
				}
			}
			else
			{
				if (ModsConfig.data.activeMods.Contains(modIdentifier))
				{
					ModsConfig.data.activeMods.Remove(modIdentifier);
				}
			}
		}
		internal static int ActiveLoadOrder(string modIdentifier)
		{
			if (!ModsConfig.data.activeMods.Contains(modIdentifier))
			{
				return -1;
			}
			return ModsConfig.data.activeMods.IndexOf(modIdentifier);
		}
		public static void Save()
		{
			XmlSaver.SaveDataObject(ModsConfig.data, GenFilePaths.ConfigFilePath);
		}
	}
}
