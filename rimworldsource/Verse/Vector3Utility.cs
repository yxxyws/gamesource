using System;
using UnityEngine;
namespace Verse
{
	public static class Vector3Utility
	{
		public static Vector2 ToScreenPosition(this Vector3 v)
		{
			Vector3 vector = Find.CameraMapGameObject.camera.WorldToScreenPoint(v);
			return new Vector2(vector.x, (float)Screen.height - vector.y);
		}
		public static Vector3 HorizontalVectorFromAngle(float angle)
		{
			return Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
		}
		public static float AngleFlat(this Vector3 v)
		{
			if (v.x == 0f && v.z == 0f)
			{
				return 0f;
			}
			return Quaternion.LookRotation(v).eulerAngles.y;
		}
		public static Vector3 RandomHorizontalOffset(float maxDist)
		{
			float d = Rand.Range(0f, maxDist);
			float y = (float)Rand.Range(0, 360);
			return Quaternion.Euler(new Vector3(0f, y, 0f)) * Vector3.forward * d;
		}
	}
}
