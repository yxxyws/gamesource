using System;
using System.Collections.Generic;
namespace Verse
{
	public static class ShortHashGiver
	{
		private static HashSet<ushort> takenHashes = new HashSet<ushort>();
		public static void GiveShortHash(Def def)
		{
			if (def.shortHash != 0)
			{
				Log.Error(def + " already has short hash.");
				return;
			}
			ushort num = (ushort)(GenText.StableStringHash(def.defName) % 65535);
			int num2 = 0;
			while (ShortHashGiver.takenHashes.Contains(num) || num == 0)
			{
				num += 1;
				num2++;
				if (num2 > 1000)
				{
					Log.Message("Short hashes are saturated because there are too many definitions. Compressed saveload will fail.");
				}
			}
			def.shortHash = num;
			ShortHashGiver.takenHashes.Add(num);
		}
		public static void Reset()
		{
			ShortHashGiver.takenHashes.Clear();
		}
	}
}
