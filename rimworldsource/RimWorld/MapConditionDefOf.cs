using System;
using Verse;
namespace RimWorld
{
	public static class MapConditionDefOf
	{
		public static MapConditionDef SolarFlare;
		public static MapConditionDef Eclipse;
		public static MapConditionDef PsychicDrone;
		public static MapConditionDef PsychicSoothe;
		public static MapConditionDef HeatWave;
		public static MapConditionDef ColdSnap;
		public static MapConditionDef Flashstorm;
		public static MapConditionDef VolcanicWinter;
		public static MapConditionDef ToxicFallout;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<MapConditionDef>(typeof(MapConditionDefOf));
		}
	}
}
