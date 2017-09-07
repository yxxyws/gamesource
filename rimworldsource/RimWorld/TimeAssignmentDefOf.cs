using System;
namespace RimWorld
{
	public static class TimeAssignmentDefOf
	{
		public static TimeAssignmentDef Anything;
		public static TimeAssignmentDef Work;
		public static TimeAssignmentDef Joy;
		public static TimeAssignmentDef Sleep;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<TimeAssignmentDef>(typeof(TimeAssignmentDefOf));
		}
	}
}
