using System;
namespace Verse
{
	public class RandomNumberGenerator_XXHash : RandomNumberGenerator
	{
		private const uint PRIME32_1 = 2654435761u;
		private const uint PRIME32_2 = 2246822519u;
		private const uint PRIME32_3 = 3266489917u;
		private const uint PRIME32_4 = 668265263u;
		private const uint PRIME32_5 = 374761393u;
		private uint GetHash(int buf)
		{
			uint num = this.seed + 374761393u;
			num += 4u;
			num += (uint)(buf * -1028477379);
			num = RandomNumberGenerator_XXHash.RotateLeft(num, 17) * 668265263u;
			num ^= num >> 15;
			num *= 2246822519u;
			num ^= num >> 13;
			num *= 3266489917u;
			return num ^ num >> 16;
		}
		public override int GetInt(uint iterations)
		{
			return (int)this.GetHash((int)iterations);
		}
		private static uint RotateLeft(uint value, int count)
		{
			return value << count | value >> 32 - count;
		}
	}
}
