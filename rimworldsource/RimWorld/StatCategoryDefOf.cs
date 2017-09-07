using System;
namespace RimWorld
{
	public static class StatCategoryDefOf
	{
		public static StatCategoryDef Basics;
		public static StatCategoryDef BasicsNonPawn;
		public static StatCategoryDef BasicsPawn;
		public static StatCategoryDef Apparel;
		public static StatCategoryDef Weapon;
		public static StatCategoryDef Building;
		public static StatCategoryDef PawnWork;
		public static StatCategoryDef PawnCombat;
		public static StatCategoryDef PawnSocial;
		public static StatCategoryDef PawnMisc;
		public static StatCategoryDef EquippedStatOffsets;
		public static StatCategoryDef StuffStatFactors;
		public static StatCategoryDef StuffStatOffsets;
		public static StatCategoryDef StuffOfEquipmentStatFactors;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<StatCategoryDef>(typeof(StatCategoryDefOf));
		}
	}
}
