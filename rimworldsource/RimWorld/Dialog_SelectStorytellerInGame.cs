using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Dialog_SelectStorytellerInGame : Window
	{
		public override Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2(1020f, 764f);
			}
		}
		public Dialog_SelectStorytellerInGame()
		{
			this.forcePause = true;
			this.doCloseButton = true;
			this.doCloseX = true;
			this.absorbInputAroundWindow = true;
		}
		public override void DoWindowContents(Rect inRect)
		{
			Storyteller storyteller = Find.Map.storyteller;
			StorytellerDef def = storyteller.def;
			StorytellerUI.DrawStorytellerSelectionInterface(inRect, ref storyteller.def, ref storyteller.difficulty);
			if (storyteller.def != def)
			{
				storyteller.Notify_DefChanged();
			}
		}
	}
}
