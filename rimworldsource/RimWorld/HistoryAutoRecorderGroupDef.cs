using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class HistoryAutoRecorderGroupDef : Def
	{
		public string graphLabelY = "Value";
		public bool useFixedScale;
		public Vector2 fixedScale = default(Vector2);
		public List<string> historyAutoRecorderDefs = new List<string>();
		public static HistoryAutoRecorderGroupDef Named(string defName)
		{
			return DefDatabase<HistoryAutoRecorderGroupDef>.GetNamed(defName, true);
		}
	}
}
