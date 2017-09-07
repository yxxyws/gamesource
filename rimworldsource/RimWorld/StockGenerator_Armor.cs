using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class StockGenerator_Armor : StockGenerator
	{
		[DebuggerHidden]
		public override IEnumerable<Thing> GenerateThings()
		{
			StockGenerator_Armor.<GenerateThings>c__IteratorFF <GenerateThings>c__IteratorFF = new StockGenerator_Armor.<GenerateThings>c__IteratorFF();
			<GenerateThings>c__IteratorFF.<>f__this = this;
			StockGenerator_Armor.<GenerateThings>c__IteratorFF expr_0E = <GenerateThings>c__IteratorFF;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		private bool IsValidThing(ThingDef td)
		{
			return td.tradeability == Tradeability.Stockable && td.techLevel <= this.maxTechLevel && td.IsApparel && (td.GetStatValueAbstract(StatDefOf.ArmorRating_Blunt, null) > 0.15f || td.GetStatValueAbstract(StatDefOf.ArmorRating_Sharp, null) > 0.15f);
		}
		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return this.IsValidThing(thingDef);
		}
	}
}
