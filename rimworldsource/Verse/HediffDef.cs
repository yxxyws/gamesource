using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
namespace Verse
{
	public class HediffDef : Def
	{
		public Type hediffClass = typeof(Hediff);
		public List<HediffCompProperties> comps;
		public bool isBad = true;
		public float initialSeverity = 0.001f;
		public bool naturallyHealed;
		public float lethalSeverity = -1f;
		public List<HediffStage> stages;
		public ThingDef spawnThingOnRemoved;
		public bool tendable;
		public float chanceToCauseNoPain;
		public bool makesSickThought;
		public bool makesAlert = true;
		public bool displayWound;
		public Color defaultLabelColor = Color.white;
		public InjuryProps injuryProps;
		public AddedBodyPartProps addedPartProps;
		public HediffCompProperties CompPropsFor(Type compClass)
		{
			if (this.comps != null)
			{
				for (int i = 0; i < this.comps.Count; i++)
				{
					if (this.comps[i].compClass == compClass)
					{
						return this.comps[i];
					}
				}
			}
			return null;
		}
		public bool HasComp(Type compClass)
		{
			return this.CompPropsFor(compClass) != null;
		}
		public override void ResolveReferences()
		{
			base.ResolveReferences();
			if (!this.tendable && this.CompPropsFor(typeof(HediffComp_Tendable)) != null)
			{
				this.tendable = true;
			}
		}
		public bool PossibleToDevelopImmunity()
		{
			HediffCompProperties hediffCompProperties = this.CompPropsFor(typeof(HediffComp_Immunizable));
			return hediffCompProperties != null && (hediffCompProperties.immunityPerDayNotSick > 0f || hediffCompProperties.immunityPerDaySick > 0f);
		}
		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			HediffDef.<ConfigErrors>c__Iterator13A <ConfigErrors>c__Iterator13A = new HediffDef.<ConfigErrors>c__Iterator13A();
			<ConfigErrors>c__Iterator13A.<>f__this = this;
			HediffDef.<ConfigErrors>c__Iterator13A expr_0E = <ConfigErrors>c__Iterator13A;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		[DebuggerHidden]
		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			HediffDef.<SpecialDisplayStats>c__Iterator13B <SpecialDisplayStats>c__Iterator13B = new HediffDef.<SpecialDisplayStats>c__Iterator13B();
			<SpecialDisplayStats>c__Iterator13B.<>f__this = this;
			HediffDef.<SpecialDisplayStats>c__Iterator13B expr_0E = <SpecialDisplayStats>c__Iterator13B;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public static HediffDef Named(string defName)
		{
			return DefDatabase<HediffDef>.GetNamed(defName, true);
		}
	}
}
