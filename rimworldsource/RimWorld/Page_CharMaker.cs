using System;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Page_CharMaker : Window
	{
		private const float TopAreaHeight = 80f;
		private Pawn curPawn;
		private static readonly Vector2 WinSize = new Vector2(1020f, 764f);
		public override Vector2 InitialWindowSize
		{
			get
			{
				return Page_CharMaker.WinSize;
			}
		}
		public Page_CharMaker()
		{
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			MapInitData.GenerateDefaultColonistsWithFaction();
			this.curPawn = MapInitData.colonists[0];
		}
		private void RandomizeCurChar()
		{
			do
			{
				this.curPawn = MapInitData.RegenerateStartingColonist(this.curPawn);
			}
			while (!MapInitData.AnyoneCanDoRequiredWorks());
		}
		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(0f, 0f, 300f, 300f), "CreateCharacters".Translate());
			Text.Font = GameFont.Small;
			Rect rect = new Rect(0f, 80f, inRect.width, inRect.height - 38f - 80f - 17f);
			Widgets.DrawMenuSection(rect, true);
			TabDrawer.DrawTabs(rect, 
				from c in MapInitData.colonists
				select new TabRecord(c.LabelCap, delegate
				{
					this.SelectPawn(c);
				}, c == this.curPawn));
			Rect rect2 = rect.ContractedBy(17f);
			Rect rect3 = rect2;
			rect3.width = rect2.width / 2f;
			CharacterCardUtility.DrawCharacterCard(rect3, this.curPawn, new Action(this.RandomizeCurChar));
			Rect rect4 = new Rect(rect3.xMax + 20f, rect2.y + 100f, rect2.width / 2f - 20f, 200f);
			Text.Font = GameFont.Medium;
			Widgets.Label(rect4, "Health".Translate());
			Text.Font = GameFont.Small;
			rect4.yMin += 35f;
			HealthCardUtility.DrawHediffListing(rect4, this.curPawn, true);
			Rect rect5 = new Rect(rect4.x, rect4.yMax, rect4.width, 200f);
			Text.Font = GameFont.Medium;
			Widgets.Label(rect5, "Relations".Translate());
			Text.Font = GameFont.Small;
			rect5.yMin += 35f;
			SocialCardUtility.DrawRelationsAndOpinions(rect5, this.curPawn);
			DialogUtility.DoNextBackButtons(inRect, "Start".Translate(), new Action(this.TryStartGame), delegate
			{
				Find.WindowStack.Add(new Page_SelectLandingSite());
				this.Close(true);
			});
		}
		private void TryStartGame()
		{
			AcceptanceReport acceptanceReport = this.CanStart();
			if (!acceptanceReport.Accepted)
			{
				Messages.Message(acceptanceReport.Reason, MessageSound.RejectInput);
				return;
			}
			Action preLoadLevelAction = delegate
			{
				MapInitData.SetColonyFactionIntoWorld();
				MapInitData.startedFromEntry = true;
			};
			LongEventHandler.QueueLongEvent(preLoadLevelAction, "Gameplay", "GeneratingMap", true, null);
			this.Close(true);
		}
		private AcceptanceReport CanStart()
		{
			foreach (Pawn current in MapInitData.colonists)
			{
				if (!current.Name.IsValid)
				{
					return new AcceptanceReport("EveryoneNeedsValidName".Translate());
				}
			}
			return AcceptanceReport.WasAccepted;
		}
		private void SelectPawn(Pawn c)
		{
			if (c != this.curPawn)
			{
				this.curPawn = c;
			}
		}
	}
}
