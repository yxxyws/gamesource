using System;
namespace Verse
{
	public abstract class ArtifactEffectDoer
	{
		public ThingDef artifact;
		public abstract void DoEffect(Pawn user, Thing target);
	}
}
