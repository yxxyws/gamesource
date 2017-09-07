using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse
{
	public class HediffStage
	{
		public float minSeverity;
		public string label;
		public bool everVisible = true;
		public bool lifeThreatening;
		public float vomitMtbDays = -1f;
		public float painFactor = 1f;
		public float painOffset;
		public float partEfficiencyFactor = 1f;
		public float forgetMemoryThoughtMtbDays = -1f;
		public float pctConditionalThoughtsNullified;
		public float opinionOfOthersFactor = 1f;
		public float hungerRateFactor = 1f;
		public float restFallFactor = 1f;
		public float socialFightChanceFactor = 1f;
		public float setMinPartEfficiency = -1f;
		public bool destroyPart;
		public TaleDef tale;
		public List<PawnCapacityModifier> capMods = new List<PawnCapacityModifier>();
		public List<HediffGiver> hediffGivers;
		public List<MentalStateGiver> mentalStateGivers;
		[DebuggerHidden]
		public IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			HediffStage.<SpecialDisplayStats>c__Iterator13C <SpecialDisplayStats>c__Iterator13C = new HediffStage.<SpecialDisplayStats>c__Iterator13C();
			<SpecialDisplayStats>c__Iterator13C.<>f__this = this;
			HediffStage.<SpecialDisplayStats>c__Iterator13C expr_0E = <SpecialDisplayStats>c__Iterator13C;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
