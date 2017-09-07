using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;
namespace RimWorld
{
	internal class Building_SunLamp : Building
	{
		public IEnumerable<IntVec3> GrowableCells
		{
			get
			{
				return GenRadial.RadialCellsAround(base.Position, this.def.specialDisplayRadius, true);
			}
		}
		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			Building_SunLamp.<GetGizmos>c__IteratorE7 <GetGizmos>c__IteratorE = new Building_SunLamp.<GetGizmos>c__IteratorE7();
			<GetGizmos>c__IteratorE.<>f__this = this;
			Building_SunLamp.<GetGizmos>c__IteratorE7 expr_0E = <GetGizmos>c__IteratorE;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		private void MakeMatchingGrowZone()
		{
			Designator_ZoneAdd_Growing designator = new Designator_ZoneAdd_Growing();
			designator.DesignateMultiCell(
				from tempCell in this.GrowableCells
				where designator.CanDesignateCell(tempCell).Accepted
				select tempCell);
		}
	}
}
