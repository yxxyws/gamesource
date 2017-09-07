using Steamworks;
using System;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Steam;
namespace RimWorld
{
	public class Page_ModsConfig : Window
	{
		private const float ModListAreaWidth = 250f;
		private const float HeaderButtonHeight = 30f;
		private const float ModsFolderButHeight = 30f;
		private const float ButtonsGap = 4f;
		private const float UploadRowHeight = 40f;
		private const float PreviewMaxHeight = 300f;
		public InstalledMod selectedMod;
		private Vector2 modListScrollPosition = Vector2.zero;
		private static readonly Vector2 WinSize = new Vector2(1000f, 760f);
		public override Vector2 InitialWindowSize
		{
			get
			{
				return Page_ModsConfig.WinSize;
			}
		}
		public Page_ModsConfig()
		{
			InstalledModLister.RebuildModList();
			this.selectedMod = InstalledModLister.AllInstalledMods.FirstOrDefault<InstalledMod>();
			this.absorbInputAroundWindow = true;
			this.doCloseButton = true;
		}
		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Small;
			float num = 0f;
			if (SteamManager.Initialized)
			{
				Rect rect = new Rect(17f, num, 216f, 30f);
				if (Widgets.TextButton(rect, "OpenSteamWorkshop".Translate(), true, false))
				{
					SteamUtility.OpenUrlSteamIfPossible("http://steamcommunity.com/workshop/browse/?appid=" + SteamUtils.GetAppID());
				}
				num += 30f;
			}
			Rect rect2 = new Rect(17f, num, 216f, 30f);
			if (Widgets.TextButton(rect2, "GetModsFromForum".Translate(), true, false))
			{
				Application.OpenURL("http://rimworldgame.com/getmods");
			}
			num += 30f;
			if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
			{
				Rect rect3 = new Rect(17f, num, 216f, 30f);
				if (Widgets.TextButton(rect3, "OpenModsDataFolder".Translate(), true, false))
				{
					Application.OpenURL(GenFilePaths.CoreModsFolderPath);
				}
				num += 30f;
			}
			float num2 = num + 17f;
			Rect rect4 = new Rect(0f, num2, 250f, inRect.height - 38f - num2);
			Widgets.DrawMenuSection(rect4, true);
			float height = (float)(InstalledModLister.AllInstalledMods.Count<InstalledMod>() * 34 + 300);
			Rect rect5 = new Rect(0f, 0f, rect4.width - 16f, height);
			Widgets.BeginScrollView(rect4, ref this.modListScrollPosition, rect5);
			Rect rect6 = rect5.ContractedBy(4f);
			Listing_Standard listing_Standard = new Listing_Standard(rect6);
			listing_Standard.OverrideColumnWidth = rect6.width;
			foreach (InstalledMod current in InstalledModLister.AllInstalledMods)
			{
				bool flag = current == this.selectedMod;
				bool active = current.Active;
				string text = string.Empty;
				if (current.Active)
				{
					string text2 = text;
					text = string.Concat(new object[]
					{
						text2,
						"[",
						ModsConfig.ActiveLoadOrder(current.Identifier) + 1,
						"] "
					});
				}
				text += current.Name;
				if (listing_Standard.DoLabelCheckboxSelectable(text, ref flag, ref active))
				{
					this.selectedMod = current;
				}
				if (current.Active && !active && current.Name == LoadedMod.CoreModFolderName)
				{
					InstalledMod coreMod = current;
					Find.WindowStack.Add(new Dialog_Confirm("ConfirmDisableCoreMod".Translate(), delegate
					{
						coreMod.Active = false;
					}, false));
				}
				else
				{
					current.Active = active;
				}
			}
			foreach (PublishedFileId_t current2 in SteamWorkshop.DownloadingMods)
			{
				listing_Standard.DoLabel("DownloadingMod".Translate(new object[]
				{
					current2.m_PublishedFileId
				}));
			}
			listing_Standard.End();
			Widgets.EndScrollView();
			Rect position = new Rect(rect4.xMax + 17f, inRect.y, inRect.width - rect4.width - 17f, inRect.height - 38f);
			GUI.BeginGroup(position);
			if (this.selectedMod != null)
			{
				Text.Font = GameFont.Medium;
				Rect rect7 = new Rect(0f, 0f, position.width, 40f);
				Text.Anchor = TextAnchor.UpperCenter;
				Widgets.Label(rect7, this.selectedMod.Name);
				Text.Anchor = TextAnchor.UpperLeft;
				Rect position2 = new Rect(0f, rect7.yMax, 0f, 20f);
				if (this.selectedMod.previewImage != null)
				{
					position2.width = Mathf.Min((float)this.selectedMod.previewImage.width, position.width);
					position2.height = (float)this.selectedMod.previewImage.height * (position2.width / (float)this.selectedMod.previewImage.width);
					if (position2.height > 300f)
					{
						position2.width *= 300f / position2.height;
						position2.height = 300f;
					}
					position2.x = position.width / 2f - position2.width / 2f;
					GUI.DrawTexture(position2, this.selectedMod.previewImage, ScaleMode.ScaleToFit);
				}
				Text.Font = GameFont.Small;
				float num3 = position2.yMax + 10f;
				if (!this.selectedMod.Author.NullOrEmpty())
				{
					Rect rect8 = new Rect(0f, num3, position.width / 2f, 25f);
					Widgets.Label(rect8, "Author".Translate() + ": " + this.selectedMod.Author);
				}
				if (!this.selectedMod.Url.NullOrEmpty())
				{
					float num4 = Mathf.Min(position.width / 2f, Text.CalcSize(this.selectedMod.Url).x);
					Rect rect9 = new Rect(position.width - num4, num3, num4, 25f);
					if (Widgets.TextButton(rect9, this.selectedMod.Url, false, false))
					{
						Application.OpenURL(this.selectedMod.Url);
					}
				}
				WidgetRow widgetRow = new WidgetRow(position.width, num3 + 25f, UIDirection.LeftThenUp, 2000f, 29f);
				if (SteamManager.Initialized && this.selectedMod.OnSteamWorkshop && widgetRow.DoTextButton("WorkshopPage".Translate(), null, true, false))
				{
					SteamUtility.OpenUrlSteamIfPossible(this.selectedMod.SteamWorkshopPageUrl);
				}
				float num5 = num3 + 25f + 24f;
				Rect rect10 = new Rect(0f, num5, position.width, position.height - num5 - 40f);
				Widgets.Label(rect10, this.selectedMod.Description);
				if (SteamManager.Initialized && this.selectedMod.CanUploadToWorkshop)
				{
					Rect rect11 = new Rect(0f, position.yMax - 40f, 200f, 40f);
					string label = (!this.selectedMod.OnSteamWorkshop) ? "UploadToSteamWorkshop".Translate() : "UpdateOnSteamWorkshop".Translate();
					if (Widgets.TextButton(rect11, label, true, false))
					{
						SteamWorkshop.Upload(this.selectedMod);
					}
					if (!this.selectedMod.OnSteamWorkshop)
					{
						Rect rect12 = new Rect(210f, rect11.y, position.width - 210f, 40f);
						Text.Font = GameFont.Tiny;
						if (Widgets.TextButton(rect12, "YouAgreeToSteamWorkshopLegal".Translate(), false, false))
						{
							SteamUtility.OpenUrlSteamIfPossible("http://steamcommunity.com/sharedfiles/workshoplegalagreement");
						}
						Text.Font = GameFont.Small;
					}
				}
			}
			GUI.EndGroup();
		}
		public override void PostClose()
		{
			ModsConfig.Save();
			LongEventHandler.QueueLongEvent(delegate
			{
				PlayDataLoader.ClearAllPlayData();
				PlayDataLoader.LoadAllPlayData(false);
			}, "LoadingLongEvent", true, null);
		}
	}
}
