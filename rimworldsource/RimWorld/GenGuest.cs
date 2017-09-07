using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public static class GenGuest
	{
		public static void PrisonerRelease(Pawn p)
		{
			if (p.ownership != null)
			{
				p.ownership.UnclaimAll();
			}
			if (p.Faction == Faction.OfColony)
			{
				p.needs.mood.thoughts.TryGainThought(ThoughtDefOf.WasImprisoned);
				p.guest.SetGuestStatus(null, false);
				return;
			}
			p.guest.released = true;
			IntVec3 vec;
			if (ExitUtility.TryFindClosestExitSpot(p, out vec, TraverseMode.ByPawn))
			{
				Job job = new Job(JobDefOf.Goto, vec);
				job.exitMapOnArrival = true;
				p.jobs.StartJob(job, JobCondition.None, null, false, true, null);
			}
		}
	}
}
