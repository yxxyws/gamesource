using RimWorld;
using System;
namespace Verse.AI.Group
{
	public class LordToil_ExitMapNearest : LordToil_ExitMap
	{
		protected override DutyDef DutyDef
		{
			get
			{
				return DutyDefOf.ExitMapNearest;
			}
		}
		public LordToil_ExitMapNearest(LocomotionUrgency locomotion = LocomotionUrgency.None) : base(locomotion)
		{
		}
	}
}
