using System;
using Verse;
namespace RimWorld.Planet
{
	public static class WorldLoader
	{
		public static void LoadWorldFromFile(string absFilePath)
		{
			Scribe.InitLoading(absFilePath);
			ScribeHeaderUtility.LoadGameDataHeader(ScribeHeaderUtility.ScribeHeaderMode.World);
			Current.World = new World();
			Scribe.EnterNode("world");
			Current.World.ExposeData();
			Scribe.ExitNode();
			CrossRefResolver.ResolveAllCrossReferences();
			PostLoadInitter.DoAllPostLoadInits();
		}
	}
}
