using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace RimWorld
{
	public class MainTabWindow_Restrict : MainTabWindow_PawnList
	{
		private const float TopAreaHeight = 65f;
		private const float CopyPasteColumnWidth = 52f;
		private const float CopyPasteIconSize = 24f;
		private const float TimeTablesWidth = 500f;
		private const float AAGapWidth = 6f;
		private TimeAssignmentDef selectedAssignment = TimeAssignmentDefOf.Work;
		private List<TimeAssignmentDef> clipboard;
		private float hourWidth;
		public override Vector2 RequestedTabSize
		{
			get
			{
				return new Vector2(1010f, 65f + (float)base.PawnsCount * 30f + 65f);
			}
		}
		public override void DoWindowContents(Rect fillRect)
		{
			base.DoWindowContents(fillRect);
			Rect position = new Rect(0f, 0f, fillRect.width, 65f);
			GUI.BeginGroup(position);
			Rect rect = new Rect(0f, 0f, 217f, position.height);
			this.DrawTimeAssignmentSelectorGrid(rect);
			float num = 227f;
			Text.Font = GameFont.Tiny;
			Text.Anchor = TextAnchor.LowerLeft;
			for (int i = 0; i < 24; i++)
			{
				Rect rect2 = new Rect(num + 4f, 0f, this.hourWidth, position.height + 3f);
				Widgets.Label(rect2, i.ToString());
				num += this.hourWidth;
			}
			num += 6f;
			Rect rect3 = new Rect(num, 0f, position.width - num, Mathf.Round(position.height / 2f));
			Text.Font = GameFont.Small;
			if (Widgets.TextButton(rect3, "ManageAreas".Translate(), true, false))
			{
				Find.WindowStack.Add(new Dialog_ManageAreas());
			}
			Text.Font = GameFont.Tiny;
			Text.Anchor = TextAnchor.LowerCenter;
			Rect rect4 = new Rect(num, 0f, position.width - num, position.height + 3f);
			Widgets.Label(rect4, "AllowedArea".Translate());
			GUI.EndGroup();
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.color = Color.white;
			Rect outRect = new Rect(0f, position.height, fillRect.width, fillRect.height - position.height);
			base.DrawRows(outRect);
		}
		private void DrawTimeAssignmentSelectorGrid(Rect rect)
		{
			rect.yMax -= 2f;
			Rect rect2 = rect;
			rect2.xMax = rect2.center.x;
			rect2.yMax = rect2.center.y;
			this.DrawTimeAssignmentSelectorFor(rect2, TimeAssignmentDefOf.Anything);
			rect2.x += rect2.width;
			this.DrawTimeAssignmentSelectorFor(rect2, TimeAssignmentDefOf.Work);
			rect2.y += rect2.height;
			rect2.x -= rect2.width;
			this.DrawTimeAssignmentSelectorFor(rect2, TimeAssignmentDefOf.Joy);
			rect2.x += rect2.width;
			this.DrawTimeAssignmentSelectorFor(rect2, TimeAssignmentDefOf.Sleep);
		}
		private void DrawTimeAssignmentSelectorFor(Rect rect, TimeAssignmentDef ta)
		{
			rect = rect.ContractedBy(2f);
			GUI.DrawTexture(rect, ta.ColorTexture);
			if (Widgets.InvisibleButton(rect))
			{
				this.selectedAssignment = ta;
				SoundDefOf.TickHigh.PlayOneShotOnCamera();
			}
			GUI.color = Color.white;
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleCenter;
			GUI.color = Color.white;
			Widgets.Label(rect, ta.LabelCap);
			Text.Anchor = TextAnchor.UpperLeft;
			if (this.selectedAssignment == ta)
			{
				Widgets.DrawBox(rect, 2);
			}
		}
		protected override void DrawPawnRow(Rect rect, Pawn p)
		{
			GUI.BeginGroup(rect);
			Rect rect2 = new Rect(175f, 0f, 24f, 24f);
			if (Widgets.ImageButton(rect2, TexButton.Copy))
			{
				this.CopyFrom(p);
				SoundDefOf.TickHigh.PlayOneShotOnCamera();
			}
			TooltipHandler.TipRegion(rect2, "Copy".Translate());
			if (this.clipboard != null)
			{
				Rect rect3 = rect2;
				rect3.x = rect2.xMax + 2f;
				if (Widgets.ImageButton(rect3, TexButton.Paste))
				{
					this.PasteTo(p);
					SoundDefOf.TickLow.PlayOneShotOnCamera();
				}
				TooltipHandler.TipRegion(rect3, "Paste".Translate());
			}
			float num = 227f;
			this.hourWidth = 20.833334f;
			for (int i = 0; i < 24; i++)
			{
				Rect rect4 = new Rect(num, 0f, this.hourWidth, rect.height);
				this.DoTimeAssignment(rect4, p, i);
				num += this.hourWidth;
			}
			GUI.color = Color.white;
			num += 6f;
			Rect rect5 = new Rect(num, 0f, rect.width - num, rect.height);
			AreaAllowedGUI.DoAllowedAreaSelectors(rect5, p, AllowedAreaMode.Humanlike);
			GUI.EndGroup();
		}
		private void DoTimeAssignment(Rect rect, Pawn p, int hour)
		{
			rect = rect.ContractedBy(1f);
			TimeAssignmentDef assignment = p.timetable.GetAssignment(hour);
			GUI.DrawTexture(rect, assignment.ColorTexture);
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawBox(rect, 2);
				if (assignment != this.selectedAssignment && Input.GetMouseButton(0))
				{
					SoundDefOf.DesignateDragStandardChanged.PlayOneShotOnCamera();
					p.timetable.SetAssignment(hour, this.selectedAssignment);
				}
			}
		}
		private void CopyFrom(Pawn p)
		{
			this.clipboard = p.timetable.times.ToList<TimeAssignmentDef>();
		}
		private void PasteTo(Pawn p)
		{
			for (int i = 0; i < 24; i++)
			{
				p.timetable.times[i] = this.clipboard[i];
			}
		}
	}
}
