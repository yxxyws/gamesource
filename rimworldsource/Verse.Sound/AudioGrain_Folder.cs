using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse.Sound
{
	public class AudioGrain_Folder : AudioGrain
	{
		[LoadAlias("clipPath")]
		public string clipFolderPath = string.Empty;
		[DebuggerHidden]
		public override IEnumerable<ResolvedGrain> GetResolvedGrains()
		{
			AudioGrain_Folder.<GetResolvedGrains>c__Iterator146 <GetResolvedGrains>c__Iterator = new AudioGrain_Folder.<GetResolvedGrains>c__Iterator146();
			<GetResolvedGrains>c__Iterator.<>f__this = this;
			AudioGrain_Folder.<GetResolvedGrains>c__Iterator146 expr_0E = <GetResolvedGrains>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
