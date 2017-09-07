using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public abstract class MainTabWindow_PawnList : MainTabWindow
	{
		public const float PawnRowHeight = 30f;
		protected const float NameColumnWidth = 175f;
		protected const float NameLeftMargin = 15f;
		protected Vector2 scrollPosition = Vector2.zero;
		protected List<Pawn> pawns = new List<Pawn>();
		protected int PawnsCount
		{
			get
			{
				return this.pawns.Count;
			}
		}
		protected abstract void DrawPawnRow(Rect r, Pawn p);
		public override void PreOpen()
		{
			base.PreOpen();
			this.BuildPawnList();
		}
		public override void PostOpen()
		{
			base.PostOpen();
			this.currentWindowRect.size = this.InitialWindowSize;
		}
		public override void DoWindowContents(Rect inRect)
		{
			base.DoWindowContents(inRect);
			this.currentWindowRect.size = this.InitialWindowSize;
		}
		protected virtual void BuildPawnList()
		{
			this.pawns.Clear();
			this.pawns.AddRange(Find.MapPawns.FreeColonists);
		}
		public void Notify_PawnsChanged()
		{
			this.BuildPawnList();
		}
		protected void DrawRows(Rect outRect)
		{
			Rect viewRect = new Rect(0f, 0f, outRect.width - 16f, (float)this.pawns.Count * 30f);
			Widgets.BeginScrollView(outRect, ref this.scrollPosition, viewRect);
			float num = 0f;
			for (int i = 0; i < this.pawns.Count; i++)
			{
				Pawn p = this.pawns[i];
				Rect rect = new Rect(0f, num, viewRect.width, 30f);
				if (num - this.scrollPosition.y + 30f >= 0f && num - this.scrollPosition.y <= outRect.height)
				{
					GUI.color = new Color(1f, 1f, 1f, 0.2f);
					Widgets.DrawLineHorizontal(0f, num, viewRect.width);
					GUI.color = Color.white;
					this.PreDrawPawnRow(rect, p);
					this.DrawPawnRow(rect, p);
					this.PostDrawPawnRow(rect, p);
				}
				num += 30f;
			}
			Widgets.EndScrollView();
			Text.Anchor = TextAnchor.UpperLeft;
		}
		private void PreDrawPawnRow(Rect rect, Pawn p)
		{
			Rect rect2 = new Rect(0f, rect.y, rect.width, 30f);
			if (Mouse.IsOver(rect2))
			{
				GUI.DrawTexture(rect2, TexUI.HighlightTex);
			}
			Rect rect3 = new Rect(0f, rect.y, 175f, 30f);
			Rect position = rect3.ContractedBy(3f);
			if (p.health.summaryHealth.SummaryHealthPercent < 0.99f)
			{
				Rect rect4 = new Rect(rect3);
				rect4.xMin -= 4f;
				rect4.yMin += 4f;
				rect4.yMax -= 6f;
				Widgets.FillableBar(rect4, p.health.summaryHealth.SummaryHealthPercent, PawnUIOverlay.OverlayHealthTex, BaseContent.ClearTex, false);
			}
			if (Mouse.IsOver(rect3))
			{
				GUI.DrawTexture(position, TexUI.HighlightTex);
			}
			string label;
			if (!p.RaceProps.Humanlike && p.Name != null && !p.Name.Numerical)
			{
				label = p.Name.ToStringShort.CapitalizeFirst() + ", " + p.KindLabel;
			}
			else
			{
				label = p.LabelCap;
			}
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleLeft;
			Text.WordWrap = false;
			Rect rect5 = new Rect(rect3);
			rect5.xMin += 15f;
			Widgets.Label(rect5, label);
			Text.WordWrap = true;
			if (Widgets.InvisibleButton(rect3))
			{
				Find.MainTabsRoot.EscapeCurrentTab(true);
				Find.Selector.ClearSelection();
				JumpToTargetUtility.TryJumpAndSelect(p);
				return;
			}
			TipSignal tooltip = p.GetTooltip();
			tooltip.text = "ClickToJumpTo".Translate() + "\n\n" + tooltip.text;
			TooltipHandler.TipRegion(rect3, tooltip);
		}
		private void PostDrawPawnRow(Rect rect, Pawn p)
		{
			if (p.Downed)
			{
				GUI.color = new Color(1f, 0f, 0f, 0.5f);
				Widgets.DrawLineHorizontal(rect.x, rect.center.y, rect.width);
				GUI.color = Color.white;
			}
		}
	}
}
