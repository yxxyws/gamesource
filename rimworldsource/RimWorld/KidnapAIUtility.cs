using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public static class KidnapAIUtility
	{
		public static Pawn ClosestKidnapVictim(Pawn kidnapper, float searchRadius, bool wantHealthyVictim = false)
		{
			Predicate<Thing> validator = delegate(Thing t)
			{
				Pawn pawn = t as Pawn;
				return pawn.RaceProps.Humanlike && pawn.Downed && pawn.Faction.HostileTo(kidnapper.Faction) && kidnapper.Position.CanReach(pawn.Position, PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Some, false)) && kidnapper.CanReserve(pawn, 1);
			};
			return (Pawn)GenClosest.ClosestThingReachable(kidnapper.Position, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(kidnapper, Danger.Deadly, TraverseMode.ByPawn, false), searchRadius, validator, null, -1, false);
		}
		public static Pawn ReachableWoundedGuest(Pawn searcher)
		{
			List<Pawn> list = Find.MapPawns.SpawnedPawnsInFaction(searcher.Faction);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].guest != null && list[i].Downed && searcher.CanReserveAndReach(list[i], PathEndMode.OnCell, Danger.Some, 1))
				{
					return list[i];
				}
			}
			return null;
		}
	}
}
