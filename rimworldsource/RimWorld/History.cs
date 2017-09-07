using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public sealed class History : IExposable
	{
		private List<HistoryAutoRecorderGroup> autoRecorderGroups;
		private SimpleCurveDrawerStyle curveDrawerStyle;
		private static List<CurveMark> marks = new List<CurveMark>();
		public History()
		{
			this.autoRecorderGroups = new List<HistoryAutoRecorderGroup>();
			foreach (HistoryAutoRecorderGroupDef current in DefDatabase<HistoryAutoRecorderGroupDef>.AllDefs)
			{
				HistoryAutoRecorderGroup historyAutoRecorderGroup = new HistoryAutoRecorderGroup();
				historyAutoRecorderGroup.def = current;
				historyAutoRecorderGroup.CreateRecorders();
				this.autoRecorderGroups.Add(historyAutoRecorderGroup);
			}
			this.curveDrawerStyle = new SimpleCurveDrawerStyle();
			this.curveDrawerStyle.DrawMeasure = true;
			this.curveDrawerStyle.DrawPoints = false;
			this.curveDrawerStyle.DrawBackground = true;
			this.curveDrawerStyle.DrawBackgroundLines = false;
			this.curveDrawerStyle.DrawLegend = true;
			this.curveDrawerStyle.DrawCurveMousePoint = true;
			this.curveDrawerStyle.OnlyPositiveValues = true;
			this.curveDrawerStyle.UseFixedSection = true;
			this.curveDrawerStyle.UseAntiAliasedLines = true;
			this.curveDrawerStyle.PointsRemoveOptimization = true;
			this.curveDrawerStyle.MeasureLabelsXCount = 10;
			this.curveDrawerStyle.MeasureLabelsYCount = 5;
			this.curveDrawerStyle.LabelX = "Day".Translate();
			this.curveDrawerStyle.LabelY = "Value".Translate();
		}
		public void HistoryTick()
		{
			for (int i = 0; i < this.autoRecorderGroups.Count; i++)
			{
				this.autoRecorderGroups[i].Tick();
			}
			HistoryUpload.Tick();
		}
		public List<HistoryAutoRecorderGroup> Groups()
		{
			return this.autoRecorderGroups;
		}
		public void DrawGraph(HistoryAutoRecorderGroup group, Rect rect, Rect legendRect, Vector2 section)
		{
			float num = rect.yMin;
			Text.Font = GameFont.Medium;
			Text.Anchor = TextAnchor.UpperCenter;
			Widgets.Label(new Rect(rect.xMin, num, rect.width, 30f), new GUIContent(group.def.LabelCap));
			num += 50f;
			Text.Font = GameFont.Small;
			History.marks.Clear();
			foreach (Tale current in Find.TaleManager.AllTales)
			{
				if (current.def.type == TaleType.PermanentHistorical)
				{
					float x = (float)GenDate.TickAbsToGame(current.date) / 60000f;
					History.marks.Add(new CurveMark(x, current.ShortSummary, current.def.historyGraphColor));
				}
			}
			rect.yMin += num;
			rect.height -= num;
			group.DrawGraph(rect, legendRect, section, this.curveDrawerStyle, History.marks);
		}
		public void ExposeData()
		{
			Scribe_Collections.LookList<HistoryAutoRecorderGroup>(ref this.autoRecorderGroups, "autoRecorderGroups", LookMode.Deep, new object[0]);
		}
	}
}
