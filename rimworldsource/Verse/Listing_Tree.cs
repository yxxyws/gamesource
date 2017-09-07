using System;
using UnityEngine;
namespace Verse
{
	public class Listing_Tree : Listing
	{
		protected const float OpenCloseWidgetSize = 18f;
		public float nestLevelPixelWidth = 13f;
		protected float labelWidth = 100f;
		protected float editAreaWidth = 250f;
		public float WholeColumnWidth
		{
			get
			{
				return this.labelWidth + this.editAreaWidth;
			}
		}
		public Listing_Tree(Rect rect, float labelColumnWidth) : base(rect)
		{
			Text.Anchor = TextAnchor.MiddleLeft;
			Text.WordWrap = false;
			this.labelWidth = labelColumnWidth;
			this.editAreaWidth = rect.width - labelColumnWidth;
		}
		public override void End()
		{
			base.End();
			Text.WordWrap = true;
			Text.Anchor = TextAnchor.UpperLeft;
		}
		protected float XAtIndentLevel(int indentLevel)
		{
			return (float)indentLevel * this.nestLevelPixelWidth;
		}
		protected void DrawLabelLeft(string label, string tipText, int indentLevel)
		{
			Rect rect = new Rect(0f, this.curY, this.labelWidth, this.lineHeight);
			rect.xMin = this.XAtIndentLevel(indentLevel) + 18f;
			Widgets.DrawHighlightIfMouseover(rect);
			if (!tipText.NullOrEmpty())
			{
				if (Mouse.IsOver(rect))
				{
					GUI.DrawTexture(rect, TexUI.HighlightTex);
				}
				TooltipHandler.TipRegion(rect, tipText);
			}
			Text.Anchor = TextAnchor.MiddleLeft;
			rect.yMax += 5f;
			rect.yMin -= 5f;
			Widgets.Label(rect, label);
			Text.Anchor = TextAnchor.UpperLeft;
		}
		protected bool DoOpenCloseWidget(TreeNode node, int indentLevel, int openMask)
		{
			if (!node.Openable)
			{
				return false;
			}
			float left = this.XAtIndentLevel(indentLevel);
			float top = this.curY + this.lineHeight / 2f - 9f;
			Rect butRect = new Rect(left, top, 18f, 18f);
			Texture2D tex = (!node.IsOpen(openMask)) ? TexButton.Reveal : TexButton.Collapse;
			if (Widgets.ImageButton(butRect, tex))
			{
				node.SetOpen(openMask, !node.IsOpen(openMask));
				return true;
			}
			return false;
		}
		public void DrawInfoText(string text, int indentLevel)
		{
			Text.WordWrap = true;
			Rect rect = new Rect(0f, this.curY, this.WholeColumnWidth, 50f);
			rect.xMin = this.labelWidth;
			rect.height = Text.CalcHeight(text, rect.width);
			Widgets.Label(rect, text);
			this.curY += rect.height;
			Text.WordWrap = false;
		}
		public bool DoTextButton(string label)
		{
			Text.WordWrap = true;
			float num = Text.CalcHeight(label, this.WholeColumnWidth);
			Rect rect = new Rect(0f, this.curY, this.WholeColumnWidth, num);
			bool result = Widgets.TextButton(rect, label, true, false);
			this.curY += num;
			Text.WordWrap = false;
			return result;
		}
		public WidgetRow StartWidgetsRow(int indentLevel)
		{
			WidgetRow result = new WidgetRow(this.labelWidth, this.curY, UIDirection.RightThenUp, 2000f, 29f);
			this.curY += 24f;
			return result;
		}
	}
}
