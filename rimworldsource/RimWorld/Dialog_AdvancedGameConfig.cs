using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Dialog_AdvancedGameConfig : Window
	{
		private const float ColumnWidth = 200f;
		private IntVec2 selCoords = IntVec2.Invalid;
		private static readonly int[] MapSizes = new int[]
		{
			200,
			225,
			250,
			275,
			300,
			325,
			350,
			400
		};
		public override Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2(700f, 500f);
			}
		}
		public Dialog_AdvancedGameConfig(IntVec2 selCoords)
		{
			this.doCloseButton = true;
			this.closeOnEscapeKey = true;
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.selCoords = selCoords;
		}
		public override void DoWindowContents(Rect inRect)
		{
			Listing_Standard listing_Standard = new Listing_Standard(inRect.AtZero());
			listing_Standard.DoLabel("MapSize".Translate());
			int[] mapSizes = Dialog_AdvancedGameConfig.MapSizes;
			for (int i = 0; i < mapSizes.Length; i++)
			{
				int num = mapSizes[i];
				if (num == 200)
				{
					listing_Standard.DoLabel("MapSizeSmall".Translate());
				}
				else
				{
					if (num == 250)
					{
						listing_Standard.DoLabel("MapSizeMedium".Translate());
					}
					else
					{
						if (num == 300)
						{
							listing_Standard.DoLabel("MapSizeLarge".Translate());
						}
						else
						{
							if (num == 350)
							{
								listing_Standard.DoLabel("MapSizeExtreme".Translate());
							}
						}
					}
				}
				string label = "MapSizeDesc".Translate(new object[]
				{
					num,
					num * num
				});
				if (listing_Standard.DoRadioButton(label, MapInitData.mapSize == num, 0f))
				{
					MapInitData.mapSize = num;
				}
			}
			listing_Standard.NewColumn();
			GenUI.SetLabelAlign(TextAnchor.MiddleCenter);
			listing_Standard.DoLabel("MapStartSeason".Translate());
			string label2;
			if (MapInitData.startingMonth == Month.Undefined)
			{
				label2 = "MapStartSeasonDefault".Translate();
			}
			else
			{
				label2 = MapInitData.startingMonth.GetSeason().LabelCap();
			}
			Rect rect = listing_Standard.GetRect(32f);
			GridLayout gridLayout = new GridLayout(rect, 5, 1, 0f, 4f);
			if (Widgets.TextButton(gridLayout.GetCellRectByIndex(0, 1, 1), "-", true, false))
			{
				Season season = MapInitData.startingMonth.GetSeason();
				if (season == Season.Undefined)
				{
					season = Season.Winter;
				}
				else
				{
					season -= 1;
				}
				MapInitData.startingMonth = season.GetFirstMonth();
			}
			Widgets.Label(gridLayout.GetCellRectByIndex(1, 3, 1), label2);
			if (Widgets.TextButton(gridLayout.GetCellRectByIndex(4, 1, 1), "+", true, false))
			{
				Season season2 = MapInitData.startingMonth.GetSeason();
				if (season2 == Season.Winter)
				{
					season2 = Season.Undefined;
				}
				else
				{
					season2 += 1;
				}
				MapInitData.startingMonth = season2.GetFirstMonth();
			}
			GenUI.ResetLabelAlign();
			listing_Standard.DoGap(12f);
			listing_Standard.DoLabelCheckbox("PermadeathMode".Translate(), ref MapInitData.permadeathMode, "PermadeathModeInfo".Translate());
			if (this.selCoords.IsValid && MapInitData.startingMonth != Month.Undefined && GenTemperature.AverageTemperatureAtWorldCoordsForMonth(this.selCoords, MapInitData.startingMonth) < 3f)
			{
				listing_Standard.DoLabel("MapTemperatureDangerWarning".Translate());
			}
			if (MapInitData.mapSize > 250)
			{
				listing_Standard.DoLabel("MapSizePerformanceWarning".Translate());
			}
			listing_Standard.End();
		}
	}
}
