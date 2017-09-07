using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
namespace RimWorld
{
	public class Building_Door : Building
	{
		private const float BaseDoorOpenTicks = 45f;
		private const int AutomaticCloseDelayTicks = 60;
		private const int ApproachCloseDelayTicks = 300;
		private const float VisualDoorOffsetStart = 0f;
		private const float VisualDoorOffsetEnd = 0.45f;
		private const int TempEqualizeIntervalOpen = 21;
		private const int TempEqualizeIntervalClosed = 250;
		private const float TempEqualizeRate = 0.55f;
		public CompPowerTrader powerComp;
		private bool openInt;
		private bool holdOpenInt;
		private bool freePassageUntilClosed;
		private int lastFriendlyTouchTick = -9999;
		protected int ticksUntilClose;
		protected int visualTicksOpen;
		public bool Open
		{
			get
			{
				return this.openInt;
			}
		}
		public bool HoldOpen
		{
			get
			{
				return this.holdOpenInt;
			}
		}
		public bool FreePassage
		{
			get
			{
				return this.openInt && (this.holdOpenInt || this.freePassageUntilClosed);
			}
		}
		public bool CloseBlocked
		{
			get
			{
				List<Thing> thingList = base.Position.GetThingList();
				for (int i = 0; i < thingList.Count; i++)
				{
					Thing thing = thingList[i];
					if (thing.def.category == ThingCategory.Pawn || thing.def.category == ThingCategory.Item)
					{
						return true;
					}
				}
				return false;
			}
		}
		public bool DoorPowerOn
		{
			get
			{
				return this.powerComp != null && this.powerComp.PowerOn;
			}
		}
		public bool SlowsPawns
		{
			get
			{
				return !this.DoorPowerOn || this.TicksToOpenNow > 20;
			}
		}
		public int TicksToOpenNow
		{
			get
			{
				float num = 45f / this.GetStatValue(StatDefOf.DoorOpenSpeed, true);
				if (this.DoorPowerOn)
				{
					num *= 0.25f;
				}
				return Mathf.RoundToInt(num);
			}
		}
		private int VisualTicksToOpen
		{
			get
			{
				return this.TicksToOpenNow;
			}
		}
		public override void PostMake()
		{
			base.PostMake();
			this.powerComp = base.GetComp<CompPowerTrader>();
		}
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			this.powerComp = base.GetComp<CompPowerTrader>();
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<bool>(ref this.openInt, "open", false, false);
			Scribe_Values.LookValue<bool>(ref this.holdOpenInt, "holdOpen", false, false);
			Scribe_Values.LookValue<bool>(ref this.freePassageUntilClosed, "freePassageUntilClosed", false, false);
			Scribe_Values.LookValue<int>(ref this.lastFriendlyTouchTick, "lastFriendlyTouchTick", 0, false);
			if (Scribe.mode == LoadSaveMode.LoadingVars && this.openInt)
			{
				this.visualTicksOpen = this.VisualTicksToOpen;
			}
		}
		public override void Tick()
		{
			base.Tick();
			if (!this.openInt)
			{
				if (this.visualTicksOpen > 0)
				{
					this.visualTicksOpen--;
				}
				if ((Find.TickManager.TicksGame + this.thingIDNumber.HashOffset()) % 250 == 0)
				{
					this.EqualizeTemps();
				}
			}
			else
			{
				if (this.openInt)
				{
					if (this.visualTicksOpen < this.VisualTicksToOpen)
					{
						this.visualTicksOpen++;
					}
					if (!this.holdOpenInt)
					{
						if (Find.ThingGrid.CellContains(base.Position, ThingCategory.Pawn))
						{
							this.ticksUntilClose = 60;
						}
						else
						{
							this.ticksUntilClose--;
							if (this.DoorPowerOn && this.ticksUntilClose <= 0 && Find.TickManager.TicksGame < this.lastFriendlyTouchTick + 200)
							{
								this.DoorTryClose();
							}
						}
					}
					if ((Find.TickManager.TicksGame + this.thingIDNumber.HashOffset()) % 21 == 0)
					{
						this.EqualizeTemps();
					}
				}
			}
		}
		public void FriendlyTouched()
		{
			this.lastFriendlyTouchTick = Find.TickManager.TicksGame;
		}
		private void EqualizeTemps()
		{
			Room[] array = new Room[4];
			int num = 0;
			float num2 = 0f;
			for (int i = 0; i < 4; i++)
			{
				IntVec3 intVec = base.Position + GenAdj.CardinalDirections[i];
				if (intVec.InBounds())
				{
					Room room = intVec.GetRoom();
					if (room != null)
					{
						num2 += room.Temperature;
						array[num] = room;
						num++;
					}
				}
			}
			if (num == 0)
			{
				return;
			}
			float num3 = num2 / (float)num;
			base.Position.GetRoom().Temperature = num3;
			for (int j = 0; j < num; j++)
			{
				float temperature = array[j].Temperature;
				float num4 = num3 - temperature;
				array[j].PushHeat(num4 * 0.55f);
			}
		}
		public void Notify_PawnApproaching(Pawn p)
		{
			if (this.PawnCanOpen(p) && !this.SlowsPawns)
			{
				this.DoorOpen(300);
			}
		}
		public bool CanPhysicallyPass(Pawn p)
		{
			return this.FreePassage || this.PawnCanOpen(p);
		}
		public virtual bool PawnCanOpen(Pawn p)
		{
			return PrisonBreakUtility.IsEscaping(p) || GenAI.MachinesLike(base.Faction, p);
		}
		public override bool BlocksPawn(Pawn p)
		{
			return !this.openInt && !this.PawnCanOpen(p);
		}
		protected void DoorOpen(int ticksToClose = 60)
		{
			this.ticksUntilClose = ticksToClose;
			if (!this.openInt)
			{
				this.openInt = true;
				if (this.holdOpenInt)
				{
					this.freePassageUntilClosed = true;
					Reachability.ClearCache();
				}
				if (this.DoorPowerOn)
				{
					this.def.building.soundDoorOpenPowered.PlayOneShot(base.Position);
				}
				else
				{
					this.def.building.soundDoorOpenManual.PlayOneShot(base.Position);
				}
			}
		}
		protected void DoorTryClose()
		{
			if (this.holdOpenInt || this.CloseBlocked)
			{
				return;
			}
			this.openInt = false;
			if (this.freePassageUntilClosed)
			{
				this.freePassageUntilClosed = false;
				Reachability.ClearCache();
			}
			if (this.DoorPowerOn)
			{
				this.def.building.soundDoorClosePowered.PlayOneShot(base.Position);
			}
			else
			{
				this.def.building.soundDoorCloseManual.PlayOneShot(base.Position);
			}
		}
		public void StartManualOpenBy(Pawn opener)
		{
			this.DoorOpen(60);
		}
		public void StartManualCloseBy(Pawn closer)
		{
			this.DoorTryClose();
		}
		public override void Draw()
		{
			base.Rotation = Building_Door.DoorRotationAt(base.Position);
			float num = Mathf.Clamp01((float)this.visualTicksOpen / (float)this.VisualTicksToOpen);
			float d = 0f + 0.45f * num;
			for (int i = 0; i < 2; i++)
			{
				Vector3 vector = default(Vector3);
				Mesh mesh;
				if (i == 0)
				{
					vector = new Vector3(0f, 0f, -1f);
					mesh = MeshPool.plane10;
				}
				else
				{
					vector = new Vector3(0f, 0f, 1f);
					mesh = MeshPool.plane10Flip;
				}
				Rot4 rotation = base.Rotation;
				rotation.Rotate(RotationDirection.Clockwise);
				vector = rotation.AsQuat * vector;
				Vector3 vector2 = this.DrawPos;
				vector2.y = Altitudes.AltitudeFor(AltitudeLayer.DoorMoveable);
				vector2 += vector * d;
				Graphics.DrawMesh(mesh, vector2, base.Rotation.AsQuat, this.Graphic.MatAt(base.Rotation, null), 0);
			}
			base.Comps_PostDraw();
		}
		private static int AlignQualityAgainst(IntVec3 c)
		{
			if (!c.InBounds())
			{
				return 0;
			}
			if (!c.Walkable())
			{
				return 9;
			}
			List<Thing> list = Find.ThingGrid.ThingsListAt(c);
			for (int i = 0; i < list.Count; i++)
			{
				Thing thing = list[i];
				if (thing.def.thingClass == typeof(Building_Door))
				{
					return 1;
				}
				Thing thing2 = thing as Blueprint;
				if (thing2 != null)
				{
					if (thing2.def.entityDefToBuild.passability == Traversability.Impassable)
					{
						return 9;
					}
					if (thing2.def.thingClass == typeof(Building_Door))
					{
						return 1;
					}
				}
			}
			return 0;
		}
		public static Rot4 DoorRotationAt(IntVec3 loc)
		{
			int num = 0;
			int num2 = 0;
			num += Building_Door.AlignQualityAgainst(loc + IntVec3.East);
			num += Building_Door.AlignQualityAgainst(loc + IntVec3.West);
			num2 += Building_Door.AlignQualityAgainst(loc + IntVec3.North);
			num2 += Building_Door.AlignQualityAgainst(loc + IntVec3.South);
			if (num >= num2)
			{
				return Rot4.North;
			}
			return Rot4.East;
		}
		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			Building_Door.<GetGizmos>c__IteratorDF <GetGizmos>c__IteratorDF = new Building_Door.<GetGizmos>c__IteratorDF();
			<GetGizmos>c__IteratorDF.<>f__this = this;
			Building_Door.<GetGizmos>c__IteratorDF expr_0E = <GetGizmos>c__IteratorDF;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
