using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
namespace Verse
{
	public static class ContentFinder<T> where T : class
	{
		public static T Get(string itemPath, bool reportFailure = true)
		{
			T t = (T)((object)null);
			foreach (LoadedMod current in LoadedModManager.LoadedMods)
			{
				t = current.GetContentHolder<T>().Get(itemPath);
				if (t != null)
				{
					T result = t;
					return result;
				}
			}
			if (typeof(T) == typeof(Texture2D))
			{
				t = (T)((object)Resources.Load<Texture2D>(GenFilePaths.ContentPath<Texture2D>() + itemPath));
			}
			if (typeof(T) == typeof(AudioClip))
			{
				t = (T)((object)Resources.Load<AudioClip>(GenFilePaths.ContentPath<AudioClip>() + itemPath));
			}
			if (t != null)
			{
				return t;
			}
			if (reportFailure)
			{
				Log.Error(string.Concat(new object[]
				{
					"Could not load ",
					typeof(T),
					" at ",
					itemPath,
					" in any active mod or in base resources."
				}));
			}
			return (T)((object)null);
		}
		[DebuggerHidden]
		public static IEnumerable<T> GetAllInFolder(string folderPath)
		{
			ContentFinder<T>.<GetAllInFolder>c__Iterator171 <GetAllInFolder>c__Iterator = new ContentFinder<T>.<GetAllInFolder>c__Iterator171();
			<GetAllInFolder>c__Iterator.folderPath = folderPath;
			<GetAllInFolder>c__Iterator.<$>folderPath = folderPath;
			ContentFinder<T>.<GetAllInFolder>c__Iterator171 expr_15 = <GetAllInFolder>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}
	}
}
