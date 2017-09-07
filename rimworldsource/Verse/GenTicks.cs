using System;
namespace Verse
{
	public static class GenTicks
	{
		public const int TicksPerRealSecond = 60;
		public const int TickRareInterval = 250;
		public const int TickLongInterval = 2000;
		public static int TicksAbs
		{
			get
			{
				return (Game.Mode != GameMode.MapPlaying) ? MapIniter_NewGame.StartingAbsTicks : Find.TickManager.TicksAbs;
			}
		}
	}
}
