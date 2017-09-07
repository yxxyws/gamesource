using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Verse
{
	[StaticConstructorOnStartup]
	public static class SimpleCurveDrawer
	{
		private const float PointSize = 10f;
		private const float MeasuresWidth = 60f;
		private const float MeasureLinePeekOut = 5f;
		private const float LegendCellWidth = 140f;
		private const float LegendCellHeight = 20f;
		private static readonly Color AxisLineColor = new Color(0.2f, 0.5f, 1f, 1f);
		private static readonly Color MajorLineColor = new Color(0.2f, 0.4f, 1f, 0.6f);
		private static readonly Color MinorLineColor = new Color(0.2f, 0.3f, 1f, 0.19f);
		private static readonly Texture2D CurvePoint = ContentFinder<Texture2D>.Get("UI/Widgets/Dev/CurvePoint", true);
		public static void DrawCurve(SimpleCurve curve, Rect screenRect, SimpleCurveDrawerStyle style = null, List<CurveMark> marks = null, Rect legendScreenRect = default(Rect))
		{
			SimpleCurveDrawer.DrawCurve(new SimpleCurveDrawInfo
			{
				curve = curve
			}, screenRect, style, marks, legendScreenRect);
		}
		public static void DrawCurve(SimpleCurveDrawInfo curve, Rect screenRect, SimpleCurveDrawerStyle style = null, List<CurveMark> marks = null, Rect legendScreenRect = default(Rect))
		{
			if (curve.curve == null)
			{
				return;
			}
			SimpleCurveDrawer.DrawCurves(new List<SimpleCurveDrawInfo>
			{
				curve
			}, screenRect, style, marks, legendScreenRect);
		}
		public static void DrawCurves(List<SimpleCurveDrawInfo> curves, Rect screenRect, SimpleCurveDrawerStyle style = null, List<CurveMark> marks = null, Rect legendScreenRect = default(Rect))
		{
			if (style == null)
			{
				style = new SimpleCurveDrawerStyle();
			}
			if (curves.Count == 0)
			{
				return;
			}
			Rect rect = screenRect;
			if (style.DrawMeasure)
			{
				rect = screenRect.ContractedBy(30f);
				rect.x += 30f;
				rect.y -= 30f;
			}
			bool flag = true;
			Rect viewRect = default(Rect);
			foreach (SimpleCurveDrawInfo current in curves)
			{
				if (current.curve != null)
				{
					if (flag)
					{
						flag = false;
						viewRect = current.curve.View.rect;
					}
					else
					{
						viewRect.xMin = Mathf.Min(viewRect.xMin, current.curve.View.rect.xMin);
						viewRect.xMax = Mathf.Max(viewRect.xMax, current.curve.View.rect.xMax);
						viewRect.yMin = Mathf.Min(viewRect.yMin, current.curve.View.rect.yMin);
						viewRect.yMax = Mathf.Max(viewRect.yMax, current.curve.View.rect.yMax);
					}
				}
			}
			if (style.UseFixedScale)
			{
				viewRect.yMin = style.FixedScale.x;
				viewRect.yMax = style.FixedScale.y;
			}
			if (style.OnlyPositiveValues)
			{
				if (viewRect.xMin < 0f)
				{
					viewRect.xMin = 0f;
				}
				if (viewRect.yMin < 0f)
				{
					viewRect.yMin = 0f;
				}
			}
			if (style.UseFixedSection)
			{
				viewRect.xMin = style.FixedSection.x;
				viewRect.xMax = style.FixedSection.y;
			}
			if (Mathf.Approximately(viewRect.width, 0f) || Mathf.Approximately(viewRect.height, 0f))
			{
				return;
			}
			if (style.DrawBackground)
			{
				GUI.color = new Color(0.302f, 0.318f, 0.365f);
				GUI.DrawTexture(rect, BaseContent.WhiteTex);
			}
			if (style.DrawBackgroundLines)
			{
				SimpleCurveDrawer.DrawGraphBackgroundLines(rect, viewRect);
			}
			if (style.DrawMeasure)
			{
				SimpleCurveDrawer.DrawCurveMeasure(viewRect, screenRect, rect, style.MeasureLabelsXCount, style.MeasureLabelsYCount);
			}
			foreach (SimpleCurveDrawInfo current2 in curves)
			{
				SimpleCurveDrawer.DrawCurveLines(current2, rect, style.DrawPoints, viewRect, style.UseAntiAliasedLines, style.PointsRemoveOptimization);
			}
			if (style.DrawLegend)
			{
				SimpleCurveDrawer.DrawCurvesLegend(legendScreenRect, curves);
			}
			if (style.DrawCurveMousePoint)
			{
				SimpleCurveDrawer.DrawCurveMousePoint(curves, rect, viewRect, style.LabelX, style.LabelY);
			}
			if (marks != null)
			{
				Rect screenRect2 = rect;
				screenRect2.yMin -= 15f;
				screenRect2.height = 15f;
				SimpleCurveDrawer.DrawCurveMarks(marks, screenRect2, viewRect);
			}
		}
		public static void DrawCurveLines(SimpleCurveDrawInfo curve, Rect screenRect, bool drawPoints, Rect viewRect, bool useAALines, bool pointsRemoveOptimization)
		{
			if (curve.curve == null)
			{
				return;
			}
			if (curve.curve.PointsCount == 0)
			{
				return;
			}
			Rect position = screenRect;
			position.yMin -= 1f;
			position.yMax += 1f;
			GUI.BeginGroup(position);
			if (Event.current.type == EventType.Repaint)
			{
				if (useAALines)
				{
					bool flag = true;
					Vector2 start = default(Vector2);
					Vector2 curvePoint = default(Vector2);
					int num = curve.curve.AllPoints.Count((CurvePoint x) => x.x >= viewRect.xMin && x.x <= viewRect.xMax);
					int num2 = SimpleCurveDrawer.RemovePointsOptimizationFreq(num);
					int num3 = 0;
					foreach (CurvePoint current in curve.curve.AllPoints)
					{
						num3++;
						if (!pointsRemoveOptimization || num3 % num2 != 0 || num3 == num - 1)
						{
							curvePoint.x = current.x;
							curvePoint.y = current.y;
							Vector2 vector = SimpleCurveDrawer.CurveToScreenCoordsInsideScreenRect(curvePoint, screenRect, viewRect);
							if (flag)
							{
								flag = false;
							}
							else
							{
								if ((start.x >= 0f && start.x <= screenRect.width) || (vector.x >= 0f && vector.x <= screenRect.width))
								{
									Widgets.DrawLine(start, vector, curve.color, 1f);
								}
							}
							start = vector;
						}
					}
					Vector2 start2 = SimpleCurveDrawer.CurveToScreenCoordsInsideScreenRect(curve.curve.AllPoints.First<CurvePoint>(), screenRect, viewRect);
					Vector2 start3 = SimpleCurveDrawer.CurveToScreenCoordsInsideScreenRect(curve.curve.AllPoints.Last<CurvePoint>(), screenRect, viewRect);
					Widgets.DrawLine(start2, new Vector2(0f, start2.y), curve.color, 1f);
					Widgets.DrawLine(start3, new Vector2(screenRect.width, start3.y), curve.color, 1f);
				}
				else
				{
					GUI.color = curve.color;
					float num4 = viewRect.x;
					float num5 = screenRect.width / 1f;
					float num6 = viewRect.width / num5;
					while (num4 < viewRect.xMax)
					{
						num4 += num6;
						Vector2 vector2 = SimpleCurveDrawer.CurveToScreenCoordsInsideScreenRect(new Vector2(num4, curve.curve.Evaluate(num4)), screenRect, viewRect);
						GUI.DrawTexture(new Rect(vector2.x, vector2.y, 1f, 1f), BaseContent.WhiteTex);
					}
				}
				GUI.color = Color.white;
			}
			if (drawPoints)
			{
				foreach (CurvePoint current2 in curve.curve.AllPoints)
				{
					Vector2 screenPoint = SimpleCurveDrawer.CurveToScreenCoordsInsideScreenRect(current2.loc, screenRect, viewRect);
					SimpleCurveDrawer.DrawPoint(screenPoint);
				}
			}
			foreach (float num7 in curve.curve.View.DebugInputValues)
			{
				GUI.color = new Color(0f, 1f, 0f, 0.25f);
				SimpleCurveDrawer.DrawInfiniteVerticalLine(num7, screenRect, viewRect);
				float y = curve.curve.Evaluate(num7);
				Vector2 curvePoint2 = new Vector2(num7, y);
				Vector2 screenPoint2 = SimpleCurveDrawer.CurveToScreenCoordsInsideScreenRect(curvePoint2, screenRect, viewRect);
				GUI.color = Color.green;
				SimpleCurveDrawer.DrawPoint(screenPoint2);
				GUI.color = Color.white;
			}
			GUI.EndGroup();
		}
		public static void DrawCurveMeasure(Rect viewRect, Rect screenRect, Rect graphRect, int xLabelsCount, int yLabelsCount)
		{
			Vector2 vector = new Vector2(viewRect.x, viewRect.y + viewRect.height);
			Vector2 vector2 = new Vector2(viewRect.x, viewRect.y);
			Vector2 vector3 = new Vector2(viewRect.x + viewRect.width, viewRect.y);
			Text.Font = GameFont.Small;
			Color color = new Color(0.45f, 0.45f, 0.45f);
			Color color2 = new Color(0.7f, 0.7f, 0.7f);
			GUI.BeginGroup(screenRect);
			Text.Anchor = TextAnchor.UpperCenter;
			string b = string.Empty;
			for (int i = 0; i < xLabelsCount; i++)
			{
				string text = ((vector3.x - vector2.x) / (float)xLabelsCount * (float)i + vector2.x).ToString("F0");
				if (!(text == b))
				{
					b = text;
					float num = graphRect.width / (float)xLabelsCount * (float)i + 60f;
					float num2 = screenRect.height - 60f;
					GUI.color = color;
					Widgets.DrawLineVertical(num, num2, 5f);
					GUI.color = color2;
					Rect rect = new Rect(num - 31f, num2 + 2f, 60f, 30f);
					Text.Font = GameFont.Tiny;
					Widgets.Label(rect, text);
					Text.Font = GameFont.Small;
				}
			}
			b = string.Empty;
			Text.Anchor = TextAnchor.UpperRight;
			for (int j = 0; j < yLabelsCount; j++)
			{
				string text2 = ((vector.y - vector2.y) / (float)yLabelsCount * (float)j + vector2.y).ToString("F0");
				if (!(text2 == b))
				{
					b = text2;
					float num3 = 60f;
					float num4 = graphRect.height / (float)yLabelsCount * (float)(yLabelsCount - j);
					GUI.color = color;
					Widgets.DrawLineHorizontal(num3 - 5f, num4, 5f + graphRect.width);
					GUI.color = color2;
					Rect rect2 = new Rect(num3 - 60f, num4 - 10f, 55f, 20f);
					Text.Font = GameFont.Tiny;
					num4 += 2f;
					Widgets.Label(rect2, text2);
					Text.Font = GameFont.Small;
				}
			}
			GUI.EndGroup();
			GUI.color = new Color(1f, 1f, 1f);
			Text.Anchor = TextAnchor.UpperLeft;
		}
		public static void DrawCurvesLegend(Rect rect, List<SimpleCurveDrawInfo> curves)
		{
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
			GUI.BeginGroup(rect);
			float num = 0f;
			float num2 = 0f;
			int num3 = (int)(rect.width / 140f);
			int num4 = 0;
			foreach (SimpleCurveDrawInfo current in curves)
			{
				GUI.color = current.color;
				GUI.DrawTexture(new Rect(num, num2 + 2f, 15f, 15f), BaseContent.WhiteTex);
				GUI.color = Color.white;
				num += 20f;
				if (current.label != null)
				{
					Widgets.Label(new Rect(num, num2, 140f, 20f), new GUIContent(current.label));
				}
				num4++;
				if (num4 == num3)
				{
					num4 = 0;
					num = 0f;
					num2 += 20f;
				}
				else
				{
					num += 140f;
				}
			}
			GUI.EndGroup();
			GUI.color = Color.white;
		}
		public static void DrawCurveMousePoint(List<SimpleCurveDrawInfo> curves, Rect screenRect, Rect viewRect, string labelX, string labelY)
		{
			if (curves.Count == 0)
			{
				return;
			}
			Vector2 vector = Event.current.mousePosition;
			if (!Mouse.IsOver(screenRect))
			{
				return;
			}
			GUI.BeginGroup(screenRect);
			vector = GUIUtility.ScreenToGUIPoint(GenUI.AbsMousePosition());
			vector.x += screenRect.x;
			Vector2 vector2 = default(Vector2);
			Vector2 vector3 = default(Vector2);
			bool flag = false;
			foreach (SimpleCurveDrawInfo current in curves)
			{
				if (current.curve.AllPoints.Any<CurvePoint>())
				{
					Vector2 vector4 = SimpleCurveDrawer.ScreenToCurveCoords(vector, screenRect, viewRect);
					vector4.y = current.curve.Evaluate(vector4.x);
					Vector2 vector5 = SimpleCurveDrawer.CurveToScreenCoordsInsideScreenRect(vector4, screenRect, viewRect);
					if (!flag || Vector2.Distance(vector5, vector) < Vector2.Distance(vector3, vector))
					{
						flag = true;
						vector2 = vector4;
						vector3 = vector5;
					}
				}
			}
			SimpleCurveDrawer.DrawPoint(vector3);
			Rect rect = new Rect(vector3.x, vector3.y, 100f, 60f);
			Text.Anchor = TextAnchor.UpperLeft;
			if (rect.x + rect.width > screenRect.width)
			{
				rect.x -= rect.width;
				Text.Anchor = TextAnchor.UpperRight;
			}
			if (rect.y + rect.height > screenRect.height)
			{
				rect.y -= rect.height;
				if (Text.Anchor == TextAnchor.UpperLeft)
				{
					Text.Anchor = TextAnchor.LowerLeft;
				}
				else
				{
					Text.Anchor = TextAnchor.LowerRight;
				}
			}
			Widgets.Label(rect, new GUIContent(string.Concat(new string[]
			{
				labelX,
				": ",
				vector2.x.ToString("0.##"),
				"\n",
				labelY,
				": ",
				vector2.y.ToString("0.##")
			})));
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.EndGroup();
		}
		public static void DrawCurveMarks(List<CurveMark> marks, Rect screenRect, Rect viewRect)
		{
			float x = viewRect.x;
			float num = viewRect.x + viewRect.width;
			float num2 = screenRect.y + 5f;
			float num3 = screenRect.yMax - 5f;
			int num4 = 0;
			foreach (CurveMark current in marks)
			{
				if (current.x >= x && current.x <= num)
				{
					GUI.color = current.color;
					Vector2 screenPoint = default(Vector2);
					screenPoint.x = screenRect.x + (current.x - x) / (num - x) * screenRect.width;
					screenPoint.y = ((num4 % 2 != 0) ? num3 : num2);
					SimpleCurveDrawer.DrawPoint(screenPoint);
					TooltipHandler.TipRegion(new Rect(screenPoint.x - 5f, screenPoint.y - 5f, 10f, 10f), new TipSignal(current.message));
				}
				num4++;
			}
			GUI.color = Color.white;
		}
		private static void DrawPoint(Vector2 screenPoint)
		{
			Rect position = new Rect(screenPoint.x - 5f, screenPoint.y - 5f, 10f, 10f);
			GUI.DrawTexture(position, SimpleCurveDrawer.CurvePoint);
		}
		private static void DrawInfiniteVerticalLine(float curveX, Rect screenRect, Rect viewRect)
		{
			Widgets.DrawLineVertical(SimpleCurveDrawer.CurveToScreenCoordsInsideScreenRect(new Vector2(curveX, 0f), screenRect, viewRect).x, -999f, 9999f);
		}
		private static void DrawInfiniteHorizontalLine(float curveY, Rect screenRect, Rect viewRect)
		{
			Widgets.DrawLineHorizontal(-999f, SimpleCurveDrawer.CurveToScreenCoordsInsideScreenRect(new Vector2(0f, curveY), screenRect, viewRect).y, 9999f);
		}
		public static Vector2 CurveToScreenCoordsInsideScreenRect(Vector2 curvePoint, Rect screenRect, Rect viewRect)
		{
			Vector2 result = curvePoint;
			result.x -= viewRect.x;
			result.y -= viewRect.y;
			result.x *= screenRect.width / viewRect.width;
			result.y *= screenRect.height / viewRect.height;
			result.y = screenRect.height - result.y;
			return result;
		}
		public static Vector2 ScreenToCurveCoords(Vector2 screenPoint, Rect screenRect, Rect viewRect)
		{
			Vector2 loc = screenPoint;
			loc.x -= screenRect.x;
			loc.y -= screenRect.y;
			loc.y = screenRect.height - loc.y;
			loc.x /= screenRect.width / viewRect.width;
			loc.y /= screenRect.height / viewRect.height;
			loc.x += viewRect.x;
			loc.y += viewRect.y;
			return new CurvePoint(loc);
		}
		public static void DrawGraphBackgroundLines(Rect screenRect, Rect viewRect)
		{
			GUI.BeginGroup(screenRect);
			float num = 0.01f;
			while (viewRect.width / (num * 10f) > 4f)
			{
				num *= 10f;
			}
			for (float num2 = (float)Mathf.FloorToInt(viewRect.x / num) * num; num2 < viewRect.xMax; num2 += num)
			{
				if (Mathf.Abs(num2 % (10f * num)) < 0.001f)
				{
					GUI.color = SimpleCurveDrawer.MajorLineColor;
				}
				else
				{
					GUI.color = SimpleCurveDrawer.MinorLineColor;
				}
				SimpleCurveDrawer.DrawInfiniteVerticalLine(num2, screenRect, viewRect);
			}
			float num3 = 0.01f;
			while (viewRect.height / (num3 * 10f) > 4f)
			{
				num3 *= 10f;
			}
			for (float num4 = (float)Mathf.FloorToInt(viewRect.y / num3) * num3; num4 < viewRect.yMax; num4 += num3)
			{
				if (Mathf.Abs(num4 % (10f * num3)) < 0.001f)
				{
					GUI.color = SimpleCurveDrawer.MajorLineColor;
				}
				else
				{
					GUI.color = SimpleCurveDrawer.MinorLineColor;
				}
				SimpleCurveDrawer.DrawInfiniteHorizontalLine(num4, screenRect, viewRect);
			}
			GUI.color = SimpleCurveDrawer.AxisLineColor;
			SimpleCurveDrawer.DrawInfiniteHorizontalLine(0f, screenRect, viewRect);
			SimpleCurveDrawer.DrawInfiniteVerticalLine(0f, screenRect, viewRect);
			GUI.color = Color.white;
			GUI.EndGroup();
		}
		private static int RemovePointsOptimizationFreq(int count)
		{
			int result = count + 1;
			if (count > 1000)
			{
				result = 5;
			}
			if (count > 1200)
			{
				result = 4;
			}
			if (count > 1400)
			{
				result = 3;
			}
			if (count > 1900)
			{
				result = 2;
			}
			return result;
		}
	}
}
