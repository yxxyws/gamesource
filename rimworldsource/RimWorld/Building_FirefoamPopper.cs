using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class Building_FirefoamPopper : Building, IUsable
	{
		[DebuggerHidden]
		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn pawn)
		{
			Building_FirefoamPopper.<GetFloatMenuOptions>c__IteratorE0 <GetFloatMenuOptions>c__IteratorE = new Building_FirefoamPopper.<GetFloatMenuOptions>c__IteratorE0();
			<GetFloatMenuOptions>c__IteratorE.pawn = pawn;
			<GetFloatMenuOptions>c__IteratorE.<$>pawn = pawn;
			<GetFloatMenuOptions>c__IteratorE.<>f__this = this;
			Building_FirefoamPopper.<GetFloatMenuOptions>c__IteratorE0 expr_1C = <GetFloatMenuOptions>c__IteratorE;
			expr_1C.$PC = -2;
			return expr_1C;
		}
		public void UsedBy(Pawn user)
		{
			base.GetComp<CompExplosive>().StartWick();
		}
	}
}
