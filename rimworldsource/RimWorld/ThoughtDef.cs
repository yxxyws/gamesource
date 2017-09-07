using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class ThoughtDef : Def
	{
		public Type thoughtClass;
		public Type workerClass;
		public List<ThoughtStage> stages = new List<ThoughtStage>();
		public int stackLimit = 1;
		public float stackedEffectMultiplier = 0.75f;
		public float durationDays;
		public bool invert;
		public bool validWhileInMentalState;
		public ThoughtDef nextThought;
		public List<TraitDef> nullifyingTraits;
		public List<TaleDef> nullifyingOwnTales;
		public List<TraitDef> requiredTraits;
		public StatDef effectMultiplyingStat;
		public HediffDef hediff;
		public bool nullifiedIfNotColonist;
		public int stackLimitPerPawn = -1;
		public float lerpOpinionToZeroAfterDurationPct = 0.7f;
		public float maxCumulatedOpinionOffset = 3.40282347E+38f;
		public TaleDef taleDef;
		[Unsaved]
		private ThoughtWorker workerInt;
		public bool IsMemory
		{
			get
			{
				return this.durationDays > 0f || typeof(Thought_Memory).IsAssignableFrom(this.thoughtClass);
			}
		}
		public int DurationTicks
		{
			get
			{
				return (int)(this.durationDays * 60000f);
			}
		}
		public ThoughtWorker Worker
		{
			get
			{
				if (this.workerInt == null && this.workerClass != null)
				{
					this.workerInt = (ThoughtWorker)Activator.CreateInstance(this.workerClass);
					this.workerInt.def = this;
				}
				return this.workerInt;
			}
		}
		public Type ThoughtClass
		{
			get
			{
				if (this.thoughtClass != null)
				{
					return this.thoughtClass;
				}
				if (this.IsMemory)
				{
					return typeof(Thought_Memory);
				}
				return typeof(Thought_Situational);
			}
		}
		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			ThoughtDef.<ConfigErrors>c__Iterator77 <ConfigErrors>c__Iterator = new ThoughtDef.<ConfigErrors>c__Iterator77();
			<ConfigErrors>c__Iterator.<>f__this = this;
			ThoughtDef.<ConfigErrors>c__Iterator77 expr_0E = <ConfigErrors>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		private bool AnyTraitUsesThisThought()
		{
			List<TraitDef> allDefsListForReading = DefDatabase<TraitDef>.AllDefsListForReading;
			for (int i = 0; i < allDefsListForReading.Count; i++)
			{
				TraitDef traitDef = allDefsListForReading[i];
				for (int j = 0; j < traitDef.degreeDatas.Count; j++)
				{
					if (traitDef.degreeDatas[j].permaThought == this)
					{
						return true;
					}
				}
			}
			return false;
		}
		public static ThoughtDef Named(string defName)
		{
			return DefDatabase<ThoughtDef>.GetNamed(defName, true);
		}
	}
}
