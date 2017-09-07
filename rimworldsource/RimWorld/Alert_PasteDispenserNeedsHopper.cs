using System;
using Verse;
namespace RimWorld
{
	public class Alert_PasteDispenserNeedsHopper : Alert_High
	{
		public override AlertReport Report
		{
			get
			{
				foreach (Building_NutrientPasteDispenser current in Find.ListerBuildings.AllBuildingsColonistOfClass<Building_NutrientPasteDispenser>())
				{
					bool flag = false;
					ThingDef thingDef = ThingDef.Named("Hopper");
					foreach (IntVec3 current2 in GenAdj.CellsAdjacentCardinal(current))
					{
						Thing edifice = current2.GetEdifice();
						if (edifice != null && edifice.def == thingDef)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						return AlertReport.CulpritIs(current);
					}
				}
				return AlertReport.Inactive;
			}
		}
		public Alert_PasteDispenserNeedsHopper()
		{
			this.baseLabel = "NeedFoodHopper".Translate();
			this.baseExplanation = "NeedFoodHopperDesc".Translate();
		}
	}
}
