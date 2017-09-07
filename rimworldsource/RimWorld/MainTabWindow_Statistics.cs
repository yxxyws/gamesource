using System;
using System.Text;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class MainTabWindow_Statistics : MainTabWindow
	{
		public override Vector2 RequestedTabSize
		{
			get
			{
				return new Vector2(300f, 315f);
			}
		}
		public override void PreOpen()
		{
			base.PreOpen();
			Find.StoryWatcher.watcherWealth.ForceRecount();
		}
		public override void DoWindowContents(Rect fillRect)
		{
			base.DoWindowContents(fillRect);
			StringBuilder stringBuilder = new StringBuilder();
			TimeSpan timeSpan = new TimeSpan(0, 0, (int)Find.ColonyInfo.realPlayTime);
			stringBuilder.AppendLine(string.Concat(new object[]
			{
				"Playtime".Translate(),
				": ",
				timeSpan.Days,
				"LetterDay".Translate(),
				" ",
				timeSpan.Hours,
				"LetterHour".Translate(),
				" ",
				timeSpan.Minutes,
				"LetterMinute".Translate(),
				" ",
				timeSpan.Seconds,
				"LetterSecond".Translate()
			}));
			stringBuilder.AppendLine("Storyteller".Translate() + ": " + Find.Storyteller.def.label);
			DifficultyDef difficulty = Find.Storyteller.difficulty;
			stringBuilder.AppendLine("Difficulty".Translate() + ": " + difficulty.LabelCap);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("ColonyWealthTotal".Translate() + ": " + Find.StoryWatcher.watcherWealth.WealthTotal.ToString("#######0"));
			stringBuilder.AppendLine("ColonyWealthItems".Translate() + ": " + Find.StoryWatcher.watcherWealth.WealthItems.ToString("#######0"));
			stringBuilder.AppendLine("ColonyWealthBuildings".Translate() + ": " + Find.StoryWatcher.watcherWealth.WealthBuildings.ToString("#######0"));
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("NumThreatBigs".Translate() + ": " + Find.StoryWatcher.statsRecord.numThreatBigs);
			stringBuilder.AppendLine("NumEnemyRaids".Translate() + ": " + Find.StoryWatcher.statsRecord.numRaidsEnemy);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("DamageTaken".Translate() + ": " + Find.StoryWatcher.watcherDamage.DamageTakenEver);
			stringBuilder.AppendLine("ColonistsKilled".Translate() + ": " + Find.StoryWatcher.statsRecord.colonistsKilled);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("ColonistsLaunched".Translate() + ": " + Find.StoryWatcher.statsRecord.colonistsLaunched);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			Rect rect = new Rect(0f, 0f, 400f, 400f);
			Widgets.Label(rect, stringBuilder.ToString());
		}
	}
}
