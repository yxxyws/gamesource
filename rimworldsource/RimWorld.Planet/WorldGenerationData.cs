using System;
using Verse;
namespace RimWorld.Planet
{
	public static class WorldGenerationData
	{
		public static readonly IntVec2 DefaultSize;
		public static IntVec2 size;
		public static string seedString;
		static WorldGenerationData()
		{
			WorldGenerationData.DefaultSize = new IntVec2(200, 150);
			WorldGenerationData.Reset();
		}
		public static void Reset()
		{
			WorldGenerationData.size = WorldGenerationData.DefaultSize;
			WorldGenerationData.seedString = WorldGenerationData.RandomSeedString();
		}
		public static string RandomSeedString()
		{
			string text = "abcdefghijklmnopqrstuvwxyz0123456789";
			char[] array = new char[4];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = text[Rand.Range(0, text.Length)];
			}
			return new string(array);
		}
	}
}
