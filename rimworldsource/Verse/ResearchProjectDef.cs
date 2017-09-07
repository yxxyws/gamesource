using RimWorld;
using System;
using System.Collections.Generic;
namespace Verse
{
	public class ResearchProjectDef : Def
	{
		private string descriptionDiscovered;
		public float totalCost = 100f;
		public List<ResearchProjectDef> prerequisites = new List<ResearchProjectDef>();
		private List<ResearchMod> researchMods = new List<ResearchMod>();
		public ThingDef requiredResearchBuilding;
		public List<ThingDef> requiredResearchFacilities;
		public bool ResearchPrereqsFulfilled
		{
			get
			{
				for (int i = 0; i < this.prerequisites.Count; i++)
				{
					if (!this.prerequisites[i].IsFinished)
					{
						return false;
					}
				}
				return true;
			}
		}
		public bool IsFinished
		{
			get
			{
				return Find.ResearchManager.IsFinished(this);
			}
		}
		public float PercentComplete
		{
			get
			{
				return Find.ResearchManager.PercentComplete(this);
			}
		}
		public float Progress
		{
			get
			{
				return Find.ResearchManager.ProgressOf(this);
			}
		}
		public string ProgressNumbersString
		{
			get
			{
				return this.Progress.ToString("F0") + " / " + this.totalCost.ToString("F0");
			}
		}
		public string DescriptionDiscovered
		{
			get
			{
				if (this.descriptionDiscovered != null)
				{
					return this.descriptionDiscovered;
				}
				return this.description;
			}
		}
		public bool PlayerCanStart
		{
			get
			{
				return !this.IsFinished && this.ResearchPrereqsFulfilled && this.ColonyHasAnyAppropriateResearchBench;
			}
		}
		private bool ColonyHasAnyAppropriateResearchBench
		{
			get
			{
				List<Building> allBuildingsColonist = Find.ListerBuildings.allBuildingsColonist;
				for (int i = 0; i < allBuildingsColonist.Count; i++)
				{
					Building_ResearchBench building_ResearchBench = allBuildingsColonist[i] as Building_ResearchBench;
					if (building_ResearchBench != null && this.CanBeResearchedAt(building_ResearchBench, true))
					{
						return true;
					}
				}
				return false;
			}
		}
		public static ResearchProjectDef Named(string defName)
		{
			return DefDatabase<ResearchProjectDef>.GetNamed(defName, true);
		}
		public void ReapplyAllMods()
		{
			for (int i = 0; i < this.researchMods.Count; i++)
			{
				try
				{
					this.researchMods[i].Apply();
				}
				catch (Exception ex)
				{
					Log.Error(string.Concat(new object[]
					{
						"Exception applying research mod for project ",
						this,
						": ",
						ex.ToString()
					}));
				}
			}
		}
		public bool CanBeResearchedAt(Building_ResearchBench bench, bool ignoreResearchBenchPowerStatus)
		{
			if (this.requiredResearchBuilding != null && bench.def != this.requiredResearchBuilding)
			{
				return false;
			}
			if (!ignoreResearchBenchPowerStatus)
			{
				CompPowerTrader comp = bench.GetComp<CompPowerTrader>();
				if (comp != null && !comp.PowerOn)
				{
					return false;
				}
			}
			if (!this.requiredResearchFacilities.NullOrEmpty<ThingDef>())
			{
				ResearchProjectDef.<CanBeResearchedAt>c__AnonStorey33A <CanBeResearchedAt>c__AnonStorey33A = new ResearchProjectDef.<CanBeResearchedAt>c__AnonStorey33A();
				<CanBeResearchedAt>c__AnonStorey33A.<>f__this = this;
				<CanBeResearchedAt>c__AnonStorey33A.affectedByFacilities = bench.TryGetComp<CompAffectedByFacilities>();
				if (<CanBeResearchedAt>c__AnonStorey33A.affectedByFacilities == null)
				{
					return false;
				}
				List<Thing> linkedFacilitiesListForReading = <CanBeResearchedAt>c__AnonStorey33A.affectedByFacilities.LinkedFacilitiesListForReading;
				int i;
				for (i = 0; i < this.requiredResearchFacilities.Count; i++)
				{
					if (linkedFacilitiesListForReading.Find((Thing x) => x.def == this.requiredResearchFacilities[i] && <CanBeResearchedAt>c__AnonStorey33A.affectedByFacilities.IsFacilityActive(x)) == null)
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
