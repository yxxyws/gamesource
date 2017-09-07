using System;
using System.Collections.Generic;
using System.Linq;
namespace Verse
{
	public static class LoadedModManager
	{
		private static List<LoadedMod> loadedMods = new List<LoadedMod>();
		public static IEnumerable<LoadedMod> LoadedMods
		{
			get
			{
				return LoadedModManager.loadedMods;
			}
		}
		public static void LoadAllActiveMods()
		{
			XmlInheritance.Clear();
			int num = 0;
			foreach (InstalledMod current in ModsConfig.ActiveMods.ToList<InstalledMod>())
			{
				DeepProfiler.Start("Loading " + current);
				if (!current.Directory.Exists)
				{
					ModsConfig.SetActive(current.Identifier, false);
					Log.Warning(string.Concat(new object[]
					{
						"Failed to find active mod ",
						current.Name,
						"(",
						current.Identifier,
						") at ",
						current.Directory
					}));
					DeepProfiler.End();
				}
				else
				{
					LoadedMod loadedMod = new LoadedMod(current.Identifier, num);
					num++;
					LoadedModManager.loadedMods.Add(loadedMod);
					loadedMod.ReloadAllContent();
					DeepProfiler.End();
				}
			}
			XmlInheritance.Clear();
		}
		public static void ClearDestroy()
		{
			foreach (LoadedMod current in LoadedModManager.loadedMods)
			{
				current.ClearDestroy();
			}
			LoadedModManager.loadedMods.Clear();
		}
	}
}
