using System;
using UnityEngine;
namespace Verse
{
	public static class GenSight
	{
		public static bool LineOfSight(IntVec3 start, IntVec3 end, bool skipFirstCell = false)
		{
			if (!start.InBounds() || !end.InBounds())
			{
				return false;
			}
			int num = Mathf.Abs(end.x - start.x);
			int num2 = Mathf.Abs(end.z - start.z);
			int num3 = start.x;
			int num4 = start.z;
			int i = 1 + num + num2;
			int num5 = (end.x <= start.x) ? -1 : 1;
			int num6 = (end.z <= start.z) ? -1 : 1;
			int num7 = num - num2;
			num *= 2;
			num2 *= 2;
			IntVec3 intVec = default(IntVec3);
			while (i > 1)
			{
				intVec.x = num3;
				intVec.z = num4;
				if ((!skipFirstCell || !(intVec == start)) && !intVec.CanBeSeenOverFast())
				{
					return false;
				}
				if (num7 > 0)
				{
					num3 += num5;
					num7 -= num2;
				}
				else
				{
					num4 += num6;
					num7 += num;
				}
				i--;
			}
			return true;
		}
		public static bool LineOfSight(IntVec3 start, IntVec3 end, CellRect startRect, CellRect endRect)
		{
			if (!start.InBounds() || !end.InBounds())
			{
				return false;
			}
			int num = Mathf.Abs(end.x - start.x);
			int num2 = Mathf.Abs(end.z - start.z);
			int num3 = start.x;
			int num4 = start.z;
			int i = 1 + num + num2;
			int num5 = (end.x <= start.x) ? -1 : 1;
			int num6 = (end.z <= start.z) ? -1 : 1;
			int num7 = num - num2;
			num *= 2;
			num2 *= 2;
			IntVec3 c = default(IntVec3);
			while (i > 1)
			{
				c.x = num3;
				c.z = num4;
				if (endRect.Contains(c))
				{
					return true;
				}
				if (!startRect.Contains(c) && !c.CanBeSeenOverFast())
				{
					return false;
				}
				if (num7 > 0)
				{
					num3 += num5;
					num7 -= num2;
				}
				else
				{
					num4 += num6;
					num7 += num;
				}
				i--;
			}
			return true;
		}
	}
}
