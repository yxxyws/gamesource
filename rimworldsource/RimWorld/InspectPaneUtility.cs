using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class InspectPaneUtility
	{
		private static List<Thing> selectedThings = new List<Thing>();
		private static Dictionary<string, string> truncatedLabels = new Dictionary<string, string>();
		public static bool CanInspectTogether(object A, object B)
		{
			Thing thing = A as Thing;
			Thing thing2 = B as Thing;
			return thing != null && thing2 != null && thing.def.category != ThingCategory.Pawn && thing.def == thing2.def;
		}
		public static string AdjustedLabelFor(IEnumerable<object> selected, Rect rect)
		{
			Zone zone = selected.First<object>() as Zone;
			string str;
			if (zone != null)
			{
				str = zone.label;
			}
			else
			{
				InspectPaneUtility.selectedThings.Clear();
				foreach (object current in selected)
				{
					Thing thing = current as Thing;
					if (thing != null)
					{
						InspectPaneUtility.selectedThings.Add(thing);
					}
				}
				if (InspectPaneUtility.selectedThings.Count == 1)
				{
					str = InspectPaneUtility.selectedThings[0].LabelCap;
				}
				else
				{
					IEnumerable<IGrouping<string, Thing>> source = 
						from th in InspectPaneUtility.selectedThings
						group th by th.LabelBaseCap into g
						select g;
					if (source.Count<IGrouping<string, Thing>>() > 1)
					{
						str = "VariousLabel".Translate();
					}
					else
					{
						str = InspectPaneUtility.selectedThings[0].LabelBaseCap + " x" + InspectPaneUtility.selectedThings.Count;
					}
				}
			}
			Text.Font = GameFont.Medium;
			return str.Truncate(rect.width, InspectPaneUtility.truncatedLabels);
		}
	}
}
