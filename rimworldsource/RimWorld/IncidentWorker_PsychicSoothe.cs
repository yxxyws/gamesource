using System;
using Verse;
namespace RimWorld
{
	public class IncidentWorker_PsychicSoothe : IncidentWorker_PsychicEmanation
	{
		private const float MaxAverageMood = 0.36f;
		protected override bool StorytellerCanUseNowSub()
		{
			if (!base.StorytellerCanUseNowSub())
			{
				return false;
			}
			float num = 0f;
			int num2 = 0;
			foreach (Pawn current in Find.MapPawns.FreeColonistsAndPrisoners)
			{
				num += current.needs.mood.CurLevel;
				num2++;
			}
			float num3 = num / (float)num2;
			return num3 < 0.36f;
		}
		protected override void DoConditionAndLetter(int duration, Gender gender)
		{
			MapCondition_PsychicEmanation mapCondition_PsychicEmanation = (MapCondition_PsychicEmanation)MapConditionMaker.MakeCondition(MapConditionDefOf.PsychicSoothe, duration);
			mapCondition_PsychicEmanation.gender = gender;
			Find.MapConditionManager.RegisterCondition(mapCondition_PsychicEmanation);
			string text = "LetterIncidentPsychicSoothe".Translate(new object[]
			{
				gender.ToString().Translate().ToLower()
			});
			Find.LetterStack.ReceiveLetter("LetterLabelPsychicSoothe".Translate(), text, LetterType.Good, null);
		}
	}
}
