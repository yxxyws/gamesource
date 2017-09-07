using System;
using Verse;
namespace RimWorld
{
	public class ArtifactTargeter_AllAnimalsOnTheMap : ArtifactTargeter_AllPawnsOnTheMap
	{
		public override TargetingParameters GetTargetingParameters()
		{
			TargetingParameters targetingParameters = base.GetTargetingParameters();
			targetingParameters.validator = delegate(TargetInfo targ)
			{
				if (!base.BaseTargetValidator(targ.Thing))
				{
					return false;
				}
				Pawn pawn = targ.Thing as Pawn;
				return pawn != null && pawn.RaceProps.Animal;
			};
			return targetingParameters;
		}
	}
}
