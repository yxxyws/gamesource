using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse.Sound
{
	public class AudioGrain_Clip : AudioGrain
	{
		public string clipPath = string.Empty;
		[DebuggerHidden]
		public override IEnumerable<ResolvedGrain> GetResolvedGrains()
		{
			AudioGrain_Clip.<GetResolvedGrains>c__Iterator145 <GetResolvedGrains>c__Iterator = new AudioGrain_Clip.<GetResolvedGrains>c__Iterator145();
			<GetResolvedGrains>c__Iterator.<>f__this = this;
			AudioGrain_Clip.<GetResolvedGrains>c__Iterator145 expr_0E = <GetResolvedGrains>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
