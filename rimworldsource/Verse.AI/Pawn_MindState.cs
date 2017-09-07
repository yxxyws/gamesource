using RimWorld;
using System;
using System.Collections.Generic;
namespace Verse.AI
{
	public class Pawn_MindState : IExposable
	{
		public Pawn pawn;
		public MentalStateHandler mentalStateHandler;
		public MentalStateStarter mentalStateStarter;
		private bool activeInt = true;
		public JobTag lastJobTag;
		public int lastPleasureDrugUseTick = -99999;
		public int nextApparelOptimizeTick = -99999;
		public ThinkNode lastJobGiver;
		public ThinkTreeDef lastJobGiverThinkTree;
		public WorkTypeDef lastGivenWorkType;
		public bool canFleeIndividual = true;
		public int exitMapAfterTick = -99999;
		public int lastDisturbanceTick = -99999;
		public IntVec3 forcedGotoPosition = IntVec3.Invalid;
		public Thing knownWickBurner;
		public bool wantsToTradeWithColony;
		public Thing lastMannedThing;
		public int canLovinTick = -99999;
		public int canSleepTick = -99999;
		public Pawn meleeThreat;
		public int lastMeleeThreatHarmTick;
		public int lastEngageTargetTick;
		public Thing enemyTarget;
		public PawnDuty duty;
		public Dictionary<int, int> thinkData = new Dictionary<int, int>();
		public int lastAssignedInteractTime = -99999;
		public int lastInventoryRawFoodUseTick;
		public Hive hive;
		public bool Active
		{
			get
			{
				return this.activeInt;
			}
			set
			{
				if (value != this.activeInt)
				{
					this.activeInt = value;
					Find.MapPawns.UpdateRegistryForPawn(this.pawn);
				}
			}
		}
		public bool IsIdle
		{
			get
			{
				return !this.pawn.Downed && this.pawn.Spawned && this.lastJobTag == JobTag.Idle;
			}
		}
		public Pawn_MindState()
		{
		}
		public Pawn_MindState(Pawn pawn)
		{
			this.pawn = pawn;
			this.mentalStateHandler = new MentalStateHandler(pawn);
			this.mentalStateStarter = new MentalStateStarter(pawn);
		}
		public void Reset()
		{
			this.mentalStateHandler.Reset();
			this.mentalStateStarter.Reset();
			this.lastJobTag = JobTag.NoTag;
			this.lastJobGiver = null;
			this.lastJobGiverThinkTree = null;
			this.lastGivenWorkType = null;
			this.duty = null;
			this.enemyTarget = null;
			this.meleeThreat = null;
			this.lastMeleeThreatHarmTick = 0;
			this.wantsToTradeWithColony = false;
			this.lastMannedThing = null;
		}
		public void ExposeData()
		{
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				if (this.enemyTarget != null && this.enemyTarget.Destroyed)
				{
					this.enemyTarget = null;
				}
				if (this.knownWickBurner != null && this.knownWickBurner.Destroyed)
				{
					this.knownWickBurner = null;
				}
			}
			Scribe_Values.LookValue<bool>(ref this.activeInt, "active", true, false);
			Scribe_Values.LookValue<JobTag>(ref this.lastJobTag, "lastJobTag", JobTag.NoTag, false);
			int num = -1;
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				num = ((this.lastJobGiver == null) ? -1 : this.lastJobGiver.UniqueSaveKey);
			}
			Scribe_Values.LookValue<int>(ref num, "lastJobGiverKey", 0, false);
			if (Scribe.mode == LoadSaveMode.PostLoadInit && num != -1 && !this.lastJobGiverThinkTree.TryGetThinkNodeWithSaveKey(num, out this.lastJobGiver))
			{
				Log.Message("Could not find think node with key " + num);
			}
			Scribe_Defs.LookDef<ThinkTreeDef>(ref this.lastJobGiverThinkTree, "lastJobGiverThinkTree");
			Scribe_Values.LookValue<int>(ref this.lastPleasureDrugUseTick, "lastPleasureDrugUseTick", -99999, false);
			Scribe_Values.LookValue<int>(ref this.nextApparelOptimizeTick, "nextApparelOptimizeTick", -99999, false);
			Scribe_Values.LookValue<int>(ref this.lastEngageTargetTick, "lastEngageTargetTick", 0, false);
			Scribe_Values.LookValue<bool>(ref this.canFleeIndividual, "canFleeIndividual", false, false);
			Scribe_Values.LookValue<int>(ref this.exitMapAfterTick, "exitMapAfterTick", -99999, false);
			Scribe_Values.LookValue<IntVec3>(ref this.forcedGotoPosition, "forcedGotoPosition", IntVec3.Invalid, false);
			Scribe_Values.LookValue<int>(ref this.lastMeleeThreatHarmTick, "lastMeleeThreatHarmTick", 0, false);
			Scribe_Values.LookValue<int>(ref this.lastAssignedInteractTime, "lastAssignedInteractTime", -99999, false);
			Scribe_Values.LookValue<int>(ref this.lastInventoryRawFoodUseTick, "lastInventoryRawFoodUseTick", 0, false);
			Scribe_Values.LookValue<int>(ref this.lastDisturbanceTick, "lastDisturbanceTick", -99999, false);
			Scribe_Values.LookValue<bool>(ref this.wantsToTradeWithColony, "wantsToTradeWithColony", false, false);
			Scribe_Values.LookValue<int>(ref this.canLovinTick, "canLovinTick", -99999, false);
			Scribe_Values.LookValue<int>(ref this.canSleepTick, "canSleepTick", -99999, false);
			Scribe_Collections.LookDictionary<int, int>(ref this.thinkData, "thinkData", LookMode.Undefined, LookMode.Undefined);
			Scribe_References.LookReference<Pawn>(ref this.meleeThreat, "meleeThreat", false);
			Scribe_References.LookReference<Thing>(ref this.enemyTarget, "enemyTarget", false);
			Scribe_References.LookReference<Thing>(ref this.knownWickBurner, "knownWickBurner", false);
			Scribe_References.LookReference<Hive>(ref this.hive, "hive", false);
			Scribe_References.LookReference<Thing>(ref this.lastMannedThing, "lastMannedThing", false);
			Scribe_Deep.LookDeep<PawnDuty>(ref this.duty, "duty", new object[0]);
			Scribe_Deep.LookDeep<MentalStateHandler>(ref this.mentalStateHandler, "mentalStateHandler", new object[]
			{
				this.pawn
			});
			Scribe_Deep.LookDeep<MentalStateStarter>(ref this.mentalStateStarter, "mentalStateStarter", new object[]
			{
				this.pawn
			});
		}
		public void MindTick()
		{
			if (this.wantsToTradeWithColony)
			{
				TradeUtility.CheckInteractWithTradersTeachOpportunity(this.pawn);
			}
			if (this.meleeThreat != null && (this.meleeThreat.Downed || !this.meleeThreat.Spawned || this.pawn.Downed || !this.pawn.Spawned))
			{
				this.meleeThreat = null;
			}
			if (this.hive != null && !this.hive.Spawned)
			{
				this.hive = null;
			}
			this.mentalStateHandler.MentalStateHandlerTick();
			this.mentalStateStarter.MentalStateStarterTick();
		}
		public void ResetLastDisturbanceTick()
		{
			this.lastDisturbanceTick = -9999999;
		}
		public void Notify_OutfitChanged()
		{
			this.nextApparelOptimizeTick = Find.TickManager.TicksGame;
		}
		public void Notify_WorkPriorityDisabled(WorkTypeDef wType)
		{
			JobGiver_Work jobGiver_Work = this.lastJobGiver as JobGiver_Work;
			if (jobGiver_Work != null && this.lastGivenWorkType == wType)
			{
				this.pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
			}
		}
		public void Notify_DamageTaken(DamageInfo dinfo)
		{
			if (dinfo.Def.externalViolence)
			{
				if (!this.mentalStateHandler.InMentalState && dinfo.Instigator != null && (dinfo.Instigator is Pawn || dinfo.Instigator is Building_Turret) && dinfo.Instigator.Faction != null && this.pawn.Faction == null && (this.pawn.CurJob == null || this.pawn.CurJob.def != JobDefOf.PredatorHunt) && Rand.Value < this.pawn.RaceProps.manhunterOnDamageChance)
				{
					this.StartManhunterBecauseOfPawnAction("AnimalManhunterFromDamage");
				}
				if (this.pawn.GetPosture() != PawnPosture.Standing)
				{
					this.lastDisturbanceTick = Find.TickManager.TicksGame;
				}
				if (dinfo.Instigator != null && this.hive != null)
				{
					this.hive.Notify_HiveOrPawnAttacked(dinfo.Instigator);
				}
			}
		}
		internal void Notify_EngagedTarget()
		{
			this.lastEngageTargetTick = Find.TickManager.TicksGame;
		}
		internal bool CheckStartMentalStateBecauseRecruitAttempted(Pawn tamer)
		{
			if (!this.pawn.RaceProps.Animal)
			{
				return false;
			}
			if (!this.mentalStateHandler.InMentalState && this.pawn.Faction == null && Rand.Value < this.pawn.RaceProps.manhunterOnTameFailChance)
			{
				this.StartManhunterBecauseOfPawnAction("AnimalManhunterFromTaming");
				return true;
			}
			return false;
		}
		private void StartManhunterBecauseOfPawnAction(string letterTextKey)
		{
			if (!this.mentalStateStarter.TryStartMentalState(MentalStateDefOf.Manhunter))
			{
				return;
			}
			string text = letterTextKey.Translate(new object[]
			{
				this.pawn.Label
			});
			if (Rand.Value < 0.5f)
			{
				int num = 1;
				Room room = this.pawn.GetRoom();
				List<Pawn> allPawnsSpawned = Find.MapPawns.AllPawnsSpawned;
				for (int i = 0; i < allPawnsSpawned.Count; i++)
				{
					if (this.pawn != allPawnsSpawned[i] && allPawnsSpawned[i].RaceProps == this.pawn.RaceProps && allPawnsSpawned[i].Faction == this.pawn.Faction && allPawnsSpawned[i].Position.InHorDistOf(this.pawn.Position, 24f) && allPawnsSpawned[i].GetRoom() == room && allPawnsSpawned[i].mindState.mentalStateStarter.TryStartMentalState(MentalStateDefOf.Manhunter))
					{
						num++;
					}
				}
				if (num > 1)
				{
					text += "\n\n";
					text += ((!"AnimalManhunterOthers".CanTranslate()) ? "AnimalManhunterFromDamageOthers".Translate(new object[]
					{
						this.pawn.def.label
					}) : "AnimalManhunterOthers".Translate(new object[]
					{
						this.pawn.def.label
					}));
				}
			}
			string label = (!"LetterLabelAnimalManhunterRevenge".CanTranslate()) ? "LetterLabelAnimalManhunterFromDamage".Translate(new object[]
			{
				this.pawn.Label
			}).CapitalizeFirst() : "LetterLabelAnimalManhunterRevenge".Translate(new object[]
			{
				this.pawn.Label
			}).CapitalizeFirst();
			Find.LetterStack.ReceiveLetter(label, text, LetterType.BadNonUrgent, this.pawn, null);
		}
		internal void Notify_DangerousWickStarted(Thing wickBurner)
		{
			if (this.pawn.RaceProps.intelligence >= Intelligence.Humanlike)
			{
				this.knownWickBurner = wickBurner;
				this.pawn.jobs.CheckForJobOverride();
			}
		}
	}
}
