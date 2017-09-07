using System;
using Verse;
using Verse.AI;
using Verse.AI.Group;
namespace RimWorld
{
	public class JobGiver_GetRest : ThinkNode_JobGiver
	{
		public override float GetPriority(Pawn pawn)
		{
			Need_Rest rest = pawn.needs.rest;
			if (rest == null)
			{
				return 0f;
			}
			if (Find.TickManager.TicksGame < pawn.mindState.canSleepTick)
			{
				return 0f;
			}
			Lord lord = pawn.GetLord();
			if (lord != null && !lord.CurLordToil.AllowSatisfyLongNeeds)
			{
				return 0f;
			}
			TimeAssignmentDef timeAssignmentDef;
			if (pawn.RaceProps.Humanlike)
			{
				timeAssignmentDef = ((pawn.timetable != null) ? pawn.timetable.CurrentAssignment : TimeAssignmentDefOf.Anything);
			}
			else
			{
				int hourOfDay = GenDate.HourOfDay;
				if (hourOfDay < 7 || hourOfDay > 21)
				{
					timeAssignmentDef = TimeAssignmentDefOf.Sleep;
				}
				else
				{
					timeAssignmentDef = TimeAssignmentDefOf.Anything;
				}
			}
			float curLevel = rest.CurLevel;
			if (timeAssignmentDef == TimeAssignmentDefOf.Anything)
			{
				if (curLevel < 0.3f)
				{
					return 8f;
				}
				return 0f;
			}
			else
			{
				if (timeAssignmentDef == TimeAssignmentDefOf.Work)
				{
					return 0f;
				}
				if (timeAssignmentDef == TimeAssignmentDefOf.Joy)
				{
					if (curLevel < 0.3f)
					{
						return 8f;
					}
					return 0f;
				}
				else
				{
					if (timeAssignmentDef != TimeAssignmentDefOf.Sleep)
					{
						throw new NotImplementedException();
					}
					if (curLevel < 0.75f)
					{
						return 8f;
					}
					return 0f;
				}
			}
		}
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			if (RestUtility.DisturbancePreventsLyingDown(pawn))
			{
				return null;
			}
			Building_Bed building_Bed = RestUtility.FindBedFor(pawn);
			if (building_Bed != null)
			{
				return new Job(JobDefOf.LayDown, building_Bed);
			}
			IntVec3 vec;
			if (pawn.playerSettings != null && pawn.playerSettings.AreaRestriction != null)
			{
				vec = CellFinder.RandomClosewalkCellNear(pawn.Position, 4, pawn.playerSettings.AreaRestriction);
			}
			else
			{
				vec = CellFinder.RandomClosewalkCellNear(pawn.Position, 4);
			}
			return new Job(JobDefOf.LayDown, vec);
		}
	}
}
