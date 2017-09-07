using System;
using Verse;
namespace RimWorld
{
	public class ArtifactEffectDoer_PsychicShock : ArtifactEffectDoer
	{
		public override void DoEffect(Pawn user, Thing target)
		{
			Pawn pawn = (Pawn)target;
			if (pawn.Dead)
			{
				return;
			}
			Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.PsychicShock, pawn, null);
			pawn.health.AddHediff(hediff, null, null);
		}
	}
}
