using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Designator_ZoneAddStockpile_Dumping : Designator_ZoneAddStockpile
	{
		public Designator_ZoneAddStockpile_Dumping()
		{
			this.preset = StorageSettingsPreset.DumpingStockpile;
			this.defaultLabel = this.preset.PresetName();
			this.defaultDesc = "DesignatorZoneCreateStorageDumpingDesc".Translate();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.icon = ContentFinder<Texture2D>.Get("UI/Designators/ZoneCreate_Stockpile", true);
			});
			this.hotKey = KeyBindingDefOf.Misc3;
		}
		protected override void FinalizeDesignationSucceeded()
		{
			base.FinalizeDesignationSucceeded();
			ConceptDecider.TeachOpportunity(ConceptDefOf.StorageTab, OpportunityType.GoodToKnow);
		}
	}
}
