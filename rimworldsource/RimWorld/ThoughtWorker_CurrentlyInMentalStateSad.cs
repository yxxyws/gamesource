using System;
using Verse;
namespace RimWorld
{
	public class ThoughtWorker_CurrentlyInMentalStateSad : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			return p.MentalStateDef == MentalStateDefOf.DazedWander || p.MentalStateDef == MentalStateDefOf.GiveUpExit;
		}
	}
}
