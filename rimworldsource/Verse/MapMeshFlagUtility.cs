using System;
using System.Collections;
using System.Collections.Generic;
namespace Verse
{
	internal static class MapMeshFlagUtility
	{
		public static List<MapMeshFlag> allFlags;
		static MapMeshFlagUtility()
		{
			MapMeshFlagUtility.allFlags = new List<MapMeshFlag>();
			IEnumerator enumerator = Enum.GetValues(typeof(MapMeshFlag)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					MapMeshFlag mapMeshFlag = (MapMeshFlag)((int)enumerator.Current);
					if (mapMeshFlag != MapMeshFlag.None)
					{
						MapMeshFlagUtility.allFlags.Add(mapMeshFlag);
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}
	}
}
