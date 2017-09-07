using RimWorld;
using System;
using UnityEngine;
using Verse.Sound;
namespace Verse
{
	public class FloatMenuOption
	{
		private const float ChoiceMouseOverTextJump = 4f;
		private const float ChoiceLabelShiftRight = 10f;
		public string label;
		public Action action;
		public MenuOptionPriority priority = MenuOptionPriority.Medium;
		public bool autoTakeable;
		public Action mouseoverGuiAction;
		public Thing revalidateClickTarget;
		private static readonly Color ChoiceMouseoverColor = new Color(1f, 0.92f, 0.6f);
		public bool Disabled
		{
			get
			{
				return this.action == null;
			}
			set
			{
				if (value)
				{
					this.action = null;
				}
				else
				{
					Log.Error("Cannot set FloatMenuOption.Disabled to false. Set action instead.");
				}
			}
		}
		public FloatMenuOption()
		{
		}
		public FloatMenuOption(string label, Action action, MenuOptionPriority priority = MenuOptionPriority.Medium, Action mouseoverGuiAction = null, Thing revalidateClickTarget = null)
		{
			this.label = label;
			this.action = action;
			this.priority = priority;
			this.revalidateClickTarget = revalidateClickTarget;
			this.mouseoverGuiAction = mouseoverGuiAction;
		}
		public static Rect GetTextRect(Rect optionRect)
		{
			Rect result = optionRect;
			result.xMin += 10f;
			result.width -= 4f;
			return result;
		}
		public static Vector2 GetTextRectSize(Vector2 optionRectSize)
		{
			return FloatMenuOption.GetTextRect(new Rect(0f, 0f, optionRectSize.x, optionRectSize.y)).size;
		}
		public virtual bool OptionOnGUI(Rect rect, Color baseColor)
		{
			bool flag = !this.Disabled && Mouse.IsOver(rect);
			Texture2D image = TexUI.ChoiceBGTexture;
			if (this.Disabled)
			{
				image = TexUI.ChoiceBGTextureDisabled;
			}
			if (flag)
			{
				GUI.color = FloatMenuOption.ChoiceMouseoverColor * baseColor;
			}
			GUI.DrawTexture(rect, image);
			if (!this.Disabled)
			{
				MouseoverSounds.DoRegion(rect);
			}
			Text.Anchor = TextAnchor.MiddleLeft;
			Rect textRect = FloatMenuOption.GetTextRect(rect);
			if (flag)
			{
				textRect.x += 4f;
			}
			if (this.Disabled)
			{
				GUI.color = new Color(0.7f, 0.7f, 0.7f) * baseColor;
			}
			string text = this.label;
			text = text.TrimEnd(new char[0]);
			Widgets.Label(textRect, text);
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.color = baseColor;
			if (flag && this.mouseoverGuiAction != null)
			{
				this.mouseoverGuiAction();
			}
			return Widgets.InvisibleButton(rect);
		}
		public void Chosen(bool colonistOrdering)
		{
			if (!this.Disabled)
			{
				if (this.action != null)
				{
					if (colonistOrdering)
					{
						SoundDefOf.ColonistOrdered.PlayOneShotOnCamera();
					}
					this.action();
				}
			}
			else
			{
				SoundDefOf.ClickReject.PlayOneShotOnCamera();
			}
		}
		public override string ToString()
		{
			return "Option(" + this.label + ")";
		}
	}
}
