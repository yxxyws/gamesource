using System;
using Verse;
namespace RimWorld
{
	public class IncidentWorker_HeatWave : IncidentWorker_MakeMapCondition
	{
		protected override bool StorytellerCanUseNowSub()
		{
			return base.StorytellerCanUseNowSub() && !Find.MapConditionManager.ConditionIsActive(MapConditionDefOf.VolcanicWinter) && GenTemperature.SeasonalTemp >= 20f;
		}
	}
}
