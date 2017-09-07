using System;
using UnityEngine;
namespace Verse
{
	public static class SteamUtility
	{
		public static void OpenUrlSteamIfPossible(string url)
		{
			Application.OpenURL(url);
		}
	}
}
