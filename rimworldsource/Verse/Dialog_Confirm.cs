using System;
using UnityEngine;
namespace Verse
{
	public class Dialog_Confirm : Window
	{
		private string text;
		private Action confirmedAction;
		private bool destructiveAction;
		public override Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2(500f, 300f);
			}
		}
		public Dialog_Confirm(string text, Action confirmedAction, bool destructive = false)
		{
			this.text = text;
			this.confirmedAction = confirmedAction;
			this.destructiveAction = destructive;
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
		}
		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Small;
			Widgets.Label(new Rect(0f, 0f, inRect.width, inRect.height), this.text);
			if (this.destructiveAction)
			{
				GUI.color = Color.red;
			}
			if (Widgets.TextButton(new Rect(0f, inRect.height - 35f, inRect.width / 2f - 20f, 35f), "Confirm".Translate(), true, false))
			{
				this.confirmedAction();
				this.Close(true);
			}
			GUI.color = Color.white;
			if (Widgets.TextButton(new Rect(inRect.width / 2f + 20f, inRect.height - 35f, inRect.width / 2f - 20f, 35f), "GoBack".Translate(), true, false))
			{
				this.Close(true);
			}
		}
	}
}
