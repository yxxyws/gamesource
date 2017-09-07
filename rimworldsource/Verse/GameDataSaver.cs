using RimWorld.Planet;
using System;
namespace Verse
{
	public static class GameDataSaver
	{
		public const string SavedWorldParentNodeName = "savedWorld";
		public const string SavedGameParentNodeName = "savegame";
		public const string WorldNodeName = "world";
		public const string MapNodeName = "map";
		public static void SaveWorld(World world)
		{
			try
			{
				SafeSaver.Save(GenFilePaths.FilePathForWorld(world.info.FileNameNoExtension), "savedWorld", delegate
				{
					ScribeHeaderUtility.WriteGameDataHeader();
					Scribe_Deep.LookDeep<World>(ref world, "world", new object[0]);
				});
			}
			catch (Exception ex)
			{
				Log.Error("Exception while saving world: " + ex.ToString());
			}
		}
		public static void SaveGame(Map map, string fileName)
		{
			try
			{
				SafeSaver.Save(GenFilePaths.FilePathForSavedGame(fileName), "savegame", delegate
				{
					ScribeHeaderUtility.WriteGameDataHeader();
					World world = map.World;
					Scribe_Deep.LookDeep<World>(ref world, "world", new object[0]);
					Scribe_Deep.LookDeep<Map>(ref map, "map", new object[0]);
				});
			}
			catch (Exception ex)
			{
				Log.Error("Exception while saving map: " + ex.ToString());
			}
		}
	}
}
