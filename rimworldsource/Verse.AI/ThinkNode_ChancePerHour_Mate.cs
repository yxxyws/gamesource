using System;
namespace Verse.AI
{
	public class ThinkNode_ChancePerHour_Mate : ThinkNode_ChancePerHour
	{
		protected override float ChancePerHour(Pawn pawn)
		{
			return pawn.RaceProps.mateChancePerHour;
		}
	}
}
