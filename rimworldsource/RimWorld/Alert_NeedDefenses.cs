using System;
using System.Linq;
using Verse;
namespace RimWorld
{
	public class Alert_NeedDefenses : Alert_Tutorial
	{
		public override AlertReport Report
		{
			get
			{
				if (!Prefs.AdaptiveTrainingEnabled)
				{
					return false;
				}
				if (GenDate.DaysPassed < 2)
				{
					return false;
				}
				if (GenDate.DaysPassed >= 8)
				{
					return false;
				}
				if (Find.ListerBuildings.allBuildingsColonist.Any((Building b) => b.def.building != null && b.def.building.IsTurret))
				{
					return false;
				}
				return true;
			}
		}
		public Alert_NeedDefenses()
		{
			this.baseLabel = "NeedDefenses".Translate();
			this.baseExplanation = "NeedDefensesDesc".Translate();
		}
	}
}
