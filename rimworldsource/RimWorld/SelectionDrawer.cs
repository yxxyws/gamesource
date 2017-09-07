using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace RimWorld
{
	[StaticConstructorOnStartup]
	public static class SelectionDrawer
	{
		private const float SelJumpDuration = 0.07f;
		private const float SelJumpDistance = 0.2f;
		private static Dictionary<object, float> selectTimes = new Dictionary<object, float>();
		private static readonly Material SelectionBracketMat = MaterialPool.MatFrom("UI/Overlays/SelectionBracket", ShaderDatabase.MetaOverlay);
		public static void Notify_Selected(object t)
		{
			SelectionDrawer.selectTimes[t] = Time.realtimeSinceStartup;
		}
		public static void Clear()
		{
			SelectionDrawer.selectTimes.Clear();
		}
		public static void DrawSelectionOverlays()
		{
			foreach (object current in Find.Selector.SelectedObjects)
			{
				SelectionDrawer.DrawSelectionBracketFor(current);
				Thing thing = current as Thing;
				if (thing != null)
				{
					thing.DrawExtraSelectionOverlays();
					Pawn pawn = thing as Pawn;
					if (pawn != null && pawn.IsColonistPlayerControlled && pawn.pather.curPath != null)
					{
						pawn.pather.curPath.DebugDrawPath(pawn);
					}
				}
			}
		}
		private static void DrawSelectionBracketFor(object obj)
		{
			Zone zone = obj as Zone;
			if (zone != null)
			{
				GenDraw.DrawFieldEdges(zone.Cells);
			}
			Thing thing = obj as Thing;
			if (thing != null)
			{
				Vector3[] array = new Vector3[]
				{
					default(Vector3),
					default(Vector3),
					default(Vector3),
					default(Vector3)
				};
				Vector3 drawPos = thing.DrawPos;
				Vector2 vector = thing.RotatedSize.ToVector2() * 0.5f;
				Vector2 vector2 = vector;
				vector2.x -= 0.5f;
				vector2.y -= 0.5f;
				float num = 1f - (Time.realtimeSinceStartup - SelectionDrawer.selectTimes[thing]) / 0.07f;
				if (num < 0f)
				{
					num = 0f;
				}
				float num2 = num * 0.2f;
				vector2.x += num2;
				vector2.y += num2;
				array[0] = new Vector3(drawPos.x - vector2.x, 0f, drawPos.z - vector2.y);
				array[1] = new Vector3(drawPos.x + vector2.x, 0f, drawPos.z - vector2.y);
				array[2] = new Vector3(drawPos.x + vector2.x, 0f, drawPos.z + vector2.y);
				array[3] = new Vector3(drawPos.x - vector2.x, 0f, drawPos.z + vector2.y);
				int num3 = 0;
				for (int i = 0; i < 4; i++)
				{
					array[i].y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays);
					Quaternion rotation = Quaternion.AngleAxis((float)num3, Vector3.up);
					Graphics.DrawMesh(MeshPool.plane10, array[i], rotation, SelectionDrawer.SelectionBracketMat, 0);
					num3 -= 90;
				}
			}
		}
	}
}
