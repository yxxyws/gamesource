using RimWorld;
using System;
using UnityEngine;
namespace Verse.AI
{
	public class Pawn_PathFollower : IExposable
	{
		private const int MaxMoveTicks = 450;
		private const int MaxCheckAheadNodes = 20;
		private const float SnowReductionFromWalking = 0.005f;
		private const int ClamorCellsInterval = 12;
		private const int MinTicksWalk = 50;
		private const int MinTicksAmble = 60;
		protected Pawn pawn;
		private bool moving;
		public IntVec3 nextCell;
		private IntVec3 lastCell;
		public int ticksUntilMove;
		public int totalMoveDuration = 1;
		private int cellsUntilClamor;
		private TargetInfo destination;
		private PathEndMode peMode;
		public PawnPath curPath;
		public IntVec3 lastPathedTargetPosition;
		public TargetInfo Destination
		{
			get
			{
				return this.destination;
			}
		}
		public bool Moving
		{
			get
			{
				return this.moving;
			}
		}
		public Pawn_PathFollower(Pawn newPawn)
		{
			this.pawn = newPawn;
		}
		public void ExposeData()
		{
			Scribe_Values.LookValue<bool>(ref this.moving, "moving", true, false);
			Scribe_Values.LookValue<IntVec3>(ref this.nextCell, "nextCell", default(IntVec3), false);
			Scribe_Values.LookValue<int>(ref this.ticksUntilMove, "ticksUntilMove", 0, false);
			Scribe_Values.LookValue<int>(ref this.totalMoveDuration, "totalMoveDuration", 0, false);
			Scribe_Values.LookValue<PathEndMode>(ref this.peMode, "peMode", PathEndMode.None, false);
			if (this.moving)
			{
				Scribe_TargetInfo.LookTargetInfo(ref this.destination, "destination");
			}
			if (Scribe.mode == LoadSaveMode.PostLoadInit && Game.Mode != GameMode.Entry)
			{
				if (this.moving)
				{
					this.StartPath(this.destination, this.peMode);
				}
				Find.PawnDestinationManager.ReserveDestinationFor(this.pawn, this.destination.Cell);
			}
		}
		public void Notify_Teleported_Int()
		{
			this.StopDead();
			this.ResetToCurrentPosition();
		}
		public void ResetToCurrentPosition()
		{
			this.nextCell = this.pawn.Position;
		}
		private bool PawnCanOccupy(IntVec3 c)
		{
			if (!c.Walkable())
			{
				return false;
			}
			Building edifice = c.GetEdifice();
			if (edifice != null)
			{
				Building_Door building_Door = edifice as Building_Door;
				if (building_Door != null && !building_Door.PawnCanOpen(this.pawn) && !building_Door.Open)
				{
					return false;
				}
			}
			return true;
		}
		public void StartPath(TargetInfo dest, PathEndMode peMode)
		{
			GenPath.ResolvePathMode(ref dest, ref peMode);
			if (!this.PawnCanOccupy(this.pawn.Position) && !this.TryRecoverFromUnwalkablePosition(dest))
			{
				return;
			}
			if (this.moving && this.curPath != null && this.destination == dest && this.peMode == peMode)
			{
				return;
			}
			if (!this.pawn.Position.CanReach(dest, peMode, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false)))
			{
				this.PatherFailed();
				return;
			}
			if (!this.nextCell.Walkable())
			{
				this.nextCell = this.pawn.Position;
			}
			this.peMode = peMode;
			this.destination = dest;
			if (!this.destination.HasThing && Find.PawnDestinationManager.DestinationReservedFor(this.pawn) != this.destination.Cell)
			{
				Find.PawnDestinationManager.UnreserveAllFor(this.pawn);
			}
			if (this.AtDestinationPosition())
			{
				this.PatherArrived();
				return;
			}
			if (this.pawn.Downed)
			{
				Log.Error(this.pawn.LabelCap + " tried to path while incapacitated. This should never happen.");
				return;
			}
			if (this.curPath != null)
			{
				this.curPath.ReleaseToPool();
			}
			this.curPath = null;
			this.moving = true;
		}
		public void StopDead()
		{
			if (this.curPath != null)
			{
				this.curPath.ReleaseToPool();
			}
			this.curPath = null;
			this.moving = false;
			this.nextCell = this.pawn.Position;
		}
		public void PatherTick()
		{
			if (this.pawn.stances.FullBodyBusy)
			{
				return;
			}
			if (this.ticksUntilMove > 0)
			{
				this.ticksUntilMove--;
				return;
			}
			if (this.moving)
			{
				this.TryEnterNextPathCell();
			}
		}
		public Thing BuildingBlockingNextPathCell()
		{
			Thing edifice = this.nextCell.GetEdifice();
			if (edifice != null && edifice.BlocksPawn(this.pawn))
			{
				return edifice;
			}
			return null;
		}
		public Building_Door NextCellDoorToManuallyOpen()
		{
			Building_Door building_Door = Find.ThingGrid.ThingAt<Building_Door>(this.nextCell);
			if (building_Door != null && building_Door.SlowsPawns && !building_Door.Open && building_Door.PawnCanOpen(this.pawn))
			{
				return building_Door;
			}
			return null;
		}
		public void PatherDraw()
		{
			if (DebugViewSettings.drawPaths && this.curPath != null && Find.Selector.IsSelected(this.pawn))
			{
				this.curPath.DebugDrawPath(this.pawn);
			}
		}
		private bool TryRecoverFromUnwalkablePosition(TargetInfo originalDest)
		{
			bool flag = false;
			for (int i = 0; i < GenRadial.RadialPattern.Length; i++)
			{
				IntVec3 intVec = this.pawn.Position + GenRadial.RadialPattern[i];
				if (this.PawnCanOccupy(intVec))
				{
					Log.Warning(string.Concat(new object[]
					{
						this.pawn,
						" on unwalkable cell ",
						this.pawn.Position,
						". Teleporting to ",
						intVec
					}));
					this.pawn.Position = intVec;
					this.moving = false;
					this.nextCell = this.pawn.Position;
					this.StartPath(originalDest, this.peMode);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.pawn.Destroy(DestroyMode.Vanish);
				Log.Error(string.Concat(new object[]
				{
					this.pawn,
					" on unwalkable cell ",
					this.pawn.Position,
					". Could not find walkable position nearby. Destroyed."
				}));
			}
			return flag;
		}
		private void PatherArrived()
		{
			this.StopDead();
			if (this.pawn.jobs.curJob == null)
			{
				return;
			}
			this.pawn.jobs.curDriver.Notify_PatherArrived();
		}
		private void PatherFailed()
		{
			this.StopDead();
			this.pawn.jobs.curDriver.Notify_PatherFailed();
		}
		private void TryEnterNextPathCell()
		{
			Thing thing = this.BuildingBlockingNextPathCell();
			if (thing != null)
			{
				Building_Door building_Door = thing as Building_Door;
				if (building_Door == null || !building_Door.FreePassage)
				{
					if ((this.pawn.CurJob != null && this.pawn.CurJob.canBash) || this.pawn.HostileTo(thing))
					{
						Job job = new Job(JobDefOf.AttackMelee, thing);
						job.expiryInterval = 1100;
						this.pawn.jobs.StartJob(job, JobCondition.Incompletable, null, false, true, null);
						return;
					}
					this.PatherFailed();
					return;
				}
			}
			Building_Door building_Door2 = this.NextCellDoorToManuallyOpen();
			if (building_Door2 != null)
			{
				Stance_Cooldown stance_Cooldown = new Stance_Cooldown(building_Door2.TicksToOpenNow, building_Door2);
				stance_Cooldown.neverAimWeapon = true;
				this.pawn.stances.SetStance(stance_Cooldown);
				building_Door2.StartManualOpenBy(this.pawn);
				return;
			}
			this.lastCell = this.pawn.Position;
			this.pawn.Position = this.nextCell;
			if (this.pawn.RaceProps.Humanlike)
			{
				this.cellsUntilClamor--;
				if (this.cellsUntilClamor <= 0)
				{
					GenClamor.DoClamor(this.pawn, 7f, ClamorType.Movement);
					this.cellsUntilClamor = 12;
				}
			}
			this.pawn.filth.Notify_EnteredNewCell();
			if (this.pawn.BodySize > 0.9f)
			{
				Find.SnowGrid.AddDepth(this.pawn.Position, -0.005f);
			}
			Building_Door building_Door3 = Find.ThingGrid.ThingAt<Building_Door>(this.lastCell);
			if (building_Door3 != null && !building_Door3.CloseBlocked && !this.pawn.HostileTo(building_Door3))
			{
				building_Door3.FriendlyTouched();
				if (!building_Door3.HoldOpen && building_Door3.SlowsPawns)
				{
					building_Door3.StartManualCloseBy(this.pawn);
					return;
				}
			}
			if (this.NeedNewPath() && !this.TrySetNewPath())
			{
				return;
			}
			if (this.AtDestinationPosition())
			{
				this.PatherArrived();
			}
			else
			{
				if (this.curPath.NodesLeftCount == 0)
				{
					Log.Error(this.pawn + " ran out of path nodes. Force-arriving.");
					this.PatherArrived();
					return;
				}
				this.SetupMoveIntoNextCell();
			}
		}
		private void SetupMoveIntoNextCell()
		{
			if (this.curPath.NodesLeftCount < 2)
			{
				Log.Error(string.Concat(new object[]
				{
					this.pawn,
					" at ",
					this.pawn.Position,
					" ran out of path nodes while pathing to ",
					this.destination,
					"."
				}));
				this.PatherFailed();
				return;
			}
			this.nextCell = this.curPath.ConsumeNextNode();
			if (!this.nextCell.Walkable())
			{
				Log.Error(string.Concat(new object[]
				{
					this.pawn,
					" entering ",
					this.nextCell,
					" which is unwalkable."
				}));
			}
			Building_Door building_Door = Find.ThingGrid.ThingAt<Building_Door>(this.nextCell);
			if (building_Door != null)
			{
				building_Door.Notify_PawnApproaching(this.pawn);
			}
			int num = this.TicksToMoveIntoCell(this.nextCell);
			this.totalMoveDuration = num;
			this.ticksUntilMove = num;
		}
		private int TicksToMoveIntoCell(IntVec3 c)
		{
			int num;
			if (c.x == this.pawn.Position.x || c.z == this.pawn.Position.z)
			{
				num = this.pawn.TicksPerMoveCardinal;
			}
			else
			{
				num = this.pawn.TicksPerMoveDiagonal;
			}
			num += PathGrid.CalculatedCostAt(c, false);
			Building edifice = c.GetEdifice();
			if (edifice != null)
			{
				num += (int)edifice.PathWalkCostFor(this.pawn);
			}
			if (num > 450)
			{
				num = 450;
			}
			if (this.pawn.jobs.curJob != null)
			{
				switch (this.pawn.jobs.curJob.locomotionUrgency)
				{
				case LocomotionUrgency.Amble:
					num *= 3;
					if (num < 60)
					{
						num = 60;
					}
					break;
				case LocomotionUrgency.Walk:
					num *= 2;
					if (num < 50)
					{
						num = 50;
					}
					break;
				case LocomotionUrgency.Jog:
					num *= 1;
					break;
				case LocomotionUrgency.Sprint:
					num = Mathf.RoundToInt((float)num * 0.75f);
					break;
				}
			}
			return Mathf.Max(num, 1);
		}
		private bool TrySetNewPath()
		{
			PawnPath pawnPath = this.GenerateNewPath();
			if (!pawnPath.Found)
			{
				this.PatherFailed();
				return false;
			}
			if (this.curPath != null)
			{
				this.curPath.ReleaseToPool();
			}
			this.curPath = pawnPath;
			return true;
		}
		private PawnPath GenerateNewPath()
		{
			this.lastPathedTargetPosition = this.destination.Cell;
			return PathFinder.FindPath(this.pawn.Position, this.destination, this.pawn, this.peMode);
		}
		private bool AtDestinationPosition()
		{
			if (this.pawn.Position == this.destination.Cell)
			{
				return true;
			}
			if (this.peMode == PathEndMode.Touch)
			{
				if (!this.destination.HasThing)
				{
					if (this.pawn.Position.AdjacentTo8WayOrInside(this.destination.Cell))
					{
						return true;
					}
				}
				else
				{
					if (this.pawn.Position.AdjacentTo8WayOrInside(this.destination.Thing))
					{
						return true;
					}
				}
			}
			return false;
		}
		private bool NeedNewPath()
		{
			if (this.curPath == null || !this.curPath.Found || this.curPath.NodesLeftCount == 0)
			{
				return true;
			}
			if (this.lastPathedTargetPosition != this.destination.Cell)
			{
				float lengthHorizontalSquared = (this.pawn.Position - this.destination.Cell).LengthHorizontalSquared;
				float num;
				if (lengthHorizontalSquared > 900f)
				{
					num = 10f;
				}
				else
				{
					if (lengthHorizontalSquared > 289f)
					{
						num = 5f;
					}
					else
					{
						if (lengthHorizontalSquared > 100f)
						{
							num = 3f;
						}
						else
						{
							if (lengthHorizontalSquared > 49f)
							{
								num = 2f;
							}
							else
							{
								num = 0.5f;
							}
						}
					}
				}
				if ((this.lastPathedTargetPosition - this.destination.Cell).LengthHorizontalSquared > num * num)
				{
					return true;
				}
			}
			int num2 = 0;
			while (num2 < 20 && num2 < this.curPath.NodesLeftCount)
			{
				IntVec3 c = this.curPath.Peek(num2);
				if (!c.Walkable())
				{
					return true;
				}
				Building_Door building_Door = c.GetEdifice() as Building_Door;
				if (building_Door != null)
				{
					if (!building_Door.CanPhysicallyPass(this.pawn) && !this.pawn.HostileTo(building_Door))
					{
						return true;
					}
					if (building_Door.IsForbiddenToPass(this.pawn))
					{
						return true;
					}
				}
				num2++;
			}
			return false;
		}
	}
}
