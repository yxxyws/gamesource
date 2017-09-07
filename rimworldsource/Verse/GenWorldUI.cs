using System;
using UnityEngine;
namespace Verse
{
	public static class GenWorldUI
	{
		public const float NameBGHeight = 12f;
		public const float NameBGExtraWidth = 4f;
		public const float LabelOffsetYStandard = -0.4f;
		private static readonly Color DefaultThingLabelColor = new Color(1f, 1f, 1f, 0.75f);
		public static Vector2 LabelDrawPosFor(Thing thing, float worldOffsetZ)
		{
			Vector3 drawPos = thing.DrawPos;
			drawPos.z += worldOffsetZ;
			Vector2 result = Find.CameraMapGameObject.camera.WorldToScreenPoint(drawPos);
			result.y = (float)Screen.height - result.y;
			return result;
		}
		public static Vector2 LabelDrawPosFor(IntVec3 center)
		{
			Vector3 position = center.ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays);
			Vector2 result = Find.CameraMapGameObject.camera.WorldToScreenPoint(position);
			result.y = (float)Screen.height - result.y;
			result.y -= 1f;
			return result;
		}
		public static void DrawThingLabel(Thing thing, string text)
		{
			GenWorldUI.DrawThingLabel(thing, text, GenWorldUI.DefaultThingLabelColor);
		}
		public static void DrawThingLabel(Thing thing, string text, Color textColor)
		{
			GenWorldUI.DrawThingLabel(GenWorldUI.LabelDrawPosFor(thing, -0.4f), text, textColor);
		}
		public static void DrawThingLabel(Vector2 screenPos, string text, Color textColor)
		{
			Text.Font = GameFont.Tiny;
			float x = Text.CalcSize(text).x;
			Rect position = new Rect(screenPos.x - x / 2f - 4f, screenPos.y, x + 8f, 12f);
			GUI.DrawTexture(position, TexUI.GrayTextBG);
			GUI.color = textColor;
			Text.Anchor = TextAnchor.UpperCenter;
			Rect rect = new Rect(screenPos.x - x / 2f, screenPos.y - 3f, x, 999f);
			Widgets.Label(rect, text);
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
		}
		public static void DrawText(Vector2 worldPos, string text, Color textColor)
		{
			Vector3 position = new Vector3(worldPos.x, 0f, worldPos.y);
			Vector2 vector = Find.CameraMapGameObject.camera.WorldToScreenPoint(position);
			vector.y = (float)Screen.height - vector.y;
			Text.Font = GameFont.Tiny;
			GUI.color = textColor;
			Text.Anchor = TextAnchor.UpperCenter;
			float x = Text.CalcSize(text).x;
			Widgets.Label(new Rect(vector.x - x / 2f, vector.y - 2f, x, 999f), text);
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
		}
	}
}
