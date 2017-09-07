using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class GlobalControls
	{
		public const float Width = 150f;
		private const int VisibilityControlsPerRow = 5;
		private WidgetRow rowVisibility = new WidgetRow();
		private static readonly int TempSearchNumRadialCells = GenRadial.NumCellsInRadius(2.9f);
		public void GlobalControlsOnGUI()
		{
			float num = (float)Screen.width - 150f;
			float num2 = (float)Screen.height;
			num2 -= 35f;
			GenUI.DrawTextWinterShadow(new Rect((float)(Screen.width - 270), (float)(Screen.height - 450), 270f, 450f));
			num2 -= 4f;
			float y = num2 - TimeControls.TimeButSize.y;
			this.rowVisibility.Init((float)Screen.width, y, UIDirection.LeftThenUp, 141f, 29f);
			Find.PlaySettings.DoPlaySettingsGlobalControls(this.rowVisibility);
			num2 = this.rowVisibility.FinalY;
			num2 -= 4f;
			float y2 = TimeControls.TimeButSize.y;
			Rect timerRect = new Rect(num + 16f, num2 - y2, 150f, y2);
			TimeControls.DoTimeControlsGUI(timerRect);
			num2 -= timerRect.height;
			num2 -= 4f;
			Rect dateRect = new Rect(num, num2 - 48f, 150f, 48f);
			DateReadout.DateOnGUI(dateRect);
			num2 -= dateRect.height;
			Rect rect = new Rect(num - 30f, num2 - 26f, 180f, 26f);
			Find.WeatherManager.DoWeatherGUI(rect);
			num2 -= rect.height;
			Rect rect2 = new Rect(num - 100f, num2 - 26f, 243f, 26f);
			Text.Anchor = TextAnchor.MiddleRight;
			Widgets.Label(rect2, GlobalControls.TemperatureString());
			Text.Anchor = TextAnchor.UpperLeft;
			num2 -= 26f;
			float num3 = 180f;
			float num4 = Find.MapConditionManager.TotalHeightAt(num3 - 15f);
			Rect rect3 = new Rect(num - 30f, num2 - num4, num3, num4);
			Find.MapConditionManager.DoConditionsUI(rect3);
			num2 -= rect3.height;
			if (Prefs.ShowRealtimeClock)
			{
				Rect rect4 = new Rect(num - 20f, num2 - 26f, 163f, 26f);
				Text.Anchor = TextAnchor.MiddleRight;
				Widgets.Label(rect4, DateTime.Now.ToString("HH:mm"));
				Text.Anchor = TextAnchor.UpperLeft;
				num2 -= 26f;
			}
			num2 -= 10f;
			Find.LetterStack.LettersOnGUI(num2);
		}
		private static string TemperatureString()
		{
			IntVec3 intVec = Gen.MouseCell();
			IntVec3 intVec2 = IntVec3.Invalid;
			Room room = null;
			for (int i = 0; i < GlobalControls.TempSearchNumRadialCells; i++)
			{
				intVec2 = intVec + GenRadial.RadialPattern[i];
				if (intVec2.InBounds())
				{
					room = intVec2.GetRoom();
					if (room != null)
					{
						break;
					}
				}
			}
			if (room == null && intVec.InBounds())
			{
				Building edifice = intVec.GetEdifice();
				if (edifice != null)
				{
					CellRect.CellRectIterator iterator = edifice.OccupiedRect().ExpandedBy(1).ClipInsideMap().GetIterator();
					while (!iterator.Done())
					{
						IntVec3 current = iterator.Current;
						room = current.GetRoom();
						if (room != null && !room.PsychologicallyOutdoors)
						{
							intVec2 = current;
							break;
						}
						iterator.MoveNext();
					}
				}
			}
			string str;
			if (intVec2.InBounds() && !intVec2.Fogged() && room != null && !room.PsychologicallyOutdoors)
			{
				if (!room.UsesOutdoorTemperature)
				{
					str = "Indoors".Translate();
				}
				else
				{
					str = "IndoorsUnroofed".Translate();
				}
			}
			else
			{
				str = "Outdoors".Translate();
			}
			float celsiusTemp = (room != null && !intVec2.Fogged()) ? room.Temperature : GenTemperature.OutdoorTemp;
			return str + " " + celsiusTemp.ToStringTemperature("F0");
		}
	}
}
