using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
namespace Verse
{
	public class ScribeHeaderUtility
	{
		public enum ScribeHeaderMode
		{
			None,
			Map,
			World
		}
		public const string HeaderNodeName = "header";
		public const string ModsListNodeName = "mods";
		public const string GameVersionNodeName = "gameVersion";
		private static ScribeHeaderUtility.ScribeHeaderMode lastMode;
		public static string loadedGameVersion;
		public static List<string> loadedModsList;
		public static void WriteGameDataHeader()
		{
			Scribe.EnterNode("header");
			string currentVersionFull = VersionControl.CurrentVersionFull;
			Scribe_Values.LookValue<string>(ref currentVersionFull, "gameVersion", null, false);
			List<string> list = (
				from mod in LoadedModManager.LoadedMods
				select mod.name).ToList<string>();
			Scribe_Collections.LookList<string>(ref list, "mods", LookMode.Undefined, new object[0]);
			Scribe.ExitNode();
		}
		public static void LoadGameDataHeader(ScribeHeaderUtility.ScribeHeaderMode mode)
		{
			Scribe.EnterNode("header");
			ScribeHeaderUtility.loadedGameVersion = null;
			ScribeHeaderUtility.loadedModsList = null;
			ScribeHeaderUtility.lastMode = mode;
			Scribe_Values.LookValue<string>(ref ScribeHeaderUtility.loadedGameVersion, "gameVersion", null, false);
			Scribe_Collections.LookList<string>(ref ScribeHeaderUtility.loadedModsList, "mods", LookMode.Undefined, new object[0]);
			if ((mode == ScribeHeaderUtility.ScribeHeaderMode.Map || !Game.isEditor) && !ScribeHeaderUtility.VersionsMatch())
			{
				Log.Warning(string.Concat(new string[]
				{
					"Loaded file is from version ",
					ScribeHeaderUtility.loadedGameVersion,
					", we are running version ",
					VersionControl.CurrentVersionFull,
					"."
				}));
			}
			Scribe.ExitNode();
		}
		private static bool VersionsMatch()
		{
			return VersionControl.BuildFromVersionString(ScribeHeaderUtility.loadedGameVersion) == VersionControl.BuildFromVersionString(VersionControl.CurrentVersionFull);
		}
		public static void CreateDialogsForVersionMismatchWarnings()
		{
			if (!ScribeHeaderUtility.VersionsMatch())
			{
				string text = (!ScribeHeaderUtility.loadedGameVersion.NullOrEmpty()) ? ScribeHeaderUtility.loadedGameVersion : ("(" + "UnknownLower".Translate() + ")");
				string text2;
				if (ScribeHeaderUtility.lastMode == ScribeHeaderUtility.ScribeHeaderMode.Map)
				{
					text2 = "SaveGameIncompatibleWarningText".Translate(new object[]
					{
						text,
						VersionControl.CurrentVersionFull
					});
				}
				else
				{
					if (ScribeHeaderUtility.lastMode != ScribeHeaderUtility.ScribeHeaderMode.World)
					{
						throw new InvalidOperationException();
					}
					text2 = "WorldFileVersionMismatch".Translate(new object[]
					{
						text,
						VersionControl.CurrentVersionFull
					});
				}
				Find.WindowStack.Add(new Dialog_Message(text2, "VersionMismatch".Translate()));
			}
			string text3;
			string text4;
			if (!ScribeHeaderUtility.LoadedHeaderModListMatchesModList(out text3, out text4))
			{
				string text5 = "ModsMismatchWarningText".Translate(new object[]
				{
					text3,
					text4
				});
				Find.WindowStack.Add(new Dialog_Message(text5, "ModsMismatchWarningTitle".Translate()));
			}
		}
		public static bool LoadedHeaderModListMatchesModList(out string loadedHeaderMods, out string loadedMods)
		{
			loadedHeaderMods = null;
			loadedMods = null;
			IEnumerable<string> enumerable = 
				from mod in LoadedModManager.LoadedMods
				select mod.name;
			if (ScribeHeaderUtility.loadedModsList == null || !ScribeHeaderUtility.ModListsMatch(ScribeHeaderUtility.loadedModsList, enumerable.ToList<string>()))
			{
				if (ScribeHeaderUtility.loadedModsList == null)
				{
					loadedHeaderMods = "None".Translate();
				}
				else
				{
					loadedHeaderMods = GenText.ToCommaList(ScribeHeaderUtility.loadedModsList);
				}
				loadedMods = GenText.ToCommaList(enumerable);
				return false;
			}
			return true;
		}
		private static bool ModListsMatch(List<string> a, List<string> b)
		{
			if (a.Count != b.Count)
			{
				return false;
			}
			for (int i = 0; i < a.Count; i++)
			{
				if (a[i] != b[i])
				{
					return false;
				}
			}
			return true;
		}
		public static string GameVersionOf(FileInfo file)
		{
			if (!file.Exists)
			{
				throw new ArgumentException();
			}
			try
			{
				using (StreamReader streamReader = new StreamReader(file.FullName))
				{
					using (XmlTextReader xmlTextReader = new XmlTextReader(streamReader))
					{
						if (xmlTextReader.ReadToDescendant("header") && xmlTextReader.ReadToDescendant("gameVersion"))
						{
							return VersionControl.VersionStringWithoutRev(xmlTextReader.ReadString());
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error("Exception getting game version of " + file.Name + ": " + ex.ToString());
			}
			return null;
		}
	}
}
