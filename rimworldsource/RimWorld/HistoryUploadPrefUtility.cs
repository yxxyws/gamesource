using System;
using Verse;
namespace RimWorld
{
	public static class HistoryUploadPrefUtility
	{
		public static string HumanName(this HistoryUploadPref pref)
		{
			switch (pref)
			{
			case HistoryUploadPref.Unspecified:
				return "Unspecified".Translate();
			case HistoryUploadPref.Always:
				return "Always".Translate();
			case HistoryUploadPref.Never:
				return "Never".Translate();
			default:
				return "Error";
			}
		}
	}
}
