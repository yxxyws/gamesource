using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public static class FoodUtility
	{
		public static int WillEatStackCountOf(Pawn eater, ThingDef def)
		{
			int num = Mathf.Min(def.ingestible.maxNumToIngestAtOnce, FoodUtility.StackCountForNutrition(def, eater.needs.food.NutritionWanted));
			if (num < 1)
			{
				num = 1;
			}
			return num;
		}
		public static float GetBodyPartNutrition(Pawn pawn, BodyPartRecord part)
		{
			if (!pawn.RaceProps.IsFlesh)
			{
				return 0f;
			}
			return 5.2f * pawn.BodySize * pawn.health.hediffSet.GetCoverageOfNotMissingNaturalParts(part);
		}
		public static int StackCountForNutrition(ThingDef def, float nutrition)
		{
			if (nutrition <= 0.0001f)
			{
				return 0;
			}
			return Mathf.Max(Mathf.RoundToInt(nutrition / def.ingestible.nutrition), 1);
		}
		public static Thing FoodInInventory(Pawn holder, Pawn eater = null, FoodPreferability minFoodPref = FoodPreferability.NeverForNutrition, FoodPreferability maxFoodPref = FoodPreferability.Lavish, float minStackNutrition = 0f)
		{
			if (holder.inventory == null)
			{
				return null;
			}
			if (eater == null)
			{
				eater = holder;
			}
			ThingContainer container = holder.inventory.container;
			for (int i = 0; i < container.Count; i++)
			{
				Thing thing = container[i];
				if (thing.def.IsNutritionSource && eater.RaceProps.CanEverEat(thing) && thing.def.ingestible.preferability >= minFoodPref && thing.def.ingestible.preferability <= maxFoodPref)
				{
					float num = thing.def.ingestible.nutrition * (float)thing.stackCount;
					if (num >= minStackNutrition && eater.RaceProps.CanEverEat(thing))
					{
						return thing;
					}
				}
			}
			return null;
		}
		public static Thing BestFoodSpawnedFor(Pawn getter, Pawn eater, bool allowPlant = true, FoodPreferability maxFoodPref = FoodPreferability.Lavish, bool allowPleasureDrug = true, bool allowCorpse = true)
		{
			FoodPreferability minPref = eater.def.race.minFoodPreferability;
			if (eater.RaceProps.Humanlike && eater.needs.food.CurCategory < HungerCategory.UrgentlyHungry)
			{
				minPref = FoodPreferability.Awful;
			}
			Predicate<Thing> predicate = (Thing t) => t.def.ingestible.preferability <= maxFoodPref && (allowPleasureDrug || !t.def.ingestible.isPleasureDrug) && (allowCorpse || !(t is Corpse)) && t.def.ingestible.preferability >= minPref && eater.RaceProps.WillAutomaticallyEat(t) && !t.IsForbidden(getter) && t.IngestibleNow && t.IsSociallyProper(getter) && getter.AnimalAwareOf(t) && getter.CanReserve(t, 1);
			ThingRequest thingRequest;
			if ((eater.RaceProps.Eats(FoodTypeFlags.Plant) || eater.RaceProps.Eats(FoodTypeFlags.Tree)) && allowPlant)
			{
				thingRequest = ThingRequest.ForGroup(ThingRequestGroup.Food);
			}
			else
			{
				thingRequest = ThingRequest.ForGroup(ThingRequestGroup.FoodNotPlantOrTree);
			}
			Thing result;
			if (getter.RaceProps.Humanlike)
			{
				Predicate<Thing> validator = predicate;
				result = FoodUtility.FoodSearchInnerScan(getter.Position, Find.ListerThings.ThingsMatching(thingRequest), PathEndMode.ClosestTouch, TraverseParms.For(getter, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator);
			}
			else
			{
				Predicate<Thing> validator = predicate;
				result = GenClosest.ClosestThingReachable(getter.Position, thingRequest, PathEndMode.ClosestTouch, TraverseParms.For(getter, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, null, 30, false);
			}
			return result;
		}
		private static float FoodOptimality(Thing t, float dist)
		{
			float num = dist;
			switch (t.def.ingestible.preferability)
			{
			case FoodPreferability.NeverForNutrition:
				return -9999999f;
			case FoodPreferability.DesperateOnly:
				num += 250f;
				break;
			case FoodPreferability.Raw:
				num += 80f;
				break;
			case FoodPreferability.Awful:
				num += 40f;
				break;
			case FoodPreferability.Simple:
				num = num;
				break;
			case FoodPreferability.Fine:
				num -= 25f;
				break;
			case FoodPreferability.Lavish:
				num -= 40f;
				break;
			default:
				Log.Error("Unknown food taste.");
				break;
			}
			return -num;
		}
		private static Thing FoodSearchInnerScan(IntVec3 root, List<Thing> searchSet, PathEndMode peMode, TraverseParms traverseParams, float maxDistance = 9999f, Predicate<Thing> validator = null)
		{
			if (searchSet == null)
			{
				return null;
			}
			int num = 0;
			int num2 = 0;
			Thing result = null;
			float num3 = -3.40282347E+38f;
			for (int i = 0; i < searchSet.Count; i++)
			{
				Thing thing = searchSet[i];
				num2++;
				float num4 = (float)(root - thing.Position).LengthManhattan;
				if (num4 <= maxDistance)
				{
					float num5 = FoodUtility.FoodOptimality(thing, num4);
					if (num5 >= num3)
					{
						if (root.CanReach(thing, peMode, traverseParams))
						{
							if (thing.Spawned)
							{
								if (validator == null || validator(thing))
								{
									result = thing;
									num3 = num5;
									num++;
								}
							}
						}
					}
				}
			}
			return result;
		}
		public static void DebugDrawFoodSearchInnerScan(IntVec3 root)
		{
			Thing thing = FoodUtility.FoodSearchInnerScan(root, Find.ListerThings.ThingsInGroup(ThingRequestGroup.FoodNotPlantOrTree), PathEndMode.ClosestTouch, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false), 9999f, null);
			if (thing != null)
			{
				GenDraw.DrawLineBetween(root.ToVector3Shifted(), thing.Position.ToVector3Shifted());
			}
		}
		public static Thing BestFoodSourceFor(Pawn getter, Pawn eater, bool fullDispensersOnly, out ThingDef foodDef)
		{
			Thing thing = FoodUtility.BestFoodSpawnedFor(getter, eater, getter == eater, FoodPreferability.Lavish, true, true);
			if (getter == eater && getter.RaceProps.predator && thing == null)
			{
				Pawn pawn = FoodUtility.BestPawnToHuntForPredator(getter);
				if (pawn != null)
				{
					foodDef = pawn.RaceProps.corpseDef;
					return pawn;
				}
			}
			if (getter.RaceProps.ToolUser && (thing == null || thing.def.ingestible.preferability < FoodPreferability.Awful))
			{
				Predicate<Thing> predicate = delegate(Thing t)
				{
					if (t.Faction != getter.Faction && t.Faction != getter.HostFaction)
					{
						return false;
					}
					if (!t.IsSociallyProper(getter))
					{
						return false;
					}
					Building_NutrientPasteDispenser building_NutrientPasteDispenser2 = (Building_NutrientPasteDispenser)t;
					return building_NutrientPasteDispenser2.InteractionCell.Standable() && !t.IsForbidden(getter) && building_NutrientPasteDispenser2.powerComp.PowerOn && (!fullDispensersOnly || building_NutrientPasteDispenser2.HasEnoughFeedstockInHoppers());
				};
				Predicate<Thing> validator = predicate;
				Building_NutrientPasteDispenser building_NutrientPasteDispenser = (Building_NutrientPasteDispenser)GenClosest.ClosestThingReachable(getter.Position, ThingRequest.ForDef(ThingDefOf.NutrientPasteDispenser), PathEndMode.InteractionCell, TraverseParms.For(getter, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, null, -1, false);
				if (building_NutrientPasteDispenser != null)
				{
					foodDef = ThingDefOf.MealNutrientPaste;
					return building_NutrientPasteDispenser;
				}
			}
			foodDef = ((thing == null) ? null : thing.def);
			return thing;
		}
		public static float NutritionAvailableFromFor(Thing t, Pawn p)
		{
			if (t.def.IsNutritionSource && p.RaceProps.WillAutomaticallyEat(t))
			{
				return t.def.ingestible.nutrition * (float)t.stackCount;
			}
			if (p.RaceProps.ToolUser)
			{
				Building_NutrientPasteDispenser building_NutrientPasteDispenser = t as Building_NutrientPasteDispenser;
				if (building_NutrientPasteDispenser != null && building_NutrientPasteDispenser.CanDispenseNow)
				{
					return 99999f;
				}
			}
			return 0f;
		}
		public static void DebugDrawPredatorFoodSource()
		{
			Pawn pawn = Find.Selector.SingleSelectedThing as Pawn;
			if (pawn == null)
			{
				return;
			}
			ThingDef thingDef;
			Thing thing = FoodUtility.BestFoodSourceFor(pawn, pawn, false, out thingDef);
			if (thing != null)
			{
				GenDraw.DrawLineBetween(pawn.Position.ToVector3Shifted(), thing.Position.ToVector3Shifted());
				if (!(thing is Pawn))
				{
					Pawn pawn2 = FoodUtility.BestPawnToHuntForPredator(pawn);
					if (pawn2 != null)
					{
						GenDraw.DrawLineBetween(pawn.Position.ToVector3Shifted(), pawn2.Position.ToVector3Shifted());
					}
				}
			}
		}
		private static Pawn BestPawnToHuntForPredator(Pawn predator)
		{
			if (predator.meleeVerbs.TryGetMeleeVerb() == null)
			{
				return null;
			}
			bool flag = false;
			float summaryHealthPercent = predator.health.summaryHealth.SummaryHealthPercent;
			if (summaryHealthPercent < 0.25f)
			{
				flag = true;
			}
			List<Pawn> allPawnsSpawned = Find.MapPawns.AllPawnsSpawned;
			Pawn pawn = null;
			float num = 0f;
			for (int i = 0; i < allPawnsSpawned.Count; i++)
			{
				Pawn pawn2 = allPawnsSpawned[i];
				if (predator.GetRoom() == pawn2.GetRoom())
				{
					if (predator != pawn2)
					{
						if (!flag || pawn2.Downed)
						{
							if (FoodUtility.AcceptablePreyFor(predator, pawn2))
							{
								if (predator.CanReach(pawn2, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn))
								{
									if (!pawn2.IsForbidden(predator))
									{
										float preyScoreFor = FoodUtility.GetPreyScoreFor(predator, pawn2);
										if (preyScoreFor > num || pawn == null)
										{
											num = preyScoreFor;
											pawn = pawn2;
										}
									}
								}
							}
						}
					}
				}
			}
			return pawn;
		}
		public static bool AcceptablePreyFor(Pawn predator, Pawn prey)
		{
			return prey.RaceProps.IsFlesh && prey.BodySize <= predator.RaceProps.maxPreyBodySize && (prey.Downed || (prey.kindDef.combatPower <= 2f * predator.kindDef.combatPower && prey.kindDef.combatPower * prey.health.summaryHealth.SummaryHealthPercent <= 0.85f * predator.kindDef.combatPower * predator.health.summaryHealth.SummaryHealthPercent)) && (predator.Faction == null || prey.Faction == null || predator.HostileTo(prey)) && (!predator.RaceProps.herdAnimal || predator.def != prey.def);
		}
		public static float GetPreyScoreFor(Pawn predator, Pawn prey)
		{
			float num = prey.kindDef.combatPower / predator.kindDef.combatPower;
			float num2 = prey.health.summaryHealth.SummaryHealthPercent;
			float lengthHorizontal = (predator.Position - prey.Position).LengthHorizontal;
			if (prey.Downed)
			{
				num2 = Mathf.Min(num2, 0.2f);
			}
			float num3 = -lengthHorizontal - 56f * num2 * num2 * num;
			if (prey.RaceProps.Humanlike)
			{
				num3 -= 35f;
			}
			return num3;
		}
		public static bool ShouldBeFedBySomeone(Pawn pawn)
		{
			return FeedPatientUtility.ShouldBeFed(pawn) || WardenFeedUtility.ShouldBeFed(pawn);
		}
	}
}
