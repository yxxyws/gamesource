using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Verse
{
	public class DebugWindowsOpener
	{
		public const KeyCode DebugActionsMenuKey = KeyCode.Slash;
		public const KeyCode DebugLogMenuKey = KeyCode.Insert;
		public const KeyCode EditInspectorKey = KeyCode.Quote;
		public const KeyCode EditInspectSettingsKey = KeyCode.RightBracket;
		public void DevToolStarterOnGUI()
		{
			if (!Prefs.DevMode)
			{
				return;
			}
			Vector2 vector = new Vector2((float)Screen.width * 0.6666667f, 3f);
			int num = 6;
			if (Game.Mode == GameMode.MapPlaying)
			{
				num += 2;
			}
			float num2 = 25f;
			if (Game.Mode == GameMode.MapPlaying && Game.GodMode)
			{
				num2 += 15f;
			}
			Find.WindowStack.ImmediateWindow(1593759361, new Rect(vector.x, vector.y, (float)num * 28f - 4f + 1f, num2), WindowLayer.GameUI, delegate
			{
				this.DrawButtons();
			}, false, false, 0f);
			if (KeyBindingDefOf.ToggleLog.KeyDownEvent)
			{
				this.ToggleLogWindow();
				Event.current.Use();
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Backslash)
			{
				this.OpenPackageEditor();
				Event.current.Use();
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.RightBracket)
			{
				this.ToggleDebugSettingsMenu();
				Event.current.Use();
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Slash)
			{
				this.ToggleDebugActionsMenu();
				Event.current.Use();
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Insert)
			{
				this.ToggleDebugLogMenu();
				Event.current.Use();
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Quote)
			{
				this.ToggleDebugInspector();
				Event.current.Use();
			}
			if (Game.Mode == GameMode.MapPlaying && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Semicolon)
			{
				this.ToggleGodMode();
				Event.current.Use();
			}
		}
		private void DrawButtons()
		{
			WidgetRow widgetRow = new WidgetRow();
			if (widgetRow.DoIconButton(TexButton.ToggleLog, "Open the debug log."))
			{
				this.ToggleLogWindow();
			}
			if (widgetRow.DoIconButton(TexButton.OpenPackageEditor, "Open the package editor.\n\nThis lets you edit game data while the game is running."))
			{
				this.OpenPackageEditor();
			}
			if (widgetRow.DoIconButton(TexButton.OpenInspectSettings, "Open the view settings.\n\nThis lets you see special debug visuals."))
			{
				this.ToggleDebugSettingsMenu();
			}
			if (widgetRow.DoIconButton(TexButton.OpenDebugActionsMenu, "Open debug actions menu.\n\nThis lets you spawn items and force various events."))
			{
				this.ToggleDebugActionsMenu();
			}
			if (widgetRow.DoIconButton(TexButton.OpenDebugActionsMenu, "Open debug logging menu."))
			{
				this.ToggleDebugLogMenu();
			}
			if (widgetRow.DoIconButton(TexButton.OpenInspector, "Open the inspector.\n\nThis lets you inspect what's happening in the game, down to individual variables."))
			{
				this.ToggleDebugInspector();
			}
			if (Game.Mode == GameMode.MapPlaying)
			{
				if (widgetRow.DoIconButton(TexButton.ToggleGodMode, "Toggle god mode.\n\nWhen god mode is on, you can build stuff instantly, for free, and sell things that aren't yours."))
				{
					this.ToggleGodMode();
				}
				if (Game.GodMode)
				{
					Text.Font = GameFont.Tiny;
					Widgets.Label(new Rect(0f, 25f, 200f, 100f), "God mode");
				}
				bool pauseOnError = Prefs.PauseOnError;
				widgetRow.DoIconToggle(ref pauseOnError, TexButton.TogglePauseOnError, "Pause the game when an error is logged.", null, null);
				Prefs.PauseOnError = pauseOnError;
			}
		}
		private void ToggleLogWindow()
		{
			if (!Find.WindowStack.TryRemove(typeof(EditWindow_Log), true))
			{
				Find.WindowStack.Add(new EditWindow_Log());
			}
		}
		private void OpenPackageEditor()
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			FloatMenuOption item = new FloatMenuOption("SoundDefs", delegate
			{
				Find.WindowStack.Add(new EditWindow_PackageEditor<SoundDef>("SoundDefs"));
			}, MenuOptionPriority.Medium, null, null);
			list.Add(item);
			item = new FloatMenuOption("HairDefs", delegate
			{
				Find.WindowStack.Add(new EditWindow_PackageEditor<HairDef>("HairDefs"));
			}, MenuOptionPriority.Medium, null, null);
			list.Add(item);
			Find.WindowStack.Add(new FloatMenu(list, false));
		}
		private void ToggleDebugSettingsMenu()
		{
			if (!Find.WindowStack.TryRemove(typeof(Dialog_DebugSettingsMenu), true))
			{
				Find.WindowStack.Add(new Dialog_DebugSettingsMenu());
			}
		}
		private void ToggleDebugActionsMenu()
		{
			if (!Find.WindowStack.TryRemove(typeof(Dialog_DebugActionsMenu), true))
			{
				Find.WindowStack.Add(new Dialog_DebugActionsMenu());
			}
		}
		private void ToggleDebugLogMenu()
		{
			if (!Find.WindowStack.TryRemove(typeof(Dialog_DebugLogMenu), true))
			{
				Find.WindowStack.Add(new Dialog_DebugLogMenu());
			}
		}
		private void ToggleDebugInspector()
		{
			if (!Find.WindowStack.TryRemove(typeof(EditWindow_DebugInspector), true))
			{
				Find.WindowStack.Add(new EditWindow_DebugInspector());
			}
		}
		private void ToggleGodMode()
		{
			Game.GodMode = !Game.GodMode;
		}
	}
}
