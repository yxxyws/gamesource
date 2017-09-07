using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class MainTabWindow_History : MainTabWindow
	{
		private const float GraphHeight = 500f;
		private const float LegendLineHeight = 40f;
		private const float TimeLineHeight = 60f;
		private const float ButtonWidth = 110f;
		private const float ButtonHeight = 40f;
		private const float ButtonSpacing = 4f;
		private HistoryAutoRecorderGroup historyAutoRecorderGroup;
		private Vector2 graphSection = default(Vector2);
		private ExternalHistory external;
		public override void PreOpen()
		{
			base.PreOpen();
			this.historyAutoRecorderGroup = Find.History.Groups().FirstOrDefault<HistoryAutoRecorderGroup>();
			if (this.historyAutoRecorderGroup != null)
			{
				this.graphSection = new Vector2(0f, (float)Find.TickManager.TicksGame / 60000f);
			}
		}
		public override void DoWindowContents(Rect fillRect)
		{
			base.DoWindowContents(fillRect);
			Rect rect = new Rect(0f, 0f, fillRect.width, 500f);
			Rect legendRect = new Rect(0f, 500f, fillRect.width / 2f, 40f);
			Rect rect2 = new Rect(0f, legendRect.yMax, fillRect.width, 60f);
			Rect rect3 = new Rect(254f, rect2.yMax, 650f, 50f);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			History history = Find.History;
			if (this.external != null)
			{
				history = this.external.history;
			}
			float num = (float)Find.TickManager.TicksGame / 60000f;
			if (this.external != null)
			{
				num = 0f;
				if (this.historyAutoRecorderGroup != null)
				{
					num = Mathf.Max(this.historyAutoRecorderGroup.GetMaxDay(), Find.TaleManager.GetMaxHistoricalTaleDay());
				}
				Widgets.Label(rect3, new GUIContent(this.external.AllInformation));
			}
			if (this.historyAutoRecorderGroup != null)
			{
				history.DrawGraph(this.historyAutoRecorderGroup, rect, legendRect, this.graphSection);
			}
			if (Widgets.TextButton(new Rect(legendRect.xMin + legendRect.width, legendRect.yMin, 110f, 40f), "Last30Days".Translate(), true, false))
			{
				this.graphSection = new Vector2(Mathf.Max(0f, num - 30f), num);
			}
			if (Widgets.TextButton(new Rect(legendRect.xMin + legendRect.width + 110f + 4f, legendRect.yMin, 110f, 40f), "Last100Days".Translate(), true, false))
			{
				this.graphSection = new Vector2(Mathf.Max(0f, num - 100f), num);
			}
			if (Widgets.TextButton(new Rect(legendRect.xMin + legendRect.width + 228f, legendRect.yMin, 110f, 40f), "Last300Days".Translate(), true, false))
			{
				this.graphSection = new Vector2(Mathf.Max(0f, num - 300f), num);
			}
			if (Widgets.TextButton(new Rect(legendRect.xMin + legendRect.width + 342f, legendRect.yMin, 110f, 40f), "AllDays".Translate(), true, false))
			{
				this.graphSection = new Vector2(0f, num);
			}
			if (Widgets.TextButton(new Rect(0f, rect2.yMax, 110f, 40f), "SelectGraph".Translate(), true, false))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				List<HistoryAutoRecorderGroup> list2 = history.Groups();
				for (int i = 0; i < list2.Count; i++)
				{
					HistoryAutoRecorderGroup groupLocal = list2[i];
					list.Add(new FloatMenuOption(groupLocal.def.LabelCap, delegate
					{
						this.historyAutoRecorderGroup = groupLocal;
					}, MenuOptionPriority.Medium, null, null));
				}
				FloatMenu window = new FloatMenu(list, "SelectGraph".Translate(), false, true);
				Find.WindowStack.Add(window);
			}
		}
	}
}
