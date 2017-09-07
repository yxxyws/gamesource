using System;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class Toils_Refuel
	{
		public static Toil PickupFuel(TargetIndex ind, Thing refuelable)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.actor;
				Job curJob = actor.CurJob;
				Pawn_CarryTracker carrier = actor.carrier;
				Thing thing = curJob.GetTarget(ind).Thing;
				int fuelCountToFullyRefuel = refuelable.TryGetComp<CompRefuelable>().GetFuelCountToFullyRefuel();
				curJob.maxNumToCarry = fuelCountToFullyRefuel;
				int count = Mathf.Min(thing.stackCount, fuelCountToFullyRefuel);
				carrier.TryStartCarry(thing, count);
				if (thing.Spawned)
				{
					Find.Reservations.Release(thing, actor);
				}
				curJob.SetTarget(ind, carrier.CarriedThing);
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			return toil;
		}
		public static Toil FinalizeRefueling(TargetIndex refuelableInd, TargetIndex fuelInd)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.actor;
				Job curJob = actor.CurJob;
				Thing thing = curJob.GetTarget(refuelableInd).Thing;
				Thing thing2 = curJob.GetTarget(fuelInd).Thing;
				thing.TryGetComp<CompRefuelable>().Refuel(thing2);
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			return toil;
		}
	}
}
