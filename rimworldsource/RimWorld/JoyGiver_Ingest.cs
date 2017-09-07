using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JoyGiver_Ingest : JoyGiver
	{
		public override Job TryGiveJob(Pawn pawn)
		{
			return this.TryGiveJobInt(pawn, null);
		}
		public override Job TryGiveJobInPartyArea(Pawn pawn, IntVec3 partySpot)
		{
			return this.TryGiveJobInt(pawn, (Thing x) => PartyUtility.InPartyArea(x.Position, partySpot));
		}
		private Job TryGiveJobInt(Pawn pawn, Predicate<Thing> extraValidator)
		{
			List<Thing> thingsOfRequiredThingDefs = base.ThingsOfRequiredThingDefs;
			if (thingsOfRequiredThingDefs.Count == 0)
			{
				return null;
			}
			Predicate<Thing> predicate = (Thing t) => pawn.CanReserve(t, 1) && !t.IsForbidden(pawn) && t.IsSociallyProper(pawn) && (extraValidator == null || extraValidator(t));
			Predicate<Thing> validator = predicate;
			Thing thing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, thingsOfRequiredThingDefs, PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, null);
			if (thing == null)
			{
				return null;
			}
			return new Job(JobDefOf.Ingest, thing)
			{
				maxNumToCarry = Mathf.Min(thing.stackCount, thing.def.ingestible.maxNumToIngestAtOnce)
			};
		}
	}
}
