using System;
using Verse;
namespace RimWorld
{
	public class Dialog_MapList_Load : Dialog_MapList
	{
		public Dialog_MapList_Load()
		{
			this.interactButLabel = "LoadGameButton".Translate();
		}
		protected override void DoMapEntryInteraction(string mapName)
		{
			Action preLoadLevelAction = delegate
			{
				MapInitData.Reset();
				MapInitData.mapToLoad = mapName;
			};
			LongEventHandler.QueueLongEvent(preLoadLevelAction, "Gameplay", "LoadingLongEvent", true, null);
		}
	}
}
