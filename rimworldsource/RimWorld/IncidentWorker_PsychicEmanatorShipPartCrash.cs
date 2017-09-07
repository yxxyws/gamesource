using System;
using Verse;
namespace RimWorld
{
	internal class IncidentWorker_PsychicEmanatorShipPartCrash : IncidentWorker_ShipPartCrash
	{
		protected override bool StorytellerCanUseNowSub()
		{
			return !Find.MapConditionManager.ConditionIsActive(MapConditionDefOf.PsychicDrone) && base.StorytellerCanUseNowSub();
		}
	}
}
