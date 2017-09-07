using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace Verse
{
	public class LoadedMod
	{
		public string name;
		public int loadOrder;
		private ModContentHolder<AudioClip> audioClips;
		private ModContentHolder<Texture2D> textures;
		private ModContentHolder<string> strings;
		public ModAssemblyHandler assemblies;
		private List<DefPackage> defPackages = new List<DefPackage>();
		public static readonly string CoreModFolderName = "Core";
		public string RootFolder
		{
			get
			{
				return Path.Combine(GenFilePaths.CoreModsFolderPath, this.name);
			}
		}
		public int OverwritePriority
		{
			get
			{
				if (this.name == LoadedMod.CoreModFolderName)
				{
					return 0;
				}
				return 1;
			}
		}
		public IEnumerable<Def> AllDefs
		{
			get
			{
				return this.defPackages.SelectMany((DefPackage x) => x.defs);
			}
		}
		public LoadedMod(string name, int loadOrder)
		{
			this.name = name;
			this.loadOrder = loadOrder;
			this.audioClips = new ModContentHolder<AudioClip>(this);
			this.textures = new ModContentHolder<Texture2D>(this);
			this.strings = new ModContentHolder<string>(this);
			this.assemblies = new ModAssemblyHandler(this);
		}
		public void ClearDestroy()
		{
			this.audioClips.ClearDestroy();
			this.textures.ClearDestroy();
		}
		public ModContentHolder<T> GetContentHolder<T>() where T : class
		{
			if (typeof(T) == typeof(Texture2D))
			{
				return (ModContentHolder<T>)this.textures;
			}
			if (typeof(T) == typeof(AudioClip))
			{
				return (ModContentHolder<T>)this.audioClips;
			}
			if (typeof(T) == typeof(string))
			{
				return (ModContentHolder<T>)this.strings;
			}
			Log.Error("Mod lacks manager for asset type " + this.strings);
			return null;
		}
		public void ReloadAllContent()
		{
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.audioClips.ReloadAll();
				this.textures.ReloadAll();
				this.strings.ReloadAll();
			});
			this.assemblies.ReloadAll();
			this.LoadDefs();
		}
		private void LoadDefs()
		{
			DeepProfiler.Start("Loading all defs");
			List<LoadableXmlAsset> list = XmlLoader.XmlAssetsInModFolder(this, "Defs/").ToList<LoadableXmlAsset>();
			for (int i = 0; i < list.Count; i++)
			{
				XmlInheritance.TryRegisterAllFrom(list[i], this);
			}
			XmlInheritance.Resolve();
			for (int j = 0; j < list.Count; j++)
			{
				string relFolder = GenFilePaths.FolderPathRelativeToDefsFolder(list[j].fullFolderPath, this);
				DefPackage defPackage = new DefPackage(list[j].name, relFolder);
				foreach (Def current in XmlLoader.AllDefsFromAsset(list[j]))
				{
					defPackage.defs.Add(current);
				}
				this.defPackages.Add(defPackage);
			}
			DeepProfiler.End();
		}
		public IEnumerable<DefPackage> GetDefPackagesInFolder(string relFolder)
		{
			string path = Path.Combine(Path.Combine(this.RootFolder, "Defs/"), relFolder);
			if (!Directory.Exists(path))
			{
				return Enumerable.Empty<DefPackage>();
			}
			string fullPath = Path.GetFullPath(path);
			return 
				from x in this.defPackages
				where x.GetFullFolderPath(this).StartsWith(fullPath)
				select x;
		}
		public void AddDefPackage(DefPackage defPackage)
		{
			this.defPackages.Add(defPackage);
		}
		public override string ToString()
		{
			return this.name;
		}
	}
}
