using System;
namespace Verse.AI.Group
{
	public static class LordUtility
	{
		public static Lord GetLord(this Pawn p)
		{
			return Find.LordManager.LordOf(p);
		}
		public static ByteGrid GetAvoidGrid(this Pawn p)
		{
			Lord lord = p.GetLord();
			if (lord != null)
			{
				return lord.avoidGrid;
			}
			return null;
		}
	}
}
