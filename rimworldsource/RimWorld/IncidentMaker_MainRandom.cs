using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class IncidentMaker_MainRandom : IncidentMaker
	{
		protected override float MinIncidentChancePopFactor
		{
			get
			{
				return 0.2f;
			}
		}
		[DebuggerHidden]
		public override IEnumerable<QueuedIncident> NewIncidentSet()
		{
			IncidentMaker_MainRandom.<NewIncidentSet>c__IteratorAB <NewIncidentSet>c__IteratorAB = new IncidentMaker_MainRandom.<NewIncidentSet>c__IteratorAB();
			<NewIncidentSet>c__IteratorAB.<>f__this = this;
			IncidentMaker_MainRandom.<NewIncidentSet>c__IteratorAB expr_0E = <NewIncidentSet>c__IteratorAB;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		protected override float IncidentChanceAdjustedForPopulation(IncidentDef def)
		{
			float num = 1f;
			if (def.populationEffect >= IncidentPopulationEffect.Increase)
			{
				num = Find.Storyteller.intenderPopulation.PopulationIntent;
			}
			else
			{
				if (def.populationEffect <= IncidentPopulationEffect.Decrease)
				{
					num = -Find.Storyteller.intenderPopulation.PopulationIntent;
				}
			}
			num = Mathf.Clamp(num, this.MinIncidentChancePopFactor, 9999f);
			return def.Worker.AdjustedChance * num;
		}
		private IncidentCategory DecideCategory()
		{
			int num = Find.TickManager.TicksGame - Find.StoryWatcher.storyState.LastThreatBigQueueTick;
			if (num > 60000 * base.Def.random_MaxThreatBigIntervalDays)
			{
				return IncidentCategory.ThreatBig;
			}
			float max = base.Def.random_GeneralWeight + base.Def.random_ThreatSmallWeight + base.Def.random_LargeThreatWeight;
			float num2 = Rand.Range(0f, max);
			if (num2 < base.Def.random_GeneralWeight)
			{
				return IncidentCategory.Misc;
			}
			if (num2 < base.Def.random_GeneralWeight + base.Def.random_ThreatSmallWeight)
			{
				return IncidentCategory.ThreatSmall;
			}
			return IncidentCategory.ThreatBig;
		}
		public override IncidentParms ParmsNow(IncidentCategory incCat)
		{
			IncidentParms incidentParms = IncidentMakerUtility.DefaultParmsNow(base.Def, incCat);
			incidentParms.points *= Rand.Range(0.5f, 1.5f);
			return incidentParms;
		}
	}
}
