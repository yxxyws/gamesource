using RimWorld;
using System;
namespace Verse
{
	public class UIRoot_Entry : UIRoot
	{
		private bool ShouldShowMainMenuGUI
		{
			get
			{
				WindowStack windowStack = Find.WindowStack;
				for (int i = 0; i < windowStack.Count; i++)
				{
					if (windowStack[i].layer == WindowLayer.Dialog && !(windowStack[i] is EditWindow_Log))
					{
						return false;
					}
				}
				return true;
			}
		}
		public override void Init()
		{
			base.Init();
			UIMenuBackgroundManager.background = new UI_BackgroundMain();
			MainMenuDrawer.Init();
			QuickStarter.CheckQuickStart();
		}
		public override void UIRootOnGUI()
		{
			base.UIRootOnGUI();
			UIMenuBackgroundManager.background.BackgroundOnGUI();
			if (this.ShouldShowMainMenuGUI)
			{
				MainMenuDrawer.MainMenuOnGUI();
			}
			ActiveTutorNoteManager.ActiveLessonManagerOnGUI();
			this.windows.WindowStackOnGUI();
		}
		public override void UIRootUpdate()
		{
			base.UIRootUpdate();
			ConceptDecider.ConceptDeciderUpdate();
		}
	}
}
