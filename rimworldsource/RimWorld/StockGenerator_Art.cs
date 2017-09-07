using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class StockGenerator_Art : StockGenerator
	{
		[DebuggerHidden]
		public override IEnumerable<Thing> GenerateThings()
		{
			StockGenerator_Art.<GenerateThings>c__IteratorFE <GenerateThings>c__IteratorFE = new StockGenerator_Art.<GenerateThings>c__IteratorFE();
			<GenerateThings>c__IteratorFE.<>f__this = this;
			StockGenerator_Art.<GenerateThings>c__IteratorFE expr_0E = <GenerateThings>c__IteratorFE;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override bool HandlesThingDef(ThingDef def)
		{
			return def.tradeability == Tradeability.Stockable && def.Minifiable && def.category == ThingCategory.Building && def.thingClass == typeof(Building_Art) && def.techLevel <= this.maxTechLevel;
		}
	}
}
