using System;
using Verse;
namespace RimWorld
{
	public class ArtifactEffectDoer_GoodwillImpact : ArtifactEffectDoer
	{
		public float goodwillImpact = -50f;
		public override void DoEffect(Pawn user, Thing target)
		{
			if (user.Faction != null && target.Faction != null && user.Faction != target.Faction)
			{
				target.Faction.AffectGoodwillWith(user.Faction, this.goodwillImpact);
			}
		}
	}
}
