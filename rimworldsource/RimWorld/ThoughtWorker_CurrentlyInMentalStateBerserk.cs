using System;
using Verse;
namespace RimWorld
{
	public class ThoughtWorker_CurrentlyInMentalStateBerserk : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			return p.MentalStateDef == MentalStateDefOf.Berserk;
		}
	}
}
