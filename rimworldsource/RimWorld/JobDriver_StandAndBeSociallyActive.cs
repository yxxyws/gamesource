using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_StandAndBeSociallyActive : JobDriver
	{
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_StandAndBeSociallyActive.<MakeNewToils>c__Iterator11 <MakeNewToils>c__Iterator = new JobDriver_StandAndBeSociallyActive.<MakeNewToils>c__Iterator11();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_StandAndBeSociallyActive.<MakeNewToils>c__Iterator11 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		private Pawn FindClosePawn()
		{
			IntVec3 position = this.pawn.Position;
			for (int i = 0; i < 24; i++)
			{
				IntVec3 intVec = position + GenRadial.RadialPattern[i];
				if (intVec.InBounds())
				{
					Thing thing = intVec.GetThingList().Find((Thing x) => x is Pawn);
					if (thing != null && thing != this.pawn)
					{
						if (GenSight.LineOfSight(position, intVec, false))
						{
							return (Pawn)thing;
						}
					}
				}
			}
			return null;
		}
	}
}
