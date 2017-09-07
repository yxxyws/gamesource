using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class Pawn_PlayerSettings : IExposable
	{
		private Pawn pawn;
		private Area areaAllowedInt;
		public int joinTick = -1;
		public Pawn master;
		public bool animalsReleased;
		public MedicalCareCategory medCare = MedicalCareCategory.Best;
		public HostilityResponseMode hostilityResponse = HostilityResponseMode.Flee;
		public Area AreaRestriction
		{
			get
			{
				if (!this.RespectsAllowedArea)
				{
					return null;
				}
				return this.areaAllowedInt;
			}
			set
			{
				this.areaAllowedInt = value;
			}
		}
		public bool RespectsAllowedArea
		{
			get
			{
				return this.pawn.Faction == Faction.OfColony || this.pawn.HostFaction == Faction.OfColony;
			}
		}
		public Pawn_PlayerSettings(Pawn pawn)
		{
			this.pawn = pawn;
			if (Game.Mode == GameMode.MapPlaying)
			{
				this.joinTick = Find.TickManager.TicksGame;
			}
			else
			{
				this.joinTick = 0;
			}
			if (pawn.RaceProps.Humanlike)
			{
				if (pawn.IsPrisoner)
				{
					this.medCare = MedicalCareCategory.NoMeds;
				}
				else
				{
					this.medCare = MedicalCareCategory.Best;
				}
			}
			else
			{
				this.medCare = MedicalCareCategory.NoMeds;
			}
		}
		public void ExposeData()
		{
			Scribe_Values.LookValue<int>(ref this.joinTick, "joinTick", 0, false);
			Scribe_Values.LookValue<bool>(ref this.animalsReleased, "animalsReleased", false, false);
			Scribe_Values.LookValue<MedicalCareCategory>(ref this.medCare, "medCare", MedicalCareCategory.NoCare, false);
			Scribe_References.LookReference<Area>(ref this.areaAllowedInt, "areaAllowed", false);
			Scribe_References.LookReference<Pawn>(ref this.master, "master", false);
			Scribe_Values.LookValue<HostilityResponseMode>(ref this.hostilityResponse, "hostilityResponse", HostilityResponseMode.Flee, false);
		}
		[DebuggerHidden]
		public IEnumerable<Gizmo> GetGizmos()
		{
			Pawn_PlayerSettings.<GetGizmos>c__IteratorA3 <GetGizmos>c__IteratorA = new Pawn_PlayerSettings.<GetGizmos>c__IteratorA3();
			<GetGizmos>c__IteratorA.<>f__this = this;
			Pawn_PlayerSettings.<GetGizmos>c__IteratorA3 expr_0E = <GetGizmos>c__IteratorA;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public void Notify_AreaRemoved(Area area)
		{
			if (this.areaAllowedInt == area)
			{
				this.areaAllowedInt = null;
			}
		}
	}
}
