using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.Grammar;
namespace RimWorld
{
	public abstract class TaleData : IExposable
	{
		public abstract void ExposeData();
		[DebuggerHidden]
		public virtual IEnumerable<Rule> GetRules(string prefix)
		{
			TaleData.<GetRules>c__IteratorB5 <GetRules>c__IteratorB = new TaleData.<GetRules>c__IteratorB5();
			<GetRules>c__IteratorB.<>f__this = this;
			TaleData.<GetRules>c__IteratorB5 expr_0E = <GetRules>c__IteratorB;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		[DebuggerHidden]
		public virtual IEnumerable<Rule> GetRules()
		{
			TaleData.<GetRules>c__IteratorB6 <GetRules>c__IteratorB = new TaleData.<GetRules>c__IteratorB6();
			<GetRules>c__IteratorB.<>f__this = this;
			TaleData.<GetRules>c__IteratorB6 expr_0E = <GetRules>c__IteratorB;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
