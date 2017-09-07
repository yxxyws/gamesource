using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
namespace Verse
{
	[StaticConstructorOnStartup]
	public static class GenUI
	{
		public const float SPad = 8f;
		public const float Pad = 10f;
		public const float GapTiny = 4f;
		public const float Gap = 17f;
		public const float GapWide = 26f;
		public const float ListSpacing = 28f;
		public const float MouseAttachIconSize = 32f;
		public const float MouseAttachIconOffset = 8f;
		public const float ScrollBarWidth = 16f;
		private const float MouseIconSize = 32f;
		private const float MouseIconOffset = 12f;
		public const float PawnDirectClickRadius = 0.4f;
		public static readonly Vector2 TradeableDrawSize = new Vector2(150f, 45f);
		public static readonly Color MouseoverColor = new Color(0.3f, 0.7f, 0.9f);
		public static readonly Vector2 MaxWinSize = new Vector2(1010f, 754f);
		private static readonly Material MouseoverBracketMaterial = MaterialPool.MatFrom("UI/Overlays/MouseoverBracketTex", ShaderDatabase.MetaOverlay);
		private static readonly Texture2D UnderShadowTex = ContentFinder<Texture2D>.Get("UI/Misc/ScreenCornerShadow", true);
		private static readonly Vector2 PieceBarSize = new Vector2(100f, 17f);
		public static void SetLabelAlign(TextAnchor a)
		{
			Text.Anchor = a;
		}
		public static void ResetLabelAlign()
		{
			Text.Anchor = TextAnchor.UpperLeft;
		}
		public static float BackgroundDarkAlphaForText()
		{
			float num = GenCelestial.CurCelestialSunGlow();
			float num2 = (Find.Map.Biome != BiomeDefOf.IceSheet) ? Mathf.Clamp01(Find.SnowGrid.TotalDepth / 1000f) : 1f;
			return num * num2 * 0.41f;
		}
		public static void DrawTextWinterShadow(Rect rect)
		{
			float num = GenUI.BackgroundDarkAlphaForText();
			if (num > 0.001f)
			{
				GUI.color = new Color(1f, 1f, 1f, num);
				GUI.DrawTexture(rect, GenUI.UnderShadowTex);
				GUI.color = Color.white;
			}
		}
		public static float IconDrawScale(ThingDef tDef)
		{
			if (tDef.graphicData == null)
			{
				return 1f;
			}
			if (tDef.graphicData.drawSize.x > (float)tDef.Size.x && tDef.graphicData.drawSize.y > (float)tDef.Size.z)
			{
				return Mathf.Min(tDef.graphicData.drawSize.x / (float)tDef.Size.x, tDef.graphicData.drawSize.y / (float)tDef.Size.z);
			}
			return 1f;
		}
		public static void ErrorDialog(string message)
		{
			if (Find.WindowStack != null)
			{
				Find.WindowStack.Add(new Dialog_Message(message, null));
			}
		}
		public static Vector2 AbsMousePosition()
		{
			Vector2 result = Input.mousePosition;
			result.y = (float)Screen.height - result.y;
			return result;
		}
		public static Vector2 AbsUIPositionOf(Vector2 localPos)
		{
			Vector2 mousePosition = Event.current.mousePosition;
			Vector2 b = GenUI.AbsMousePosition();
			Vector2 b2 = mousePosition - b;
			return localPos - b2;
		}
		public static float DistFromRect(Rect r, Vector2 p)
		{
			float num = Mathf.Abs(p.x - r.center.x) - r.width / 2f;
			if (num < 0f)
			{
				num = 0f;
			}
			float num2 = Mathf.Abs(p.y - r.center.y) - r.height / 2f;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			return Mathf.Sqrt(num * num + num2 * num2);
		}
		public static void DrawMouseAttachment(Texture2D iconTex, string text)
		{
			Vector2 mousePosition = Event.current.mousePosition;
			float num = mousePosition.y + 12f;
			if (iconTex != null)
			{
				Rect position = new Rect(mousePosition.x + 12f, num, 32f, 32f);
				GUI.DrawTexture(position, iconTex);
				num += position.height;
			}
			if (text != string.Empty)
			{
				Rect rect = new Rect(mousePosition.x + 12f, num, 200f, 9999f);
				Widgets.Label(rect, text);
			}
		}
		public static void RenderMouseoverBracket()
		{
			Vector3 position = Gen.MouseCell().ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays);
			Graphics.DrawMesh(MeshPool.plane10, position, Quaternion.identity, GenUI.MouseoverBracketMaterial, 0);
		}
		public static void DrawStatusLevel(Need status, Rect rect)
		{
			GUI.BeginGroup(rect);
			Rect rect2 = new Rect(0f, 2f, rect.width, 25f);
			Widgets.Label(rect2, status.LabelCap);
			Rect rect3 = new Rect(100f, 3f, GenUI.PieceBarSize.x, GenUI.PieceBarSize.y);
			Widgets.FillableBar(rect3, status.CurLevelPercentage);
			Widgets.FillableBarChangeArrows(rect3, status.GUIChangeArrow);
			GUI.EndGroup();
			TooltipHandler.TipRegion(rect, status.GetTipString());
			if (Mouse.IsOver(rect))
			{
				GUI.DrawTexture(rect, TexUI.HighlightTex);
			}
		}
		public static IEnumerable<TargetInfo> TargetsAtMouse(TargetingParameters clickParams, bool thingsOnly = false)
		{
			return GenUI.TargetsAt(Gen.MouseMapPosVector3(), clickParams, thingsOnly);
		}
		[DebuggerHidden]
		public static IEnumerable<TargetInfo> TargetsAt(Vector3 clickPos, TargetingParameters clickParams, bool thingsOnly = false)
		{
			GenUI.<TargetsAt>c__Iterator18F <TargetsAt>c__Iterator18F = new GenUI.<TargetsAt>c__Iterator18F();
			<TargetsAt>c__Iterator18F.clickPos = clickPos;
			<TargetsAt>c__Iterator18F.clickParams = clickParams;
			<TargetsAt>c__Iterator18F.thingsOnly = thingsOnly;
			<TargetsAt>c__Iterator18F.<$>clickPos = clickPos;
			<TargetsAt>c__Iterator18F.<$>clickParams = clickParams;
			<TargetsAt>c__Iterator18F.<$>thingsOnly = thingsOnly;
			GenUI.<TargetsAt>c__Iterator18F expr_31 = <TargetsAt>c__Iterator18F;
			expr_31.$PC = -2;
			return expr_31;
		}
		public static List<Thing> ThingsUnderMouse(Vector3 clickPos, float pawnWideClickRadius, TargetingParameters clickParams)
		{
			IntVec3 c = IntVec3.FromVector3(clickPos);
			List<Thing> list = new List<Thing>();
			List<Pawn> list2 = new List<Pawn>();
			foreach (Pawn current in Find.MapPawns.AllPawnsSpawned)
			{
				if ((current.DrawPos - clickPos).MagnitudeHorizontal() < 0.4f && clickParams.CanTarget(current))
				{
					list2.Add(current);
				}
			}
			list2.Sort(new Comparison<Pawn>(GenUI.CompareThingsByDistanceToMousePointer));
			foreach (Pawn current2 in list2)
			{
				list.Add(current2);
			}
			List<Thing> list3 = new List<Thing>();
			foreach (Thing current3 in Find.ThingGrid.ThingsAt(c))
			{
				if (!list.Contains(current3) && clickParams.CanTarget(current3))
				{
					list3.Add(current3);
				}
			}
			list3.Sort(new Comparison<Thing>(GenUI.CompareThingsByDrawAltitude));
			list.AddRange(list3);
			List<Thing> list4 = new List<Thing>();
			foreach (Pawn current4 in Find.MapPawns.AllPawnsSpawned)
			{
				if ((current4.DrawPos - clickPos).MagnitudeHorizontal() < pawnWideClickRadius && clickParams.CanTarget(current4))
				{
					list4.Add(current4);
				}
			}
			list4.Sort(new Comparison<Thing>(GenUI.CompareThingsByDistanceToMousePointer));
			foreach (Thing current5 in list4)
			{
				if (!list.Contains(current5))
				{
					list.Add(current5);
				}
			}
			list.RemoveAll((Thing t) => !t.Spawned);
			return list;
		}
		private static int CompareThingsByDistanceToMousePointer(Thing A, Thing B)
		{
			Vector3 b = Gen.MouseMapPosVector3();
			float num = (A.DrawPos - b).MagnitudeHorizontalSquared();
			float num2 = (B.DrawPos - b).MagnitudeHorizontalSquared();
			if (num < num2)
			{
				return -1;
			}
			if (num == num2)
			{
				return 0;
			}
			return 1;
		}
		private static int CompareThingsByDrawAltitude(Thing A, Thing B)
		{
			if (A.def.Altitude < B.def.Altitude)
			{
				return 1;
			}
			if (A.def.Altitude == B.def.Altitude)
			{
				return 0;
			}
			return -1;
		}
		public static Rect GetInnerRect(this Rect rect)
		{
			return rect.ContractedBy(17f);
		}
		public static Rect ContractedBy(this Rect rect, float margin)
		{
			return new Rect(rect.x + margin, rect.y + margin, rect.width - margin * 2f, rect.height - margin * 2f);
		}
		public static Rect CenteredOnXIn(this Rect rect, Rect otherRect)
		{
			return new Rect(otherRect.x + (otherRect.width - rect.width) / 2f, rect.y, rect.width, rect.height);
		}
		public static Rect CenteredOnYIn(this Rect rect, Rect otherRect)
		{
			return new Rect(rect.x, otherRect.y + (otherRect.height - rect.height) / 2f, rect.width, rect.height);
		}
		public static Rect AtZero(this Rect rect)
		{
			return new Rect(0f, 0f, rect.width, rect.height);
		}
		public static void AbsorbClicksInRect(Rect r)
		{
			if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition))
			{
				Event.current.Use();
			}
		}
	}
}
