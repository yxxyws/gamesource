using System;
using System.Collections.Generic;
namespace Verse
{
	public static class DebugTools_MapGen
	{
		public static List<DebugMenuOption> Options_Scatterers()
		{
			List<DebugMenuOption> list = new List<DebugMenuOption>();
			foreach (Type current in typeof(Genstep_Scatterer).AllLeafSubclasses())
			{
				Type localSt = current;
				list.Add(new DebugMenuOption(localSt.ToString(), DebugMenuOptionMode.Tool, delegate
				{
					Genstep_Scatterer genstep_Scatterer = (Genstep_Scatterer)Activator.CreateInstance(localSt);
					genstep_Scatterer.DebugForceScatterAt(Gen.MouseCell());
				}));
			}
			return list;
		}
	}
}
