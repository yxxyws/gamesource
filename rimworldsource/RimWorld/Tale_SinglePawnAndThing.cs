using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.Grammar;
namespace RimWorld
{
	public class Tale_SinglePawnAndThing : Tale_SinglePawn
	{
		public TaleData_Thing thingData;
		public Tale_SinglePawnAndThing()
		{
		}
		public Tale_SinglePawnAndThing(Pawn pawn, Thing item) : base(pawn)
		{
			this.thingData = TaleData_Thing.GenerateFrom(item);
		}
		public override bool Concerns(Thing th)
		{
			return base.Concerns(th) || th.thingIDNumber == this.thingData.thingID;
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.LookDeep<TaleData_Thing>(ref this.thingData, "thingData", new object[0]);
		}
		[DebuggerHidden]
		protected override IEnumerable<Rule> SpecialTextGenerationRules()
		{
			Tale_SinglePawnAndThing.<SpecialTextGenerationRules>c__IteratorC2 <SpecialTextGenerationRules>c__IteratorC = new Tale_SinglePawnAndThing.<SpecialTextGenerationRules>c__IteratorC2();
			<SpecialTextGenerationRules>c__IteratorC.<>f__this = this;
			Tale_SinglePawnAndThing.<SpecialTextGenerationRules>c__IteratorC2 expr_0E = <SpecialTextGenerationRules>c__IteratorC;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override void GenerateTestData()
		{
			base.GenerateTestData();
			this.thingData = TaleData_Thing.GenerateRandom();
		}
	}
}
