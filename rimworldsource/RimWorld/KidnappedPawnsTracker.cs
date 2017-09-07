using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;
namespace RimWorld
{
	public class KidnappedPawnsTracker : IExposable
	{
		private const int TryRecruitInterval = 15051;
		private const float RecruitMTBDays = 30f;
		private Faction faction;
		private List<Pawn> kidnappedPawns = new List<Pawn>();
		public List<Pawn> KidnappedPawnsListForReading
		{
			get
			{
				return this.kidnappedPawns;
			}
		}
		public KidnappedPawnsTracker(Faction faction)
		{
			this.faction = faction;
		}
		public void ExposeData()
		{
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				this.kidnappedPawns.RemoveAll((Pawn x) => x.Destroyed);
			}
			Scribe_Collections.LookList<Pawn>(ref this.kidnappedPawns, "kidnappedPawns", LookMode.MapReference, new object[0]);
		}
		public void KidnapPawn(Pawn pawn, Pawn kidnapper)
		{
			if (this.kidnappedPawns.Contains(pawn))
			{
				Log.Error("Tried to kidnap already kidnapped pawn " + pawn);
				return;
			}
			if (pawn.Faction == this.faction)
			{
				Log.Error("Tried to kidnap pawn with the same faction: " + pawn);
				return;
			}
			pawn.PreKidnapped(kidnapper);
			if (pawn.Spawned)
			{
				pawn.DeSpawn();
			}
			this.kidnappedPawns.Add(pawn);
			Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Keep);
		}
		public void LogKidnappedPawns()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.faction.name + ":");
			for (int i = 0; i < this.kidnappedPawns.Count; i++)
			{
				stringBuilder.AppendLine(this.kidnappedPawns[i].Name.ToStringFull);
			}
			Log.Message(stringBuilder.ToString());
		}
		public void KidnappedPawnsTrackerTick()
		{
			this.kidnappedPawns.RemoveAll((Pawn x) => x.Dead);
			if (Find.TickManager.TicksGame % 15051 == 0)
			{
				for (int i = this.kidnappedPawns.Count - 1; i >= 0; i--)
				{
					if (Rand.MTBEventOccurs(30f, 60000f, 15051f))
					{
						this.kidnappedPawns[i].SetFaction(this.faction, null);
						this.kidnappedPawns.RemoveAt(i);
					}
				}
			}
		}
	}
}
