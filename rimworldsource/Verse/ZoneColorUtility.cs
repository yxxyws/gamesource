using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
namespace Verse
{
	public static class ZoneColorUtility
	{
		private const float ZoneOpacity = 0.09f;
		public static List<Color> zoneColors;
		private static int nextColIndex;
		static ZoneColorUtility()
		{
			ZoneColorUtility.zoneColors = new List<Color>();
			ZoneColorUtility.nextColIndex = 0;
			foreach (Color current in ZoneColorUtility.ZoneColors())
			{
				Color item = new Color(current.r, current.g, current.b, 0.09f);
				ZoneColorUtility.zoneColors.Add(item);
			}
		}
		public static Color NextZoneColor()
		{
			Color result = ZoneColorUtility.zoneColors[ZoneColorUtility.nextColIndex];
			ZoneColorUtility.nextColIndex++;
			if (ZoneColorUtility.nextColIndex >= ZoneColorUtility.zoneColors.Count)
			{
				ZoneColorUtility.nextColIndex = 0;
			}
			return result;
		}
		[DebuggerHidden]
		private static IEnumerable<Color> ZoneColors()
		{
			ZoneColorUtility.<ZoneColors>c__Iterator170 <ZoneColors>c__Iterator = new ZoneColorUtility.<ZoneColors>c__Iterator170();
			ZoneColorUtility.<ZoneColors>c__Iterator170 expr_07 = <ZoneColors>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}
