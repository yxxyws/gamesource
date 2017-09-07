using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;
namespace RimWorld
{
	public class StockGenerator_Category : StockGenerator
	{
		private ThingCategoryDef categoryDef;
		private IntRange thingDefCountRange = IntRange.one;
		[DebuggerHidden]
		public override IEnumerable<Thing> GenerateThings()
		{
			StockGenerator_Category.<GenerateThings>c__IteratorF9 <GenerateThings>c__IteratorF = new StockGenerator_Category.<GenerateThings>c__IteratorF9();
			<GenerateThings>c__IteratorF.<>f__this = this;
			StockGenerator_Category.<GenerateThings>c__IteratorF9 expr_0E = <GenerateThings>c__IteratorF;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return this.categoryDef.DescendantThingDefs.Contains(thingDef) && thingDef.techLevel <= this.maxTechLevel;
		}
	}
}
