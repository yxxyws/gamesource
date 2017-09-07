using System;
using UnityEngine;
namespace Verse
{
	public static class DebugRender
	{
		private static int lastCameraUpdateFrame = -1;
		private static CellRect viewRect;
		public static void RenderCell(IntVec3 c)
		{
			DebugRender.RenderCell(c, 0.5f);
		}
		public static void RenderCell(IntVec3 c, float colorPct)
		{
			int num = Mathf.RoundToInt(colorPct * 100f);
			num %= 100;
			DebugRender.RenderCell(c, DebugMatsSpectrum.Mat(num));
		}
		public static void RenderCell(IntVec3 c, Material mat)
		{
			if (Time.frameCount != DebugRender.lastCameraUpdateFrame)
			{
				DebugRender.viewRect = Find.CameraMap.CurrentViewRect;
				DebugRender.lastCameraUpdateFrame = Time.frameCount;
			}
			if (!DebugRender.viewRect.Contains(c))
			{
				return;
			}
			Graphics.DrawMesh(MeshPool.plane10, c.ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays), Quaternion.identity, mat, 0);
		}
	}
}
