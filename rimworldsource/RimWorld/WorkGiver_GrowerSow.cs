using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class WorkGiver_GrowerSow : WorkGiver_Grower
	{
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.ClosestTouch;
			}
		}
		protected override bool ExtraRequirements(IPlantToGrowSettable settable)
		{
			if (!settable.CanAcceptSowNow())
			{
				return false;
			}
			Zone_Growing zone_Growing = settable as Zone_Growing;
			IntVec3 c;
			if (zone_Growing != null)
			{
				if (!zone_Growing.allowSow)
				{
					return false;
				}
				c = zone_Growing.Cells[0];
			}
			else
			{
				c = ((Thing)settable).Position;
			}
			base.DetermineWantedPlantDef(c);
			return WorkGiver_Grower.wantedPlantDef != null;
		}
		public override Job JobOnCell(Pawn pawn, IntVec3 c)
		{
			if (c.IsForbidden(pawn))
			{
				return null;
			}
			if (!GenPlant.GrowthSeasonNow(c))
			{
				return null;
			}
			if (WorkGiver_Grower.wantedPlantDef == null)
			{
				base.DetermineWantedPlantDef(c);
				if (WorkGiver_Grower.wantedPlantDef == null)
				{
					return null;
				}
			}
			Plant plant = c.GetPlant();
			if (plant != null)
			{
				if (plant.def == WorkGiver_Grower.wantedPlantDef)
				{
					return null;
				}
				if (plant.def.plant.blockAdjacentSow)
				{
					if (!pawn.CanReserve(plant, 1) || plant.IsForbidden(pawn))
					{
						return null;
					}
					return new Job(JobDefOf.CutPlant, plant);
				}
			}
			Thing thing = GenPlant.AdjacentSowBlocker(WorkGiver_Grower.wantedPlantDef, c);
			if (thing != null)
			{
				Plant plant2 = thing as Plant;
				if (plant2 != null && pawn.CanReserve(plant2, 1) && !plant2.IsForbidden(pawn))
				{
					Zone_Growing zone_Growing = Find.ZoneManager.ZoneAt(plant2.Position) as Zone_Growing;
					if (zone_Growing == null || zone_Growing.GetPlantDefToGrow() != plant2.def)
					{
						return new Job(JobDefOf.CutPlant, plant2);
					}
				}
				return null;
			}
			List<Thing> list = Find.ThingGrid.ThingsListAt(c);
			int i = 0;
			while (i < list.Count)
			{
				Thing thing2 = list[i];
				if (thing2.def == WorkGiver_Grower.wantedPlantDef)
				{
					return null;
				}
				if (thing2.def.BlockPlanting)
				{
					if (!pawn.CanReserve(thing2, 1))
					{
						return null;
					}
					if (thing2.def.category == ThingCategory.Plant)
					{
						if (!thing2.IsForbidden(pawn))
						{
							return new Job(JobDefOf.CutPlant, thing2);
						}
						return null;
					}
					else
					{
						if (thing2.def.EverHaulable)
						{
							return HaulAIUtility.HaulAsideJobFor(pawn, thing2);
						}
						return null;
					}
				}
				else
				{
					i++;
				}
			}
			if (!WorkGiver_Grower.wantedPlantDef.CanEverPlantAt(c) || !GenPlant.GrowthSeasonNow(c) || !pawn.CanReserve(c, 1))
			{
				return null;
			}
			return new Job(JobDefOf.Sow, c)
			{
				plantDefToSow = WorkGiver_Grower.wantedPlantDef
			};
		}
	}
}
