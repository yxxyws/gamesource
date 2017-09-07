using System;
using System.Threading;
using UnityEngine;
namespace Verse
{
	public static class Game
	{
		private static bool godMode;
		private static GameMode mode;
		public static bool isDebugBuild;
		public static bool isEditor;
		public static string dataPath;
		public static RuntimePlatform platform;
		public static string persistentDataPath;
		private static int mainThreadId;
		public static GameMode Mode
		{
			get
			{
				return Game.mode;
			}
			set
			{
				Game.mode = value;
			}
		}
		public static bool GodMode
		{
			get
			{
				return Game.godMode && Prefs.DevMode;
			}
			set
			{
				Game.godMode = value;
			}
		}
		public static bool IsInMainThread
		{
			get
			{
				return Game.mainThreadId == Thread.CurrentThread.ManagedThreadId;
			}
		}
		public static void Init()
		{
			Game.mainThreadId = Thread.CurrentThread.ManagedThreadId;
			Game.isDebugBuild = Debug.isDebugBuild;
			Game.isEditor = Application.isEditor;
			Game.dataPath = Application.dataPath;
			Game.platform = Application.platform;
			Game.persistentDataPath = Application.persistentDataPath;
		}
	}
}
