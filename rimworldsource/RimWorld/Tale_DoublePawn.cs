using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.Grammar;
namespace RimWorld
{
	public class Tale_DoublePawn : Tale
	{
		public TaleData_Pawn firstPawnData;
		public TaleData_Pawn secondPawnData;
		public override int DominantPawnId
		{
			get
			{
				return this.firstPawnData.pawnID;
			}
		}
		public override string ShortSummary
		{
			get
			{
				string text = this.def.LabelCap + ": " + this.firstPawnData.name;
				if (this.secondPawnData != null)
				{
					text = text + ", " + this.secondPawnData.name;
				}
				return text;
			}
		}
		public Tale_DoublePawn()
		{
		}
		public Tale_DoublePawn(Pawn firstPawn, Pawn secondPawn)
		{
			this.firstPawnData = TaleData_Pawn.GenerateFrom(firstPawn);
			if (secondPawn != null)
			{
				this.secondPawnData = TaleData_Pawn.GenerateFrom(secondPawn);
			}
			if (firstPawn.Spawned || firstPawn.holder != null)
			{
				this.surroundings = TaleData_Surroundings.GenerateFrom(firstPawn.PositionHeld);
			}
		}
		public override bool Concerns(Thing th)
		{
			return (this.secondPawnData != null && this.secondPawnData.pawnID == th.thingIDNumber) || base.Concerns(th) || th.thingIDNumber == this.firstPawnData.pawnID;
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.LookDeep<TaleData_Pawn>(ref this.firstPawnData, "firstPawnData", new object[0]);
			Scribe_Deep.LookDeep<TaleData_Pawn>(ref this.secondPawnData, "secondPawnData", new object[0]);
		}
		[DebuggerHidden]
		protected override IEnumerable<Rule> SpecialTextGenerationRules()
		{
			Tale_DoublePawn.<SpecialTextGenerationRules>c__IteratorBD <SpecialTextGenerationRules>c__IteratorBD = new Tale_DoublePawn.<SpecialTextGenerationRules>c__IteratorBD();
			<SpecialTextGenerationRules>c__IteratorBD.<>f__this = this;
			Tale_DoublePawn.<SpecialTextGenerationRules>c__IteratorBD expr_0E = <SpecialTextGenerationRules>c__IteratorBD;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override void GenerateTestData()
		{
			base.GenerateTestData();
			this.firstPawnData = TaleData_Pawn.GenerateRandom();
			this.secondPawnData = TaleData_Pawn.GenerateRandom();
		}
	}
}
