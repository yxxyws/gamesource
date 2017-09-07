using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public abstract class WorkGiver_ConstructDeliverResources : WorkGiver_Scanner
	{
		private const float NearbyConstructScanRadius = 8f;
		private static List<Thing> resourcesAvailable = new List<Thing>();
		private static string MissingMaterialsTranslated = null;
		public WorkGiver_ConstructDeliverResources()
		{
			if (WorkGiver_ConstructDeliverResources.MissingMaterialsTranslated == null)
			{
				WorkGiver_ConstructDeliverResources.MissingMaterialsTranslated = "MissingMaterials".Translate();
			}
		}
		private static bool ResourceValidator(Pawn pawn, ThingCount need, Thing th)
		{
			return th.def == need.thingDef && !th.IsForbidden(pawn) && pawn.CanReserve(th, 1);
		}
		protected Job ResourceDeliverJobFor(Pawn pawn, IConstructible c)
		{
			WorkGiver_ConstructDeliverResources.<ResourceDeliverJobFor>c__AnonStorey1FA <ResourceDeliverJobFor>c__AnonStorey1FA = new WorkGiver_ConstructDeliverResources.<ResourceDeliverJobFor>c__AnonStorey1FA();
			<ResourceDeliverJobFor>c__AnonStorey1FA.pawn = pawn;
			Blueprint_Install blueprint_Install = c as Blueprint_Install;
			if (blueprint_Install == null)
			{
				bool flag = false;
				List<ThingCount> list = c.MaterialsNeeded();
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					WorkGiver_ConstructDeliverResources.<ResourceDeliverJobFor>c__AnonStorey1FB <ResourceDeliverJobFor>c__AnonStorey1FB = new WorkGiver_ConstructDeliverResources.<ResourceDeliverJobFor>c__AnonStorey1FB();
					<ResourceDeliverJobFor>c__AnonStorey1FB.<>f__ref$506 = <ResourceDeliverJobFor>c__AnonStorey1FA;
					<ResourceDeliverJobFor>c__AnonStorey1FB.need = list[i];
					if (!ItemAvailabilityUtility.ThingsAvailableAnywhere(<ResourceDeliverJobFor>c__AnonStorey1FB.need, <ResourceDeliverJobFor>c__AnonStorey1FA.pawn))
					{
						flag = true;
						break;
					}
					WorkGiver_ConstructDeliverResources.<ResourceDeliverJobFor>c__AnonStorey1FB arg_130_0 = <ResourceDeliverJobFor>c__AnonStorey1FB;
					Predicate<Thing> validator = (Thing r) => WorkGiver_ConstructDeliverResources.ResourceValidator(<ResourceDeliverJobFor>c__AnonStorey1FB.<>f__ref$506.pawn, <ResourceDeliverJobFor>c__AnonStorey1FB.need, r);
					arg_130_0.foundRes = GenClosest.ClosestThingReachable(<ResourceDeliverJobFor>c__AnonStorey1FA.pawn.Position, ThingRequest.ForGroup(ThingRequestGroup.HaulableAlways), PathEndMode.ClosestTouch, TraverseParms.For(<ResourceDeliverJobFor>c__AnonStorey1FA.pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, null, -1, false);
					if (<ResourceDeliverJobFor>c__AnonStorey1FB.foundRes != null)
					{
						int num = 0;
						WorkGiver_ConstructDeliverResources.resourcesAvailable.Clear();
						WorkGiver_ConstructDeliverResources.resourcesAvailable.Add(<ResourceDeliverJobFor>c__AnonStorey1FB.foundRes);
						num += <ResourceDeliverJobFor>c__AnonStorey1FB.foundRes.stackCount;
						foreach (Thing current in GenRadial.RadialDistinctThingsAround(<ResourceDeliverJobFor>c__AnonStorey1FB.foundRes.Position, 5f, false))
						{
							if (num >= <ResourceDeliverJobFor>c__AnonStorey1FB.foundRes.def.stackLimit)
							{
								break;
							}
							if (current.def == <ResourceDeliverJobFor>c__AnonStorey1FB.foundRes.def)
							{
								if (GenAI.CanUseItemForWork(<ResourceDeliverJobFor>c__AnonStorey1FA.pawn, current))
								{
									WorkGiver_ConstructDeliverResources.resourcesAvailable.Add(current);
									num += current.stackCount;
								}
							}
						}
						int num2 = <ResourceDeliverJobFor>c__AnonStorey1FB.need.count;
						HashSet<Thing> hashSet = new HashSet<Thing>();
						foreach (Thing current2 in GenRadial.RadialDistinctThingsAround(((Thing)c).Position, 8f, false))
						{
							if (num2 >= num)
							{
								break;
							}
							IConstructible constructible = current2 as IConstructible;
							if (constructible != null)
							{
								if (constructible != c)
								{
									if (!(constructible is Blueprint_Install))
									{
										if (current2.Faction == <ResourceDeliverJobFor>c__AnonStorey1FA.pawn.Faction)
										{
											if (!current2.IsForbidden(<ResourceDeliverJobFor>c__AnonStorey1FA.pawn))
											{
												if (current2 != c && !hashSet.Contains(current2))
												{
													if (GenConstruct.CanConstruct(current2, <ResourceDeliverJobFor>c__AnonStorey1FA.pawn))
													{
														int num3 = GenConstruct.AmountNeededByOf(constructible, <ResourceDeliverJobFor>c__AnonStorey1FB.foundRes.def);
														if (num3 != 0)
														{
															hashSet.Add(current2);
															num2 += num3;
														}
													}
												}
											}
										}
									}
								}
							}
						}
						hashSet.Add((Thing)c);
						Thing thing = hashSet.MinBy((Thing nee) => Gen.ManhattanDistanceFlat(<ResourceDeliverJobFor>c__AnonStorey1FB.foundRes.Position, nee.Position));
						hashSet.Remove(thing);
						int num4 = 0;
						int j = 0;
						do
						{
							num4 += WorkGiver_ConstructDeliverResources.resourcesAvailable[j].stackCount;
							j++;
						}
						while (num4 < num2 && j < WorkGiver_ConstructDeliverResources.resourcesAvailable.Count);
						WorkGiver_ConstructDeliverResources.resourcesAvailable.RemoveRange(j, WorkGiver_ConstructDeliverResources.resourcesAvailable.Count - j);
						WorkGiver_ConstructDeliverResources.resourcesAvailable.Remove(<ResourceDeliverJobFor>c__AnonStorey1FB.foundRes);
						Job job = new Job(JobDefOf.HaulToContainer);
						job.targetA = <ResourceDeliverJobFor>c__AnonStorey1FB.foundRes;
						job.targetQueueA = new List<TargetInfo>();
						for (j = 0; j < WorkGiver_ConstructDeliverResources.resourcesAvailable.Count; j++)
						{
							job.targetQueueA.Add(WorkGiver_ConstructDeliverResources.resourcesAvailable[j]);
						}
						job.targetB = thing;
						if (hashSet.Count > 0)
						{
							job.targetQueueB = new List<TargetInfo>();
							foreach (Thing current3 in hashSet)
							{
								job.targetQueueB.Add(current3);
							}
						}
						job.maxNumToCarry = num2;
						job.haulMode = HaulMode.ToContainer;
						return job;
					}
				}
				if (flag)
				{
					JobFailReason.Is(WorkGiver_ConstructDeliverResources.MissingMaterialsTranslated);
				}
				return null;
			}
			if (blueprint_Install.MiniToInstallOrBuildingToReinstall.IsForbidden(<ResourceDeliverJobFor>c__AnonStorey1FA.pawn) || !<ResourceDeliverJobFor>c__AnonStorey1FA.pawn.CanReserveAndReach(blueprint_Install.MiniToInstallOrBuildingToReinstall, PathEndMode.OnCell, <ResourceDeliverJobFor>c__AnonStorey1FA.pawn.NormalMaxDanger(), 1))
			{
				return null;
			}
			return new Job(JobDefOf.HaulToContainer)
			{
				targetA = blueprint_Install.MiniToInstallOrBuildingToReinstall,
				targetB = blueprint_Install,
				maxNumToCarry = 1,
				haulMode = HaulMode.ToContainer
			};
		}
	}
}
