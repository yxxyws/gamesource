using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class StockGenerator_Tag : StockGenerator
	{
		private string tradeTag;
		private IntRange thingDefCountRange = IntRange.one;
		[DebuggerHidden]
		public override IEnumerable<Thing> GenerateThings()
		{
			StockGenerator_Tag.<GenerateThings>c__IteratorFA <GenerateThings>c__IteratorFA = new StockGenerator_Tag.<GenerateThings>c__IteratorFA();
			<GenerateThings>c__IteratorFA.<>f__this = this;
			StockGenerator_Tag.<GenerateThings>c__IteratorFA expr_0E = <GenerateThings>c__IteratorFA;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return thingDef.tradeTags != null && thingDef.tradeability == Tradeability.Stockable && thingDef.techLevel <= this.maxTechLevel && thingDef.tradeTags.Contains(this.tradeTag);
		}
	}
}
