using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;
namespace Verse
{
	public static class ModContentLoader<T> where T : class
	{
		private static string[] AcceptableExtensionsAudio = new string[]
		{
			".wav",
			".mp3",
			".ogg",
			".xm",
			".it",
			".mod",
			".s3m"
		};
		private static string[] AcceptableExtensionsTexture = new string[]
		{
			".png",
			".jpg"
		};
		private static string[] AcceptableExtensionsString = new string[]
		{
			".txt"
		};
		private static bool IsAcceptableExtension(string extension)
		{
			string[] array;
			if (typeof(T) == typeof(AudioClip))
			{
				array = ModContentLoader<T>.AcceptableExtensionsAudio;
			}
			else
			{
				if (typeof(T) == typeof(Texture2D))
				{
					array = ModContentLoader<T>.AcceptableExtensionsTexture;
				}
				else
				{
					if (typeof(T) != typeof(string))
					{
						Log.Error("Unknown content type " + typeof(T));
						return false;
					}
					array = ModContentLoader<T>.AcceptableExtensionsString;
				}
			}
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string b = array2[i];
				if (extension.ToLower() == b)
				{
					return true;
				}
			}
			return false;
		}
		[DebuggerHidden]
		public static IEnumerable<LoadedContentItem<T>> LoadAllForMod(LoadedMod mod)
		{
			ModContentLoader<T>.<LoadAllForMod>c__Iterator173 <LoadAllForMod>c__Iterator = new ModContentLoader<T>.<LoadAllForMod>c__Iterator173();
			<LoadAllForMod>c__Iterator.mod = mod;
			<LoadAllForMod>c__Iterator.<$>mod = mod;
			ModContentLoader<T>.<LoadAllForMod>c__Iterator173 expr_15 = <LoadAllForMod>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}
		public static LoadedContentItem<T> LoadItem(string absFilePath, string contentDirPath = null)
		{
			string text = absFilePath;
			if (contentDirPath != null)
			{
				text = text.Substring(contentDirPath.ToString().Length);
			}
			text = text.Substring(0, text.Length - Path.GetExtension(text).Length);
			text = text.Replace('\\', '/');
			try
			{
				if (typeof(T) == typeof(string))
				{
					LoadedContentItem<T> result = new LoadedContentItem<T>(text, (T)((object)GenFile.TextFromRawFile(absFilePath)));
					return result;
				}
				if (typeof(T) == typeof(Texture2D))
				{
					LoadedContentItem<T> result = new LoadedContentItem<T>(text, (T)((object)ModContentLoader<T>.LoadPNG(absFilePath)));
					return result;
				}
				if (typeof(T) == typeof(AudioClip))
				{
					if (Prefs.LogVerbose)
					{
						DeepProfiler.Start("Loading file " + text);
					}
					string url = GenFilePaths.SafeURIForUnityWWWFromPath(absFilePath);
					WWW wWW = new WWW(url);
					while (!wWW.isDone)
					{
						Thread.Sleep(1);
					}
					if (Prefs.LogVerbose)
					{
						DeepProfiler.End();
					}
					if (wWW.error != null)
					{
						throw new InvalidOperationException(wWW.error);
					}
					T t = (T)((object)wWW.audioClip);
					UnityEngine.Object @object = t as UnityEngine.Object;
					if (@object != null)
					{
						@object.name = Path.GetFileNameWithoutExtension(new FileInfo(absFilePath).Name);
					}
					LoadedContentItem<T> result = new LoadedContentItem<T>(text, t);
					return result;
				}
			}
			catch (Exception ex)
			{
				Log.Error(string.Concat(new object[]
				{
					"Exception loading ",
					typeof(T),
					" from file.\nabsFilePath: ",
					absFilePath,
					"\ncontentDirPath: ",
					contentDirPath,
					"\nException: ",
					ex.ToString()
				}));
			}
			if (typeof(T) == typeof(Texture2D))
			{
				return (LoadedContentItem<T>)new LoadedContentItem<Texture2D>(absFilePath, BaseContent.BadTex);
			}
			return null;
		}
		private static Texture2D LoadPNG(string filePath)
		{
			Texture2D texture2D = null;
			if (File.Exists(filePath))
			{
				byte[] data = File.ReadAllBytes(filePath);
				texture2D = new Texture2D(2, 2);
				texture2D.LoadImage(data);
				texture2D.Compress(true);
				texture2D.name = Path.GetFileNameWithoutExtension(filePath);
			}
			return texture2D;
		}
	}
}
