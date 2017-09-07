using System;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.AI.Group;
namespace RimWorld
{
	public class LordToil_PanicFlee : LordToil
	{
		public override bool AllowSatisfyLongNeeds
		{
			get
			{
				return false;
			}
		}
		public override void Init()
		{
			base.Init();
			foreach (Pawn current in this.lord.ownedPawns.ToList<Pawn>())
			{
				current.mindState.mentalStateHandler.StartMentalState(MentalStateDefOf.PanicFlee);
			}
		}
		public override void UpdateAllDuties()
		{
			foreach (Pawn current in this.lord.ownedPawns)
			{
				current.mindState.duty = new PawnDuty(DutyDefOf.ExitMapAnywhere);
			}
		}
	}
}
