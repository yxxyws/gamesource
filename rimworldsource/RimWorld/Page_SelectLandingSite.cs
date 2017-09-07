using RimWorld.Planet;
using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Page_SelectLandingSite : Window
	{
		private const float TopAreaHeight = 50f;
		private WorldInterface worldInterface = new WorldInterface();
		private static readonly Vector2 WinSize = new Vector2(1020f, 764f);
		public override Vector2 InitialWindowSize
		{
			get
			{
				return Page_SelectLandingSite.WinSize;
			}
		}
		public Page_SelectLandingSite()
		{
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			MapInitData.ChooseDecentLandingSite();
			ConceptDecider.TeachOpportunity(ConceptDefOf.WorldCameraMovement, OpportunityType.Important);
		}
		public override void PostOpen()
		{
			base.PostOpen();
			ScribeHeaderUtility.CreateDialogsForVersionMismatchWarnings();
		}
		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(0f, 0f, 999f, 300f), "SelectLandingSite".Translate());
			Text.Font = GameFont.Small;
			Rect mainRect = new Rect(0f, 50f, inRect.width, inRect.height - 38f - 50f - 17f);
			this.worldInterface.Draw(mainRect, true);
			DialogUtility.DoNextBackButtons(inRect, "SelectSite".Translate(), new Action(this.TryContinue), delegate
			{
				Find.WindowStack.Add(new Page_SelectWorld());
				this.Close(true);
			});
			if (DialogUtility.DoMiddleButton(inRect, "Advanced".Translate()))
			{
				Find.WindowStack.Add(new Dialog_AdvancedGameConfig(this.worldInterface.selectedCoords));
			}
		}
		private void TryContinue()
		{
			if (!this.worldInterface.selectedCoords.IsValid)
			{
				Messages.Message("MustSelectLandingSite".Translate(), MessageSound.RejectInput);
				return;
			}
			WorldSquare worldSquare = Find.World.grid.Get(this.worldInterface.selectedCoords);
			if (!worldSquare.biome.canBuildBase)
			{
				Messages.Message("CannotLandBiome".Translate(new object[]
				{
					worldSquare.biome.LabelCap
				}), MessageSound.RejectInput);
				return;
			}
			if (!worldSquare.biome.implemented)
			{
				Messages.Message("BiomeNotImplemented".Translate() + ": " + worldSquare.biome.LabelCap, MessageSound.RejectInput);
				return;
			}
			Faction faction = Find.World.factionManager.FactionInSquare(this.worldInterface.selectedCoords);
			if (faction != null)
			{
				Messages.Message("BaseAlreadyThere".Translate(new object[]
				{
					faction.name
				}), MessageSound.RejectInput);
				return;
			}
			MapInitData.landingCoords = this.worldInterface.selectedCoords;
			Find.WindowStack.Add(new Page_CharMaker());
			this.Close(true);
		}
	}
}
