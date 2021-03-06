using System;
using Verse;
namespace RimWorld
{
	public class ArtifactEffectDoer_Berserk : ArtifactEffectDoer
	{
		public override void DoEffect(Pawn user, Thing target)
		{
			Pawn pawn = (Pawn)target;
			if (pawn.Dead)
			{
				return;
			}
			pawn.mindState.mentalStateHandler.StartMentalState(MentalStateDefOf.Berserk);
		}
	}
}
