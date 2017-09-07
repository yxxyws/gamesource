using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Page_CreateWorldReview : Window
	{
		private const float TopAreaHeight = 50f;
		private WorldInterface worldInterface = new WorldInterface();
		private static readonly Vector2 WinSize = new Vector2(1020f, 764f);
		public override Vector2 InitialWindowSize
		{
			get
			{
				return Page_CreateWorldReview.WinSize;
			}
		}
		public Page_CreateWorldReview()
		{
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			ConceptDecider.TeachOpportunity(ConceptDefOf.WorldCameraMovement, OpportunityType.Important);
		}
		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(0f, 0f, 999f, 300f), "ReviewWorld".Translate());
			Text.Font = GameFont.Small;
			Rect mainRect = new Rect(0f, 50f, inRect.width, inRect.height - 38f - 50f - 2f);
			this.worldInterface.Draw(mainRect, false);
			DialogUtility.DoNextBackButtons(inRect, "WorldSaveAndFinish".Translate(), new Action(this.SaveAndExit), new Action(this.GoBackToParams));
		}
		private void GoBackToParams()
		{
			Current.World = null;
			Find.WindowStack.Add(new Page_CreateWorldParams());
			this.Close(true);
		}
		private void SaveAndExit()
		{
			GameDataSaver.SaveWorld(Current.World);
			Current.World = null;
			MainMenuDrawer.Notify_WorldFilesChanged();
			this.Close(true);
		}
	}
}
