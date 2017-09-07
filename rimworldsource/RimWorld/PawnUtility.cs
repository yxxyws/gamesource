using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public static class PawnUtility
	{
		public static IEnumerable<Pawn> AllPawnsMapOrWorldAliveOrDead
		{
			get
			{
				PawnUtility.<>c__Iterator93 <>c__Iterator = new PawnUtility.<>c__Iterator93();
				PawnUtility.<>c__Iterator93 expr_07 = <>c__Iterator;
				expr_07.$PC = -2;
				return expr_07;
			}
		}
		public static IEnumerable<Pawn> AllPawnsMapOrWorldAlive
		{
			get
			{
				PawnUtility.<>c__Iterator94 <>c__Iterator = new PawnUtility.<>c__Iterator94();
				PawnUtility.<>c__Iterator94 expr_07 = <>c__Iterator;
				expr_07.$PC = -2;
				return expr_07;
			}
		}
		public static IEnumerable<Pawn> AllPawnsMapOrWorldDead
		{
			get
			{
				PawnUtility.<>c__Iterator95 <>c__Iterator = new PawnUtility.<>c__Iterator95();
				PawnUtility.<>c__Iterator95 expr_07 = <>c__Iterator;
				expr_07.$PC = -2;
				return expr_07;
			}
		}
		public static void TryDestroyStartingColonistFamily(Pawn pawn)
		{
			if (!pawn.relations.RelatedPawns.Any((Pawn x) => MapInitData.colonists.Contains(x)))
			{
				foreach (Pawn current in pawn.relations.RelatedPawns.ToList<Pawn>())
				{
					WorldPawnSituation situation = Find.WorldPawns.GetSituation(current);
					if (situation == WorldPawnSituation.Free || situation == WorldPawnSituation.Dead)
					{
						Find.WorldPawns.RemovePawn(current);
						current.relations.ClearAllRelations();
						Find.WorldPawns.PassToWorld(current, PawnDiscardDecideMode.Discard);
					}
				}
			}
		}
		public static bool WillSoonHaveBasicNeed(Pawn p)
		{
			return p.needs != null && ((p.needs.rest != null && p.needs.rest.CurLevel < 0.33f) || (p.needs.food != null && p.needs.food.CurLevelPercentage < p.needs.food.PercentageThreshHungry + 0.05f));
		}
		public static float AnimalFilthChancePerCell(ThingDef def, float bodySize)
		{
			float num = bodySize * 0.00125f;
			return num * (1f - def.race.petness);
		}
		public static bool CasualInterruptibleNow(this Pawn p)
		{
			return !p.Drafted && (p.playerSettings == null || p.playerSettings.master == null || !p.playerSettings.master.Drafted) && !p.InAggroMentalState && p.Awake() && (p.CurJob == null || (p.CurJob.def.casualInterruptible && !p.CurJob.playerForced));
		}
		[DebuggerHidden]
		public static IEnumerable<Pawn> SpawnedMasteredPawns(Pawn master)
		{
			PawnUtility.<SpawnedMasteredPawns>c__Iterator96 <SpawnedMasteredPawns>c__Iterator = new PawnUtility.<SpawnedMasteredPawns>c__Iterator96();
			<SpawnedMasteredPawns>c__Iterator.master = master;
			<SpawnedMasteredPawns>c__Iterator.<$>master = master;
			PawnUtility.<SpawnedMasteredPawns>c__Iterator96 expr_15 = <SpawnedMasteredPawns>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}
		public static bool InValidState(Pawn p)
		{
			return p.health != null && (p.Dead || (p.stances != null && p.mindState != null && p.needs != null && p.ageTracker != null));
		}
		public static PawnPosture GetPosture(this Pawn p)
		{
			if (p.Downed || p.Dead)
			{
				return PawnPosture.LayingAny;
			}
			if (p.jobs == null)
			{
				return PawnPosture.Standing;
			}
			if (p.jobs.curJob == null)
			{
				return PawnPosture.Standing;
			}
			return p.jobs.curDriver.Posture;
		}
		public static void ForceWait(Pawn pawn, int ticks, Thing faceTarget = null)
		{
			Job job = new Job(JobDefOf.Wait, faceTarget);
			job.expiryInterval = ticks;
			pawn.jobs.StartJob(job, JobCondition.InterruptForced, null, true, true, null);
		}
		public static void GiveNameBecauseOfNuzzle(Pawn namer, Pawn namee)
		{
			string text = (namee.Name != null) ? namee.Name.ToStringFull : namee.LabelIndefinite();
			namee.Name = NameGenerator.GeneratePawnName(namee, NameStyle.Full, null);
			if (namer.Faction == Faction.OfColony)
			{
				Messages.Message("MessageNuzzledPawnGaveNameTo".Translate(new object[]
				{
					namer,
					text,
					namee.Name.ToStringFull
				}), namee, MessageSound.Standard);
			}
		}
		public static void GainComfortFromCellIfPossible(this Pawn p)
		{
			if (Find.TickManager.TicksGame % 10 == 0)
			{
				Building edifice = p.Position.GetEdifice();
				if (edifice != null)
				{
					float statValue = edifice.GetStatValue(StatDefOf.Comfort, true);
					if (statValue >= 0f && p.needs != null && p.needs.comfort != null)
					{
						p.needs.comfort.ComfortUsed(statValue);
					}
				}
			}
		}
		public static float BodyResourceGrowthSpeed(Pawn pawn)
		{
			if (pawn.needs != null && pawn.needs.food != null)
			{
				switch (pawn.needs.food.CurCategory)
				{
				case HungerCategory.Fed:
					return 1f;
				case HungerCategory.Hungry:
					return 0.666f;
				case HungerCategory.UrgentlyHungry:
					return 0.333f;
				case HungerCategory.Starving:
					return 0f;
				}
			}
			return 1f;
		}
		public static bool FertileMateTarget(Pawn male, Pawn female)
		{
			if (female.gender != Gender.Female || !female.ageTracker.CurLifeStage.reproductive)
			{
				return false;
			}
			CompEggLayer compEggLayer = female.TryGetComp<CompEggLayer>();
			if (compEggLayer != null)
			{
				return !compEggLayer.FullyFertilized;
			}
			return !female.health.hediffSet.HasHediff(HediffDefOf.Pregnant);
		}
		public static void Mated(Pawn male, Pawn female)
		{
			if (!female.ageTracker.CurLifeStage.reproductive)
			{
				return;
			}
			CompEggLayer compEggLayer = female.TryGetComp<CompEggLayer>();
			if (compEggLayer != null)
			{
				compEggLayer.Fertilize(male);
			}
			else
			{
				if (Rand.Value < 0.5f && !female.health.hediffSet.HasHediff(HediffDefOf.Pregnant))
				{
					Hediff_Pregnant hediff_Pregnant = (Hediff_Pregnant)HediffMaker.MakeHediff(HediffDefOf.Pregnant, female, null);
					hediff_Pregnant.father = male;
					female.health.AddHediff(hediff_Pregnant, null, null);
				}
			}
		}
		public static bool PlayerForcedJobNowOrSoon(Pawn pawn)
		{
			Job curJob = pawn.CurJob;
			return (curJob == null && JobQueueUtility.NextJobIsPlayerForced(pawn)) || (curJob != null && curJob.playerForced);
		}
		public static bool TrySpawnHatchedOrBornPawn(Pawn pawn, Thing motherOrEgg)
		{
			if (motherOrEgg.Spawned)
			{
				return GenSpawn.Spawn(pawn, motherOrEgg.Position) != null;
			}
			Pawn p = motherOrEgg as Pawn;
			if (p.IsWorldPawn())
			{
				Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Keep);
				return true;
			}
			if (motherOrEgg.holder != null && motherOrEgg.holder.owner.Spawned)
			{
				GenSpawn.Spawn(pawn, motherOrEgg.holder.owner.GetPosition());
				return true;
			}
			return false;
		}
		public static bool ShouldSendNotificationAbout(Pawn p)
		{
			if (Game.Mode != GameMode.MapPlaying)
			{
				return false;
			}
			if (p.IsWorldPawn())
			{
				return false;
			}
			if (p.Faction != Faction.OfColony)
			{
				if (p.HostFaction != Faction.OfColony)
				{
					return false;
				}
				if (p.RaceProps.Humanlike && p.guest.released && !p.Downed && !p.InBed())
				{
					return false;
				}
				if (p.CurJob != null && p.CurJob.exitMapOnArrival && !PrisonBreakUtility.IsEscaping(p))
				{
					return false;
				}
			}
			return true;
		}
		public static LocomotionUrgency ResolveLocomotion(Pawn pawn, LocomotionUrgency secondPriority)
		{
			if (!pawn.Dead && pawn.mindState.duty != null && pawn.mindState.duty.locomotion != LocomotionUrgency.None)
			{
				return pawn.mindState.duty.locomotion;
			}
			return secondPriority;
		}
		public static LocomotionUrgency ResolveLocomotion(Pawn pawn, LocomotionUrgency secondPriority, LocomotionUrgency thirdPriority)
		{
			LocomotionUrgency locomotionUrgency = PawnUtility.ResolveLocomotion(pawn, secondPriority);
			if (locomotionUrgency != LocomotionUrgency.None)
			{
				return locomotionUrgency;
			}
			return thirdPriority;
		}
		public static Danger ResolveMaxDanger(Pawn pawn, Danger secondPriority)
		{
			if (!pawn.Dead && pawn.mindState.duty != null && pawn.mindState.duty.maxDanger != Danger.Unspecified)
			{
				return pawn.mindState.duty.maxDanger;
			}
			return secondPriority;
		}
		public static Danger ResolveMaxDanger(Pawn pawn, Danger secondPriority, Danger thirdPriority)
		{
			Danger danger = PawnUtility.ResolveMaxDanger(pawn, secondPriority);
			if (danger != Danger.Unspecified)
			{
				return danger;
			}
			return thirdPriority;
		}
	}
}
