using System;
using Verse;
namespace RimWorld
{
	public class IncidentWorker_ColdSnap : IncidentWorker_MakeMapCondition
	{
		protected override bool StorytellerCanUseNowSub()
		{
			return base.StorytellerCanUseNowSub() && GenTemperature.SeasonalTemp >= 0f && GenTemperature.SeasonalTemp <= 10f;
		}
	}
}
