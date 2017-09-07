using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JoyGiver_SocialRelax : JoyGiver
	{
		private const float GatherRadius = 3.9f;
		private static List<CompGatherSpot> workingSpots = new List<CompGatherSpot>();
		private static readonly int NumRadiusCells = GenRadial.NumCellsInRadius(3.9f);
		private static readonly List<IntVec3> RadialPatternMiddleOutward = (
			from c in GenRadial.RadialPattern.Take(JoyGiver_SocialRelax.NumRadiusCells)
			orderby Mathf.Abs((c - IntVec3.Zero).LengthHorizontal - 1.95f)
			select c).ToList<IntVec3>();
		public override Job TryGiveJob(Pawn pawn)
		{
			return this.TryGiveJobInt(pawn, null);
		}
		public override Job TryGiveJobInPartyArea(Pawn pawn, IntVec3 partySpot)
		{
			return this.TryGiveJobInt(pawn, (CompGatherSpot x) => PartyUtility.InPartyArea(x.parent.Position, partySpot));
		}
		private Job TryGiveJobInt(Pawn pawn, Predicate<CompGatherSpot> gatherSpotValidator)
		{
			if (GatherSpotLister.activeSpots.Count == 0)
			{
				return null;
			}
			JoyGiver_SocialRelax.workingSpots.Clear();
			for (int i = 0; i < GatherSpotLister.activeSpots.Count; i++)
			{
				JoyGiver_SocialRelax.workingSpots.Add(GatherSpotLister.activeSpots[i]);
			}
			CompGatherSpot compGatherSpot;
			while (JoyGiver_SocialRelax.workingSpots.TryRandomElement(out compGatherSpot))
			{
				JoyGiver_SocialRelax.workingSpots.Remove(compGatherSpot);
				if (!compGatherSpot.parent.IsForbidden(pawn) && pawn.CanReach(compGatherSpot.parent, PathEndMode.Touch, Danger.None, false, TraverseMode.ByPawn) && compGatherSpot.parent.IsSociallyProper(pawn) && (gatherSpotValidator == null || gatherSpotValidator(compGatherSpot)))
				{
					Job job = null;
					if (compGatherSpot.parent.def.surfaceType == SurfaceType.Eat)
					{
						for (int j = 0; j < 30; j++)
						{
							IntVec3 c = compGatherSpot.parent.RandomAdjacentCellCardinal();
							Building edifice = c.GetEdifice();
							if (edifice != null && edifice.def.building.isSittable && pawn.CanReserve(edifice, 1))
							{
								job = new Job(JobDefOf.SocialRelax, compGatherSpot.parent, edifice);
							}
						}
					}
					else
					{
						for (int k = 0; k < JoyGiver_SocialRelax.RadialPatternMiddleOutward.Count; k++)
						{
							IntVec3 c2 = compGatherSpot.parent.Position + JoyGiver_SocialRelax.RadialPatternMiddleOutward[k];
							Building edifice2 = c2.GetEdifice();
							if (edifice2 != null && edifice2.def.building.isSittable && pawn.CanReserve(edifice2, 1) && !edifice2.IsForbidden(pawn) && GenSight.LineOfSight(compGatherSpot.parent.Position, edifice2.Position, true))
							{
								job = new Job(JobDefOf.SocialRelax, compGatherSpot.parent, edifice2);
								break;
							}
						}
						if (job == null)
						{
							for (int l = 0; l < 30; l++)
							{
								IntVec3 intVec = compGatherSpot.parent.Position + GenRadial.RadialPattern[Rand.Range(1, JoyGiver_SocialRelax.NumRadiusCells)];
								if (pawn.CanReserveAndReach(intVec, PathEndMode.OnCell, Danger.None, 1) && intVec.GetEdifice() == null && GenSight.LineOfSight(compGatherSpot.parent.Position, intVec, true))
								{
									job = new Job(JobDefOf.SocialRelax, compGatherSpot.parent, intVec);
								}
							}
						}
					}
					if (job == null)
					{
						return null;
					}
					if (pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) && (pawn.story == null || pawn.story.traits.DegreeOfTrait(TraitDefOf.DrugDesire) >= 0))
					{
						List<Thing> list = Find.ListerThings.ThingsOfDef(ThingDefOf.Beer);
						if (list.Count > 0)
						{
							Predicate<Thing> validator = (Thing t) => pawn.CanReserve(t, 1) && !t.IsForbidden(pawn);
							Thing thing = GenClosest.ClosestThing_Global_Reachable(compGatherSpot.parent.Position, list, PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 40f, validator, null);
							if (thing != null)
							{
								job.targetC = thing;
								job.maxNumToCarry = Mathf.Min(thing.stackCount, thing.def.ingestible.maxNumToIngestAtOnce);
							}
						}
					}
					return job;
				}
			}
			return null;
		}
	}
}
