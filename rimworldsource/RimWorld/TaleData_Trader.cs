using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;
using Verse.Grammar;
namespace RimWorld
{
	public class TaleData_Trader : TaleData
	{
		public string name;
		public int pawnID = -1;
		public Gender gender = Gender.Male;
		private bool IsPawn
		{
			get
			{
				return this.pawnID >= 0;
			}
		}
		public override void ExposeData()
		{
			Scribe_Values.LookValue<string>(ref this.name, "name", null, false);
			Scribe_Values.LookValue<int>(ref this.pawnID, "pawnID", -1, false);
			Scribe_Values.LookValue<Gender>(ref this.gender, "gender", Gender.Male, false);
		}
		[DebuggerHidden]
		public override IEnumerable<Rule> GetRules(string prefix)
		{
			TaleData_Trader.<GetRules>c__IteratorBA <GetRules>c__IteratorBA = new TaleData_Trader.<GetRules>c__IteratorBA();
			<GetRules>c__IteratorBA.prefix = prefix;
			<GetRules>c__IteratorBA.<$>prefix = prefix;
			<GetRules>c__IteratorBA.<>f__this = this;
			TaleData_Trader.<GetRules>c__IteratorBA expr_1C = <GetRules>c__IteratorBA;
			expr_1C.$PC = -2;
			return expr_1C;
		}
		public static TaleData_Trader GenerateFrom(ITrader trader)
		{
			TaleData_Trader taleData_Trader = new TaleData_Trader();
			taleData_Trader.name = trader.TraderName;
			Pawn pawn = trader as Pawn;
			if (pawn != null)
			{
				taleData_Trader.pawnID = pawn.thingIDNumber;
				taleData_Trader.gender = pawn.gender;
			}
			return taleData_Trader;
		}
		public static TaleData_Trader GenerateRandom()
		{
			PawnKindDef pawnKindDef = (
				from d in DefDatabase<PawnKindDef>.AllDefs
				where d.trader
				select d).RandomElement<PawnKindDef>();
			Pawn pawn = PawnGenerator.GeneratePawn(pawnKindDef, FactionUtility.DefaultFactionFrom(pawnKindDef.defaultFactionType));
			pawn.mindState.wantsToTradeWithColony = true;
			PawnComponentsUtility.AddAndRemoveDynamicComponents(pawn, true);
			return TaleData_Trader.GenerateFrom(pawn);
		}
	}
}
