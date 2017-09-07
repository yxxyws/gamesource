using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class StockGenerator_BuyWeirdOrganic : StockGenerator
	{
		[DebuggerHidden]
		public override IEnumerable<Thing> GenerateThings()
		{
			StockGenerator_BuyWeirdOrganic.<GenerateThings>c__Iterator100 <GenerateThings>c__Iterator = new StockGenerator_BuyWeirdOrganic.<GenerateThings>c__Iterator100();
			StockGenerator_BuyWeirdOrganic.<GenerateThings>c__Iterator100 expr_07 = <GenerateThings>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}
		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return thingDef == ThingDefOf.InsectJelly;
		}
	}
}
