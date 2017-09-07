using RimWorld.Planet;
using System;
using System.Linq;
using Verse;
namespace RimWorld
{
	public static class FactionGenerator
	{
		private const int MinStartVisibleFactions = 5;
		public static Faction NewColonyFaction()
		{
			Faction faction = FactionGenerator.NewGeneratedFaction(FactionDef.Named("Colony"));
			faction.name = "Colony".Translate();
			return faction;
		}
		public static void GenerateFactionsIntoCurrentWorld()
		{
			int i = 0;
			foreach (FactionDef current in DefDatabase<FactionDef>.AllDefs)
			{
				for (int j = 0; j < current.requiredCountAtGameStart; j++)
				{
					Faction faction = FactionGenerator.NewGeneratedFaction(current);
					Current.World.factionManager.Add(faction);
					if (!current.hidden)
					{
						i++;
					}
				}
			}
			while (i < 5)
			{
				Faction faction2 = FactionGenerator.NewGeneratedFaction((
					from fa in DefDatabase<FactionDef>.AllDefs
					where fa.canMakeRandomly
					select fa).RandomElement<FactionDef>());
				Current.World.factionManager.Add(faction2);
				i++;
			}
		}
		public static void EnsureRequiredEnemies(Faction col)
		{
			FactionGenerator.<EnsureRequiredEnemies>c__AnonStorey246 <EnsureRequiredEnemies>c__AnonStorey = new FactionGenerator.<EnsureRequiredEnemies>c__AnonStorey246();
			<EnsureRequiredEnemies>c__AnonStorey.col = col;
			foreach (FactionDef facDef in DefDatabase<FactionDef>.AllDefs)
			{
				if (facDef.mustStartOneEnemy && Current.World.factionManager.AllFactions.Any((Faction f) => f.def == facDef) && !Current.World.factionManager.AllFactions.Any((Faction f) => f.def == facDef && f.HostileTo(<EnsureRequiredEnemies>c__AnonStorey.col)))
				{
					Faction faction = (
						from f in Current.World.factionManager.AllFactions
						where f.def == facDef
						select f).RandomElement<Faction>();
					float goodwillChange = -(faction.GoodwillWith(<EnsureRequiredEnemies>c__AnonStorey.col) + 100f) * Rand.Range(0.8f, 0.9f);
					faction.AffectGoodwillWith(<EnsureRequiredEnemies>c__AnonStorey.col, goodwillChange);
				}
			}
		}
		public static Faction NewGeneratedFaction()
		{
			return FactionGenerator.NewGeneratedFaction(DefDatabase<FactionDef>.GetRandom());
		}
		public static Faction NewGeneratedFaction(FactionDef facDef)
		{
			Faction faction = new Faction();
			faction.def = facDef;
			faction.loadID = Find.World.uniqueIDsManager.GetNextFactionID();
			if (facDef.fixedName != null)
			{
				faction.name = facDef.fixedName;
			}
			else
			{
				faction.name = NameGenerator.GenerateName(facDef.factionNameMaker, 
					from fac in Find.FactionManager.AllFactionsVisible
					select fac.name);
			}
			foreach (Faction current in Find.FactionManager.AllFactionsListForReading)
			{
				faction.TryMakeInitialRelationsWith(current);
			}
			if (!facDef.hidden)
			{
				faction.homeSquare = FactionGenerator.RandomHomeSquareFor(faction);
			}
			faction.GenerateNewLeader();
			return faction;
		}
		private static IntVec2 RandomHomeSquareFor(Faction faction)
		{
			for (int i = 0; i < 2000; i++)
			{
				IntVec2 intVec = new IntVec2(Rand.Range(0, Current.World.Size.x), Rand.Range(0, Current.World.Size.z));
				WorldSquare worldSquare = Current.World.grid.Get(intVec);
				if (worldSquare.biome.canBuildBase)
				{
					if (Find.FactionManager.FactionInSquare(intVec) == null)
					{
						return intVec;
					}
				}
			}
			Log.Error("Failed to find home square for " + faction);
			return new IntVec2(0, 0);
		}
	}
}
