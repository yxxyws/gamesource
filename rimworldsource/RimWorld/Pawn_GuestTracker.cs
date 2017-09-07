using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI.Group;
namespace RimWorld
{
	public class Pawn_GuestTracker : IExposable
	{
		private const float MinRecruitDifficulty = 30f;
		private const float MaxRecruitDifficulty = 99f;
		private const int CheckInitiatePrisonBreakIntervalTicks = 2500;
		private Pawn pawn;
		private bool getsFoodInt = true;
		public PrisonerInteractionMode interactionMode;
		private Faction hostFactionInt;
		public bool isPrisonerInt;
		public bool released;
		private float recruitDifficulty = -1f;
		public int MinInteractionInterval = 7500;
		public Faction HostFaction
		{
			get
			{
				return this.hostFactionInt;
			}
		}
		public bool GetsFood
		{
			get
			{
				if (this.HostFaction == null)
				{
					Log.Error("GetsFood without host faction.");
					return true;
				}
				return this.getsFoodInt;
			}
			set
			{
				this.getsFoodInt = value;
			}
		}
		public bool ShouldBeBroughtFood
		{
			get
			{
				return this.GetsFood && this.interactionMode != PrisonerInteractionMode.Execution && this.interactionMode != PrisonerInteractionMode.Release;
			}
		}
		public bool IsPrisoner
		{
			get
			{
				return this.isPrisonerInt;
			}
		}
		public bool ScheduledForInteraction
		{
			get
			{
				return this.pawn.mindState.lastAssignedInteractTime < Find.TickManager.TicksGame - this.MinInteractionInterval;
			}
		}
		public float RecruitDifficulty
		{
			get
			{
				return this.recruitDifficulty;
			}
		}
		public bool PrisonerIsSecure
		{
			get
			{
				return !this.released && this.pawn.HostFaction != null && this.pawn.MentalStateDef == null && (this.pawn.jobs.curJob == null || !this.pawn.jobs.curJob.exitMapOnArrival) && !PrisonBreakUtility.IsEscaping(this.pawn);
			}
		}
		public Pawn_GuestTracker()
		{
		}
		public Pawn_GuestTracker(Pawn pawn)
		{
			this.pawn = pawn;
		}
		public void GuestTrackerTick()
		{
			if (this.pawn.IsHashIntervalTick(2500))
			{
				float num = PrisonBreakUtility.InitiatePrisonBreakMtbDays(this.pawn);
				if (num >= 0f && Rand.MTBEventOccurs(num, 60000f, 2500f))
				{
					PrisonBreakUtility.StartPrisonBreak(this.pawn);
				}
			}
		}
		public void Init()
		{
			if (this.recruitDifficulty < 0f)
			{
				float num = this.pawn.kindDef.baseRecruitDifficulty + Rand.Range(-30f, 30f);
				float popIntent = (Game.Mode != GameMode.MapPlaying) ? 1f : Find.Storyteller.intenderPopulation.PopulationIntent;
				num = Pawn_GuestTracker.PopIntentAdjustedRecruitDifficulty(num, popIntent);
				this.recruitDifficulty = Mathf.Clamp(num, 30f, 99f);
			}
		}
		public void ExposeData()
		{
			Scribe_References.LookReference<Faction>(ref this.hostFactionInt, "hostFaction", false);
			Scribe_Values.LookValue<bool>(ref this.isPrisonerInt, "prisoner", false, false);
			Scribe_Values.LookValue<bool>(ref this.getsFoodInt, "getsFood", false, false);
			Scribe_Values.LookValue<PrisonerInteractionMode>(ref this.interactionMode, "interactionMode", PrisonerInteractionMode.NoInteraction, false);
			Scribe_Values.LookValue<bool>(ref this.released, "released", false, false);
			Scribe_Values.LookValue<float>(ref this.recruitDifficulty, "recruitDifficulty", 0f, false);
		}
		private static float PopIntentAdjustedRecruitDifficulty(float baseDifficulty, float popIntent)
		{
			float num = Mathf.Clamp(popIntent, 0.25f, 3f);
			float value = 100f - (100f - baseDifficulty) * num;
			return Mathf.Clamp(value, 30f, 99f);
		}
		public static void DoTables_PopIntentRecruitDifficulty()
		{
			List<float> list = new List<float>();
			for (float num = -1f; num < 3f; num += 0.1f)
			{
				list.Add(num);
			}
			List<float> colValues = new List<float>
			{
				10f,
				20f,
				30f,
				40f,
				50f,
				60f,
				70f,
				80f,
				90f,
				99f
			};
			DebugTables.MakeTablesDialog<float, float>(colValues, (float d) => "d=" + d.ToString("F0"), list, (float rv) => rv.ToString("F1"), (float d, float pi) => Pawn_GuestTracker.PopIntentAdjustedRecruitDifficulty(d, pi).ToString("F2"), "intents");
		}
		public void SetGuestStatus(Faction newHost, bool prisoner = false)
		{
			if (newHost != null)
			{
				this.released = false;
			}
			if (newHost == this.HostFaction && prisoner == this.IsPrisoner)
			{
				return;
			}
			if (!prisoner && this.pawn.Faction.HostileTo(newHost))
			{
				Log.Error(string.Concat(new object[]
				{
					"Tried to make ",
					this.pawn,
					" a guest of ",
					newHost,
					" but their faction ",
					this.pawn.Faction,
					" is hostile to ",
					newHost
				}));
				return;
			}
			if (newHost == this.pawn.Faction && !prisoner)
			{
				Log.Error(string.Concat(new object[]
				{
					"Tried to make ",
					this.pawn,
					" a guest of their own faction ",
					this.pawn.Faction
				}));
				return;
			}
			bool flag = prisoner && (!this.IsPrisoner || this.HostFaction != newHost);
			this.isPrisonerInt = prisoner;
			this.hostFactionInt = newHost;
			this.pawn.ClearMind(false);
			this.pawn.ClearReservations();
			if (flag)
			{
				this.pawn.DropAndForbidEverything(false);
				Lord lord = this.pawn.GetLord();
				if (lord != null)
				{
					lord.Notify_PawnLost(this.pawn, PawnLostCondition.MadePrisoner);
				}
				if (this.pawn.Drafted)
				{
					this.pawn.drafter.Drafted = false;
				}
			}
			PawnComponentsUtility.AddAndRemoveDynamicComponents(this.pawn, false);
			this.pawn.health.surgeryBills.Clear();
			if (this.pawn.ownership != null)
			{
				this.pawn.ownership.Notify_ChangedGuestStatus();
			}
			Reachability.ClearCache();
			Find.MapPawns.UpdateRegistryForPawn(this.pawn);
			Find.AttackTargetsCache.UpdateTarget(this.pawn);
		}
		internal void Notify_PawnUndowned()
		{
		}
	}
}
