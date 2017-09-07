using System;
using Verse;
namespace RimWorld
{
	public static class MentalStateDefOf
	{
		public static MentalStateDef Berserk;
		public static MentalStateDef DazedWander;
		public static MentalStateDef BingingAlcohol;
		public static MentalStateDef PanicFlee;
		public static MentalStateDef Manhunter;
		public static MentalStateDef ManhunterPermanent;
		public static MentalStateDef SocialFighting;
		public static MentalStateDef GiveUpExit;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<MentalStateDef>(typeof(MentalStateDefOf));
		}
	}
}
