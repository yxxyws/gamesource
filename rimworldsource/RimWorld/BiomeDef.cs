using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class BiomeDef : Def
	{
		public Type workerClass;
		public bool implemented = true;
		public bool canBuildBase = true;
		public float animalDensity;
		public float plantDensity;
		public List<WeatherCommonalityRecord> baseWeatherCommonalities = new List<WeatherCommonalityRecord>();
		public List<TerrainThreshold> terrainsByFertility = new List<TerrainThreshold>();
		public List<SoundDef> soundsAmbient = new List<SoundDef>();
		public List<TerrainPatchMaker> terrainPatchMakers = new List<TerrainPatchMaker>();
		private List<BiomePlantRecord> wildPlants = new List<BiomePlantRecord>();
		private List<BiomeAnimalRecord> wildAnimals = new List<BiomeAnimalRecord>();
		private List<BiomeDiseaseRecord> diseases = new List<BiomeDiseaseRecord>();
		public Texture2D baseTexture = BaseContent.WhiteTex;
		public Color baseColor = Color.red;
		public bool hideTerrain;
		[Unsaved]
		private Dictionary<PawnKindDef, float> cachedAnimalCommonalities;
		[Unsaved]
		private Dictionary<ThingDef, float> cachedPlantCommonalities;
		[Unsaved]
		private Dictionary<IncidentDef, float> cachedDiseaseCommonalities;
		[Unsaved]
		private BiomeWorker workerInt;
		public BiomeWorker Worker
		{
			get
			{
				if (this.workerInt == null)
				{
					this.workerInt = (BiomeWorker)Activator.CreateInstance(this.workerClass);
				}
				return this.workerInt;
			}
		}
		public Material DrawMaterial
		{
			get
			{
				return MaterialPool.MatFrom(new MaterialRequest
				{
					shader = ShaderDatabase.Transparent,
					mainTex = this.baseTexture,
					color = this.baseColor
				});
			}
		}
		public IEnumerable<ThingDef> AllWildPlants
		{
			get
			{
				BiomeDef.<>c__Iterator6F <>c__Iterator6F = new BiomeDef.<>c__Iterator6F();
				<>c__Iterator6F.<>f__this = this;
				BiomeDef.<>c__Iterator6F expr_0E = <>c__Iterator6F;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public IEnumerable<PawnKindDef> AllWildAnimals
		{
			get
			{
				BiomeDef.<>c__Iterator70 <>c__Iterator = new BiomeDef.<>c__Iterator70();
				<>c__Iterator.<>f__this = this;
				BiomeDef.<>c__Iterator70 expr_0E = <>c__Iterator;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public float CommonalityOfAnimal(PawnKindDef animalDef)
		{
			if (this.cachedAnimalCommonalities == null)
			{
				this.cachedAnimalCommonalities = new Dictionary<PawnKindDef, float>();
				for (int i = 0; i < this.wildAnimals.Count; i++)
				{
					this.cachedAnimalCommonalities.Add(this.wildAnimals[i].animal, this.wildAnimals[i].commonality);
				}
				foreach (PawnKindDef current in DefDatabase<PawnKindDef>.AllDefs)
				{
					if (current.RaceProps.wildBiomes != null)
					{
						for (int j = 0; j < current.RaceProps.wildBiomes.Count; j++)
						{
							this.cachedAnimalCommonalities.Add(current.RaceProps.wildBiomes[j].animal, current.RaceProps.wildBiomes[j].commonality);
						}
					}
				}
			}
			float result;
			if (this.cachedAnimalCommonalities.TryGetValue(animalDef, out result))
			{
				return result;
			}
			return 0f;
		}
		public float CommonalityOfPlant(ThingDef plantDef)
		{
			if (this.cachedPlantCommonalities == null)
			{
				this.cachedPlantCommonalities = new Dictionary<ThingDef, float>();
				for (int i = 0; i < this.wildPlants.Count; i++)
				{
					this.cachedPlantCommonalities.Add(this.wildPlants[i].plant, this.wildPlants[i].commonality);
				}
				foreach (ThingDef current in DefDatabase<ThingDef>.AllDefs)
				{
					if (current.plant != null && current.plant.wildBiomes != null)
					{
						for (int j = 0; j < current.plant.wildBiomes.Count; j++)
						{
							this.cachedPlantCommonalities.Add(current.plant.wildBiomes[j].plant, current.plant.wildBiomes[j].commonality);
						}
					}
				}
			}
			float result;
			if (this.cachedPlantCommonalities.TryGetValue(plantDef, out result))
			{
				return result;
			}
			return 0f;
		}
		public float MTBDaysOfDisease(IncidentDef diseaseInc)
		{
			if (this.cachedDiseaseCommonalities == null)
			{
				this.cachedDiseaseCommonalities = new Dictionary<IncidentDef, float>();
				for (int i = 0; i < this.diseases.Count; i++)
				{
					this.cachedDiseaseCommonalities.Add(this.diseases[i].diseaseInc, this.diseases[i].mtbDays);
				}
				foreach (IncidentDef current in DefDatabase<IncidentDef>.AllDefs)
				{
					if (current.diseaseBiomeRecords != null)
					{
						for (int j = 0; j < current.diseaseBiomeRecords.Count; j++)
						{
							this.cachedDiseaseCommonalities.Add(current.diseaseBiomeRecords[j].diseaseInc, current.diseaseBiomeRecords[j].mtbDays);
						}
					}
				}
			}
			float result;
			if (this.cachedDiseaseCommonalities.TryGetValue(diseaseInc, out result))
			{
				return result;
			}
			return 9999999f;
		}
		public static BiomeDef Named(string defName)
		{
			return DefDatabase<BiomeDef>.GetNamed(defName, true);
		}
	}
}
