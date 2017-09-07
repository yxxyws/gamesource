using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
namespace RimWorld
{
	public class Genstep_Plants : Genstep
	{
		private const float PlantMinGrowth = 0.07f;
		private const float PlantGrowthFactor = 1.2f;
		private static Dictionary<ThingDef, int> numExtant = new Dictionary<ThingDef, int>();
		private static int totalExtant = 0;
		private static Dictionary<ThingDef, float> desiredProportions = new Dictionary<ThingDef, float>();
		private static float PlantChoiceWeight(ThingDef def)
		{
			float num = Find.Map.Biome.CommonalityOfPlant(def);
			if (Genstep_Plants.totalExtant > 100)
			{
				float num2 = (float)Genstep_Plants.numExtant[def] / (float)Genstep_Plants.totalExtant;
				if (num2 < Genstep_Plants.desiredProportions[def] * 0.8f)
				{
					num *= 4f;
				}
			}
			return num / def.plant.wildClusterSizeRange.Average;
		}
		private static void RecordAdded(ThingDef plantDef)
		{
			Genstep_Plants.totalExtant++;
			Dictionary<ThingDef, int> dictionary;
			Dictionary<ThingDef, int> expr_11 = dictionary = Genstep_Plants.numExtant;
			int num = dictionary[plantDef];
			expr_11[plantDef] = num + 1;
		}
		public override void Generate()
		{
			RegionAndRoomUpdater.Enabled = false;
			List<ThingDef> list = Find.Map.Biome.AllWildPlants.ToList<ThingDef>();
			for (int i = 0; i < list.Count; i++)
			{
				Genstep_Plants.numExtant.Add(list[i], 0);
			}
			Genstep_Plants.desiredProportions = GenPlant.CalculateDesiredPlantProportions(Find.Map.Biome);
			float plantDensity = Find.Map.Biome.plantDensity;
			foreach (IntVec3 c in Find.Map.AllCells.InRandomOrder(null))
			{
				if (c.GetEdifice() == null && c.GetCover() == null)
				{
					float num = Find.FertilityGrid.FertilityAt(c);
					float num2 = num * plantDensity;
					if (Rand.Value < num2)
					{
						IEnumerable<ThingDef> source = 
							from def in list
							where def.CanEverPlantAt(c)
							select def;
						if (source.Any<ThingDef>())
						{
							ThingDef thingDef = source.RandomElementByWeight(new Func<ThingDef, float>(Genstep_Plants.PlantChoiceWeight));
							int randomInRange = thingDef.plant.wildClusterSizeRange.RandomInRange;
							for (int j = 0; j < randomInRange; j++)
							{
								IntVec3 c2;
								if (j == 0)
								{
									c2 = c;
								}
								else
								{
									if (!GenPlantReproduction.TryFindSeedTargFor(thingDef, c, SeedTargFindMode.Cluster, out c2))
									{
										break;
									}
								}
								Plant plant = (Plant)ThingMaker.MakeThing(thingDef, null);
								plant.growth = Rand.Range(0.07f, 1f);
								if (plant.def.plant.LimitedLifespan)
								{
									plant.age = Rand.Range(0, plant.def.plant.LifespanTicks - 50);
								}
								GenSpawn.Spawn(plant, c2);
								Genstep_Plants.RecordAdded(thingDef);
							}
						}
					}
				}
			}
			Genstep_Plants.numExtant.Clear();
			Genstep_Plants.desiredProportions.Clear();
			Genstep_Plants.totalExtant = 0;
			RegionAndRoomUpdater.Enabled = true;
		}
	}
}
