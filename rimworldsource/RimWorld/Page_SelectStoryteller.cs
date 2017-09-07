using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Page_SelectStoryteller : Window
	{
		public override Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2(1020f, 764f);
			}
		}
		public Page_SelectStoryteller()
		{
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			MapInitData.ChooseDefaultStoryteller();
		}
		public override void DoWindowContents(Rect inRect)
		{
			StorytellerUI.DrawStorytellerSelectionInterface(inRect, ref MapInitData.chosenStoryteller, ref MapInitData.difficulty);
			DialogUtility.DoNextBackButtons(inRect, "Next".Translate(), new Action(this.TryGoNext), new Action(this.GoBack));
		}
		private void TryGoNext()
		{
			if (MapInitData.difficulty == null)
			{
				if (!Prefs.DevMode)
				{
					Messages.Message("MustChooseDifficulty".Translate(), MessageSound.RejectInput);
					return;
				}
				Messages.Message("Difficulty has been automatically selected (debug mode only)", MessageSound.Silent);
				MapInitData.ChooseDefaultDifficulty();
			}
			Find.WindowStack.Add(new Page_SelectWorld());
			this.Close(true);
		}
		private void GoBack()
		{
			this.Close(true);
		}
	}
}
