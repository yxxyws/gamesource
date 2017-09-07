using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Verse
{
	public sealed class MapPawns
	{
		private List<Pawn> pawnsSpawned = new List<Pawn>();
		private Dictionary<Faction, List<Pawn>> pawnsInFactionSpawned = new Dictionary<Faction, List<Pawn>>();
		private List<Pawn> prisonersOfColonySpawned = new List<Pawn>();
		public IEnumerable<Pawn> AllPawns
		{
			get
			{
				MapPawns.<>c__Iterator161 <>c__Iterator = new MapPawns.<>c__Iterator161();
				<>c__Iterator.<>f__this = this;
				MapPawns.<>c__Iterator161 expr_0E = <>c__Iterator;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public IEnumerable<Pawn> AllPawnsUnspawned
		{
			get
			{
				MapPawns.<>c__Iterator162 <>c__Iterator = new MapPawns.<>c__Iterator162();
				<>c__Iterator.<>f__this = this;
				MapPawns.<>c__Iterator162 expr_0E = <>c__Iterator;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public IEnumerable<Pawn> FreeColonists
		{
			get
			{
				return 
					from p in this.PawnsInFaction(Faction.OfColony)
					where p.HostFaction == null && p.RaceProps.Humanlike
					select p;
			}
		}
		public IEnumerable<Pawn> PrisonersOfColony
		{
			get
			{
				return 
					from x in this.AllPawns
					where x.IsPrisonerOfColony
					select x;
			}
		}
		public IEnumerable<Pawn> FreeColonistsAndPrisoners
		{
			get
			{
				return this.FreeColonists.Concat(this.PrisonersOfColony);
			}
		}
		public int ColonistCount
		{
			get
			{
				if (Game.Mode != GameMode.MapPlaying)
				{
					return 3;
				}
				return (
					from x in this.AllPawns
					where x.RaceProps.Humanlike && x.Faction == Faction.OfColony
					select x).Count<Pawn>();
			}
		}
		public int AllPawnsCount
		{
			get
			{
				return this.AllPawns.Count<Pawn>();
			}
		}
		public int AllPawnsUnspawnedCount
		{
			get
			{
				return this.AllPawnsUnspawned.Count<Pawn>();
			}
		}
		public int FreeColonistsCount
		{
			get
			{
				return this.FreeColonists.Count<Pawn>();
			}
		}
		public int PrisonersOfColonyCount
		{
			get
			{
				return this.PrisonersOfColony.Count<Pawn>();
			}
		}
		public int FreeColonistsAndPrisonersCount
		{
			get
			{
				return this.PrisonersOfColony.Count<Pawn>();
			}
		}
		public List<Pawn> AllPawnsSpawned
		{
			get
			{
				return this.pawnsSpawned;
			}
		}
		public IEnumerable<Pawn> FreeColonistsSpawned
		{
			get
			{
				return 
					from p in this.SpawnedPawnsInFaction(Faction.OfColony)
					where p.HostFaction == null && p.RaceProps.Humanlike
					select p;
			}
		}
		public List<Pawn> PrisonersOfColonySpawned
		{
			get
			{
				return this.prisonersOfColonySpawned;
			}
		}
		public IEnumerable<Pawn> FreeColonistsAndPrisonersSpawned
		{
			get
			{
				return this.FreeColonistsSpawned.Concat(this.PrisonersOfColonySpawned);
			}
		}
		public int AllPawnsSpawnedCount
		{
			get
			{
				return this.pawnsSpawned.Count;
			}
		}
		public int FreeColonistsSpawnedCount
		{
			get
			{
				return this.FreeColonistsSpawned.Count<Pawn>();
			}
		}
		public int PrisonersOfColonySpawnedCount
		{
			get
			{
				return this.PrisonersOfColonySpawned.Count;
			}
		}
		public int FreeColonistsAndPrisonersSpawnedCount
		{
			get
			{
				return this.FreeColonistsAndPrisonersSpawned.Count<Pawn>();
			}
		}
		public int ColonistsSpawnedCount
		{
			get
			{
				int num = 0;
				List<Pawn> list = this.SpawnedPawnsInFaction(Faction.OfColony);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].RaceProps.Humanlike)
					{
						num++;
					}
				}
				return num;
			}
		}
		public int FreeColonistsSpawnedOrInPlayerEjectablePodsCount
		{
			get
			{
				int num = 0;
				for (int i = 0; i < this.pawnsSpawned.Count; i++)
				{
					if (this.pawnsSpawned[i].Faction == Faction.OfColony && this.pawnsSpawned[i].HostFaction == null && this.pawnsSpawned[i].RaceProps.Humanlike)
					{
						num++;
					}
				}
				List<Thing> list = Find.ListerThings.ThingsInGroup(ThingRequestGroup.Container);
				for (int j = 0; j < list.Count; j++)
				{
					Building_CryptosleepCasket building_CryptosleepCasket = list[j] as Building_CryptosleepCasket;
					if (building_CryptosleepCasket != null && building_CryptosleepCasket.def.building.isPlayerEjectable)
					{
						Pawn pawn = building_CryptosleepCasket.ContainedThing as Pawn;
						if (pawn != null)
						{
							if (pawn.Faction == Faction.OfColony && pawn.HostFaction == null && pawn.RaceProps.Humanlike)
							{
								num++;
							}
						}
					}
				}
				return num;
			}
		}
		private void EnsureFactionsListsInit()
		{
			List<Faction> allFactionsListForReading = Find.FactionManager.AllFactionsListForReading;
			for (int i = 0; i < allFactionsListForReading.Count; i++)
			{
				if (!this.pawnsInFactionSpawned.ContainsKey(allFactionsListForReading[i]))
				{
					this.pawnsInFactionSpawned.Add(allFactionsListForReading[i], new List<Pawn>());
				}
			}
		}
		public IEnumerable<Pawn> PawnsInFaction(Faction faction)
		{
			if (faction == null)
			{
				Log.Error("Called PawnsInFaction with null faction.");
				return new List<Pawn>();
			}
			return 
				from x in this.AllPawns
				where x.Faction == faction
				select x;
		}
		public List<Pawn> SpawnedPawnsInFaction(Faction faction)
		{
			this.EnsureFactionsListsInit();
			if (faction == null)
			{
				Log.Error("Called SpawnedPawnsInFaction with null faction.");
				return new List<Pawn>();
			}
			return this.pawnsInFactionSpawned[faction];
		}
		public void RegisterPawn(Pawn p)
		{
			if (!p.mindState.Active)
			{
				return;
			}
			if (!p.Spawned)
			{
				Log.Warning(string.Concat(new object[]
				{
					"Tried to register despawned pawn ",
					p,
					" in ",
					base.GetType(),
					"."
				}));
				return;
			}
			this.EnsureFactionsListsInit();
			if (!this.pawnsSpawned.Contains(p))
			{
				this.pawnsSpawned.Add(p);
			}
			if (p.Faction != null && !this.pawnsInFactionSpawned[p.Faction].Contains(p))
			{
				this.pawnsInFactionSpawned[p.Faction].Add(p);
				if (p.Faction == Faction.OfColony)
				{
					this.pawnsInFactionSpawned[Faction.OfColony].InsertionSort(delegate(Pawn a, Pawn b)
					{
						int num = (a.playerSettings == null) ? 0 : a.playerSettings.joinTick;
						int value = (b.playerSettings == null) ? 0 : b.playerSettings.joinTick;
						return num.CompareTo(value);
					});
				}
			}
			if (p.IsPrisonerOfColony && !this.prisonersOfColonySpawned.Contains(p))
			{
				this.prisonersOfColonySpawned.Add(p);
			}
			this.DoListChangedNotifications();
		}
		public void DeRegisterPawn(Pawn p)
		{
			this.EnsureFactionsListsInit();
			this.pawnsSpawned.Remove(p);
			List<Faction> allFactionsListForReading = Find.FactionManager.AllFactionsListForReading;
			for (int i = 0; i < allFactionsListForReading.Count; i++)
			{
				Faction key = allFactionsListForReading[i];
				this.pawnsInFactionSpawned[key].Remove(p);
			}
			this.prisonersOfColonySpawned.Remove(p);
			this.DoListChangedNotifications();
		}
		public void UpdateRegistryForPawn(Pawn p)
		{
			this.DeRegisterPawn(p);
			if (p.Spawned)
			{
				this.RegisterPawn(p);
			}
			this.DoListChangedNotifications();
		}
		private void DoListChangedNotifications()
		{
			if (Find.WindowStack != null)
			{
				WindowStack windowStack = Find.WindowStack;
				for (int i = 0; i < windowStack.Count; i++)
				{
					MainTabWindow_PawnList mainTabWindow_PawnList = windowStack[i] as MainTabWindow_PawnList;
					if (mainTabWindow_PawnList != null)
					{
						mainTabWindow_PawnList.Notify_PawnsChanged();
					}
				}
			}
		}
		public void LogListedPawns()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("ListerPawns:");
			stringBuilder.AppendLine("pawnsSpawned");
			foreach (Pawn current in this.pawnsSpawned)
			{
				stringBuilder.AppendLine("    " + current.ToString());
			}
			foreach (KeyValuePair<Faction, List<Pawn>> current2 in this.pawnsInFactionSpawned)
			{
				stringBuilder.AppendLine("pawnsInFactionSpawned[" + current2.Key.ToString() + "]");
				foreach (Pawn current3 in current2.Value)
				{
					stringBuilder.AppendLine("    " + current3.ToString());
				}
			}
			stringBuilder.AppendLine("prisonersOfColonySpawned");
			foreach (Pawn current4 in this.prisonersOfColonySpawned)
			{
				stringBuilder.AppendLine("    " + current4.ToString());
			}
			Log.Message(stringBuilder.ToString());
		}
	}
}
