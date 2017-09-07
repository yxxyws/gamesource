using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JoyGiver_InteractBuildingInteractionCell : JoyGiver_InteractBuilding
	{
		protected override Job TryGivePlayJob(Pawn pawn, Thing t)
		{
			if (t.InteractionCell.Standable() && !t.IsForbidden(pawn) && !t.InteractionCell.IsForbidden(pawn) && !Find.PawnDestinationManager.DestinationIsReserved(t.InteractionCell))
			{
				return new Job(this.def.jobDef, t, t.InteractionCell);
			}
			return null;
		}
	}
}
