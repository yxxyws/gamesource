using System;
namespace RimWorld
{
	public static class FactionDefOf
	{
		public static FactionDef Colony;
		public static FactionDef Spacer;
		public static FactionDef SpacerHostile;
		public static FactionDef Outlander;
		public static FactionDef Pirate;
		public static FactionDef Tribe;
		public static FactionDef Mechanoid;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<FactionDef>(typeof(FactionDefOf));
		}
	}
}
