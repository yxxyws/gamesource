using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class IngestibleProperties
	{
		public Type workerClass = typeof(IngestibleWorker);
		public int maxNumToIngestAtOnce = 15;
		public ThoughtDef ingestedDirectThought;
		public ThoughtDef ingestedAsIngredientThought;
		public List<IngestibleHediffGiver> hediffGivers;
		public bool isPleasureDrug;
		public EffecterDef eatEffect;
		public SoundDef soundEat;
		public float nutrition;
		public float joy;
		public JoyKindDef joyKind;
		public FoodPreferability preferability;
		public ThingDef sourceDef;
		public FoodTypeFlags foodType;
		private IngestibleWorker workerInt;
		public IngestibleWorker Worker
		{
			get
			{
				return this.workerInt;
			}
		}
		public JoyKindDef JoyKind
		{
			get
			{
				return (this.joyKind == null) ? JoyKindDefOf.Gluttonous : this.joyKind;
			}
		}
		public bool HumanEdible
		{
			get
			{
				return (FoodTypeFlags.OmnivoreHuman & this.foodType) != FoodTypeFlags.None;
			}
		}
		public void PostLoadSpecial(ThingDef parentDef)
		{
			this.workerInt = (IngestibleWorker)Activator.CreateInstance(this.workerClass);
			this.workerInt.def = parentDef;
		}
		[DebuggerHidden]
		public IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			IngestibleProperties.<ConfigErrors>c__Iterator66 <ConfigErrors>c__Iterator = new IngestibleProperties.<ConfigErrors>c__Iterator66();
			<ConfigErrors>c__Iterator.parentDef = parentDef;
			<ConfigErrors>c__Iterator.<$>parentDef = parentDef;
			<ConfigErrors>c__Iterator.<>f__this = this;
			IngestibleProperties.<ConfigErrors>c__Iterator66 expr_1C = <ConfigErrors>c__Iterator;
			expr_1C.$PC = -2;
			return expr_1C;
		}
		[DebuggerHidden]
		internal IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			IngestibleProperties.<SpecialDisplayStats>c__Iterator67 <SpecialDisplayStats>c__Iterator = new IngestibleProperties.<SpecialDisplayStats>c__Iterator67();
			<SpecialDisplayStats>c__Iterator.<>f__this = this;
			IngestibleProperties.<SpecialDisplayStats>c__Iterator67 expr_0E = <SpecialDisplayStats>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
