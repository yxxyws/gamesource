using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class IncidentWorker_MakeMapCondition : IncidentWorker
	{
		protected override bool StorytellerCanUseNowSub()
		{
			return !Find.MapConditionManager.ConditionIsActive(this.def.mapCondition);
		}
		public override bool TryExecute(IncidentParms parms)
		{
			int duration = Mathf.RoundToInt(this.def.durationDays.RandomInRange * 60000f);
			Find.MapConditionManager.RegisterCondition(MapConditionMaker.MakeCondition(this.def.mapCondition, duration));
			base.SendStandardLetter();
			return true;
		}
	}
}
