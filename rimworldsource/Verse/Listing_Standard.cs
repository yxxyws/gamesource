using RimWorld;
using System;
using UnityEngine;
using Verse.Sound;
namespace Verse
{
	public class Listing_Standard : Listing
	{
		public const float LineHeightFactor = 1.2f;
		private const float DefaultGapHeight = 12f;
		public const float ColumnSpacing = 17f;
		private const float DefSelectionLineHeight = 21f;
		protected float curX;
		public Listing_Standard(Rect rect, GameFont font) : base(rect)
		{
			Text.Font = font;
		}
		public Listing_Standard(Rect rect) : base(rect)
		{
			Text.Font = GameFont.Small;
		}
		public void NewColumn()
		{
			this.curY = 0f;
			this.curX += base.ColumnWidth + 17f;
		}
		private void CheckLoop(float neededHeight)
		{
			if (this.curY + neededHeight > this.listingRect.height)
			{
				this.NewColumn();
			}
		}
		public void DoLabel(string label)
		{
			float num = Text.CalcHeight(label, base.ColumnWidth);
			this.CheckLoop(num);
			Widgets.Label(new Rect(this.curX, this.curY, base.ColumnWidth, num), label);
			this.curY += num * 1.2f;
		}
		public void DoLabelCheckbox(string label, ref bool checkOn, string tooltip = null)
		{
			float lineHeight = Text.LineHeight;
			this.CheckLoop(lineHeight);
			Rect rect = new Rect(this.curX, this.curY, base.ColumnWidth, lineHeight);
			if (!tooltip.NullOrEmpty())
			{
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawHighlight(rect);
				}
				TooltipHandler.TipRegion(rect, tooltip);
			}
			Widgets.LabelCheckbox(rect, label, ref checkOn, false);
			this.curY += lineHeight * 1.2f;
		}
		public bool DoLabelCheckboxSelectable(string label, ref bool selected, ref bool checkOn)
		{
			float lineHeight = Text.LineHeight;
			this.CheckLoop(lineHeight);
			bool result = Widgets.LabelCheckboxSelectable(new Rect(this.curX, this.curY, base.ColumnWidth, lineHeight), label, ref selected, ref checkOn);
			this.curY += lineHeight * 1.2f;
			return result;
		}
		public bool DoRadioButton(string label, bool active, float tabIn = 0f)
		{
			float lineHeight = Text.LineHeight;
			this.CheckLoop(lineHeight);
			bool result = Widgets.LabelRadioButton(new Rect(this.curX + tabIn, this.curY, base.ColumnWidth - tabIn, lineHeight), label, active);
			this.curY += lineHeight * 1.2f;
			return result;
		}
		public void DoLabelCheckboxDebug(string label, ref bool checkOn)
		{
			Text.Font = GameFont.Tiny;
			this.CheckLoop(24f);
			Widgets.LabelCheckbox(new Rect(this.curX, this.curY, base.ColumnWidth, 22f), label, ref checkOn, false);
			this.curY += 24f;
		}
		public bool DoButtonDebug(string label)
		{
			this.CheckLoop(24f);
			Text.Font = GameFont.Tiny;
			bool wordWrap = Text.WordWrap;
			Text.WordWrap = false;
			bool result = Widgets.TextButton(new Rect(this.curX, this.curY, base.ColumnWidth, 22f), label, true, false);
			Text.WordWrap = wordWrap;
			this.curY += 24f;
			return result;
		}
		public string DoTextField(string text)
		{
			this.CheckLoop(28f);
			Rect rect = new Rect(this.curX, this.curY, base.ColumnWidth, 26f);
			string result = Widgets.TextField(rect, text);
			this.curY += 28f;
			return result;
		}
		public float DoSlider(float val, float min, float max)
		{
			this.CheckLoop(30f);
			Rect position = new Rect(this.curX, this.curY, base.ColumnWidth, 30f);
			GUI.skin.horizontalSlider.alignment = TextAnchor.MiddleCenter;
			float result = GUI.HorizontalSlider(position, val, min, max);
			this.curY += 30f;
			return result;
		}
		public Rect GetRect(float height)
		{
			this.CheckLoop(height);
			Rect result = new Rect(this.curX, this.curY, base.ColumnWidth, height);
			this.curY += height;
			return result;
		}
		public void DoGap(float gapHeight = 12f)
		{
			this.curY += gapHeight;
		}
		public bool DoTextButtonLabeled(string label, string buttonLabel)
		{
			this.CheckLoop(34f);
			Widgets.Label(new Rect(this.curX, this.curY, base.ColumnWidth, 30f), label);
			bool result = Widgets.TextButton(new Rect(this.curX + base.ColumnWidth / 2f, this.curY, base.ColumnWidth / 2f, 30f), buttonLabel, true, false);
			this.curY += 34f;
			return result;
		}
		public bool DoTextButton(string label)
		{
			this.CheckLoop(34f);
			bool result = Widgets.TextButton(new Rect(this.curX, this.curY, base.ColumnWidth, 30f), label, true, false);
			this.curY += 34f;
			return result;
		}
		public override bool DoImageButton(Texture2D tex, float width, float height)
		{
			this.CheckLoop(height + 4f);
			bool result = Widgets.ImageButton(new Rect(this.curX, this.curY, width, height), tex);
			this.curY += height + 4f;
			return result;
		}
		public void DoIntAdjuster(ref int val, int countChange, int min = 0)
		{
			this.CheckLoop(24f);
			Rect rect = new Rect(0f, this.curY, 42f, 24f);
			if (Widgets.TextButton(rect, "-" + countChange, true, false))
			{
				SoundDefOf.AmountDecrement.PlayOneShotOnCamera();
				val -= countChange;
				if (val < min)
				{
					val = min;
				}
			}
			rect.x += rect.width + 2f;
			if (Widgets.TextButton(rect, "+" + countChange, true, false))
			{
				SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
				val += countChange;
				if (val < min)
				{
					val = min;
				}
			}
			this.curY += 26f;
		}
		public void DoIntSetter(ref int val, int target, string label, float width = 42f)
		{
			this.CheckLoop(24f);
			Rect rect = new Rect(0f, this.curY, width, 24f);
			if (Widgets.TextButton(rect, label, true, false))
			{
				SoundDefOf.TickLow.PlayOneShotOnCamera();
				val = target;
			}
			this.curY += 26f;
		}
		public bool DoSelectableDef(string name, bool selected, Action deleteCallback)
		{
			Text.Font = GameFont.Tiny;
			float width = this.listingRect.width - 21f;
			Text.Anchor = TextAnchor.MiddleLeft;
			Rect rect = new Rect(this.curX, this.curY, width, 21f);
			if (selected)
			{
				Widgets.DrawHighlight(rect);
			}
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawBox(rect, 1);
			}
			Text.WordWrap = false;
			Widgets.Label(rect, name);
			Text.WordWrap = true;
			if (deleteCallback != null)
			{
				Rect butRect = new Rect(rect.xMax, rect.y, 21f, 21f);
				if (Widgets.ImageButton(butRect, TexButton.DeleteX))
				{
					deleteCallback();
				}
			}
			Text.Anchor = TextAnchor.UpperLeft;
			this.curY += 21f;
			return Widgets.InvisibleButton(rect);
		}
	}
}
