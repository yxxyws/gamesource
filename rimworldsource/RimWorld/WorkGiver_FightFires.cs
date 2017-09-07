using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	internal class WorkGiver_FightFires : WorkGiver_Scanner
	{
		private const int NearbyPawnRadius = 15;
		private const int MaxReservationCheckDistance = 15;
		private const float HandledDistance = 5f;
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForDef(ThingDefOf.Fire);
			}
		}
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.Touch;
			}
		}
		public override bool HasJobOnThing(Pawn pawn, Thing t)
		{
			Fire fire = t as Fire;
			if (fire == null)
			{
				return false;
			}
			Pawn pawn2 = fire.parent as Pawn;
			if (pawn2 != null)
			{
				if (pawn2 == pawn)
				{
					return false;
				}
				if ((pawn2.Faction == pawn.Faction || pawn2.HostFaction == pawn.Faction || pawn2.HostFaction == pawn.HostFaction) && !Find.AreaHome[fire.Position] && Gen.ManhattanDistanceFlat(pawn.Position, pawn2.Position) > 15)
				{
					return false;
				}
			}
			else
			{
				if (!Find.AreaHome[fire.Position])
				{
					return false;
				}
			}
			return ((pawn.Position - fire.Position).LengthHorizontalSquared <= 225f || pawn.CanReserve(fire, 1)) && !WorkGiver_FightFires.FireIsBeingHandled(fire, pawn);
		}
		public override Job JobOnThing(Pawn pawn, Thing t)
		{
			return new Job(JobDefOf.BeatFire, t);
		}
		public static bool FireIsBeingHandled(Fire f, Pawn potentialHandler)
		{
			Pawn pawn = Find.Reservations.FirstReserverOf(f, potentialHandler.Faction);
			return pawn != null && pawn.Position.InHorDistOf(f.Position, 5f);
		}
	}
}
