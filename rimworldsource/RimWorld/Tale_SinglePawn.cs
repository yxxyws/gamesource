using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.Grammar;
namespace RimWorld
{
	public class Tale_SinglePawn : Tale
	{
		public TaleData_Pawn pawnData;
		public override int DominantPawnId
		{
			get
			{
				return this.pawnData.pawnID;
			}
		}
		public override string ShortSummary
		{
			get
			{
				return this.def.LabelCap + ": " + this.pawnData.name;
			}
		}
		public Tale_SinglePawn()
		{
		}
		public Tale_SinglePawn(Pawn pawn)
		{
			this.pawnData = TaleData_Pawn.GenerateFrom(pawn);
			if (pawn.Spawned || pawn.holder != null)
			{
				this.surroundings = TaleData_Surroundings.GenerateFrom(pawn.PositionHeld);
			}
		}
		public override bool Concerns(Thing th)
		{
			return base.Concerns(th) || this.pawnData.pawnID == th.thingIDNumber;
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.LookDeep<TaleData_Pawn>(ref this.pawnData, "pawnData", new object[0]);
		}
		[DebuggerHidden]
		protected override IEnumerable<Rule> SpecialTextGenerationRules()
		{
			Tale_SinglePawn.<SpecialTextGenerationRules>c__IteratorC0 <SpecialTextGenerationRules>c__IteratorC = new Tale_SinglePawn.<SpecialTextGenerationRules>c__IteratorC0();
			<SpecialTextGenerationRules>c__IteratorC.<>f__this = this;
			Tale_SinglePawn.<SpecialTextGenerationRules>c__IteratorC0 expr_0E = <SpecialTextGenerationRules>c__IteratorC;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override void GenerateTestData()
		{
			base.GenerateTestData();
			this.pawnData = TaleData_Pawn.GenerateRandom();
		}
	}
}
