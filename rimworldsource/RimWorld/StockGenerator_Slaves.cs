using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class StockGenerator_Slaves : StockGenerator
	{
		[DebuggerHidden]
		public override IEnumerable<Thing> GenerateThings()
		{
			StockGenerator_Slaves.<GenerateThings>c__IteratorFB <GenerateThings>c__IteratorFB = new StockGenerator_Slaves.<GenerateThings>c__IteratorFB();
			<GenerateThings>c__IteratorFB.<>f__this = this;
			StockGenerator_Slaves.<GenerateThings>c__IteratorFB expr_0E = <GenerateThings>c__IteratorFB;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return thingDef.category == ThingCategory.Pawn && thingDef.race.Humanlike;
		}
	}
}
