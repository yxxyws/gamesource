using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class Building_BlastingCharge : Building
	{
		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			Building_BlastingCharge.<GetGizmos>c__IteratorCF <GetGizmos>c__IteratorCF = new Building_BlastingCharge.<GetGizmos>c__IteratorCF();
			<GetGizmos>c__IteratorCF.<>f__this = this;
			Building_BlastingCharge.<GetGizmos>c__IteratorCF expr_0E = <GetGizmos>c__IteratorCF;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		private void Command_Detonate()
		{
			base.GetComp<CompExplosive>().StartWick();
		}
	}
}
