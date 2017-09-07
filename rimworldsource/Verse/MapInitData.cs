using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Verse
{
	public static class MapInitData
	{
		public const int StartingColonistCount = 3;
		public static bool startedFromEntry;
		public static string mapToLoad;
		public static int mapSize;
		public static DifficultyDef difficulty;
		public static StorytellerDef chosenStoryteller;
		public static IntVec2 landingCoords;
		public static List<Pawn> colonists;
		public static Faction colonyFaction;
		public static Month startingMonth;
		public static bool permadeathMode;
		private static int debug_loadsSinceReset;
		public static bool StartedDirectInEditor
		{
			get
			{
				return MapInitData.mapToLoad.NullOrEmpty() && !MapInitData.startedFromEntry;
			}
		}
		static MapInitData()
		{
			MapInitData.startedFromEntry = false;
			MapInitData.colonists = new List<Pawn>();
			MapInitData.colonyFaction = null;
			MapInitData.startingMonth = Month.Undefined;
			MapInitData.debug_loadsSinceReset = 0;
			MapInitData.Reset();
		}
		public static void Reset()
		{
			MapInitData.startedFromEntry = false;
			MapInitData.mapToLoad = null;
			MapInitData.chosenStoryteller = null;
			MapInitData.difficulty = null;
			MapInitData.mapSize = 250;
			MapInitData.landingCoords = IntVec2.Invalid;
			MapInitData.debug_loadsSinceReset = 0;
			MapInitData.colonists.Clear();
			MapInitData.colonyFaction = null;
			MapInitData.startingMonth = Month.Undefined;
			MapInitData.permadeathMode = false;
		}
		public static void ChooseDecentLandingSite()
		{
			MapInitData.landingCoords = WorldSquareFinder.RandomDecentLandingSite();
		}
		public static void ChooseDefaultStoryteller()
		{
			MapInitData.chosenStoryteller = DefDatabase<StorytellerDef>.GetNamed("Cassandra", true);
			if (MapInitData.chosenStoryteller == null)
			{
				MapInitData.chosenStoryteller = (
					from d in DefDatabase<StorytellerDef>.AllDefs
					orderby d.listOrder
					select d).First<StorytellerDef>();
			}
		}
		public static void ChooseDefaultDifficulty()
		{
			MapInitData.difficulty = DefDatabase<DifficultyDef>.GetNamed("Challenge", true);
		}
		public static void GenerateDefaultColonistsWithFaction()
		{
			MapInitData.ClearAllColonists();
			if (MapInitData.colonyFaction != null)
			{
				MapInitData.colonyFaction.RemoveAllRelations();
			}
			MapInitData.colonyFaction = FactionGenerator.NewColonyFaction();
			do
			{
				MapInitData.ClearAllColonists();
				for (int i = 0; i < 3; i++)
				{
					MapInitData.colonists.Add(MapInitData.NewGeneratedColonist());
				}
			}
			while (!MapInitData.AnyoneCanDoRequiredWorks());
		}
		public static void Notify_MapInited()
		{
			MapInitData.colonists.Clear();
			MapInitData.debug_loadsSinceReset++;
			if (MapInitData.debug_loadsSinceReset > 1)
			{
				Log.Warning("Failed to reset map init params.");
			}
		}
		public static bool AnyoneCanDoRequiredWorks()
		{
			if (MapInitData.colonists.Count == 0)
			{
				return false;
			}
			foreach (WorkTypeDef wt in 
				from w in DefDatabase<WorkTypeDef>.AllDefs
				where w.requireCapableColonist
				select w)
			{
				if (!MapInitData.colonists.Any((Pawn col) => !col.story.WorkTypeIsDisabled(wt)))
				{
					return false;
				}
			}
			return true;
		}
		public static Pawn RegenerateStartingColonist(Pawn p)
		{
			int index = MapInitData.colonists.IndexOf(p);
			PawnUtility.TryDestroyStartingColonistFamily(p);
			p.relations.ClearAllRelations();
			Find.WorldPawns.PassToWorld(p, PawnDiscardDecideMode.Discard);
			MapInitData.colonists[index] = null;
			for (int i = 0; i < MapInitData.colonists.Count; i++)
			{
				if (MapInitData.colonists[i] != null)
				{
					PawnUtility.TryDestroyStartingColonistFamily(MapInitData.colonists[i]);
				}
			}
			Pawn pawn = MapInitData.NewGeneratedColonist();
			MapInitData.colonists[index] = pawn;
			return pawn;
		}
		internal static void SetColonyFactionIntoWorld()
		{
			MapInitData.colonyFaction.homeSquare = MapInitData.landingCoords;
			Find.FactionManager.Add(MapInitData.colonyFaction);
			FactionGenerator.EnsureRequiredEnemies(MapInitData.colonyFaction);
			MapInitData.colonyFaction = null;
		}
		private static Pawn NewGeneratedColonist()
		{
			Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest
			{
				kindDef = PawnKindDefOf.Colonist,
				faction = Faction.OfColony,
				colonistRelationChanceFactor = 26f,
				forceGenerateNewPawn = true
			});
			pawn.relations.everSeenByPlayer = true;
			PawnComponentsUtility.AddComponentsForSpawn(pawn);
			return pawn;
		}
		private static void ClearAllColonists()
		{
			for (int i = MapInitData.colonists.Count - 1; i >= 0; i--)
			{
				MapInitData.colonists[i].relations.ClearAllRelations();
				Find.WorldPawns.PassToWorld(MapInitData.colonists[i], PawnDiscardDecideMode.Discard);
				MapInitData.colonists.RemoveAt(i);
			}
			while (true)
			{
				Pawn pawn = Find.WorldPawns.AllPawnsAliveOrDead.FirstOrDefault((Pawn x) => x.Faction != null && x.Faction.def == FactionDefOf.Colony);
				if (pawn == null)
				{
					break;
				}
				Find.WorldPawns.RemovePawn(pawn);
				pawn.relations.ClearAllRelations();
				Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
			}
		}
	}
}
