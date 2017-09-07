using System;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobGiver_Binge : ThinkNode_JobGiver
	{
		private const int BaseDrugUseInterval = 600;
		private static readonly SimpleCurve DrugUseIntervalFactorCurve_Drunkness = new SimpleCurve
		{
			new CurvePoint(0f, 1f),
			new CurvePoint(1f, 4f)
		};
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			int num = 600;
			Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Alcohol);
			if (firstHediffOfDef != null)
			{
				num = (int)((float)num * JobGiver_Binge.DrugUseIntervalFactorCurve_Drunkness.Evaluate(firstHediffOfDef.Severity));
			}
			int num2 = Find.TickManager.TicksGame - pawn.mindState.lastPleasureDrugUseTick;
			if (num2 > num)
			{
				Job job = JobGiver_Binge.DrinkAlcoholJob(pawn);
				if (job != null)
				{
					return job;
				}
			}
			return null;
		}
		private static Job DrinkAlcoholJob(Pawn pawn)
		{
			bool ignoreForbid = pawn.MentalStateDef != null;
			Predicate<Thing> predicate = (Thing t) => (ignoreForbid || !t.IsForbidden(pawn)) && pawn.CanReserve(t, 1);
			Predicate<Thing> validator = predicate;
			Thing thing = GenClosest.ClosestThingReachable(pawn.Position, ThingRequest.ForDef(ThingDefOf.Beer), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, null, -1, false);
			if (thing == null)
			{
				return null;
			}
			return new Job(JobDefOf.Ingest, thing)
			{
				maxNumToCarry = Mathf.Min(thing.stackCount, thing.def.ingestible.maxNumToIngestAtOnce),
				ignoreForbidden = ignoreForbid
			};
		}
	}
}
