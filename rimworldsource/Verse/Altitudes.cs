using System;
using UnityEngine;
namespace Verse
{
	public static class Altitudes
	{
		private const float LayerSpacing = 0.35f;
		public const float AltInc = 0.05f;
		private static readonly float[] Alts;
		public static readonly Vector3 AltIncVect;
		static Altitudes()
		{
			Altitudes.Alts = new float[Enum.GetValues(typeof(AltitudeLayer)).Length];
			Altitudes.AltIncVect = new Vector3(0f, 0.05f, 0f);
			int length = Enum.GetValues(typeof(AltitudeLayer)).Length;
			for (int i = 0; i < length; i++)
			{
				Altitudes.Alts[i] = (float)i * 0.35f;
			}
		}
		public static float AltitudeFor(AltitudeLayer Alt)
		{
			return Altitudes.Alts[(int)Alt];
		}
	}
}
