using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Page_CreateWorldParams : Window
	{
		private const float TopAreaHeight = 50f;
		private const float WorldAspectRatio = 0.75f;
		private static readonly Vector2 WinSize = new Vector2(1020f, 764f);
		private static readonly int[] WorldSizes = new int[]
		{
			150,
			200,
			250,
			300,
			350,
			400
		};
		public override Vector2 InitialWindowSize
		{
			get
			{
				return Page_CreateWorldParams.WinSize;
			}
		}
		public Page_CreateWorldParams()
		{
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
		}
		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(0f, 0f, 999f, 300f), "CreateWorld".Translate());
			Rect position = new Rect(0f, 50f, inRect.width, inRect.height - 38f - 50f);
			GUI.BeginGroup(position);
			Text.Font = GameFont.Small;
			float num = 0f;
			Rect rect = new Rect(200f, num, 200f, 30f);
			if (Widgets.TextButton(rect, "Reset".Translate(), true, false))
			{
				WorldGenerationData.Reset();
			}
			num += 40f;
			Widgets.Label(new Rect(0f, num, 200f, 30f), "WorldSeed".Translate());
			Rect rect2 = new Rect(200f, num, 200f, 30f);
			WorldGenerationData.seedString = Widgets.TextField(rect2, WorldGenerationData.seedString);
			num += 40f;
			Rect rect3 = new Rect(200f, num, 200f, 30f);
			if (Widgets.TextButton(rect3, "RandomizeSeed".Translate(), true, false))
			{
				WorldGenerationData.seedString = WorldGenerationData.RandomSeedString();
			}
			num += 40f;
			Widgets.Label(new Rect(0f, num, 200f, 30f), "WorldSize".Translate());
			Rect rect4 = new Rect(200f, num, 200f, 30f);
			if (Widgets.TextButton(rect4, WorldGenerationData.size.ToStringCross(), true, false))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				int[] worldSizes = Page_CreateWorldParams.WorldSizes;
				for (int i = 0; i < worldSizes.Length; i++)
				{
					int num2 = worldSizes[i];
					IntVec2 sz = new IntVec2(num2, Mathf.RoundToInt((float)num2 * 0.75f));
					FloatMenuOption item = new FloatMenuOption(sz.ToStringCross(), delegate
					{
						WorldGenerationData.size = sz;
					}, MenuOptionPriority.Medium, null, null);
					list.Add(item);
				}
				Find.WindowStack.Add(new FloatMenu(list, false));
			}
			num += 40f;
			GUI.EndGroup();
			DialogUtility.DoNextBackButtons(inRect, "WorldGenerate".Translate(), new Action(this.GenerateAndReview), delegate
			{
				this.Close(true);
			});
		}
		private void GenerateAndReview()
		{
			LongEventHandler.QueueLongEvent(delegate
			{
				WorldGenerator.GenerateWorld();
				LongEventHandler.ExecuteWhenFinished(delegate
				{
					Find.WindowStack.Add(new Page_CreateWorldReview());
					this.Close(true);
				});
			}, "GeneratingWorld", true, null);
		}
	}
}
