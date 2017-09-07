using System;
using System.Linq;
using Verse;
namespace RimWorld
{
	public class Alert_BuildARoom : Alert_Tutorial
	{
		public override AlertReport Report
		{
			get
			{
				if (!Prefs.AdaptiveTrainingEnabled)
				{
					return false;
				}
				if (GenDate.DaysPassed > 2)
				{
					return false;
				}
				if (Find.ListerBuildings.allBuildingsColonist.Count >= 4)
				{
					if (Find.ListerBuildings.allBuildingsColonist.Any((Building b) => b.def.thingClass == typeof(Building_Door)))
					{
						return AlertReport.Inactive;
					}
				}
				return AlertReport.Active;
			}
		}
		public Alert_BuildARoom()
		{
			this.baseLabel = "BuildARoom".Translate();
			this.baseExplanation = "BuildARoomDesc".Translate();
		}
	}
}
