using System;
using UnityEngine;
using Verse.Sound;
namespace Verse
{
	public class WidgetRow
	{
		private const float DefaultMaxWidth = 2000f;
		private const float DefaultYGap = 29f;
		public const float ButtonSize = 24f;
		public const float Height = 24f;
		private float startX;
		private float curX;
		private float curY;
		private float yGap = 29f;
		private float maxWidth = 2000f;
		private UIDirection growDirection = UIDirection.RightThenUp;
		public float FinalX
		{
			get
			{
				return this.curX;
			}
		}
		public float FinalY
		{
			get
			{
				return this.curY;
			}
		}
		public WidgetRow()
		{
		}
		public WidgetRow(float x, float y, UIDirection growDirection = UIDirection.RightThenUp, float maxWidth = 2000f, float yGap = 29f)
		{
			this.Init(x, y, growDirection, maxWidth, yGap);
		}
		public void Init(float x, float y, UIDirection growDirection = UIDirection.RightThenUp, float maxWidth = 2000f, float yGap = 29f)
		{
			this.growDirection = growDirection;
			this.startX = x;
			this.curX = x;
			this.curY = y;
			this.maxWidth = maxWidth;
			this.yGap = yGap;
		}
		private float LeftX(float elementWidth)
		{
			if (this.growDirection == UIDirection.RightThenUp || this.growDirection == UIDirection.RightThenDown)
			{
				return this.curX;
			}
			return this.curX - elementWidth;
		}
		private void IncrementPosition(float amount)
		{
			if (this.growDirection == UIDirection.RightThenUp || this.growDirection == UIDirection.RightThenDown)
			{
				this.curX += amount;
			}
			else
			{
				this.curX -= amount;
			}
			if (Mathf.Abs(this.curX - this.startX) >= this.maxWidth)
			{
				this.IncrementY();
			}
		}
		private void IncrementY()
		{
			if (this.growDirection == UIDirection.RightThenUp || this.growDirection == UIDirection.LeftThenUp)
			{
				this.curY -= this.yGap;
			}
			else
			{
				this.curY += this.yGap;
			}
			this.curX = this.startX;
		}
		private void IncrementYIfWillExceedMaxWidth(float width)
		{
			if (Mathf.Abs(this.curX - this.startX) + Mathf.Abs(width) >= this.maxWidth)
			{
				this.IncrementY();
			}
		}
		public void DoGap(float width)
		{
			if (this.curX != this.startX)
			{
				this.IncrementPosition(width);
			}
		}
		public bool DoIconButton(Texture2D tex, string tooltip = null)
		{
			this.IncrementYIfWillExceedMaxWidth(24f);
			Rect rect = new Rect(this.LeftX(24f), this.curY, 24f, 24f);
			bool result = Widgets.ImageButton(rect, tex);
			this.IncrementPosition(28f);
			if (!tooltip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, tooltip);
			}
			return result;
		}
		public void DoIconButtonGap()
		{
			if (this.curY != this.startX)
			{
				this.IncrementPosition(28f);
			}
		}
		public void DoIconToggle(ref bool toggleable, Texture2D tex, string tooltip, SoundDef mouseoverSound = null, string tutorTag = null)
		{
			this.IncrementYIfWillExceedMaxWidth(24f);
			Rect rect = new Rect(this.LeftX(24f), this.curY, 24f, 24f);
			bool flag = Widgets.ImageButton(rect, tex);
			this.IncrementPosition(28f);
			if (!tooltip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, tooltip);
			}
			Rect position = new Rect(rect.x + rect.width / 2f, rect.y, rect.height / 2f, rect.height / 2f);
			Texture2D image = (!toggleable) ? Widgets.CheckboxOffTex : Widgets.CheckboxOnTex;
			GUI.DrawTexture(position, image);
			if (mouseoverSound != null)
			{
				MouseoverSounds.DoRegion(rect, mouseoverSound);
			}
			if (flag)
			{
				toggleable = (toggleable == 0);
			}
			if (tutorTag != null)
			{
				TutorUIHighlighter.HighlightOpportunity(tutorTag, rect);
			}
		}
		public Rect DoIcon(Texture2D tex)
		{
			this.IncrementYIfWillExceedMaxWidth(24f);
			Rect rect = new Rect(this.LeftX(24f), this.curY, 24f, 24f);
			GUI.DrawTexture(rect, tex);
			this.IncrementPosition(28f);
			return rect;
		}
		public bool DoTextButton(string label, string tooltip = null, bool drawBackground = true, bool doMouseoverSound = false)
		{
			Vector2 vector = Text.CalcSize(label);
			vector.x += 16f;
			vector.y += 2f;
			this.IncrementYIfWillExceedMaxWidth(vector.x);
			Rect rect = new Rect(this.LeftX(vector.x), this.curY, vector.x, vector.y);
			bool result = Widgets.TextButton(rect, label, drawBackground, doMouseoverSound);
			this.IncrementPosition(rect.width + 4f);
			if (!tooltip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, tooltip);
			}
			return result;
		}
		public Rect DoLabel(string text, float width = -1f)
		{
			if (width < 0f)
			{
				width = Text.CalcSize(text).x;
			}
			this.IncrementYIfWillExceedMaxWidth(width);
			Rect rect = new Rect(this.LeftX(width), this.curY, width, 24f);
			this.IncrementPosition(2f);
			Widgets.Label(rect, text);
			this.IncrementPosition(2f);
			this.IncrementPosition(rect.width);
			return rect;
		}
		public Rect DoBar(float width, float height, float fillPct, string label, Texture2D fillTex, Texture2D bgTex = null)
		{
			this.IncrementYIfWillExceedMaxWidth(width);
			Rect rect = new Rect(this.LeftX(width), this.curY, width, height);
			Widgets.FillableBar(rect, fillPct, fillTex, bgTex, false);
			if (!label.NullOrEmpty())
			{
				Rect rect2 = rect;
				rect2.xMin += 2f;
				rect2.xMax -= 2f;
				if (Text.Anchor >= TextAnchor.UpperLeft)
				{
					rect2.height += 14f;
				}
				Text.Font = GameFont.Tiny;
				Text.WordWrap = false;
				Widgets.Label(rect2, label);
				Text.WordWrap = true;
			}
			this.IncrementPosition(width);
			return rect;
		}
	}
}
