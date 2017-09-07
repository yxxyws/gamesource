using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class RoomStatWorker_ResearchSpeedFactor : RoomStatWorker
	{
		public override float GetScore(Room room)
		{
			float stat = room.GetStat(RoomStatDefOf.Cleanliness);
			float value = GenMath.UnboundedValueToFactor(stat * 0.1f);
			return Mathf.Clamp(value, 0.8f, 1.2f);
		}
	}
}
