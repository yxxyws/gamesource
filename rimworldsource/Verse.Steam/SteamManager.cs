using Steamworks;
using System;
using System.Text;
namespace Verse.Steam
{
	public static class SteamManager
	{
		private static bool initializedInt;
		private static string cachedPersonaName;
		public static bool Initialized
		{
			get
			{
				return SteamManager.initializedInt;
			}
		}
		public static bool Active
		{
			get
			{
				return true;
			}
		}
		public static string SteamLoginName
		{
			get
			{
				if (SteamManager.Initialized && SteamManager.cachedPersonaName == null)
				{
					SteamManager.cachedPersonaName = SteamFriends.GetPersonaName();
				}
				return (SteamManager.cachedPersonaName == null) ? "???" : SteamManager.cachedPersonaName;
			}
		}
		public static void InitIfNeeded()
		{
		}
		public static void Update()
		{
			if (!SteamManager.initializedInt)
			{
				return;
			}
			SteamAPI.RunCallbacks();
		}
		public static void ShutdownSteam()
		{
			if (!SteamManager.initializedInt)
			{
				return;
			}
			SteamAPI.Shutdown();
		}
		private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
		{
			Log.Error(pchDebugText.ToString());
		}
	}
}
