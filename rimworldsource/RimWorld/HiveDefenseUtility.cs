using System;
using Verse;
namespace RimWorld
{
	public static class HiveDefenseUtility
	{
		public const float DistToHiveToStopAttacking_NotAngry = 17f;
		public const float DistToHiveToStopAttacking_Angry = 32f;
		public static bool ShouldStopAttacking(Pawn hiveDefender, Thing target)
		{
			if (hiveDefender.CurJob != null && !hiveDefender.CurJob.stopIfTooFarFromHive)
			{
				return false;
			}
			Hive hive = hiveDefender.mindState.hive;
			if (hive == null)
			{
				return false;
			}
			float maxDist;
			if (hive.AngryNow)
			{
				maxDist = 32f;
			}
			else
			{
				maxDist = 17f;
			}
			return !target.Position.InHorDistOf(hive.Position, maxDist);
		}
	}
}
