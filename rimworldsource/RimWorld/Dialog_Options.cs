using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Dialog_Options : Window
	{
		private const float SubOptionTabWidth = 40f;
		public override Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2(900f, 700f);
			}
		}
		public Dialog_Options()
		{
			this.closeOnEscapeKey = true;
			this.doCloseButton = true;
			this.doCloseX = true;
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
		}
		public override void DoWindowContents(Rect inRect)
		{
			Rect rect = inRect.AtZero();
			rect.yMax -= 45f;
			Listing_Standard listing_Standard = new Listing_Standard(rect);
			listing_Standard.OverrideColumnWidth = (rect.width - 34f) / 3f;
			Text.Font = GameFont.Medium;
			listing_Standard.DoLabel("Audiovisuals".Translate());
			Text.Font = GameFont.Small;
			listing_Standard.DoGap(12f);
			listing_Standard.DoGap(12f);
			listing_Standard.DoLabel("GameVolume".Translate());
			Prefs.VolumeGame = listing_Standard.DoSlider(Prefs.VolumeGame, 0f, 1f);
			listing_Standard.DoLabel("MusicVolume".Translate());
			Prefs.VolumeMusic = listing_Standard.DoSlider(Prefs.VolumeMusic, 0f, 1f);
			listing_Standard.DoGap(12f);
			if (listing_Standard.DoTextButtonLabeled("Resolution".Translate(), Dialog_Options.ResToString(Screen.width, Screen.height)))
			{
				Find.WindowStack.Add(new Dialog_ResolutionPicker());
			}
			listing_Standard.DoLabel("Antialiasing".Translate());
			if (listing_Standard.DoRadioButton("None".Translate(), Prefs.GraphicsQualityIndex == 0, 40f))
			{
				Prefs.GraphicsQualityIndex = 0;
			}
			if (listing_Standard.DoRadioButton("2x", Prefs.GraphicsQualityIndex == 1, 40f))
			{
				Prefs.GraphicsQualityIndex = 1;
			}
			if (listing_Standard.DoRadioButton("4x", Prefs.GraphicsQualityIndex == 2, 40f))
			{
				Prefs.GraphicsQualityIndex = 2;
			}
			if (listing_Standard.DoRadioButton("8x", Prefs.GraphicsQualityIndex == 3, 40f))
			{
				Prefs.GraphicsQualityIndex = 3;
			}
			listing_Standard.DoGap(12f);
			bool customCursorEnabled = Prefs.CustomCursorEnabled;
			listing_Standard.DoLabelCheckbox("CustomCursor".Translate(), ref customCursorEnabled, null);
			Prefs.CustomCursorEnabled = customCursorEnabled;
			bool fullScreen = Screen.fullScreen;
			bool flag = fullScreen;
			listing_Standard.DoLabelCheckbox("Fullscreen".Translate(), ref fullScreen, null);
			if (fullScreen != flag)
			{
				ResolutionUtility.SafeSetFullscreen(fullScreen);
			}
			listing_Standard.NewColumn();
			Text.Font = GameFont.Medium;
			listing_Standard.DoLabel("Gameplay".Translate());
			Text.Font = GameFont.Small;
			listing_Standard.DoGap(12f);
			listing_Standard.DoGap(12f);
			if (listing_Standard.DoTextButton("KeyboardConfig".Translate()))
			{
				Find.WindowStack.Add(new Dialog_KeyBindings());
			}
			if (listing_Standard.DoTextButton("ChooseLanguage".Translate()))
			{
				if (Game.Mode == GameMode.MapPlaying)
				{
					Messages.Message("ChangeLanguageFromMainMenu".Translate(), MessageSound.RejectInput);
				}
				else
				{
					List<FloatMenuOption> list = new List<FloatMenuOption>();
					foreach (LoadedLanguage current in LanguageDatabase.AllLoadedLanguages)
					{
						LoadedLanguage localLang = current;
						list.Add(new FloatMenuOption(localLang.FriendlyNameNative, delegate
						{
							LanguageDatabase.SelectLanguage(localLang);
						}, MenuOptionPriority.Medium, null, null));
					}
					Find.WindowStack.Add(new FloatMenu(list, false));
				}
			}
			if ((Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) && listing_Standard.DoTextButton("OpenSaveGameDataFolder".Translate()))
			{
				Application.OpenURL(GenFilePaths.SaveDataFolderPath);
			}
			bool adaptiveTrainingEnabled = Prefs.AdaptiveTrainingEnabled;
			listing_Standard.DoLabelCheckbox("AdaptiveTutor".Translate(), ref adaptiveTrainingEnabled, null);
			Prefs.AdaptiveTrainingEnabled = adaptiveTrainingEnabled;
			if (listing_Standard.DoTextButton("ResetAdaptiveTutor".Translate()))
			{
				Messages.Message("AdaptiveTutorIsReset".Translate(), MessageSound.Benefit);
				ConceptDatabase.ResetPersistent();
			}
			bool runInBackground = Prefs.RunInBackground;
			listing_Standard.DoLabelCheckbox("RunInBackground".Translate(), ref runInBackground, null);
			Prefs.RunInBackground = runInBackground;
			bool edgeScreenScroll = Prefs.EdgeScreenScroll;
			listing_Standard.DoLabelCheckbox("EdgeScreenScroll".Translate(), ref edgeScreenScroll, null);
			Prefs.EdgeScreenScroll = edgeScreenScroll;
			bool pauseOnLoad = Prefs.PauseOnLoad;
			listing_Standard.DoLabelCheckbox("PauseOnLoad".Translate(), ref pauseOnLoad, null);
			Prefs.PauseOnLoad = pauseOnLoad;
			bool showRealtimeClock = Prefs.ShowRealtimeClock;
			listing_Standard.DoLabelCheckbox("ShowRealtimeClock".Translate(), ref showRealtimeClock, null);
			Prefs.ShowRealtimeClock = showRealtimeClock;
			if (listing_Standard.DoTextButtonLabeled("TemperatureMode".Translate(), Prefs.TemperatureMode.ToString()))
			{
				List<FloatMenuOption> list2 = new List<FloatMenuOption>();
				IEnumerator enumerator2 = Enum.GetValues(typeof(TemperatureDisplayMode)).GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						TemperatureDisplayMode localTmode2 = (TemperatureDisplayMode)((byte)enumerator2.Current);
						TemperatureDisplayMode localTmode = localTmode2;
						FloatMenuOption item = new FloatMenuOption(localTmode.ToString(), delegate
						{
							Prefs.TemperatureMode = localTmode;
						}, MenuOptionPriority.Medium, null, null);
						list2.Add(item);
					}
				}
				finally
				{
					IDisposable disposable = enumerator2 as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				Find.WindowStack.Add(new FloatMenu(list2, false));
			}
			float autosaveIntervalDays = Prefs.AutosaveIntervalDays;
			string text = "Days".Translate();
			string text2 = "Day".Translate().ToLower();
			if (listing_Standard.DoTextButtonLabeled("AutosaveInterval".Translate(), autosaveIntervalDays + " " + ((autosaveIntervalDays != 1f) ? text : text2)))
			{
				List<FloatMenuOption> list3 = new List<FloatMenuOption>();
				if (Prefs.DevMode)
				{
					list3.Add(new FloatMenuOption("0.125 " + text + "(debug)", delegate
					{
						Prefs.AutosaveIntervalDays = 0.125f;
					}, MenuOptionPriority.Medium, null, null));
					list3.Add(new FloatMenuOption("0.25 " + text + "(debug)", delegate
					{
						Prefs.AutosaveIntervalDays = 0.25f;
					}, MenuOptionPriority.Medium, null, null));
					list3.Add(new FloatMenuOption("0.5 " + text + "(debug)", delegate
					{
						Prefs.AutosaveIntervalDays = 0.5f;
					}, MenuOptionPriority.Medium, null, null));
				}
				list3.Add(new FloatMenuOption(1.ToString() + " " + text2, delegate
				{
					Prefs.AutosaveIntervalDays = 1f;
				}, MenuOptionPriority.Medium, null, null));
				list3.Add(new FloatMenuOption(3.ToString() + " " + text, delegate
				{
					Prefs.AutosaveIntervalDays = 3f;
				}, MenuOptionPriority.Medium, null, null));
				list3.Add(new FloatMenuOption(7.ToString() + " " + text, delegate
				{
					Prefs.AutosaveIntervalDays = 7f;
				}, MenuOptionPriority.Medium, null, null));
				list3.Add(new FloatMenuOption(14.ToString() + " " + text, delegate
				{
					Prefs.AutosaveIntervalDays = 14f;
				}, MenuOptionPriority.Medium, null, null));
				Find.WindowStack.Add(new FloatMenu(list3, false));
			}
			if (Game.Mode == GameMode.MapPlaying && Find.Map.info.permadeathMode && Prefs.AutosaveIntervalDays > 1f)
			{
				GUI.color = Color.red;
				listing_Standard.DoLabel("MaxPermadeathAutosaveIntervalInfo".Translate(new object[]
				{
					1f
				}));
				GUI.color = Color.white;
			}
			if (Game.Mode == GameMode.MapPlaying && listing_Standard.DoTextButton("ChangeStoryteller".Translate()))
			{
				Find.WindowStack.Add(new Dialog_SelectStorytellerInGame());
			}
			bool devMode = Prefs.DevMode;
			listing_Standard.DoLabelCheckbox("DevelopmentMode".Translate(), ref devMode, null);
			Prefs.DevMode = devMode;
			if (Prefs.DevMode)
			{
				bool resetModsConfigOnCrash = Prefs.ResetModsConfigOnCrash;
				listing_Standard.DoLabelCheckbox("ResetModsConfigOnCrash".Translate(), ref resetModsConfigOnCrash, null);
				Prefs.ResetModsConfigOnCrash = resetModsConfigOnCrash;
				bool logVerbose = Prefs.LogVerbose;
				listing_Standard.DoLabelCheckbox("LogVerbose".Translate(), ref logVerbose, null);
				Prefs.LogVerbose = logVerbose;
			}
			listing_Standard.NewColumn();
			Text.Font = GameFont.Medium;
			listing_Standard.DoLabel(string.Empty);
			Text.Font = GameFont.Small;
			listing_Standard.DoGap(12f);
			listing_Standard.DoGap(12f);
			listing_Standard.DoLabel("NamesYouWantToSee".Translate());
			while (Prefs.PreferredNames.Count < 4)
			{
				Prefs.PreferredNames.Add(string.Empty);
			}
			for (int i = 0; i < 4; i++)
			{
				string text3 = Prefs.PreferredNames[i];
				text3 = listing_Standard.DoTextField(text3);
				Prefs.PreferredNames[i] = text3;
			}
			listing_Standard.End();
		}
		public override void PreClose()
		{
			base.PreClose();
			Prefs.Save();
		}
		public static string ResToString(int width, int height)
		{
			string text = width + "x" + height;
			if (width == 1280 && height == 720)
			{
				text += " (720p)";
			}
			if (width == 1920 && height == 1080)
			{
				text += " (1080p)";
			}
			return text;
		}
	}
}
