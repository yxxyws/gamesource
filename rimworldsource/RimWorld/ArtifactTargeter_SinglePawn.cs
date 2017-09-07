using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class ArtifactTargeter_SinglePawn : ArtifactTargeter
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
			return true;
		}
		[DebuggerHidden]
		public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
		{
			ArtifactTargeter_SinglePawn.<GetTargets>c__IteratorCE <GetTargets>c__IteratorCE = new ArtifactTargeter_SinglePawn.<GetTargets>c__IteratorCE();
			<GetTargets>c__IteratorCE.targetChosenByPlayer = targetChosenByPlayer;
			<GetTargets>c__IteratorCE.<$>targetChosenByPlayer = targetChosenByPlayer;
			ArtifactTargeter_SinglePawn.<GetTargets>c__IteratorCE expr_15 = <GetTargets>c__IteratorCE;
			expr_15.$PC = -2;
			return expr_15;
		}
	}
}
