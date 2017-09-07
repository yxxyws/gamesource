using System;
using Verse;
namespace RimWorld
{
	public class ArtifactEffectDoer_BrainDamageChance : ArtifactEffectDoer
	{
		private const float Chance = 0.3f;
		public override void DoEffect(Pawn user, Thing target)
		{
			Pawn pawn = (Pawn)target;
			if (pawn.Dead)
			{
				return;
			}
			if (Rand.Value <= 0.3f)
			{
				BodyPartRecord brain = pawn.health.hediffSet.GetBrain();
				if (brain == null)
				{
					return;
				}
				Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.BrainDamage, pawn, null);
				hediff.Part = brain;
				hediff.Severity = (float)Rand.RangeInclusive(1, 5);
				pawn.health.AddHediff(hediff, null, null);
			}
		}
	}
}
