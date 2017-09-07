using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class ArtifactTargeter_AllPawnsOnTheMap : ArtifactTargeter
	{
		public override TargetingParameters GetTargetingParameters()
		{
			return new TargetingParameters
			{
				canTargetPawns = true,
				canTargetBuildings = false,
				validator = (TargetInfo x) => base.BaseTargetValidator(x.Thing)
			};
		}
		public override bool PlayerChoosesTarget()
		{
			return false;
		}
		[DebuggerHidden]
		public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
		{
			ArtifactTargeter_AllPawnsOnTheMap.<GetTargets>c__IteratorCD <GetTargets>c__IteratorCD = new ArtifactTargeter_AllPawnsOnTheMap.<GetTargets>c__IteratorCD();
			<GetTargets>c__IteratorCD.<>f__this = this;
			ArtifactTargeter_AllPawnsOnTheMap.<GetTargets>c__IteratorCD expr_0E = <GetTargets>c__IteratorCD;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
