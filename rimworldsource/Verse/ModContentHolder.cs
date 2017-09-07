using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
namespace Verse
{
	public class ModContentHolder<T> where T : class
	{
		private LoadedMod mod;
		public Dictionary<string, T> contentList = new Dictionary<string, T>();
		public ModContentHolder(LoadedMod mod)
		{
			this.mod = mod;
		}
		public void ClearDestroy()
		{
			if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
			{
				foreach (T obj in this.contentList.Values)
				{
					LongEventHandler.ExecuteWhenFinished(delegate
					{
						UnityEngine.Object.Destroy((UnityEngine.Object)((object)obj));
					});
				}
			}
			this.contentList.Clear();
		}
		public void ReloadAll()
		{
			foreach (LoadedContentItem<T> current in ModContentLoader<T>.LoadAllForMod(this.mod))
			{
				if (this.contentList.ContainsKey(current.internalPath))
				{
					Log.Warning(string.Concat(new object[]
					{
						"Tried to load duplicate ",
						typeof(T),
						" with path: ",
						current.internalPath
					}));
				}
				else
				{
					this.contentList.Add(current.internalPath, current.contentItem);
				}
			}
		}
		public T Get(string path)
		{
			T result;
			if (this.contentList.TryGetValue(path, out result))
			{
				return result;
			}
			return (T)((object)null);
		}
		[DebuggerHidden]
		public IEnumerable<T> GetAllUnderPath(string pathRoot)
		{
			ModContentHolder<T>.<GetAllUnderPath>c__Iterator172 <GetAllUnderPath>c__Iterator = new ModContentHolder<T>.<GetAllUnderPath>c__Iterator172();
			<GetAllUnderPath>c__Iterator.pathRoot = pathRoot;
			<GetAllUnderPath>c__Iterator.<$>pathRoot = pathRoot;
			<GetAllUnderPath>c__Iterator.<>f__this = this;
			ModContentHolder<T>.<GetAllUnderPath>c__Iterator172 expr_1C = <GetAllUnderPath>c__Iterator;
			expr_1C.$PC = -2;
			return expr_1C;
		}
	}
}
