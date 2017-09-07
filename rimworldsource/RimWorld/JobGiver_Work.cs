using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobGiver_Work : ThinkNode_JobGiver
	{
		public bool emergency;
		public override ThinkNode DeepCopy()
		{
			JobGiver_Work jobGiver_Work = (JobGiver_Work)base.DeepCopy();
			jobGiver_Work.emergency = this.emergency;
			return jobGiver_Work;
		}
		public override float GetPriority(Pawn pawn)
		{
			if (pawn.workSettings == null || !pawn.workSettings.EverWork)
			{
				return 0f;
			}
			TimeAssignmentDef timeAssignmentDef = (pawn.timetable != null) ? pawn.timetable.CurrentAssignment : TimeAssignmentDefOf.Anything;
			if (timeAssignmentDef == TimeAssignmentDefOf.Anything)
			{
				return 5.5f;
			}
			if (timeAssignmentDef == TimeAssignmentDefOf.Work)
			{
				return 9f;
			}
			if (timeAssignmentDef == TimeAssignmentDefOf.Sleep)
			{
				return 2f;
			}
			if (timeAssignmentDef == TimeAssignmentDefOf.Joy)
			{
				return 2f;
			}
			throw new NotImplementedException();
		}
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			List<WorkGiver> list = this.emergency ? pawn.workSettings.WorkGiversInOrderEmergency : pawn.workSettings.WorkGiversInOrderNormal;
			int num = -999;
			TargetInfo targetInfo = TargetInfo.Invalid;
			WorkGiver_Scanner workGiver_Scanner = null;
			for (int i = 0; i < list.Count; i++)
			{
				WorkGiver workGiver = list[i];
				if (workGiver.def.priorityInType != num && targetInfo.IsValid)
				{
					break;
				}
				if (workGiver.def.canBeDoneByNonColonists || pawn.IsColonist)
				{
					if (workGiver.MissingRequiredCapacity(pawn) == null)
					{
						if (!workGiver.ShouldSkip(pawn))
						{
							try
							{
								Job job = workGiver.NonScanJob(pawn);
								if (job != null)
								{
									Job result = job;
									return result;
								}
								WorkGiver_Scanner scanner = workGiver as WorkGiver_Scanner;
								if (scanner != null)
								{
									if (workGiver.def.scanThings)
									{
										Predicate<Thing> predicate = (Thing t) => !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t);
										IEnumerable<Thing> enumerable = scanner.PotentialWorkThingsGlobal(pawn);
										Thing thing;
										if (scanner.Prioritized)
										{
											IEnumerable<Thing> enumerable2 = enumerable;
											if (enumerable2 == null)
											{
												enumerable2 = Find.ListerThings.ThingsMatching(scanner.PotentialWorkThingRequest);
											}
											Predicate<Thing> validator = predicate;
											thing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, enumerable2, scanner.PathEndMode, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, (Thing x) => scanner.GetPriority(pawn, x));
										}
										else
										{
											Predicate<Thing> validator = predicate;
											thing = GenClosest.ClosestThingReachable(pawn.Position, scanner.PotentialWorkThingRequest, scanner.PathEndMode, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, enumerable, scanner.LocalRegionsToScanFirst, enumerable != null);
										}
										if (thing != null)
										{
											targetInfo = thing;
											workGiver_Scanner = scanner;
										}
									}
									if (workGiver.def.scanCells)
									{
										IntVec3 position = pawn.Position;
										float num2 = 99999f;
										foreach (IntVec3 current in scanner.PotentialWorkCellsGlobal(pawn))
										{
											float lengthHorizontalSquared = (current - position).LengthHorizontalSquared;
											if (lengthHorizontalSquared < num2 && !current.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, current))
											{
												targetInfo = current;
												workGiver_Scanner = scanner;
												num2 = lengthHorizontalSquared;
											}
										}
									}
									num = workGiver.def.priorityInType;
								}
							}
							catch (Exception ex)
							{
								Log.Error(string.Concat(new object[]
								{
									pawn,
									" threw exception in WorkGiver ",
									workGiver.def.defName,
									": ",
									ex.ToString()
								}));
							}
							finally
							{
							}
							if (!targetInfo.IsValid)
							{
								goto IL_3D4;
							}
							pawn.mindState.lastGivenWorkType = workGiver.def.workType;
							if (targetInfo.HasThing)
							{
								return workGiver_Scanner.JobOnThing(pawn, targetInfo.Thing);
							}
							return workGiver_Scanner.JobOnCell(pawn, targetInfo.Cell);
						}
					}
				}
				IL_3D4:;
			}
			return null;
		}
		public void TryStartPrioritizedWorkOn(Pawn pawn, Job job, WorkTypeDef wt)
		{
			if (!pawn.IsColonistPlayerControlled || !pawn.drafter.CanTakeOrderedJob())
			{
				return;
			}
			job.playerForced = true;
			pawn.mindState.lastGivenWorkType = wt;
			pawn.QueueJob(job);
			pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
		}
	}
}
