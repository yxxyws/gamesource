using System;
using Verse;
namespace RimWorld
{
	public class ThinkNode_ConditionalHasHive : ThinkNode_Conditional
	{
		protected override bool Satisfied(Pawn pawn)
		{
			return pawn.mindState.hive != null && pawn.mindState.hive.Spawned;
		}
	}
}
