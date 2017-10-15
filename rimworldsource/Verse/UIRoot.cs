using RimWorld;
using System;
using UnityEngine;
using Verse.Noise;
using Verse.Sound;
namespace Verse
{
	public abstract class UIRoot
	{
		public WindowStack windows = new WindowStack();
		protected DebugWindowsOpener debugWindowOpener = new DebugWindowsOpener();
		protected ScreenshotModeHandler screenshotMode = new ScreenshotModeHandler();
		private ShortcutKeys shortcutKeys = new ShortcutKeys();
		public FeedbackFloaters feedbackFloaters = new FeedbackFloaters();
		public virtual void Init()
		{
		}
		public virtual void UIRootOnGUI()
		{
			Text.StartOfOnGUI();
			this.CheckOpenLogWindow();
			DelayedErrorWindowRequest.DelayedErrorWindowRequestOnGUI();
			DebugInputLogger.InputLogOnGUI();
			this.windows.HandleEventsHighPriority();
			this.screenshotMode.ScreenshotModesOnGUI();
			if (!this.screenshotMode.FiltersCurrentEvent)
			{
				TooltipHandler.DoTooltipGUI();
				this.feedbackFloaters.FeedbackOnGUI();
				DragSliderManager.DragSlidersOnGUI();
				Messages.MessagesDoGUI();
				this.debugWindowOpener.DevToolStarterOnGUI();
			}
			this.shortcutKeys.ShortcutKeysOnGUI();
			NoiseDebugUI.NoiseDebugOnGUI();
			Debug.developerConsoleVisible = false;
		}
		public virtual void UIRootUpdate()
		{
			this.windows.WindowsUpdate();
			MouseoverSounds.ResolveFrame();
		}
		private void CheckOpenLogWindow()
		{
			if (EditWindow_Log.wantsToOpen && !Find.WindowStack.IsOpen(typeof(EditWindow_Log)))
			{
				Find.WindowStack.Add(new EditWindow_Log());
				EditWindow_Log.wantsToOpen = false;
			}
		}
	}
}
