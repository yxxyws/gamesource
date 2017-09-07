using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	[StaticConstructorOnStartup]
	public static class MainMenuDrawer
	{
		private const float GameRectWidth = 200f;
		private const float NewsRectWidth = 350f;
		private const int ButCount = 3;
		private const float TitleShift = 50f;
		private static bool anyWorldFiles;
		private static bool anyMapFiles;
		private static readonly Vector2 PaneSize = new Vector2(450f, 450f);
		private static readonly Vector2 TitleSize = new Vector2(1034f, 158f);
		private static readonly Texture2D TexTitle = ContentFinder<Texture2D>.Get("UI/HeroArt/GameTitle", true);
		private static readonly Vector2 LudeonLogoSize = new Vector2(200f, 58f);
		private static readonly Texture2D TexLudeonLogo = ContentFinder<Texture2D>.Get("UI/HeroArt/LudeonLogoSmall", true);
		public static void Init()
		{
			ConceptDatabase.Save();
			ShipCountdown.CancelCountdown();
			MainMenuDrawer.anyWorldFiles = SavedWorldsDatabase.AllWorldFiles.Any<FileInfo>();
			MainMenuDrawer.anyMapFiles = MapFilesUtility.AllMapFiles.Any<FileInfo>();
		}
		public static void Notify_WorldFilesChanged()
		{
			MainMenuDrawer.anyWorldFiles = SavedWorldsDatabase.AllWorldFiles.Any<FileInfo>();
		}
		public static void MainMenuOnGUI()
		{
			VersionControl.DrawInfoInCorner();
			Rect rect = new Rect((float)(Screen.width / 2) - MainMenuDrawer.PaneSize.x / 2f, (float)(Screen.height / 2) - MainMenuDrawer.PaneSize.y / 2f, MainMenuDrawer.PaneSize.x, MainMenuDrawer.PaneSize.y);
			rect.y += 50f;
			rect.x = (float)Screen.width - rect.width - 30f;
			Vector2 a = MainMenuDrawer.TitleSize;
			if (a.x > (float)Screen.width)
			{
				a *= (float)Screen.width / a.x;
			}
			a *= 0.7f;
			Rect rect2 = new Rect((float)(Screen.width / 2) - a.x / 2f, rect.y - a.y - 10f, a.x, a.y);
			rect2.x = (float)Screen.width - a.x - 50f;
			GUI.DrawTexture(rect2, MainMenuDrawer.TexTitle, ScaleMode.StretchToFill, true);
			Rect rect3 = rect2;
			rect3.y += rect2.height;
			rect3.xMax -= 55f;
			rect3.height = 30f;
			rect3.y += 3f;
			string text = "MainPageCredit".Translate();
			Text.Font = GameFont.Medium;
			Text.Anchor = TextAnchor.UpperRight;
			if (Screen.width < 990)
			{
				Rect position = rect3;
				position.xMin = position.xMax - Text.CalcSize(text).x;
				position.xMin -= 4f;
				position.xMax += 4f;
				GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
				GUI.DrawTexture(position, BaseContent.WhiteTex);
				GUI.color = Color.white;
			}
			Widgets.Label(rect3, text);
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
			GUI.color = new Color(1f, 1f, 1f, 0.5f);
			Rect position2 = new Rect((float)(Screen.width - 8) - MainMenuDrawer.LudeonLogoSize.x, 8f, MainMenuDrawer.LudeonLogoSize.x, MainMenuDrawer.LudeonLogoSize.y);
			GUI.DrawTexture(position2, MainMenuDrawer.TexLudeonLogo, ScaleMode.StretchToFill, true);
			GUI.color = Color.white;
			Rect rect4 = rect.ContractedBy(17f);
			GUI.BeginGroup(rect4);
			MainMenuDrawer.DoMainMenuButtons(rect4, MainMenuDrawer.anyWorldFiles, MainMenuDrawer.anyMapFiles, null);
			GUI.EndGroup();
		}
		public static void DoMainMenuButtons(Rect rect, bool anyWorldFiles, bool anyMapFiles, Action backToGameButtonAction = null)
		{
			Rect rect2 = new Rect(0f, 0f, 200f, rect.height);
			Rect rect3 = new Rect(rect2.xMax + 17f, 0f, -1f, rect.height);
			rect3.xMax = rect.width;
			Text.Font = GameFont.Small;
			List<ListableOption> list = new List<ListableOption>();
			ListableOption item;
			if (Game.Mode == GameMode.Entry)
			{
				item = new ListableOption("CreateWorld".Translate(), delegate
				{
					MapInitData.Reset();
					Find.WindowStack.Add(new Page_CreateWorldParams());
				});
				list.Add(item);
				if (anyWorldFiles)
				{
					item = new ListableOption("NewColony".Translate(), delegate
					{
						MapInitData.Reset();
						Find.WindowStack.Add(new Page_SelectStoryteller());
					});
					list.Add(item);
				}
			}
			if (Game.Mode == GameMode.MapPlaying)
			{
				if (backToGameButtonAction != null)
				{
					item = new ListableOption("BackToGame".Translate(), backToGameButtonAction);
					list.Add(item);
				}
				if (!Find.Map.info.permadeathMode)
				{
					item = new ListableOption("Save".Translate(), delegate
					{
						MainMenuDrawer.CloseMainTab();
						Find.WindowStack.Add(new Dialog_MapList_Save());
					});
					list.Add(item);
				}
			}
			if (anyMapFiles)
			{
				item = new ListableOption("Load".Translate(), delegate
				{
					MainMenuDrawer.CloseMainTab();
					Find.WindowStack.Add(new Dialog_MapList_Load());
				});
				list.Add(item);
			}
			item = new ListableOption("Options".Translate(), delegate
			{
				MainMenuDrawer.CloseMainTab();
				Find.WindowStack.Add(new Dialog_Options());
			});
			list.Add(item);
			if (Game.Mode == GameMode.Entry)
			{
				item = new ListableOption("Mods".Translate(), delegate
				{
					Find.WindowStack.Add(new Page_ModsConfig());
				});
				list.Add(item);
				item = new ListableOption("Credits".Translate(), delegate
				{
					Find.WindowStack.Add(new Page_Credits());
				});
				list.Add(item);
			}
			if (Game.Mode == GameMode.MapPlaying)
			{
				if (Find.Map.info.permadeathMode)
				{
					item = new ListableOption("SaveAndQuitToMainMenu".Translate(), delegate
					{
						LongEventHandler.QueueLongEvent(delegate
						{
							GameDataSaver.SaveGame(Find.Map, Find.Map.info.permadeathModeUniqueName);
						}, "Entry", "SavingLongEvent", false, null);
					});
					list.Add(item);
					item = new ListableOption("SaveAndQuitToOS".Translate(), delegate
					{
						LongEventHandler.QueueLongEvent(delegate
						{
							GameDataSaver.SaveGame(Find.Map, Find.Map.info.permadeathModeUniqueName);
							LongEventHandler.ExecuteWhenFinished(delegate
							{
								Root.Shutdown();
							});
						}, "SavingLongEvent", false, null);
					});
					list.Add(item);
				}
				else
				{
					Action action = delegate
					{
						Find.WindowStack.Add(new Dialog_Confirm("ConfirmQuit".Translate(), delegate
						{
							Application.LoadLevel("Entry");
						}, true));
					};
					item = new ListableOption("QuitToMainMenu".Translate(), action);
					list.Add(item);
					Action action2 = delegate
					{
						Find.WindowStack.Add(new Dialog_Confirm("ConfirmQuit".Translate(), delegate
						{
							Root.Shutdown();
						}, true));
					};
					item = new ListableOption("QuitToOS".Translate(), action2);
					list.Add(item);
				}
			}
			else
			{
				item = new ListableOption("QuitToOS".Translate(), delegate
				{
					Root.Shutdown();
				});
				list.Add(item);
			}
			Rect rect4 = rect2.ContractedBy(17f);
			OptionListingUtility.DrawOptionListing(rect4, list);
			Text.Font = GameFont.Small;
			List<ListableOption> list2 = new List<ListableOption>();
			ListableOption item2 = new ListableOption_WebLink("FictionPrimer".Translate(), "https://docs.google.com/document/d/1pIZyKif0bFbBWten4drrm7kfSSfvBoJPgG9-ywfN8j8/pub", TexButton.IconBlog);
			list2.Add(item2);
			item2 = new ListableOption_WebLink("LudeonBlog".Translate(), "http://ludeon.com/blog", TexButton.IconBlog);
			list2.Add(item2);
			item2 = new ListableOption_WebLink("Forums".Translate(), "http://ludeon.com/forums", TexButton.IconForums);
			list2.Add(item2);
			item2 = new ListableOption_WebLink("OfficialWiki".Translate(), "http://rimworldwiki.com", TexButton.IconBlog);
			list2.Add(item2);
			item2 = new ListableOption_WebLink("TynansTwitter".Translate(), "https://twitter.com/TynanSylvester", TexButton.IconTwitter);
			list2.Add(item2);
			item2 = new ListableOption_WebLink("TynansDesignBook".Translate(), "http://tynansylvester.com/book", TexButton.IconBook);
			list2.Add(item2);
			item2 = new ListableOption_WebLink("HelpTranslate".Translate(), "http://ludeon.com/forums/index.php?topic=2933.0", TexButton.IconForums);
			list2.Add(item2);
			item2 = new ListableOption_WebLink("BuySoundtrack".Translate(), "http://www.lasgameaudio.co.uk/#!store/t04fw", TexButton.IconSoundtrack);
			list2.Add(item2);
			Rect rect5 = rect3.ContractedBy(17f);
			float num = OptionListingUtility.DrawOptionListing(rect5, list2);
			GUI.BeginGroup(rect5);
			if (Game.Mode == GameMode.Entry && Widgets.ImageButton(new Rect(0f, num + 10f, 64f, 32f), LanguageDatabase.activeLanguage.icon))
			{
				List<FloatMenuOption> list3 = new List<FloatMenuOption>();
				foreach (LoadedLanguage current in LanguageDatabase.AllLoadedLanguages)
				{
					LoadedLanguage localLang = current;
					list3.Add(new FloatMenuOption(localLang.FriendlyNameNative, delegate
					{
						LanguageDatabase.SelectLanguage(localLang);
						Prefs.Save();
					}, MenuOptionPriority.Medium, null, null));
				}
				Find.WindowStack.Add(new FloatMenu(list3, false));
			}
			GUI.EndGroup();
		}
		private static void CloseMainTab()
		{
			if (Game.Mode == GameMode.MapPlaying)
			{
				Find.MainTabsRoot.EscapeCurrentTab(false);
			}
		}
	}
}
