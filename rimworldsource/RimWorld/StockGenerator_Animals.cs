using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class StockGenerator_Animals : StockGenerator
	{
		private IntRange kindCountRange = new IntRange(1, 1);
		private float minWildness;
		private List<string> tradeTags;
		private bool checkTemperature;
		[DebuggerHidden]
		public override IEnumerable<Thing> GenerateThings()
		{
			StockGenerator_Animals.<GenerateThings>c__IteratorFC <GenerateThings>c__IteratorFC = new StockGenerator_Animals.<GenerateThings>c__IteratorFC();
			<GenerateThings>c__IteratorFC.<>f__this = this;
			StockGenerator_Animals.<GenerateThings>c__IteratorFC expr_0E = <GenerateThings>c__IteratorFC;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return thingDef.category == ThingCategory.Pawn && thingDef.race.Animal;
		}
		private bool PawnKindAllowed(PawnKindDef kind)
		{
			return kind.RaceProps.Animal && kind.RaceProps.wildness >= this.minWildness && kind.RaceProps.wildness < 1f && (!this.checkTemperature || GenTemperature.SeasonAndOutdoorTemperatureAcceptableFor(kind.race)) && kind.race.tradeTags != null && this.tradeTags.Find((string x) => kind.race.tradeTags.Contains(x)) != null;
		}
	}
}
