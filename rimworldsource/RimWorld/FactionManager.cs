using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
namespace RimWorld
{
	public class FactionManager : IExposable
	{
		private List<Faction> allFactions = new List<Faction>();
		public List<Faction> AllFactionsListForReading
		{
			get
			{
				return this.allFactions;
			}
		}
		public IEnumerable<Faction> AllFactions
		{
			get
			{
				return this.allFactions;
			}
		}
		public IEnumerable<Faction> AllFactionsVisible
		{
			get
			{
				return 
					from fa in this.allFactions
					where !fa.def.hidden
					select fa;
			}
		}
		public IEnumerable<Faction> AllFactionsInViewOrder
		{
			get
			{
				return 
					from fa in this.AllFactionsVisible
					orderby fa.def.startingGoodwill.Average descending
					select fa;
			}
		}
		public void ExposeData()
		{
			Scribe_Collections.LookList<Faction>(ref this.allFactions, "allFactions", LookMode.Deep, new object[0]);
		}
		public void Add(Faction faction)
		{
			this.allFactions.Add(faction);
			Find.World.renderer.Notify_WorldChanged();
		}
		public void FactionManagerTick()
		{
			for (int i = 0; i < this.allFactions.Count; i++)
			{
				this.allFactions[i].FactionTick();
			}
		}
		public Faction FirstFactionOfDef(FactionDef facDef)
		{
			for (int i = 0; i < this.allFactions.Count; i++)
			{
				if (this.allFactions[i].def == facDef)
				{
					return this.allFactions[i];
				}
			}
			return null;
		}
		public Faction FactionInSquare(IntVec2 coords)
		{
			for (int i = 0; i < this.allFactions.Count; i++)
			{
				if (this.allFactions[i].homeSquare == coords)
				{
					return this.allFactions[i];
				}
			}
			return null;
		}
		public bool TryGetRandomNonColonyHumanlikeFaction(out Faction faction, bool tryMedievalOrBetter)
		{
			IEnumerable<Faction> source = 
				from x in this.AllFactions
				where x != Faction.OfColony && !x.def.hidden && x.def.humanlikeFaction
				select x;
			return source.TryRandomElementByWeight(delegate(Faction x)
			{
				if (tryMedievalOrBetter && x.def.techLevel < TechLevel.Medieval)
				{
					return 0.1f;
				}
				return 1f;
			}, out faction);
		}
		public void LogKidnappedPawns()
		{
			Log.Message("Kidnapped pawns:");
			for (int i = 0; i < this.allFactions.Count; i++)
			{
				this.allFactions[i].kidnapped.LogKidnappedPawns();
			}
		}
	}
}
