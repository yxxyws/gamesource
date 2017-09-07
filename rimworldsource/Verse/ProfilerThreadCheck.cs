using System;
namespace Verse
{
	public static class ProfilerThreadCheck
	{
		public static void BeginSample(string name)
		{
			if (Game.IsInMainThread)
			{
			}
		}
		public static void EndSample()
		{
			if (Game.IsInMainThread)
			{
			}
		}
	}
}
