using System;
using Verse;
namespace RimWorld
{
	public sealed class PlaySettings : IExposable
	{
		public bool showZones = true;
		public bool showBeauty;
		public bool showRoomStats;
		public bool autoHomeArea = true;
		public bool showTameAnimalNames;
		public bool useWorkPriorities;
		public void ExposeData()
		{
			Scribe_Values.LookValue<bool>(ref this.showZones, "showZones", false, false);
			Scribe_Values.LookValue<bool>(ref this.showBeauty, "showBeauty", false, false);
			Scribe_Values.LookValue<bool>(ref this.showRoomStats, "showRoomStats", false, false);
			Scribe_Values.LookValue<bool>(ref this.autoHomeArea, "autoHomeArea", false, false);
			Scribe_Values.LookValue<bool>(ref this.showTameAnimalNames, "showTameAnimalNames", false, false);
			Scribe_Values.LookValue<bool>(ref this.useWorkPriorities, "useWorkPriorities", false, false);
		}
		public void DoPlaySettingsGlobalControls(WidgetRow row)
		{
			row.DoIconToggle(ref this.showZones, TexButton.ShowZones, "ZoneVisibilityToggleButton".Translate(), SoundDefOf.MouseoverButtonToggle, null);
			row.DoIconToggle(ref this.showBeauty, TexButton.ShowBeauty, "ShowBeautyToggleButton".Translate(), SoundDefOf.MouseoverButtonToggle, null);
			row.DoIconToggle(ref this.showRoomStats, TexButton.ShowRoomStats, "ShowRoomStatsToggleButton".Translate(), SoundDefOf.MouseoverButtonToggle, "InspectRoomStats");
			row.DoIconToggle(ref this.autoHomeArea, TexButton.AutoHomeArea, "AutoHomeAreaToggleButton".Translate(), SoundDefOf.MouseoverButtonToggle, null);
			bool resourceReadoutCategorized = Prefs.ResourceReadoutCategorized;
			bool flag = resourceReadoutCategorized;
			row.DoIconToggle(ref resourceReadoutCategorized, TexButton.CategorizedResourceReadout, "CategorizedResourceReadoutToggleButton".Translate(), SoundDefOf.MouseoverButtonToggle, null);
			if (resourceReadoutCategorized != flag)
			{
				Prefs.ResourceReadoutCategorized = resourceReadoutCategorized;
			}
			row.DoIconToggle(ref this.showTameAnimalNames, TexButton.ShowTameAnimalNames, "ShowAnimalNamesToggleButton".Translate(), SoundDefOf.MouseoverButtonToggle, null);
		}
	}
}
