using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
namespace RimWorld
{
	public class Building_PlantGrower : Building, IPlantToGrowSettable
	{
		private ThingDef plantDefToGrow;
		private CompPowerTrader compPower;
		public IEnumerable<Plant> PlantsOnMe
		{
			get
			{
				Building_PlantGrower.<>c__IteratorE5 <>c__IteratorE = new Building_PlantGrower.<>c__IteratorE5();
				<>c__IteratorE.<>f__this = this;
				Building_PlantGrower.<>c__IteratorE5 expr_0E = <>c__IteratorE;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public override void PostMake()
		{
			base.PostMake();
			this.plantDefToGrow = this.def.building.defaultPlantToGrow;
		}
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			this.compPower = base.GetComp<CompPowerTrader>();
			ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.GrowingFood, KnowledgeAmount.Total);
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.LookDef<ThingDef>(ref this.plantDefToGrow, "plantDefToGrow");
		}
		public override void TickRare()
		{
			if (this.compPower != null && !this.compPower.PowerOn)
			{
				foreach (Plant current in this.PlantsOnMe)
				{
					DamageInfo dinfo = new DamageInfo(DamageDefOf.Rotting, 4, null, null, null);
					current.TakeDamage(dinfo);
				}
			}
		}
		public override void DeSpawn()
		{
			base.DeSpawn();
			if (this.def.building.plantsDestroyWithMe)
			{
				foreach (Plant current in this.PlantsOnMe.ToList<Plant>())
				{
					current.Destroy(DestroyMode.Vanish);
				}
			}
		}
		public override string GetInspectString()
		{
			string text = base.GetInspectString();
			if (GenPlant.GrowthSeasonNow(base.Position))
			{
				text = text + "\n" + "GrowSeasonHereNow".Translate();
			}
			else
			{
				text = text + "\n" + "CannotGrowTooCold".Translate();
			}
			return text;
		}
		public ThingDef GetPlantDefToGrow()
		{
			return this.plantDefToGrow;
		}
		public void SetPlantDefToGrow(ThingDef plantDef)
		{
			this.plantDefToGrow = plantDef;
		}
		public bool CanAcceptSowNow()
		{
			return this.compPower == null || this.compPower.PowerOn;
		}
	}
}
