using System;
using UnityEngine;
namespace Verse
{
	public class Dialog_Message : Window
	{
		private string text;
		private string title;
		private static Vector2 scrollPosition = Vector2.zero;
		private static float scrollViewHeight = 300f;
		public override Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2(600f, 400f);
			}
		}
		public Dialog_Message(string text, string title = null)
		{
			this.text = text;
			this.title = title;
			this.forcePause = true;
			this.closeOnEscapeKey = true;
			this.absorbInputAroundWindow = true;
		}
		public override void DoWindowContents(Rect inRect)
		{
			float num = inRect.y;
			if (!this.title.NullOrEmpty())
			{
				Text.Font = GameFont.Medium;
				Widgets.Label(new Rect(0f, num, inRect.width, 60f), this.title);
				num += 60f;
			}
			Text.Font = GameFont.Small;
			Rect viewRect = new Rect(inRect.x, inRect.y, inRect.width - 16f, Dialog_Message.scrollViewHeight);
			Rect outRect = new Rect(inRect.x, num, inRect.width, inRect.height - 40f - num);
			Widgets.BeginScrollView(outRect, ref Dialog_Message.scrollPosition, viewRect);
			Widgets.Label(new Rect(0f, 0f, inRect.width, Dialog_Message.scrollViewHeight), this.text);
			if (Event.current.type == EventType.Layout)
			{
				Dialog_Message.scrollViewHeight = Text.CalcHeight(this.text, inRect.width);
			}
			Widgets.EndScrollView();
			if (Widgets.TextButton(new Rect(inRect.width / 2f - 80f, inRect.height - 35f, 160f, 35f), "OK".Translate(), true, false))
			{
				this.Close(true);
			}
		}
	}
}
