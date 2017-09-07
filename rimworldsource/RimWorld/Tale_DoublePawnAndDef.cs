using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.Grammar;
namespace RimWorld
{
	public class Tale_DoublePawnAndDef : Tale_DoublePawn
	{
		public TaleData_Def defData;
		public Tale_DoublePawnAndDef()
		{
		}
		public Tale_DoublePawnAndDef(Pawn firstPawn, Pawn secondPawn, Def def) : base(firstPawn, secondPawn)
		{
			this.defData = TaleData_Def.GenerateFrom(def);
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.LookDeep<TaleData_Def>(ref this.defData, "defData", new object[0]);
		}
		[DebuggerHidden]
		protected override IEnumerable<Rule> SpecialTextGenerationRules()
		{
			Tale_DoublePawnAndDef.<SpecialTextGenerationRules>c__IteratorBE <SpecialTextGenerationRules>c__IteratorBE = new Tale_DoublePawnAndDef.<SpecialTextGenerationRules>c__IteratorBE();
			<SpecialTextGenerationRules>c__IteratorBE.<>f__this = this;
			Tale_DoublePawnAndDef.<SpecialTextGenerationRules>c__IteratorBE expr_0E = <SpecialTextGenerationRules>c__IteratorBE;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override void GenerateTestData()
		{
			base.GenerateTestData();
			this.defData = TaleData_Def.GenerateFrom(DefDatabase<TrainableDef>.GetRandom());
		}
	}
}
