using System;
using Verse;
namespace RimWorld
{
	public static class TerrainDefOf
	{
		public static TerrainDef Sand;
		public static TerrainDef Soil;
		public static TerrainDef Underwall;
		public static TerrainDef Concrete;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<TerrainDef>(typeof(TerrainDefOf));
		}
	}
}
