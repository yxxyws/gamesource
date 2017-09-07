using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
namespace RimWorld
{
	public static class IncidentDefUtility
	{
		public static IEnumerable<IncidentDef> CurrentlyUsableIncidents
		{
			get
			{
				return 
					from def in DefDatabase<IncidentDef>.AllDefs
					where def.Worker.StorytellerCanUseNow()
					select def;
			}
		}
	}
}
