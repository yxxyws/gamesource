using System;
using Verse;
namespace RimWorld
{
	public class ThoughtWorker_CurrentlyInMentalStatePanicFlee : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			return p.MentalStateDef == MentalStateDefOf.PanicFlee;
		}
	}
}
