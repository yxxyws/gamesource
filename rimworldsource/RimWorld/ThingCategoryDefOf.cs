using System;
using Verse;
namespace RimWorld
{
	public static class ThingCategoryDefOf
	{
		public static ThingCategoryDef Apparel;
		public static ThingCategoryDef Resources;
		public static ThingCategoryDef Items;
		public static ThingCategoryDef Art;
		public static ThingCategoryDef Weapons;
		public static ThingCategoryDef BodyPartsAndImplants;
		public static ThingCategoryDef Corpses;
		public static ThingCategoryDef Chunks;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<ThingCategoryDef>(typeof(ThingCategoryDefOf));
		}
	}
}
