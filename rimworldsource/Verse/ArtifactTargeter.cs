using RimWorld;
using System;
using System.Collections.Generic;
namespace Verse
{
	public abstract class ArtifactTargeter
	{
		public ThingDef artifact;
		public abstract TargetingParameters GetTargetingParameters();
		public abstract bool PlayerChoosesTarget();
		public abstract IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null);
		public bool BaseTargetValidator(Thing t)
		{
			if (this.artifact.artifact.psychicSensitiveTargetsOnly)
			{
				Pawn pawn = t as Pawn;
				if (pawn != null && pawn.GetStatValue(StatDefOf.PsychicSensitivity, true) <= 0f)
				{
					return false;
				}
			}
			return true;
		}
	}
}
