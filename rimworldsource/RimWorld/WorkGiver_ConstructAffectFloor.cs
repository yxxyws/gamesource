using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public abstract class WorkGiver_ConstructAffectFloor : WorkGiver_Scanner
	{
		protected abstract DesignationDef DesDef
		{
			get;
		}
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.Touch;
			}
		}
		[DebuggerHidden]
		public override IEnumerable<IntVec3> PotentialWorkCellsGlobal(Pawn pawn)
		{
			WorkGiver_ConstructAffectFloor.<PotentialWorkCellsGlobal>c__Iterator4C <PotentialWorkCellsGlobal>c__Iterator4C = new WorkGiver_ConstructAffectFloor.<PotentialWorkCellsGlobal>c__Iterator4C();
			<PotentialWorkCellsGlobal>c__Iterator4C.pawn = pawn;
			<PotentialWorkCellsGlobal>c__Iterator4C.<$>pawn = pawn;
			<PotentialWorkCellsGlobal>c__Iterator4C.<>f__this = this;
			WorkGiver_ConstructAffectFloor.<PotentialWorkCellsGlobal>c__Iterator4C expr_1C = <PotentialWorkCellsGlobal>c__Iterator4C;
			expr_1C.$PC = -2;
			return expr_1C;
		}
		public override bool HasJobOnCell(Pawn pawn, IntVec3 c)
		{
			return pawn.Faction == Faction.OfColony && Find.DesignationManager.DesignationAt(c, this.DesDef) != null && pawn.CanReserveAndReach(c, PathEndMode.Touch, pawn.NormalMaxDanger(), 1);
		}
	}
}
