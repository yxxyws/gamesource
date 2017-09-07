using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse
{
	public static class GraphicDatabaseUtility
	{
		[DebuggerHidden]
		public static IEnumerable<string> GraphicNamesInFolder(string folderPath)
		{
			GraphicDatabaseUtility.<GraphicNamesInFolder>c__Iterator18B <GraphicNamesInFolder>c__Iterator18B = new GraphicDatabaseUtility.<GraphicNamesInFolder>c__Iterator18B();
			<GraphicNamesInFolder>c__Iterator18B.folderPath = folderPath;
			<GraphicNamesInFolder>c__Iterator18B.<$>folderPath = folderPath;
			GraphicDatabaseUtility.<GraphicNamesInFolder>c__Iterator18B expr_15 = <GraphicNamesInFolder>c__Iterator18B;
			expr_15.$PC = -2;
			return expr_15;
		}
	}
}
