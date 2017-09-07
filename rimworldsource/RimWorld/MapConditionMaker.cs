using System;
using Verse;
namespace RimWorld
{
	public static class MapConditionMaker
	{
		public static MapCondition MakeCondition(MapConditionDef def, int duration = -1)
		{
			MapCondition mapCondition = (MapCondition)Activator.CreateInstance(def.conditionClass);
			mapCondition.startTick = Find.TickManager.TicksGame;
			mapCondition.def = def;
			mapCondition.duration = duration;
			return mapCondition;
		}
	}
}
