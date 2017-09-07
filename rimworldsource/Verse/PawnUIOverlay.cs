using RimWorld;
using System;
using UnityEngine;
namespace Verse
{
	[StaticConstructorOnStartup]
	public class PawnUIOverlay
	{
		private const float PawnLabelOffsetY = -0.6f;
		private const int PawnStatBarWidth = 32;
		private const float ActivityIconSize = 13f;
		private const float ActivityIconOffsetY = 12f;
		private const float NameUnderlineDist = 11f;
		private const float MinNameWidth = 20f;
		private const float AnimalLabelNudgeUpPixels = 4f;
		private Pawn pawn;
		public static readonly Texture2D OverlayHealthTex = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 0f, 0f, 0.25f));
		public PawnUIOverlay(Pawn pawn)
		{
			this.pawn = pawn;
		}
		public void DrawPawnGUIOverlay()
		{
			if (!this.pawn.Spawned || Find.FogGrid.IsFogged(this.pawn.Position))
			{
				return;
			}
			if (!this.pawn.RaceProps.Humanlike && (!Find.Map.playSettings.showTameAnimalNames || this.pawn.Name == null))
			{
				return;
			}
			Vector3 vector = GenWorldUI.LabelDrawPosFor(this.pawn, -0.6f);
			Text.Font = GameFont.Tiny;
			string text = this.pawn.NameStringShort.CapitalizeFirst();
			float num = Text.CalcSize(text).x;
			if (num < 20f)
			{
				num = 20f;
			}
			Rect rect = new Rect(vector.x - num / 2f - 4f, vector.y, num + 8f, 12f);
			if (!this.pawn.RaceProps.Humanlike)
			{
				rect.y -= 4f;
			}
			GUI.DrawTexture(rect, TexUI.GrayTextBG);
			if (this.pawn.health.summaryHealth.SummaryHealthPercent < 0.999f)
			{
				Rect rect2 = rect.ContractedBy(1f);
				Widgets.FillableBar(rect2, this.pawn.health.summaryHealth.SummaryHealthPercent, PawnUIOverlay.OverlayHealthTex, BaseContent.ClearTex, false);
			}
			GUI.color = PawnNameColorUtility.PawnNameColorOf(this.pawn);
			Text.Anchor = TextAnchor.UpperCenter;
			Rect rect3 = new Rect(rect.center.x - num / 2f, rect.y - 2f, num, 100f);
			Widgets.Label(rect3, text);
			if (this.pawn.Drafted)
			{
				Widgets.DrawLineHorizontal(vector.x - num / 2f, vector.y + 11f, num);
			}
			if (this.pawn.CanTradeNow)
			{
				OverlayDrawer.DrawOverlay(this.pawn, OverlayTypes.QuestionMark);
			}
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
		}
	}
}
