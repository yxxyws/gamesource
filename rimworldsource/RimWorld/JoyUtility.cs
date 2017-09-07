using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public static class JoyUtility
	{
		public const float BaseJoyGainPerHour = 0.36f;
		public static bool EnjoyableOutsideNow()
		{
			return Find.WeatherManager.RainRate < 0.25f && Find.MapConditionManager.AllowEnjoyableOutsideNow();
		}
		public static bool EnjoyableOutsideNow(Pawn pawn)
		{
			return JoyUtility.EnjoyableOutsideNow() && pawn.ComfortableTemperatureRange().Includes(GenTemperature.OutdoorTemp);
		}
		public static void JoyTickCheckEnd(Pawn pawn, JoyTickFullJoyAction fullJoyAction = JoyTickFullJoyAction.EndJob, float extraJoyGainFactor = 1f)
		{
			Job curJob = pawn.CurJob;
			if (curJob.def.joyKind == null)
			{
				Log.Warning("This method can only be called for jobs with joyKind.");
				return;
			}
			pawn.needs.joy.GainJoy(extraJoyGainFactor * curJob.def.joyGainRate * 0.000144f, curJob.def.joyKind);
			if (curJob.def.joySkill != null)
			{
				pawn.skills.GetSkill(curJob.def.joySkill).Learn(curJob.def.joyXpPerTick);
			}
			if (!curJob.ignoreJoyTimeAssignment && !pawn.GetTimeAssignment().allowJoy)
			{
				pawn.jobs.curDriver.EndJobWith(JobCondition.InterruptForced);
			}
			if (pawn.needs.joy.CurLevel > 0.9999f)
			{
				if (fullJoyAction == JoyTickFullJoyAction.EndJob)
				{
					pawn.jobs.curDriver.EndJobWith(JobCondition.Succeeded);
				}
				else
				{
					if (fullJoyAction == JoyTickFullJoyAction.GoToNextToil)
					{
						pawn.jobs.curDriver.ReadyForNextToil();
					}
				}
			}
		}
		public static void TryGainRecRoomThought(Pawn pawn)
		{
			Room room = pawn.GetRoom();
			if (room != null && room.Role == RoomRoleDefOf.RecRoom)
			{
				int scoreStageIndex = RoomStatDefOf.Impressiveness.GetScoreStageIndex(room.GetStat(RoomStatDefOf.Impressiveness));
				if (ThoughtDefOf.AteInImpressiveDiningRoom.stages[scoreStageIndex] != null)
				{
					pawn.needs.mood.thoughts.TryGainThought(ThoughtMaker.MakeThought(ThoughtDefOf.JoyActivityInImpressiveRecRoom, scoreStageIndex));
				}
			}
		}
	}
}
