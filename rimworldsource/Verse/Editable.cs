using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse
{
	public class Editable
	{
		public virtual void ResolveReferences()
		{
		}
		public virtual void PostLoad()
		{
		}
		[DebuggerHidden]
		public virtual IEnumerable<string> ConfigErrors()
		{
			Editable.<ConfigErrors>c__Iterator6C <ConfigErrors>c__Iterator6C = new Editable.<ConfigErrors>c__Iterator6C();
			Editable.<ConfigErrors>c__Iterator6C expr_07 = <ConfigErrors>c__Iterator6C;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}
