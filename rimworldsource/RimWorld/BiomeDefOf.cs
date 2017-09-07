using System;
namespace RimWorld
{
	public static class BiomeDefOf
	{
		public static BiomeDef Ocean;
		public static BiomeDef IceSheet;
		public static BiomeDef Tundra;
		public static BiomeDef BorealForest;
		public static BiomeDef TemperateForest;
		public static BiomeDef TropicalRainforest;
		public static BiomeDef Desert;
		public static BiomeDef AridShrubland;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<BiomeDef>(typeof(BiomeDefOf));
		}
	}
}
