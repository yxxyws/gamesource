using System;
using UnityEngine;
namespace Verse
{
	internal static class QuickStarter
	{
		private static bool quickStarted;
		public static bool CheckQuickStart()
		{
			if (GenCommandLine.CommandLineArgPassed("quicktest") && !QuickStarter.quickStarted && Application.loadedLevelName == "Entry")
			{
				QuickStarter.quickStarted = true;
				Application.LoadLevel("Gameplay");
				return true;
			}
			return false;
		}
	}
}
