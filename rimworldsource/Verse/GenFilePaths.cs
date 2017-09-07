using System;
using System.IO;
using UnityEngine;
namespace Verse
{
	public static class GenFilePaths
	{
		public const string SoundsFolder = "Sounds/";
		public const string TexturesFolder = "Textures/";
		public const string StringsFolder = "Strings/";
		public const string DefsFolder = "Defs/";
		public const string BackstoriesPath = "Backstories";
		public const string SavedGameExtension = ".rws";
		public const string WorldExtension = ".rww";
		public const string ExternalHistoryFileExtension = ".rwh";
		private const string SaveDataFolderCommand = "savedatafolder";
		private static string saveDataPath = null;
		private static string coreModsFolderPath = null;
		private static readonly string[] FilePathRaw = new string[]
		{
			"Ž",
			"ž",
			"Ÿ",
			"¡",
			"¢",
			"£",
			"¤",
			"¥",
			"¦",
			"§",
			"¨",
			"©",
			"ª",
			"À",
			"Á",
			"Â",
			"Ã",
			"Ä",
			"Å",
			"Æ",
			"Ç",
			"È",
			"É",
			"Ê",
			"Ë",
			"Ì",
			"Í",
			"Î",
			"Ï",
			"Ð",
			"Ñ",
			"Ò",
			"Ó",
			"Ô",
			"Õ",
			"Ö",
			"Ù",
			"Ú",
			"Û",
			"Ü",
			"Ý",
			"Þ",
			"ß",
			"à",
			"á",
			"â",
			"ã",
			"ä",
			"å",
			"æ",
			"ç",
			"è",
			"é",
			"ê",
			"ë",
			"ì",
			"í",
			"î",
			"ï",
			"ð",
			"ñ",
			"ò",
			"ó",
			"ô",
			"õ",
			"ö",
			"ù",
			"ú",
			"û",
			"ü",
			"ý",
			"þ",
			"ÿ"
		};
		private static readonly string[] FilePathSafe = new string[]
		{
			"%8E",
			"%9E",
			"%9F",
			"%A1",
			"%A2",
			"%A3",
			"%A4",
			"%A5",
			"%A6",
			"%A7",
			"%A8",
			"%A9",
			"%AA",
			"%C0",
			"%C1",
			"%C2",
			"%C3",
			"%C4",
			"%C5",
			"%C6",
			"%C7",
			"%C8",
			"%C9",
			"%CA",
			"%CB",
			"%CC",
			"%CD",
			"%CE",
			"%CF",
			"%D0",
			"%D1",
			"%D2",
			"%D3",
			"%D4",
			"%D5",
			"%D6",
			"%D9",
			"%DA",
			"%DB",
			"%DC",
			"%DD",
			"%DE",
			"%DF",
			"%E0",
			"%E1",
			"%E2",
			"%E3",
			"%E4",
			"%E5",
			"%E6",
			"%E7",
			"%E8",
			"%E9",
			"%EA",
			"%EB",
			"%EC",
			"%ED",
			"%EE",
			"%EF",
			"%F0",
			"%F1",
			"%F2",
			"%F3",
			"%F4",
			"%F5",
			"%F6",
			"%F9",
			"%FA",
			"%FB",
			"%FC",
			"%FD",
			"%FE",
			"%FF"
		};
		public static string SaveDataFolderPath
		{
			get
			{
				if (GenFilePaths.saveDataPath == null)
				{
					string text;
					if (GenCommandLine.TryGetCommandLineArg("savedatafolder", out text))
					{
						text.TrimEnd(new char[]
						{
							'\\',
							'/'
						});
						if (text == string.Empty)
						{
							text = string.Empty + Path.DirectorySeparatorChar;
						}
						GenFilePaths.saveDataPath = text;
						Log.Message("Save data folder overridden to " + GenFilePaths.saveDataPath);
					}
					else
					{
						DirectoryInfo directoryInfo = new DirectoryInfo(Game.dataPath);
						if (Game.isEditor)
						{
							GenFilePaths.saveDataPath = Path.Combine(directoryInfo.Parent.ToString(), "SaveData");
						}
						else
						{
							if (Game.platform == RuntimePlatform.OSXPlayer || Game.platform == RuntimePlatform.OSXWebPlayer || Game.platform == RuntimePlatform.OSXEditor || Game.platform == RuntimePlatform.OSXDashboardPlayer)
							{
								DirectoryInfo parent = Directory.GetParent(Game.persistentDataPath);
								string path = Path.Combine(parent.ToString(), "RimWorld");
								if (!Directory.Exists(path))
								{
									Directory.CreateDirectory(path);
								}
								GenFilePaths.saveDataPath = path;
							}
							else
							{
								GenFilePaths.saveDataPath = Application.persistentDataPath;
							}
						}
					}
					DirectoryInfo directoryInfo2 = new DirectoryInfo(GenFilePaths.saveDataPath);
					if (!directoryInfo2.Exists)
					{
						directoryInfo2.Create();
					}
				}
				return GenFilePaths.saveDataPath;
			}
		}
		public static string CoreModsFolderPath
		{
			get
			{
				if (GenFilePaths.coreModsFolderPath == null)
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(Game.dataPath);
					DirectoryInfo directoryInfo2 = directoryInfo.Parent;
					if (Game.isEditor)
					{
						directoryInfo2 = directoryInfo;
					}
					else
					{
						directoryInfo2 = directoryInfo.Parent;
					}
					GenFilePaths.coreModsFolderPath = Path.Combine(directoryInfo2.ToString(), "Mods");
					if (Game.isDebugBuild)
					{
						DirectoryInfo directoryInfo3 = new DirectoryInfo(GenFilePaths.coreModsFolderPath);
						if (!directoryInfo3.Exists)
						{
							GenFilePaths.coreModsFolderPath = Path.Combine(directoryInfo.Parent.Parent.ToString(), "RimWorld/Assets/Mods");
						}
					}
					DirectoryInfo directoryInfo4 = new DirectoryInfo(GenFilePaths.coreModsFolderPath);
					if (!directoryInfo4.Exists)
					{
						directoryInfo4.Create();
					}
				}
				return GenFilePaths.coreModsFolderPath;
			}
		}
		public static string ConfigFolderPath
		{
			get
			{
				return GenFilePaths.FolderUnderSaveData("Config");
			}
		}
		public static string SavedMapsFolderPath
		{
			get
			{
				return GenFilePaths.FolderUnderSaveData("Saves");
			}
		}
		public static string WorldsFolderPath
		{
			get
			{
				return GenFilePaths.FolderUnderSaveData("Worlds");
			}
		}
		public static string ExternalHistoryFolderPath
		{
			get
			{
				return GenFilePaths.FolderUnderSaveData("External");
			}
		}
		public static string ScreenshotFolderPath
		{
			get
			{
				return GenFilePaths.FolderUnderSaveData("Screenshots");
			}
		}
		public static string ConfigFilePath
		{
			get
			{
				return Path.Combine(GenFilePaths.ConfigFolderPath, "ModsConfig.xml");
			}
		}
		public static string ConceptKnowledgeFilePath
		{
			get
			{
				return Path.Combine(GenFilePaths.ConfigFolderPath, "Knowledge.xml");
			}
		}
		public static string PrefsFilePath
		{
			get
			{
				return Path.Combine(GenFilePaths.ConfigFolderPath, "Prefs.xml");
			}
		}
		public static string KeyPrefsFilePath
		{
			get
			{
				return Path.Combine(GenFilePaths.ConfigFolderPath, "KeyPrefs.xml");
			}
		}
		private static string FolderUnderSaveData(string folderName)
		{
			string text = Path.Combine(GenFilePaths.SaveDataFolderPath, folderName);
			DirectoryInfo directoryInfo = new DirectoryInfo(text);
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
			}
			return text;
		}
		public static string FilePathForSavedGame(string gameName)
		{
			return Path.Combine(GenFilePaths.SavedMapsFolderPath, gameName + ".rws");
		}
		public static string FilePathForWorld(string worldName)
		{
			return Path.Combine(GenFilePaths.WorldsFolderPath, worldName + ".rww");
		}
		public static string ContentPath<T>()
		{
			if (typeof(T) == typeof(AudioClip))
			{
				return "Sounds/";
			}
			if (typeof(T) == typeof(Texture2D))
			{
				return "Textures/";
			}
			if (typeof(T) == typeof(string))
			{
				return "Strings/";
			}
			throw new ArgumentException();
		}
		public static string FolderPathRelativeToDefsFolder(string fullFolderPath, LoadedMod mod)
		{
			fullFolderPath = Path.GetFullPath(fullFolderPath).Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
			string text = Path.GetFullPath(Path.Combine(mod.RootFolder, "Defs/")).Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
			if (!fullFolderPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
			{
				fullFolderPath += Path.DirectorySeparatorChar;
			}
			if (!text.EndsWith(Path.DirectorySeparatorChar.ToString()))
			{
				text += Path.DirectorySeparatorChar;
			}
			if (!fullFolderPath.StartsWith(text))
			{
				Log.Error(string.Concat(new string[]
				{
					"Can't get relative path. Path \"",
					fullFolderPath,
					"\" does not start with \"",
					text,
					"\"."
				}));
				return null;
			}
			if (fullFolderPath == text)
			{
				return string.Empty;
			}
			string text2 = fullFolderPath.Substring(text.Length);
			while (text2.StartsWith("/") || text2.StartsWith("\\"))
			{
				if (text2.Length == 1)
				{
					return string.Empty;
				}
				text2 = text2.Substring(1);
			}
			return text2;
		}
		public static string SafeURIForUnityWWWFromPath(string rawPath)
		{
			string text = rawPath;
			for (int i = 0; i < GenFilePaths.FilePathRaw.Length; i++)
			{
				text = text.Replace(GenFilePaths.FilePathRaw[i], GenFilePaths.FilePathSafe[i]);
			}
			return "file:///" + text;
		}
	}
}
