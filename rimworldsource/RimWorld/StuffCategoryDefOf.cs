using System;
namespace RimWorld
{
	public static class StuffCategoryDefOf
	{
		public static StuffCategoryDef Metallic;
		public static StuffCategoryDef Woody;
		public static StuffCategoryDef Stony;
		public static StuffCategoryDef Fabric;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<StuffCategoryDef>(typeof(StuffCategoryDefOf));
		}
	}
}
