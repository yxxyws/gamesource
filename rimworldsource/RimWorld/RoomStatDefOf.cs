using System;
using Verse;
namespace RimWorld
{
	public static class RoomStatDefOf
	{
		public static RoomStatDef Cleanliness;
		public static RoomStatDef Wealth;
		public static RoomStatDef Space;
		public static RoomStatDef Beauty;
		public static RoomStatDef Impressiveness;
		public static RoomStatDef InfectionChanceFactor;
		public static RoomStatDef ResearchSpeedFactor;
		public static RoomStatDef GraveVisitingJoyGainFactor;
		public static RoomStatDef FoodPoisonChanceFactor;
		public static RoomStatDef SurgerySuccessChanceFactor;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<RoomStatDef>(typeof(RoomStatDefOf));
		}
	}
}
