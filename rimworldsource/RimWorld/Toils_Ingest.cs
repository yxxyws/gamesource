using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class Toils_Ingest
	{
		private static List<IntVec3> spotSearchList = new List<IntVec3>();
		private static List<IntVec3> cardinals = GenAdj.CardinalDirections.ToList<IntVec3>();
		private static List<IntVec3> diagonals = GenAdj.DiagonalDirections.ToList<IntVec3>();
		public static Toil TakeMealFromDispenser(TargetIndex ind, Pawn eater)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				Building_NutrientPasteDispenser building_NutrientPasteDispenser = (Building_NutrientPasteDispenser)curJob.GetTarget(ind).Thing;
				Thing thing = (Meal)building_NutrientPasteDispenser.TryDispenseFood();
				if (thing == null)
				{
					actor.jobs.curDriver.EndJobWith(JobCondition.Incompletable);
					return;
				}
				actor.carrier.TryStartCarry(thing);
				actor.jobs.curJob.targetA = actor.carrier.CarriedThing;
			};
			toil.defaultCompleteMode = ToilCompleteMode.Delay;
			toil.defaultDuration = Building_NutrientPasteDispenser.CollectDuration;
			return toil;
		}
		public static Toil PickupIngestible(TargetIndex ind, Pawn eater)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				Thing thing = curJob.GetTarget(ind).Thing;
				if (curJob.maxNumToCarry <= 0)
				{
					Log.Error("Tried to do PickupIngestible toil with job.maxNumToCarry = " + curJob.maxNumToCarry);
					actor.jobs.EndCurrentJob(JobCondition.Errored);
					return;
				}
				int count = Mathf.Min(thing.stackCount, curJob.maxNumToCarry);
				actor.carrier.TryStartCarry(thing, count);
				if (thing != actor.carrier.CarriedThing && Find.Reservations.FirstReserverOf(thing, actor.Faction) == actor)
				{
					Find.Reservations.Release(thing, actor);
				}
				actor.jobs.curJob.targetA = actor.carrier.CarriedThing;
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			return toil;
		}
		public static Toil CarryIngestibleToChewSpot(Pawn pawn)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.actor;
				Predicate<Thing> validator = delegate(Thing t)
				{
					if (t.def.building == null || !t.def.building.isSittable)
					{
						return false;
					}
					if (t.IsForbidden(pawn))
					{
						return false;
					}
					if (!actor.CanReserve(t, 1))
					{
						return false;
					}
					if (!t.IsSociallyProper(actor))
					{
						return false;
					}
					if (t.IsBurning())
					{
						return false;
					}
					bool result = false;
					for (int i = 0; i < 4; i++)
					{
						IntVec3 c = t.Position + GenAdj.CardinalDirections[i];
						Building edifice = c.GetEdifice();
						if (edifice != null && edifice.def.surfaceType == SurfaceType.Eat)
						{
							result = true;
							break;
						}
					}
					return result;
				};
				Thing thing = GenClosest.ClosestThingReachable(actor.Position, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.OnCell, TraverseParms.For(actor, Danger.Deadly, TraverseMode.ByPawn, false), 25f, validator, null, -1, false);
				IntVec3 intVec;
				if (thing != null)
				{
					intVec = thing.Position;
					Find.Reservations.Reserve(actor, thing, 1);
				}
				else
				{
					intVec = RCellFinder.SpotToChewStandingNear(actor, actor.CurJob.targetA.Thing);
				}
				Find.PawnDestinationManager.ReserveDestinationFor(actor, intVec);
				actor.pather.StartPath(intVec, PathEndMode.OnCell);
			};
			toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
			return toil;
		}
		public static bool TryFindAdjacentIngestionPlaceSpot(IntVec3 root, ThingDef ingestibleDef, Pawn pawn, out IntVec3 placeSpot)
		{
			placeSpot = IntVec3.Invalid;
			for (int i = 0; i < 4; i++)
			{
				IntVec3 intVec = root + GenAdj.CardinalDirections[i];
				if (intVec.HasEatSurface())
				{
					if (!(
						from t in Find.ThingGrid.ThingsAt(intVec)
						where t.def == ingestibleDef
						select t).Any<Thing>())
					{
						if (!intVec.IsForbidden(pawn))
						{
							placeSpot = intVec;
							return true;
						}
					}
				}
			}
			if (!placeSpot.IsValid)
			{
				Toils_Ingest.spotSearchList.Clear();
				Toils_Ingest.cardinals.Shuffle<IntVec3>();
				for (int j = 0; j < 4; j++)
				{
					Toils_Ingest.spotSearchList.Add(Toils_Ingest.cardinals[j]);
				}
				Toils_Ingest.diagonals.Shuffle<IntVec3>();
				for (int k = 0; k < 4; k++)
				{
					Toils_Ingest.spotSearchList.Add(Toils_Ingest.diagonals[k]);
				}
				Toils_Ingest.spotSearchList.Add(IntVec3.Zero);
				for (int l = 0; l < Toils_Ingest.spotSearchList.Count; l++)
				{
					IntVec3 intVec2 = root + Toils_Ingest.spotSearchList[l];
					if (intVec2.Walkable() && !intVec2.IsForbidden(pawn))
					{
						if (!(
							from t in Find.ThingGrid.ThingsAt(intVec2)
							where t.def == ingestibleDef
							select t).Any<Thing>())
						{
							placeSpot = intVec2;
							return true;
						}
					}
				}
			}
			return false;
		}
		public static Toil FindAdjacentEatSurface(TargetIndex eatSurfaceInd, TargetIndex foodInd)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				IntVec3 position = toil.actor.Position;
				for (int i = 0; i < 4; i++)
				{
					Rot4 rot = new Rot4(i);
					IntVec3 intVec = position + rot.FacingCell;
					if (intVec.HasEatSurface())
					{
						toil.actor.CurJob.SetTarget(eatSurfaceInd, intVec);
						toil.actor.jobs.curDriver.rotateToFace = eatSurfaceInd;
						Thing thing = toil.actor.CurJob.GetTarget(foodInd).Thing;
						if (thing.def.rotatable)
						{
							thing.Rotation = Rot4.FromIntVec3(intVec - toil.actor.Position);
						}
						return;
					}
				}
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			return toil;
		}
		public static Toil ChewIngestible(Pawn chewer, float durationMultiplier, TargetIndex foodInd, TargetIndex eatSurfaceInd = TargetIndex.None)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.actor;
				Thing thing = actor.CurJob.GetTarget(foodInd).Thing;
				actor.Drawer.rotator.FaceCell(chewer.Position);
				if (!thing.IngestibleNow)
				{
					chewer.jobs.EndCurrentJob(JobCondition.Incompletable);
					return;
				}
				actor.jobs.curDriver.ticksLeftThisToil = Mathf.RoundToInt(500f * durationMultiplier);
			};
			toil.tickAction = delegate
			{
				toil.actor.GainComfortFromCellIfPossible();
			};
			toil.WithProgressBarToilDelay(foodInd, 500, false, -0.5f);
			toil.defaultCompleteMode = ToilCompleteMode.Delay;
			toil.FailOnDestroyedOrNull(foodInd);
			toil.WithEffect(() => toil.actor.CurJob.GetTarget(foodInd).Thing.def.ingestible.eatEffect, delegate
			{
				Thing thing = toil.actor.CurJob.GetTarget(foodInd).Thing;
				if (chewer != toil.actor)
				{
					return chewer;
				}
				if (eatSurfaceInd != TargetIndex.None && toil.actor.CurJob.GetTarget(eatSurfaceInd).IsValid)
				{
					return toil.actor.CurJob.GetTarget(eatSurfaceInd);
				}
				return thing;
			});
			toil.WithSustainer(delegate
			{
				if (!chewer.RaceProps.Humanlike)
				{
					return null;
				}
				return toil.actor.CurJob.GetTarget(foodInd).Thing.def.ingestible.soundEat;
			});
			return toil;
		}
		public static Toil FinalizeIngest(Pawn ingester, TargetIndex ingestibleInd)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.actor;
				Thing thing = actor.jobs.curJob.GetTarget(ingestibleInd).Thing;
				if (ingester.needs.mood != null)
				{
					if (!(ingester.Position + ingester.Rotation.FacingCell).HasEatSurface() && ingester.GetPosture() == PawnPosture.Standing)
					{
						ingester.needs.mood.thoughts.TryGainThought(ThoughtDefOf.AteWithoutTable);
					}
					Room room = ingester.GetRoom();
					if (room != null && room.Role == RoomRoleDefOf.DiningRoom)
					{
						int scoreStageIndex = RoomStatDefOf.Impressiveness.GetScoreStageIndex(room.GetStat(RoomStatDefOf.Impressiveness));
						if (ThoughtDefOf.AteInImpressiveDiningRoom.stages[scoreStageIndex] != null)
						{
							ingester.needs.mood.thoughts.TryGainThought(ThoughtMaker.MakeThought(ThoughtDefOf.AteInImpressiveDiningRoom, scoreStageIndex));
						}
					}
				}
				float nutritionWanted = ingester.needs.food.NutritionWanted;
				thing.Ingested(ingester, nutritionWanted);
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			return toil;
		}
	}
}
