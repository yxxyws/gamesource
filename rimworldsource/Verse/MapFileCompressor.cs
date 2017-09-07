using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse
{
	public class MapFileCompressor : IExposable
	{
		private string compressedString;
		public void ExposeData()
		{
			Scribe_Values.LookValue<string>(ref this.compressedString, "compressedThingMap", null, false);
		}
		public void ReadDataFromMap()
		{
			CompressibilityDecider.DetermineReferences();
			this.compressedString = GridSaveUtility.CompressedStringForShortGrid(new Func<IntVec3, ushort>(this.HashValueForSquare));
		}
		private ushort HashValueForSquare(IntVec3 curSq)
		{
			ushort num = 0;
			foreach (Thing current in Find.ThingGrid.ThingsAt(curSq))
			{
				if (current.IsSaveCompressible())
				{
					if (num != 0)
					{
						Log.Error(string.Concat(new object[]
						{
							"Found two compressible things in ",
							curSq,
							". The last was ",
							current
						}));
					}
					num = current.def.shortHash;
				}
			}
			return num;
		}
		[DebuggerHidden]
		public IEnumerable<Thing> ThingsToSpawnAfterLoad()
		{
			MapFileCompressor.<ThingsToSpawnAfterLoad>c__Iterator158 <ThingsToSpawnAfterLoad>c__Iterator = new MapFileCompressor.<ThingsToSpawnAfterLoad>c__Iterator158();
			<ThingsToSpawnAfterLoad>c__Iterator.<>f__this = this;
			MapFileCompressor.<ThingsToSpawnAfterLoad>c__Iterator158 expr_0E = <ThingsToSpawnAfterLoad>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
