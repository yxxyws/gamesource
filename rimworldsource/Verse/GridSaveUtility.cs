using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse
{
	public static class GridSaveUtility
	{
		public struct LoadedGridShort
		{
			public ushort val;
			public IntVec3 cell;
		}
		public static string CompressedStringForShortGrid(Func<IntVec3, ushort> shortGetter)
		{
			int numCells = Find.Map.info.NumCells;
			byte[] array = new byte[numCells * 2];
			IntVec3 arg = new IntVec3(0, 0, 0);
			int num = 0;
			while (true)
			{
				ushort num2 = shortGetter(arg);
				byte b = (byte)(num2 % 256);
				byte b2 = (byte)(num2 / 256);
				array[num] = b;
				array[num + 1] = b2;
				num += 2;
				arg.x++;
				if (arg.x >= Find.Map.Size.x)
				{
					arg.x = 0;
					arg.z++;
					if (arg.z >= Find.Map.Size.z)
					{
						break;
					}
				}
			}
			string str = Convert.ToBase64String(array);
			return ArrayExposeUtility.AddLineBreaksToLongString(str);
		}
		[DebuggerHidden]
		public static IEnumerable<GridSaveUtility.LoadedGridShort> LoadedUShortGrid(string compressedString)
		{
			GridSaveUtility.<LoadedUShortGrid>c__Iterator16F <LoadedUShortGrid>c__Iterator16F = new GridSaveUtility.<LoadedUShortGrid>c__Iterator16F();
			<LoadedUShortGrid>c__Iterator16F.compressedString = compressedString;
			<LoadedUShortGrid>c__Iterator16F.<$>compressedString = compressedString;
			GridSaveUtility.<LoadedUShortGrid>c__Iterator16F expr_15 = <LoadedUShortGrid>c__Iterator16F;
			expr_15.$PC = -2;
			return expr_15;
		}
	}
}
