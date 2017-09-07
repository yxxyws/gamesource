using System;
using Verse.AI;
namespace RimWorld
{
	public static class DutyDefOf
	{
		public static DutyDef Travel;
		public static DutyDef Kidnap;
		public static DutyDef TakeWoundedGuest;
		public static DutyDef Follow;
		public static DutyDef PrisonerEscape;
		public static DutyDef PrisonerEscapeSapper;
		public static DutyDef AssaultColony;
		public static DutyDef Sapper;
		public static DutyDef Escort;
		public static DutyDef Defend;
		public static DutyDef Build;
		public static DutyDef HuntEnemiesIndividual;
		public static DutyDef ExitMapAnywhere;
		public static DutyDef ExitMapNearest;
		public static DutyDef MarryPawn;
		public static DutyDef Spectate;
		public static DutyDef Party;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<DutyDef>(typeof(DutyDefOf));
		}
	}
}
