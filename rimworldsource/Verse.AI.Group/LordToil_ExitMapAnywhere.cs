using RimWorld;
using System;
namespace Verse.AI.Group
{
	public class LordToil_ExitMapAnywhere : LordToil_ExitMap
	{
		protected override DutyDef DutyDef
		{
			get
			{
				return DutyDefOf.ExitMapAnywhere;
			}
		}
		public LordToil_ExitMapAnywhere(LocomotionUrgency locomotion = LocomotionUrgency.None) : base(locomotion)
		{
		}
	}
}
