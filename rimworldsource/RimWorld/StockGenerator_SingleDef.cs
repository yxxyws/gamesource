using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class StockGenerator_SingleDef : StockGenerator
	{
		private ThingDef thingDef;
		[DebuggerHidden]
		public override IEnumerable<Thing> GenerateThings()
		{
			StockGenerator_SingleDef.<GenerateThings>c__IteratorF8 <GenerateThings>c__IteratorF = new StockGenerator_SingleDef.<GenerateThings>c__IteratorF8();
			<GenerateThings>c__IteratorF.<>f__this = this;
			StockGenerator_SingleDef.<GenerateThings>c__IteratorF8 expr_0E = <GenerateThings>c__IteratorF;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return thingDef == this.thingDef;
		}
	}
}
