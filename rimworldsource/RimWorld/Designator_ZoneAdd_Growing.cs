using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Designator_ZoneAdd_Growing : Designator_ZoneAdd
	{
		protected override string NewZoneLabel
		{
			get
			{
				return "GrowingZone".Translate();
			}
		}
		public Designator_ZoneAdd_Growing()
		{
			this.zoneTypeToPlace = typeof(Zone_Growing);
			this.defaultLabel = "GrowingZone".Translate();
			this.defaultDesc = "DesignatorGrowingZoneDesc".Translate();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.icon = ContentFinder<Texture2D>.Get("UI/Designators/ZoneCreate_Growing", true);
			});
			this.hotKey = KeyBindingDefOf.Misc2;
			this.tutorHighlightTag = "DesignatorZoneCreateGrowing";
		}
		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			if (!base.CanDesignateCell(c).Accepted)
			{
				return false;
			}
			if (Find.FertilityGrid.FertilityAt(c) < ThingDefOf.PlantPotato.plant.fertilityMin)
			{
				return false;
			}
			return true;
		}
		protected override Zone MakeNewZone()
		{
			ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.GrowingFood, KnowledgeAmount.Total);
			return new Zone_Growing();
		}
	}
}
