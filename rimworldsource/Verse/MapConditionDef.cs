using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse
{
	public class MapConditionDef : Def
	{
		public Type conditionClass = typeof(MapCondition);
		public PsychicDroneLevel droneLevel = PsychicDroneLevel.BadMedium;
		public string endMessage;
		public static MapConditionDef Named(string defName)
		{
			return DefDatabase<MapConditionDef>.GetNamed(defName, true);
		}
		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			MapConditionDef.<ConfigErrors>c__Iterator13E <ConfigErrors>c__Iterator13E = new MapConditionDef.<ConfigErrors>c__Iterator13E();
			<ConfigErrors>c__Iterator13E.<>f__this = this;
			MapConditionDef.<ConfigErrors>c__Iterator13E expr_0E = <ConfigErrors>c__Iterator13E;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
