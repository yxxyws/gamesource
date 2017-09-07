using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;
namespace RimWorld
{
	public class Zone_Growing : Zone, IPlantToGrowSettable
	{
		private ThingDef plantDefToGrow = DefDatabase<ThingDef>.GetNamed("PlantPotato", true);
		public bool allowSow = true;
		private static readonly ITab GrowTab = new ITab_Growing();
		public Zone_Growing() : base("GrowingZone".Translate())
		{
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.LookDef<ThingDef>(ref this.plantDefToGrow, "plantDefToGrow");
			Scribe_Values.LookValue<bool>(ref this.allowSow, "allowSow", true, false);
		}
		[DebuggerHidden]
		public override IEnumerable<ITab> GetInspectionTabs()
		{
			Zone_Growing.<GetInspectionTabs>c__Iterator85 <GetInspectionTabs>c__Iterator = new Zone_Growing.<GetInspectionTabs>c__Iterator85();
			Zone_Growing.<GetInspectionTabs>c__Iterator85 expr_07 = <GetInspectionTabs>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}
		public override string GetInspectString()
		{
			string text = string.Empty;
			if (!base.Cells.NullOrEmpty<IntVec3>())
			{
				IntVec3 c = base.Cells.First<IntVec3>();
				if (c.UsesOutdoorTemperature())
				{
					text += Zone_Growing.GrowingMonthsDescription(Find.Map.WorldCoords);
				}
				if (GenPlant.GrowthSeasonNow(c))
				{
					text = text + "\n" + "GrowSeasonHereNow".Translate();
				}
				else
				{
					text = text + "\n" + "CannotGrowTooCold".Translate();
				}
			}
			return text;
		}
		public static string GrowingMonthsDescription(IntVec2 worldCoords)
		{
			List<Month> list = GenTemperature.MonthsInTemperatureRange(worldCoords, 10f, 42f);
			string text;
			if (list.NullOrEmpty<Month>())
			{
				text = "NoGrowingPeriod".Translate();
			}
			else
			{
				if (list.Count == 12)
				{
					text = "GrowYearRound".Translate();
				}
				else
				{
					text = SeasonUtility.SeasonsRangeLabel(list);
				}
			}
			return "OutdoorGrowingPeriod".Translate(new object[]
			{
				text
			});
		}
		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			Zone_Growing.<GetGizmos>c__Iterator86 <GetGizmos>c__Iterator = new Zone_Growing.<GetGizmos>c__Iterator86();
			<GetGizmos>c__Iterator.<>f__this = this;
			Zone_Growing.<GetGizmos>c__Iterator86 expr_0E = <GetGizmos>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
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
			return true;
		}
	}
}
