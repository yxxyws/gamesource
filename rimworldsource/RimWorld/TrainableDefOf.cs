using System;
namespace RimWorld
{
	public static class TrainableDefOf
	{
		public static TrainableDef Obedience;
		public static TrainableDef Release;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<TrainableDef>(typeof(TrainableDefOf));
		}
	}
}
