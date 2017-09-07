using RimWorld.Planet;
using System;
namespace RimWorld
{
	public static class Current
	{
		private static World worldInt;
		public static World World
		{
			get
			{
				return Current.worldInt;
			}
			set
			{
				if (Current.worldInt == value)
				{
					return;
				}
				WorldRenderModeDatabase.Reset();
				Current.worldInt = value;
			}
		}
	}
}
