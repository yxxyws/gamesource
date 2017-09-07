using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class StockGenerator_WeaponsRanged : StockGenerator
	{
		[DebuggerHidden]
		public override IEnumerable<Thing> GenerateThings()
		{
			StockGenerator_WeaponsRanged.<GenerateThings>c__IteratorFD <GenerateThings>c__IteratorFD = new StockGenerator_WeaponsRanged.<GenerateThings>c__IteratorFD();
			<GenerateThings>c__IteratorFD.<>f__this = this;
			StockGenerator_WeaponsRanged.<GenerateThings>c__IteratorFD expr_0E = <GenerateThings>c__IteratorFD;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return thingDef.IsRangedWeapon && thingDef.tradeability == Tradeability.Stockable && thingDef.techLevel <= this.maxTechLevel;
		}
	}
}
