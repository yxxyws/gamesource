using RimWorld.Planet;
using System;
namespace RimWorld
{
	public class BiomeWorker_TemperateForest : BiomeWorker
	{
		public override float GetScore(WorldSquare square)
		{
			if (square.elevation <= 0f)
			{
				return -100f;
			}
			if (square.temperature < -10f)
			{
				return 0f;
			}
			if (square.rainfall < 600f)
			{
				return 0f;
			}
			return 15f + (square.temperature - 7f) + (square.rainfall - 600f) / 180f;
		}
	}
}
