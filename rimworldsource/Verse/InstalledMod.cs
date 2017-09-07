using Steamworks;
using System;
using System.IO;
using UnityEngine;
using Verse.Steam;
namespace Verse
{
	public class InstalledMod
	{
		private const string AboutFolderName = "About";
		public ModMetaData meta = new ModMetaData();
		private DirectoryInfo directoryInt;
		public Texture2D previewImage;
		private PublishedFileId_t publishedFileIdInt = PublishedFileId_t.Invalid;
		private CSteamID steamAuthorCached = CSteamID.Nil;
		public DirectoryInfo Directory
		{
			get
			{
				return this.directoryInt;
			}
		}
		public string Name
		{
			get
			{
				return this.meta.name;
			}
			set
			{
				this.meta.name = value;
			}
		}
		public string Author
		{
			get
			{
				return this.meta.author;
			}
		}
		public string Url
		{
			get
			{
				return this.meta.url;
			}
		}
		public string TargetVersion
		{
			get
			{
				return this.meta.targetVersion;
			}
		}
		public string Description
		{
			get
			{
				return this.meta.description;
			}
		}
		public string PreviewImagePath
		{
			get
			{
				return string.Concat(new object[]
				{
					this.directoryInt.FullName,
					Path.DirectorySeparatorChar,
					"About",
					Path.DirectorySeparatorChar,
					"Preview.png"
				});
			}
		}
		private string PublishedFileIdPath
		{
			get
			{
				return string.Concat(new object[]
				{
					this.directoryInt.FullName,
					Path.DirectorySeparatorChar,
					"About",
					Path.DirectorySeparatorChar,
					"PublishedFileId.txt"
				});
			}
		}
		public string Identifier
		{
			get
			{
				return this.Directory.Name;
			}
		}
		public bool IsCoreMod
		{
			get
			{
				return this.Identifier == LoadedMod.CoreModFolderName;
			}
		}
		public string SteamWorkshopPageUrl
		{
			get
			{
				if (!this.OnSteamWorkshop)
				{
					throw new InvalidOperationException();
				}
				return "steam://url/CommunityFilePage/" + this.PublishedFileId;
			}
		}
		public bool Active
		{
			get
			{
				return ModsConfig.IsActive(this.Identifier);
			}
			set
			{
				ModsConfig.SetActive(this.Identifier, value);
			}
		}
		public PublishedFileId_t PublishedFileId
		{
			get
			{
				return this.publishedFileIdInt;
			}
			set
			{
				if (this.publishedFileIdInt == value)
				{
					return;
				}
				this.publishedFileIdInt = value;
				File.WriteAllText(this.PublishedFileIdPath, value.ToString());
			}
		}
		public bool OnSteamWorkshop
		{
			get
			{
				return this.PublishedFileId != PublishedFileId_t.Invalid;
			}
		}
		public bool CanUploadToWorkshop
		{
			get
			{
				return !(this.Identifier == LoadedMod.CoreModFolderName) && (this.PublishedFileId == PublishedFileId_t.Invalid || this.SteamAuthor == SteamUser.GetSteamID());
			}
		}
		public CSteamID SteamAuthor
		{
			get
			{
				return this.steamAuthorCached;
			}
		}
		public InstalledMod(string absPath)
		{
			this.directoryInt = new DirectoryInfo(absPath);
			this.meta = XmlLoader.ItemFromXmlFile<ModMetaData>(string.Concat(new object[]
			{
				this.Directory.FullName,
				Path.DirectorySeparatorChar,
				"About",
				Path.DirectorySeparatorChar,
				"About.xml"
			}), true);
			if (this.meta.name.NullOrEmpty())
			{
				if (this.OnSteamWorkshop)
				{
					this.meta.name = "Workshop mod " + this.Identifier;
				}
				else
				{
					this.meta.name = this.Identifier;
				}
			}
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				string url = GenFilePaths.SafeURIForUnityWWWFromPath(this.PreviewImagePath);
				WWW wWW = new WWW(url);
				int num = 0;
				while (!wWW.isDone)
				{
					num++;
				}
				if (wWW.error == null)
				{
					this.previewImage = wWW.texture;
				}
			});
			string publishedFileIdPath = this.PublishedFileIdPath;
			if (File.Exists(this.PublishedFileIdPath))
			{
				string s = File.ReadAllText(publishedFileIdPath);
				this.publishedFileIdInt = new PublishedFileId_t(ulong.Parse(s));
			}
			if (SteamManager.Initialized)
			{
				this.SendSteamDetailsQuery();
			}
		}
		private void SendSteamDetailsQuery()
		{
			CallResult<SteamUGCRequestUGCDetailsResult_t> callResult = CallResult<SteamUGCRequestUGCDetailsResult_t>.Create(new CallResult<SteamUGCRequestUGCDetailsResult_t>.APIDispatchDelegate(this.OnDetailsQueryReturned));
			SteamAPICall_t hAPICall = SteamUGC.RequestUGCDetails(this.PublishedFileId, 999999u);
			callResult.Set(hAPICall, null);
		}
		private void OnDetailsQueryReturned(SteamUGCRequestUGCDetailsResult_t result, bool IOFailure)
		{
			this.steamAuthorCached = (CSteamID)result.m_details.m_ulSteamIDOwner;
		}
		internal void DeleteContent()
		{
			this.directoryInt.Delete(true);
			InstalledModLister.RebuildModList();
		}
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"[",
				this.Identifier,
				"|",
				this.Name,
				"]"
			});
		}
	}
}
