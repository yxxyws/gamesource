using System;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.Steam;
namespace RimWorld
{
	public static class VersionControl
	{
		private static Version version;
		private static string versionStringFull;
		private static string versionStringShort;
		private static DateTime buildDate;
		public static Version CurrentVersion
		{
			get
			{
				return VersionControl.version;
			}
		}
		public static string CurrentVersionShort
		{
			get
			{
				return VersionControl.versionStringShort;
			}
		}
		public static string CurrentVersionFull
		{
			get
			{
				return VersionControl.versionStringFull;
			}
		}
		public static int CurrentMajor
		{
			get
			{
				return VersionControl.version.Major;
			}
		}
		public static int CurrentMinor
		{
			get
			{
				return VersionControl.version.Minor;
			}
		}
		public static int CurrentBuild
		{
			get
			{
				return VersionControl.version.Build;
			}
		}
		public static int CurrentRevision
		{
			get
			{
				return VersionControl.version.Revision;
			}
		}
		public static DateTime CurrentBuildDate
		{
			get
			{
				return VersionControl.buildDate;
			}
		}
		static VersionControl()
		{
			Version version = Assembly.GetExecutingAssembly().GetName().Version;
			DateTime dateTime = new DateTime(2000, 1, 1);
			VersionControl.buildDate = dateTime.AddDays((double)version.Build);
			int build = version.Build - 4805;
			int revision = version.Revision * 2 / 60;
			VersionControl.version = new Version(version.Major, version.Minor, build, revision);
			VersionControl.versionStringFull = string.Concat(new object[]
			{
				VersionControl.version.Major,
				".",
				VersionControl.version.Minor,
				".",
				VersionControl.version.Build,
				" rev",
				VersionControl.version.Revision
			});
			VersionControl.versionStringShort = string.Concat(new object[]
			{
				VersionControl.version.Major,
				".",
				VersionControl.version.Minor,
				".",
				VersionControl.version.Build
			});
		}
		public static void DrawInfoInCorner()
		{
			Text.Font = GameFont.Small;
			GUI.color = new Color(1f, 1f, 1f, 0.5f);
			string text = "VersionIndicator".Translate(new object[]
			{
				VersionControl.versionStringShort
			});
			if (Game.isDebugBuild)
			{
				text = text + " (" + "DevelopmentBuildLower".Translate() + ")";
			}
			text = text + "\n" + "CompiledOn".Translate(new object[]
			{
				VersionControl.buildDate.ToString("MMM d yyyy")
			});
			if (SteamManager.Initialized)
			{
				text = text + "\n" + "LoggedIntoSteamAs".Translate(new object[]
				{
					SteamManager.SteamLoginName
				});
			}
			Rect rect = new Rect(10f, 10f, 330f, Text.CalcHeight(text, 330f));
			Widgets.Label(rect, text);
			GUI.color = Color.white;
			LatestVersionGetter component = Find.RootGameObject.gameObject.GetComponent<LatestVersionGetter>();
			Rect rect2 = new Rect(10f, rect.yMax - 5f, 330f, 999f);
			component.DrawAt(rect2);
		}
		public static void LogVersionNumber()
		{
			Log.Message("RimWorld " + VersionControl.versionStringFull);
		}
		public static int BuildFromVersionString(string str)
		{
			str = VersionControl.VersionStringWithoutRev(str);
			int result = 0;
			if (!int.TryParse(str.Split(new char[]
			{
				'.'
			})[2], out result))
			{
				Log.Warning("Could not get build from version string " + str);
			}
			return result;
		}
		public static int MinorFromVersionString(string str)
		{
			str = VersionControl.VersionStringWithoutRev(str);
			int result = 0;
			if (!int.TryParse(str.Split(new char[]
			{
				'.'
			})[1], out result))
			{
				Log.Warning("Could not get minor version from version string " + str);
			}
			return result;
		}
		public static int MajorFromVersionString(string str)
		{
			str = VersionControl.VersionStringWithoutRev(str);
			int result = 0;
			if (!int.TryParse(str.Split(new char[]
			{
				'.'
			})[0], out result))
			{
				Log.Warning("Could not get major version from version string " + str);
			}
			return result;
		}
		public static string VersionStringWithoutRev(string str)
		{
			return str.Split(new char[]
			{
				' '
			})[0];
		}
	}
}
