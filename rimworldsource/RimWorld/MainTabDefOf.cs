using System;
namespace RimWorld
{
	public static class MainTabDefOf
	{
		public static MainTabDef Inspect;
		public static MainTabDef Architect;
		public static MainTabDef Research;
		public static MainTabDef Menu;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<MainTabDef>(typeof(MainTabDefOf));
		}
	}
}
