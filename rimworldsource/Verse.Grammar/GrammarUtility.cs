using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse.Grammar
{
	public static class GrammarUtility
	{
		[DebuggerHidden]
		public static IEnumerable<Rule> RulesForPawn(string prefix, Name name, PawnKindDef kind, Gender gender, Faction faction = null)
		{
			GrammarUtility.<RulesForPawn>c__Iterator156 <RulesForPawn>c__Iterator = new GrammarUtility.<RulesForPawn>c__Iterator156();
			<RulesForPawn>c__Iterator.name = name;
			<RulesForPawn>c__Iterator.kind = kind;
			<RulesForPawn>c__Iterator.prefix = prefix;
			<RulesForPawn>c__Iterator.faction = faction;
			<RulesForPawn>c__Iterator.gender = gender;
			<RulesForPawn>c__Iterator.<$>name = name;
			<RulesForPawn>c__Iterator.<$>kind = kind;
			<RulesForPawn>c__Iterator.<$>prefix = prefix;
			<RulesForPawn>c__Iterator.<$>faction = faction;
			<RulesForPawn>c__Iterator.<$>gender = gender;
			GrammarUtility.<RulesForPawn>c__Iterator156 expr_4F = <RulesForPawn>c__Iterator;
			expr_4F.$PC = -2;
			return expr_4F;
		}
	}
}
