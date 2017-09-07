using RimWorld;
using System;
namespace Verse
{
	public static class PermadeathModeUtility
	{
		public static string GeneratePermadeathSaveName()
		{
			string text = NameGenerator.GenerateName(RulePackDefOf.NamerColony, null);
			int num = 0;
			string text2;
			do
			{
				num++;
				text2 = text;
				if (num != 1)
				{
					text2 += num;
				}
				text2 = PermadeathModeUtility.AppendPermadeathModeSuffix(text2);
			}
			while (MapFilesUtility.HaveMapNamed(text2));
			return text2;
		}
		public static void CheckUpdatePermadeathModeUniqueNameOnGameLoad(string filename)
		{
			if (Find.Map.info.permadeathMode && Find.Map.info.permadeathModeUniqueName != filename)
			{
				Log.Warning("Savefile's name has changed and doesn't match permadeath mode's unique name. Fixing...");
				Find.Map.info.permadeathModeUniqueName = filename;
			}
		}
		private static string AppendPermadeathModeSuffix(string str)
		{
			return str + " " + "PermadeathModeSaveSuffix".Translate();
		}
	}
}
