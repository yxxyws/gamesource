using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public abstract class WorkGiver_Grower : WorkGiver_Scanner
	{
		protected static ThingDef wantedPlantDef;
		protected virtual bool ExtraRequirements(IPlantToGrowSettable settable)
		{
			return true;
		}
		[DebuggerHidden]
		public override IEnumerable<IntVec3> PotentialWorkCellsGlobal(Pawn pawn)
		{
			WorkGiver_Grower.<PotentialWorkCellsGlobal>c__Iterator4E <PotentialWorkCellsGlobal>c__Iterator4E = new WorkGiver_Grower.<PotentialWorkCellsGlobal>c__Iterator4E();
			<PotentialWorkCellsGlobal>c__Iterator4E.pawn = pawn;
			<PotentialWorkCellsGlobal>c__Iterator4E.<$>pawn = pawn;
			<PotentialWorkCellsGlobal>c__Iterator4E.<>f__this = this;
			WorkGiver_Grower.<PotentialWorkCellsGlobal>c__Iterator4E expr_1C = <PotentialWorkCellsGlobal>c__Iterator4E;
			expr_1C.$PC = -2;
			return expr_1C;
		}
		protected void DetermineWantedPlantDef(IntVec3 c)
		{
			IPlantToGrowSettable plantToGrowSettable = c.GetEdifice() as IPlantToGrowSettable;
			if (plantToGrowSettable == null)
			{
				plantToGrowSettable = (Find.ZoneManager.ZoneAt(c) as IPlantToGrowSettable);
			}
			if (plantToGrowSettable == null)
			{
				WorkGiver_Grower.wantedPlantDef = null;
			}
			else
			{
				WorkGiver_Grower.wantedPlantDef = plantToGrowSettable.GetPlantDefToGrow();
			}
		}
	}
}
