using System;
using UnityEngine;
namespace Verse
{
	public static class RootManager
	{
		public static Root root;
		public static RootEntry rootEntry;
		public static RootMap rootMap;
		public static void Notify_LoadedLevelChanged()
		{
			if (Application.loadedLevelName == "Entry")
			{
				Game.Mode = GameMode.Entry;
				RootManager.rootEntry = new RootEntry();
				RootManager.rootMap = null;
				RootManager.root = RootManager.rootEntry;
			}
			else
			{
				if (Application.loadedLevelName == "Gameplay")
				{
					Game.Mode = GameMode.MapInitializing;
					RootManager.rootEntry = null;
					RootManager.rootMap = new RootMap();
					RootManager.root = RootManager.rootMap;
				}
			}
			Find.ResetReferences();
		}
	}
}
