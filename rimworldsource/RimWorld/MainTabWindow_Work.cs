using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class MainTabWindow_Work : MainTabWindow_PawnList
	{
		private const float TopAreaHeight = 40f;
		protected const float LabelRowHeight = 50f;
		private float workColumnSpacing = -1f;
		private static List<WorkTypeDef> VisibleWorkTypeDefsInPriorityOrder;
		public override Vector2 RequestedTabSize
		{
			get
			{
				return new Vector2(1010f, 90f + (float)base.PawnsCount * 30f + 65f);
			}
		}
		public override void PreOpen()
		{
			base.PreOpen();
			MainTabWindow_Work.Reinit();
		}
		public static void Reinit()
		{
			MainTabWindow_Work.VisibleWorkTypeDefsInPriorityOrder = (
				from def in WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder
				where def.visible
				select def).ToList<WorkTypeDef>();
		}
		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			Rect position = new Rect(0f, 0f, rect.width, 40f);
			GUI.BeginGroup(position);
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
			Rect rect2 = new Rect(5f, 5f, 140f, 30f);
			bool useWorkPriorities = Find.Map.playSettings.useWorkPriorities;
			Widgets.LabelCheckbox(rect2, "ManualPriorities".Translate(), ref Find.Map.playSettings.useWorkPriorities, false);
			if (useWorkPriorities != Find.Map.playSettings.useWorkPriorities)
			{
				foreach (Pawn current in Find.MapPawns.FreeColonists)
				{
					current.workSettings.Notify_UseWorkPrioritiesChanged();
				}
			}
			float num = position.width / 3f;
			float num2 = position.width * 2f / 3f;
			Rect rect3 = new Rect(num - 50f, 5f, 160f, 30f);
			Rect rect4 = new Rect(num2 - 50f, 5f, 160f, 30f);
			GUI.color = new Color(1f, 1f, 1f, 0.5f);
			Text.Anchor = TextAnchor.UpperCenter;
			Text.Font = GameFont.Tiny;
			Widgets.Label(rect3, "<= " + "HigherPriority".Translate());
			Widgets.Label(rect4, "LowerPriority".Translate() + " =>");
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.EndGroup();
			Rect position2 = new Rect(0f, 40f, rect.width, rect.height - 40f);
			GUI.BeginGroup(position2);
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			Rect outRect = new Rect(0f, 50f, position2.width, position2.height - 50f);
			this.workColumnSpacing = (position2.width - 16f - 175f) / (float)MainTabWindow_Work.VisibleWorkTypeDefsInPriorityOrder.Count;
			float num3 = 175f;
			int num4 = 0;
			foreach (WorkTypeDef current2 in MainTabWindow_Work.VisibleWorkTypeDefsInPriorityOrder)
			{
				Vector2 vector = Text.CalcSize(current2.labelShort);
				float num5 = num3 + 15f;
				Rect rect5 = new Rect(num5 - vector.x / 2f, 0f, vector.x, vector.y);
				if (num4 % 2 == 1)
				{
					rect5.y += 20f;
				}
				if (Mouse.IsOver(rect5))
				{
					Widgets.DrawHighlight(rect5);
				}
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.Label(rect5, current2.labelShort);
				WorkTypeDef localDef = current2;
				TooltipHandler.TipRegion(rect5, new TipSignal(() => string.Concat(new string[]
				{
					localDef.gerundLabel,
					"\n\n",
					localDef.description,
					"\n\n",
					MainTabWindow_Work.SpecificWorkListString(localDef)
				}), localDef.GetHashCode()));
				GUI.color = new Color(1f, 1f, 1f, 0.3f);
				Widgets.DrawLineVertical(num5, rect5.yMax - 3f, 50f - rect5.yMax + 3f);
				Widgets.DrawLineVertical(num5 + 1f, rect5.yMax - 3f, 50f - rect5.yMax + 3f);
				GUI.color = Color.white;
				num3 += this.workColumnSpacing;
				num4++;
			}
			base.DrawRows(outRect);
			GUI.EndGroup();
		}
		private static string SpecificWorkListString(WorkTypeDef def)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < def.workGiversByPriority.Count; i++)
			{
				stringBuilder.Append(def.workGiversByPriority[i].LabelCap);
				if (def.workGiversByPriority[i].emergency)
				{
					stringBuilder.Append(" (" + "EmergencyWorkMarker".Translate() + ")");
				}
				if (i < def.workGiversByPriority.Count - 1)
				{
					stringBuilder.AppendLine();
				}
			}
			return stringBuilder.ToString();
		}
		protected override void DrawPawnRow(Rect rect, Pawn p)
		{
			float num = 175f;
			Text.Font = GameFont.Medium;
			for (int i = 0; i < MainTabWindow_Work.VisibleWorkTypeDefsInPriorityOrder.Count; i++)
			{
				WorkTypeDef workTypeDef = MainTabWindow_Work.VisibleWorkTypeDefsInPriorityOrder[i];
				Vector2 topLeft = new Vector2(num, rect.y + 2.5f);
				bool incapableBecauseOfCapacities = this.IsIncapableOfWholeWorkType(p, MainTabWindow_Work.VisibleWorkTypeDefsInPriorityOrder[i]);
				WidgetsWork.DrawWorkBoxFor(topLeft, p, workTypeDef, incapableBecauseOfCapacities);
				Rect rect2 = new Rect(topLeft.x, topLeft.y, 25f, 25f);
				TooltipHandler.TipRegion(rect2, WidgetsWork.TipForPawnWorker(p, workTypeDef, incapableBecauseOfCapacities));
				num += this.workColumnSpacing;
			}
		}
		private bool IsIncapableOfWholeWorkType(Pawn p, WorkTypeDef work)
		{
			for (int i = 0; i < work.workGiversByPriority.Count; i++)
			{
				bool flag = true;
				for (int j = 0; j < work.workGiversByPriority[i].requiredCapacities.Count; j++)
				{
					PawnCapacityDef activity = work.workGiversByPriority[i].requiredCapacities[j];
					if (!p.health.capacities.CapableOf(activity))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					return false;
				}
			}
			return true;
		}
	}
}
