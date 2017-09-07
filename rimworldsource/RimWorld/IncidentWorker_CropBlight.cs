using System;
using System.Collections.Generic;
using Verse;
namespace RimWorld
{
	public class IncidentWorker_CropBlight : IncidentWorker
	{
		private const float MaxDaysToGrown = 16f;
		public override bool TryExecute(IncidentParms parms)
		{
			List<Thing> list = Find.Map.listerThings.ThingsInGroup(ThingRequestGroup.CultivatedPlant);
			bool flag = false;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				Plant plant = (Plant)list[i];
				if (plant.def.plant.growDays <= 16f)
				{
					if (plant.LifeStage == PlantLifeStage.Growing || plant.LifeStage == PlantLifeStage.Mature)
					{
						plant.CropBlighted();
						flag = true;
					}
				}
			}
			if (!flag)
			{
				return false;
			}
			Find.LetterStack.ReceiveLetter("LetterLabelCropBlight".Translate(), "CropBlight".Translate(), LetterType.BadNonUrgent, null);
			return true;
		}
	}
}
