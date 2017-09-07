using System;
using UnityEngine;
namespace Verse
{
	public static class IntVec3Utility
	{
		public static IntVec3 ToIntVec3(this Vector3 vect)
		{
			return new IntVec3(vect);
		}
		public static float DistanceToSquared(this IntVec3 a, IntVec3 b)
		{
			return (a - b).LengthHorizontalSquared;
		}
	}
}
