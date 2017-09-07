using System;
using Verse;
namespace RimWorld.Planet
{
	public static class WorldGenerator
	{
		public static void GenerateWorld()
		{
			Current.World = new World();
			Current.World.info.size = WorldGenerationData.size;
			Current.World.info.seedString = WorldGenerationData.seedString.ToLowerInvariant();
			Rand.Seed = (GenText.StableStringHash(WorldGenerationData.seedString) ^ 4323276);
			Current.World.info.name = NameGenerator.GenerateName(RulePackDefOf.NamerWorld, null);
			WorldGenerator_Grid.GenerateGridIntoCurrentWorld(WorldGenerationData.seedString.ToLowerInvariant());
			FactionGenerator.GenerateFactionsIntoCurrentWorld();
			Current.World.renderer.Notify_WorldChanged();
		}
	}
}
