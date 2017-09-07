using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public static class ThingSelectionUtility
	{
		private static HashSet<Thing> yieldedThings = new HashSet<Thing>();
		public static bool SelectableNow(this Thing t)
		{
			if (!t.def.selectable || !t.Spawned)
			{
				return false;
			}
			if (t.def.size.x == 1 && t.def.size.z == 1)
			{
				return !t.Position.Fogged();
			}
			CellRect.CellRectIterator iterator = t.OccupiedRect().GetIterator();
			while (!iterator.Done())
			{
				if (!iterator.Current.Fogged())
				{
					return true;
				}
				iterator.MoveNext();
			}
			return false;
		}
		[DebuggerHidden]
		public static IEnumerable<Thing> MultiSelectableThingsInScreenRectDistinct(Rect rect)
		{
			ThingSelectionUtility.<MultiSelectableThingsInScreenRectDistinct>c__Iterator117 <MultiSelectableThingsInScreenRectDistinct>c__Iterator = new ThingSelectionUtility.<MultiSelectableThingsInScreenRectDistinct>c__Iterator117();
			<MultiSelectableThingsInScreenRectDistinct>c__Iterator.rect = rect;
			<MultiSelectableThingsInScreenRectDistinct>c__Iterator.<$>rect = rect;
			ThingSelectionUtility.<MultiSelectableThingsInScreenRectDistinct>c__Iterator117 expr_15 = <MultiSelectableThingsInScreenRectDistinct>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}
	}
}
