using System;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	[StaticConstructorOnStartup]
	public static class StorytellerUI
	{
		private const float TitleAreaHeight = 50f;
		private static Vector2 scrollPosition = default(Vector2);
		private static readonly Texture2D StorytellerHighlightTex = ContentFinder<Texture2D>.Get("UI/HeroArt/Storytellers/Highlight", true);
		internal static void DrawStorytellerSelectionInterface(Rect inRect, ref StorytellerDef chosenStoryteller, ref DifficultyDef difficulty)
		{
			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(0f, 0f, 300f, 300f), "ChooseAIStoryteller".Translate());
			float num = 50f;
			Rect position = new Rect(0f, num, inRect.width, inRect.height - num - 50f);
			GUI.BeginGroup(position);
			Rect position2 = new Rect(390f, position.height - Storyteller.PortraitSizeLarge.y - 1f, Storyteller.PortraitSizeLarge.x, Storyteller.PortraitSizeLarge.y);
			GUI.DrawTexture(position2, chosenStoryteller.portraitLargeTex);
			Widgets.DrawLineHorizontal(0f, position.height, position.width);
			Rect outRect = new Rect(0f, 0f, Storyteller.PortraitSizeTiny.x + 16f, position.height);
			Rect viewRect = new Rect(0f, 0f, Storyteller.PortraitSizeTiny.x, (float)DefDatabase<StorytellerDef>.AllDefs.Count<StorytellerDef>() * (Storyteller.PortraitSizeTiny.y + 10f));
			Widgets.BeginScrollView(outRect, ref StorytellerUI.scrollPosition, viewRect);
			Rect rect = new Rect(0f, 0f, Storyteller.PortraitSizeTiny.x, Storyteller.PortraitSizeTiny.y);
			foreach (StorytellerDef current in 
				from tel in DefDatabase<StorytellerDef>.AllDefs
				orderby tel.listOrder
				select tel)
			{
				if (Widgets.ImageButton(rect, current.portraitTinyTex))
				{
					chosenStoryteller = current;
				}
				if (chosenStoryteller == current)
				{
					GUI.DrawTexture(rect, StorytellerUI.StorytellerHighlightTex);
				}
				rect.y += rect.height + 8f;
			}
			Widgets.EndScrollView();
			Text.Font = GameFont.Small;
			Rect rect2 = new Rect(outRect.xMax + 8f, 0f, 240f, 999f);
			Widgets.Label(rect2, "HowStorytellersWork".Translate());
			Rect rect3 = new Rect(outRect.xMax + 8f, outRect.yMin + 200f, 300f, position.height - 300f);
			Text.Font = GameFont.Medium;
			Rect rect4 = new Rect(rect3.x + 15f, rect3.y - 40f, 9999f, 40f);
			Widgets.Label(rect4, chosenStoryteller.label);
			Rect position3 = rect3.ContractedBy(0f);
			GUI.BeginGroup(position3);
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
			Rect rect5 = new Rect(0f, 0f, position3.width, 120f);
			Widgets.Label(rect5, chosenStoryteller.description);
			float num2 = rect5.yMax + 6f;
			foreach (DifficultyDef current2 in DefDatabase<DifficultyDef>.AllDefs)
			{
				Rect rect6 = new Rect(0f, num2, position3.width, 30f);
				if (Mouse.IsOver(rect6))
				{
					Widgets.DrawHighlight(rect6);
				}
				TooltipHandler.TipRegion(rect6, current2.description);
				if (Widgets.LabelRadioButton(rect6, current2.LabelCap, difficulty == current2))
				{
					difficulty = current2;
				}
				num2 += 30f;
			}
			GUI.EndGroup();
			GUI.EndGroup();
		}
	}
}
