using System;
using Verse;
namespace RimWorld
{
	public class Genstep_Animals : Genstep
	{
		public override void Generate()
		{
			while (!WildSpawner.AnimalEcosystemFull)
			{
				IntVec3 loc = RCellFinder.RandomAnimalSpawnCell_MapGen();
				WildSpawner.SpawnRandomWildAnimalAt(loc);
			}
		}
	}
}
