using System;
using Verse;
namespace RimWorld
{
	public class ArtifactEffectDoer_MoodBoost : ArtifactEffectDoer
	{
		public override void DoEffect(Pawn user, Thing target)
		{
			Pawn pawn = (Pawn)target;
			if (pawn.Dead || pawn.needs == null || pawn.needs.mood == null)
			{
				return;
			}
			pawn.needs.mood.thoughts.TryGainThought(ThoughtMaker.MakeThought(ThoughtDefOf.ArtifactMoodBoost));
		}
	}
}
