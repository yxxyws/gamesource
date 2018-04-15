using RimWorld;
using System;
using System.Collections.Generic;
using Verse.AI.Group;
namespace Verse.AI
{
	public class Pawn_JobTracker : IExposable
	{
		private const int ConstantThinkTreeJobCheckIntervalTicks = 30;
		private const int RecentJobQueueMaxLength = 10;
		private const int MaxRecentJobs = 10;
		private const int DamageCheckMinInterval = 180;
		protected Pawn pawn;
		public Job curJob;
		public JobDriver curDriver;
		public JobQueue jobQueue;
		private int jobsGivenThisTick;
		private Queue<int> jobsGivenRecentTicks = new Queue<int>();
		public bool debugLog;
		private int lastDamageCheckTick = -99999;
		public Pawn_JobTracker(Pawn newPawn)
		{
			this.pawn = newPawn;
		}
		public virtual void ExposeData()
		{
			Scribe_Deep.LookDeep<Job>(ref this.curJob, "curJob", new object[0]);
			Scribe_Deep.LookDeep<JobDriver>(ref this.curDriver, "curDriver", new object[0]);
			Scribe_Deep.LookDeep<JobQueue>(ref this.jobQueue, "jobQueue", new object[0]);
			if (Scribe.mode == LoadSaveMode.LoadingVars && this.curDriver != null)
			{
				this.curDriver.pawn = this.pawn;
			}
		}
		public virtual void JobTrackerTick()
		{
			this.jobsGivenThisTick = 0;
			if (this.pawn.IsHashIntervalTick(30))
			{
				ThinkResult result = this.DetermineNextConstantThinkTreeJob();
				if (result.Job != null && (this.curJob == null || result.Job.def != this.curJob.def || result.SourceNode != this.pawn.mindState.lastJobGiver))
				{
					this.CheckLeaveJoinableLordBecauseJobIssued(result);
					this.StartJob(result.Job, JobCondition.InterruptForced, result.SourceNode, false, false, this.pawn.thinker.ConstantThinkTree);
				}
			}
			if (this.curDriver != null)
			{
				if (this.curJob.expiryInterval > 0 && (Find.TickManager.TicksGame - this.curJob.startTick) % this.curJob.expiryInterval == 0 && Find.TickManager.TicksGame != this.curJob.startTick)
				{
					if (this.debugLog)
					{
						this.DebugLogEvent("Job expire");
					}
					if (!this.curJob.checkOverrideOnExpire)
					{
						this.EndCurrentJob(JobCondition.Succeeded);
					}
					else
					{
						this.CheckForJobOverride();
					}
					this.FinalizeTick();
					return;
				}
				this.curDriver.DriverTick();
			}
			if (this.curJob == null && !this.pawn.Dead && this.pawn.mindState.Active && this.CanDoAnyJob())
			{
				if (this.debugLog)
				{
					this.DebugLogEvent("Starting job from Tick because curJob == null.");
				}
				this.TryFindAndStartJob();
			}
			this.FinalizeTick();
		}
		private void FinalizeTick()
		{
			this.jobsGivenRecentTicks.Enqueue(this.jobsGivenThisTick);
			if (this.jobsGivenRecentTicks.Count > 0)
			{
				while (this.jobsGivenRecentTicks.Count > 10)
				{
					this.jobsGivenRecentTicks.Dequeue();
				}
				int num = 0;
				foreach (int current in this.jobsGivenRecentTicks)
				{
					num += current;
				}
				if (num >= 10)
				{
					this.StartErrorRecoverJob(string.Concat(new object[]
					{
						this.pawn,
						" started ",
						10,
						" jobs in ",
						10,
						" ticks."
					}));
				}
			}
		}
		public void StartJob(Job newJob, JobCondition lastJobEndCondition = JobCondition.None, ThinkNode jobGiver = null, bool resumeCurJobAfterwards = false, bool cancelBusyStances = true, ThinkTreeDef thinkTree = null)
		{
			if (this.debugLog)
			{
				this.DebugLogEvent(string.Concat(new object[]
				{
					"StartJob [",
					newJob,
					"] lastJobEndCondition=",
					lastJobEndCondition,
					", jobGiver=",
					jobGiver,
					", cancelBusyStances=",
					cancelBusyStances
				}));
			}
			if (cancelBusyStances && this.pawn.stances.FullBodyBusy)
			{
				this.pawn.stances.CancelBusyStanceHard();
			}
			if (this.curJob != null)
			{
				if (lastJobEndCondition == JobCondition.None)
				{
					Log.Warning(string.Concat(new object[]
					{
						this.pawn,
						" starting job ",
						newJob,
						" from JobGiver ",
						this.pawn.mindState.lastJobGiver,
						" while already having job ",
						this.curJob,
						" without a specific job end condition."
					}));
					lastJobEndCondition = JobCondition.InterruptForced;
				}
				if (resumeCurJobAfterwards && this.curJob.def.suspendable)
				{
					this.EnqueueJob(this.curJob);
					if (this.debugLog)
					{
						this.DebugLogEvent("   JobQueue EnqueueFirst curJob: " + this.curJob);
					}
				}
				this.CleanupCurrentJob(lastJobEndCondition, !resumeCurJobAfterwards, cancelBusyStances);
			}
			if (newJob == null)
			{
				Log.Warning(this.pawn + " tried to start doing a null job.");
				return;
			}
			newJob.startTick = Find.TickManager.TicksGame;
			if (this.pawn.Drafted || newJob.playerForced)
			{
				newJob.ignoreForbidden = true;
				newJob.ignoreDesignations = true;
			}
			this.curJob = newJob;
			this.pawn.mindState.lastJobGiver = jobGiver;
			this.pawn.mindState.lastJobGiverThinkTree = thinkTree;
			this.curDriver = this.curJob.MakeDriver(this.pawn);
			this.curDriver.Notify_Starting();
			this.curDriver.SetupToils();
			this.curDriver.ReadyForNextToil();
		}
		public void EndCurrentJob(JobCondition condition)
		{
			if (this.debugLog)
			{
				this.DebugLogEvent(string.Concat(new object[]
				{
					"EndCurrentJob ",
					(this.curJob == null) ? "null" : this.curJob.ToString(),
					" condition=",
					condition,
					" curToil=",
					this.curDriver.CurToilIndex
				}));
			}
			Job job = this.curJob;
			this.CleanupCurrentJob(condition, true, true);
			if (condition == JobCondition.ErroredPather || condition == JobCondition.Errored)
			{
				this.StartJob(new Job(JobDefOf.Wait, 250, false), JobCondition.None, null, false, true, null);
				return;
			}
			if (condition == JobCondition.Succeeded && job != null && job.def != JobDefOf.Wait && !this.pawn.pather.Moving)
			{
				this.StartJob(new Job(JobDefOf.Wait, 1, false), JobCondition.None, null, false, false, null);
			}
			else
			{
				this.TryFindAndStartJob();
			}
		}
		public void CheckForJobOverride()
		{
			if (this.debugLog)
			{
				this.DebugLogEvent("CheckForJobOverride");
			}
			ThinkTreeDef thinkTree;
			ThinkResult result = this.DetermineNextJob(out thinkTree);
			if (this.curJob == null || result.Job.def != this.curJob.def || result.SourceNode != this.pawn.mindState.lastJobGiver)
			{
				this.CheckLeaveJoinableLordBecauseJobIssued(result);
				this.StartJob(result.Job, JobCondition.InterruptOptional, result.SourceNode, false, false, thinkTree);
			}
		}
		public void StopAll(bool ifSleepingKeepSleeping = false)
		{
			if (ifSleepingKeepSleeping && this.curJob != null && this.curDriver.layingDown)
			{
				return;
			}
			this.CleanupCurrentJob(JobCondition.Incompletable, true, true);
		}
		public void EnqueueJob(Job job)
		{
			if (this.jobQueue == null)
			{
				this.jobQueue = new JobQueue();
			}
			this.jobQueue.EnqueueFirst(job);
		}
		public void DebugLogEvent(string s)
		{
			if (this.debugLog)
			{
				Log.Message(string.Concat(new object[]
				{
					Find.TickManager.TicksGame,
					" ",
					this.pawn,
					": ",
					s
				}));
			}
		}
		private bool CanDoAnyJob()
		{
			return !this.pawn.Dead && this.pawn.Spawned && this.pawn.holder == null;
		}
		private void TryFindAndStartJob()
		{
			if (this.pawn.thinker == null)
			{
				Log.ErrorOnce(this.pawn + " did TryFindAndStartJob but had no thinker.", 8573261);
				return;
			}
			if (this.pawn.jobs.curJob != null)
			{
				Log.Warning(this.pawn + " doing TryFindAndStartJob while still having job " + this.pawn.jobs.curJob);
			}
			if (this.debugLog)
			{
				this.DebugLogEvent("TryFindAndStartJob");
			}
			this.pawn.mindState.lastJobTag = JobTag.NoTag;
			if (!this.CanDoAnyJob())
			{
				if (this.debugLog)
				{
					this.DebugLogEvent("   CanDoAnyJob is false. Clearing queue and returning");
				}
				if (this.jobQueue != null)
				{
					this.jobQueue.Clear();
				}
				return;
			}
			ThinkTreeDef thinkTreeDef;
			ThinkResult result = this.DetermineNextJob(out thinkTreeDef);
			if (!Find.TickManager.Paused)
			{
				this.jobsGivenThisTick++;
			}
			if (this.jobsGivenThisTick > 10)
			{
				this.StartErrorRecoverJob(this.pawn + " started 10 jobs in one tick. thinkResult=" + result.ToString());
				return;
			}
			if (result.IsValid)
			{
				this.CheckLeaveJoinableLordBecauseJobIssued(result);
				ThinkNode sourceNode = result.SourceNode;
				ThinkTreeDef thinkTree = thinkTreeDef;
				this.StartJob(result.Job, JobCondition.None, sourceNode, false, false, thinkTree);
			}
		}
		private ThinkResult DetermineNextJob(out ThinkTreeDef thinkTree)
		{
			ThinkResult result = this.DetermineNextConstantThinkTreeJob();
			if (result.Job != null)
			{
				thinkTree = this.pawn.thinker.ConstantThinkTree;
				return result;
			}
			if (this.jobQueue != null)
			{
				while (this.jobQueue.Count > 0 && !this.jobQueue.Peek().CanBeginNow(this.pawn))
				{
					Job arg = this.jobQueue.Dequeue();
					if (this.debugLog)
					{
						this.DebugLogEvent("   Throwing away queued job that I cannot begin now: " + arg);
					}
				}
				if (this.jobQueue.Count > 0)
				{
					Job job = this.jobQueue.Dequeue();
					if (this.debugLog)
					{
						this.DebugLogEvent("   Returning queued job: " + job);
					}
					thinkTree = null;
					return new ThinkResult(job, null);
				}
			}
			ThinkResult result2 = ThinkResult.NoJob;
			try
			{
				result2 = this.pawn.thinker.MainThinkNodeRoot.TryIssueJobPackage(this.pawn);
			}
			catch (Exception ex)
			{
				this.StartErrorRecoverJob(this.pawn + " threw exception while determining job (main): " + ex.ToString());
				thinkTree = null;
				return ThinkResult.NoJob;
			}
			finally
			{
			}
			thinkTree = this.pawn.thinker.MainThinkTree;
			return result2;
		}
		private ThinkResult DetermineNextConstantThinkTreeJob()
		{
			if (this.pawn.thinker.ConstantThinkTree == null)
			{
				return ThinkResult.NoJob;
			}
			try
			{
				return this.pawn.thinker.ConstantThinkNodeRoot.TryIssueJobPackage(this.pawn);
			}
			catch (Exception ex)
			{
				this.StartErrorRecoverJob(this.pawn + " threw exception while determining job (constant): " + ex.ToString());
			}
			finally
			{
			}
			return ThinkResult.NoJob;
		}
		private void CleanupCurrentJob(JobCondition condition, bool releaseReservations, bool cancelBusyStancesSoft = true)
		{
			if (this.debugLog)
			{
				this.DebugLogEvent(string.Concat(new object[]
				{
					"CleanupCurrentJob ",
					(this.curJob == null) ? "null" : this.curJob.def.ToString(),
					" condition ",
					condition
				}));
			}
			if (this.curJob == null)
			{
				return;
			}
			this.curDriver.ended = true;
			this.curDriver.Cleanup(condition);
			this.curDriver = null;
			this.curJob = null;
			if (releaseReservations)
			{
				Find.Reservations.ReleaseAllClaimedBy(this.pawn);
			}
			if (cancelBusyStancesSoft)
			{
				this.pawn.stances.CancelBusyStanceSoft();
			}
			if (!this.pawn.Destroyed && this.pawn.carrier != null && this.pawn.carrier.CarriedThing != null)
			{
				Thing thing;
				this.pawn.carrier.TryDropCarriedThing(this.pawn.Position, ThingPlaceMode.Near, out thing);
			}
		}
		private void StartErrorRecoverJob(string message)
		{
			string text = message + " lastJobGiver=" + this.pawn.mindState.lastJobGiver;
			if (this.curJob != null)
			{
				text = text + ", curJob.def=" + this.curJob.def.defName;
			}
			if (this.curDriver != null)
			{
				text = text + ", curDriver=" + this.curDriver.GetType();
			}
			Log.Error(text);
			if (this.curJob != null)
			{
				this.EndCurrentJob(JobCondition.Errored);
			}
			else
			{
				this.StartJob(new Job(JobDefOf.Wait, 300, false), JobCondition.None, null, false, true, null);
			}
		}
		private void CheckLeaveJoinableLordBecauseJobIssued(ThinkResult result)
		{
			if (!result.IsValid || result.SourceNode == null)
			{
				return;
			}
			Lord lord = this.pawn.GetLord();
			if (lord == null || !(lord.LordJob is LordJob_VoluntarilyJoinable))
			{
				return;
			}
			bool flag = false;
			ThinkNode thinkNode = result.SourceNode;
			while (!thinkNode.leaveJoinableLordIfIssuesJob)
			{
				thinkNode = thinkNode.parent;
				if (thinkNode == null)
				{
					IL_6F:
					if (flag)
					{
						lord.Notify_PawnLost(this.pawn, PawnLostCondition.LeftVoluntarily);
					}
					return;
				}
			}
			flag = true;
			goto IL_6F;
		}
		public void Notify_TuckedIntoBed(Building_Bed bed)
		{
			this.pawn.Position = RestUtility.GetBedSleepingSlotPosFor(this.pawn, bed);
			this.pawn.Notify_Teleported();
			this.pawn.stances.CancelBusyStanceHard();
			this.StartJob(new Job(JobDefOf.LayDown, bed), JobCondition.InterruptForced, null, false, true, null);
		}
		public void Notify_DamageTaken(DamageInfo dinfo)
		{
			if (this.curJob == null)
			{
				return;
			}
			if (Find.TickManager.TicksGame < this.lastDamageCheckTick + 180)
			{
				return;
			}
			if (dinfo.Def.externalViolence && dinfo.Def.canInterruptJobs && !this.curJob.playerForced && this.curJob.def.canCheckOverrideOnDamage)
			{
				this.lastDamageCheckTick = Find.TickManager.TicksGame;
				this.CheckForJobOverride();
			}
		}
		internal void Notify_MasterDrafted()
		{
			this.EndCurrentJob(JobCondition.InterruptForced);
		}
	}
}
