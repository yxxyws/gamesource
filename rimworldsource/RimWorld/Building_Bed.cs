using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class Building_Bed : Building, IAssignableBuilding
	{
		private bool forPrisonersInt;
		private bool medicalInt;
		private bool alreadySetDefaultMed;
		public List<Pawn> owners = new List<Pawn>();
		private static readonly Color SheetColorNormal = new Color(0.6313726f, 0.8352941f, 0.7058824f);
		private static readonly Color SheetColorRoyal = new Color(0.670588255f, 0.9137255f, 0.745098054f);
		private static readonly Color SheetColorForPrisoner = new Color(1f, 0.7176471f, 0.129411772f);
		private static readonly Color SheetColorMedical = new Color(0.3882353f, 0.623529434f, 0.8862745f);
		private static readonly Color SheetColorMedicalForPrisoner = new Color(0.654902f, 0.3764706f, 0.152941182f);
		private static int lastConfirmFrame = -1;
		public bool ForPrisoners
		{
			get
			{
				return this.forPrisonersInt;
			}
			set
			{
				if (value == this.forPrisonersInt || !this.def.building.bed_humanlike)
				{
					return;
				}
				if (Game.Mode != GameMode.MapPlaying)
				{
					Log.Error("Tried to set ForPrisoners while game mode was " + Game.Mode);
					return;
				}
				this.RemoveAllOwners();
				this.forPrisonersInt = value;
				this.Notify_ColorChanged();
				Find.MapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things);
				this.NotifyRoomBedTypeChanged();
			}
		}
		public bool Medical
		{
			get
			{
				return this.medicalInt;
			}
			set
			{
				if (value == this.medicalInt || !this.def.building.bed_humanlike)
				{
					return;
				}
				this.RemoveAllOwners();
				this.medicalInt = value;
				this.Notify_ColorChanged();
				Find.MapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things);
				this.NotifyRoomBedTypeChanged();
				this.FacilityChanged();
			}
		}
		public bool AnyUnownedSleepingSlot
		{
			get
			{
				if (this.Medical)
				{
					Log.Warning("Tried to check for unowned sleeping slot on medical bed " + this);
					return false;
				}
				return this.owners.Count < this.SleepingSlotsCount;
			}
		}
		public bool AnyUnoccupiedSleepingSlot
		{
			get
			{
				for (int i = 0; i < this.SleepingSlotsCount; i++)
				{
					if (this.GetCurOccupant(i) == null)
					{
						return true;
					}
				}
				return false;
			}
		}
		public IEnumerable<Pawn> CurOccupants
		{
			get
			{
				Building_Bed.<>c__IteratorDC <>c__IteratorDC = new Building_Bed.<>c__IteratorDC();
				<>c__IteratorDC.<>f__this = this;
				Building_Bed.<>c__IteratorDC expr_0E = <>c__IteratorDC;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public override Color DrawColor
		{
			get
			{
				if (this.def.MadeFromStuff)
				{
					return base.DrawColor;
				}
				return this.DrawColorTwo;
			}
		}
		public override Color DrawColorTwo
		{
			get
			{
				if (!this.def.building.bed_humanlike)
				{
					return base.DrawColorTwo;
				}
				bool forPrisoners = this.ForPrisoners;
				bool medical = this.Medical;
				if (forPrisoners && medical)
				{
					return Building_Bed.SheetColorMedicalForPrisoner;
				}
				if (forPrisoners)
				{
					return Building_Bed.SheetColorForPrisoner;
				}
				if (medical)
				{
					return Building_Bed.SheetColorMedical;
				}
				if (this.def == ThingDefOf.RoyalBed)
				{
					return Building_Bed.SheetColorRoyal;
				}
				return Building_Bed.SheetColorNormal;
			}
		}
		public int SleepingSlotsCount
		{
			get
			{
				return this.def.size.x;
			}
		}
		public IEnumerable<Pawn> AssigningCandidates
		{
			get
			{
				return Find.MapPawns.FreeColonists;
			}
		}
		public IEnumerable<Pawn> AssignedPawns
		{
			get
			{
				return this.owners;
			}
		}
		public int MaxAssignedPawnsCount
		{
			get
			{
				return this.SleepingSlotsCount;
			}
		}
		public void TryAssignPawn(Pawn owner)
		{
			owner.ownership.ClaimBedIfNonMedical(this);
		}
		public void TryUnassignPawn(Pawn pawn)
		{
			if (this.owners.Contains(pawn))
			{
				pawn.ownership.UnclaimBed();
			}
		}
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			Room room = RoomQuery.RoomAt(base.Position);
			if (room != null && room.isPrisonCell)
			{
				this.ForPrisoners = true;
			}
			if (!this.alreadySetDefaultMed)
			{
				this.alreadySetDefaultMed = true;
				if (this.def.building.bed_defaultMedical)
				{
					this.Medical = true;
				}
			}
		}
		public override void DeSpawn()
		{
			this.RemoveAllOwners();
			this.ForPrisoners = false;
			this.Medical = false;
			this.alreadySetDefaultMed = false;
			Room room = base.Position.GetRoom();
			base.DeSpawn();
			if (room != null)
			{
				room.RoomChanged();
			}
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<bool>(ref this.forPrisonersInt, "forPrisoners", false, false);
			Scribe_Values.LookValue<bool>(ref this.medicalInt, "medical", false, false);
			Scribe_Values.LookValue<bool>(ref this.alreadySetDefaultMed, "alreadySetDefaultMed", false, false);
			Scribe_Collections.LookList<Pawn>(ref this.owners, "owners", LookMode.MapReference, new object[0]);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				this.owners.RemoveAll((Pawn x) => x == null);
			}
		}
		public override void DrawExtraSelectionOverlays()
		{
			base.DrawExtraSelectionOverlays();
			if (this.ForPrisoners)
			{
				Room room = this.GetRoom();
				if (room != null)
				{
					room.DrawFieldEdges();
				}
			}
		}
		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			Building_Bed.<GetGizmos>c__IteratorDD <GetGizmos>c__IteratorDD = new Building_Bed.<GetGizmos>c__IteratorDD();
			<GetGizmos>c__IteratorDD.<>f__this = this;
			Building_Bed.<GetGizmos>c__IteratorDD expr_0E = <GetGizmos>c__IteratorDD;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		private void TrySetForPrisonersByInterface(bool newForPrisoners)
		{
			Room room = RoomQuery.RoomAt(base.Position);
			List<Building_Bed> bedsToAffect;
			if (room == null || room.TouchesMapEdge || room.IsHuge)
			{
				bedsToAffect = new List<Building_Bed>();
				bedsToAffect.Add(this);
			}
			else
			{
				bedsToAffect = room.ContainedBeds.ToList<Building_Bed>();
			}
			bedsToAffect.RemoveAll((Building_Bed b) => b.ForPrisoners == newForPrisoners);
			Action action = delegate
			{
				foreach (Building_Bed current2 in bedsToAffect)
				{
					current2.ForPrisoners = newForPrisoners;
					for (int j = 0; j < this.SleepingSlotsCount; j++)
					{
						Pawn curOccupant = this.GetCurOccupant(j);
						if (curOccupant != null)
						{
							curOccupant.jobs.EndCurrentJob(JobCondition.InterruptForced);
						}
					}
				}
				if (room != null)
				{
					room.RoomChanged();
				}
			};
			if ((
				from b in bedsToAffect
				where b.owners.Any<Pawn>() && b != this
				select b).Count<Building_Bed>() == 0)
			{
				action();
			}
			else
			{
				if (Building_Bed.lastConfirmFrame != Time.frameCount)
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (newForPrisoners)
					{
						stringBuilder.Append("TurningOnPrisonerBedWarning".Translate());
					}
					else
					{
						stringBuilder.Append("TurningOffPrisonerBedWarning".Translate());
					}
					stringBuilder.AppendLine();
					foreach (Building_Bed current in bedsToAffect)
					{
						if ((newForPrisoners && !current.ForPrisoners) || (!newForPrisoners && current.ForPrisoners))
						{
							for (int i = 0; i < current.owners.Count; i++)
							{
								stringBuilder.AppendLine();
								stringBuilder.Append(current.owners[i].NameStringShort);
							}
						}
					}
					stringBuilder.AppendLine();
					stringBuilder.AppendLine();
					stringBuilder.Append("AreYouSure".Translate());
					Find.WindowStack.Add(new Dialog_Confirm(stringBuilder.ToString(), action, false));
					Building_Bed.lastConfirmFrame = Time.frameCount;
				}
			}
		}
		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetInspectString());
			if (this.def.building.bed_humanlike)
			{
				if (this.ForPrisoners)
				{
					stringBuilder.AppendLine("ForPrisonerUse".Translate());
				}
				else
				{
					stringBuilder.AppendLine("ForColonistUse".Translate());
				}
				if (this.Medical)
				{
					stringBuilder.AppendLine("MedicalBed".Translate());
				}
				else
				{
					if (this.owners.Count == 0)
					{
						stringBuilder.AppendLine("Owner".Translate() + ": " + "Nobody".Translate());
					}
					else
					{
						if (this.owners.Count == 1)
						{
							stringBuilder.AppendLine("Owner".Translate() + ": " + this.owners[0].LabelCap);
						}
						else
						{
							stringBuilder.Append("Owners".Translate() + ": ");
							bool flag = false;
							for (int i = 0; i < this.owners.Count; i++)
							{
								if (flag)
								{
									stringBuilder.Append(", ");
								}
								flag = true;
								stringBuilder.Append(this.owners[i].LabelBaseShort);
							}
							stringBuilder.AppendLine();
						}
					}
				}
			}
			return stringBuilder.ToString();
		}
		[DebuggerHidden]
		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
		{
			Building_Bed.<GetFloatMenuOptions>c__IteratorDE <GetFloatMenuOptions>c__IteratorDE = new Building_Bed.<GetFloatMenuOptions>c__IteratorDE();
			<GetFloatMenuOptions>c__IteratorDE.myPawn = myPawn;
			<GetFloatMenuOptions>c__IteratorDE.<$>myPawn = myPawn;
			<GetFloatMenuOptions>c__IteratorDE.<>f__this = this;
			Building_Bed.<GetFloatMenuOptions>c__IteratorDE expr_1C = <GetFloatMenuOptions>c__IteratorDE;
			expr_1C.$PC = -2;
			return expr_1C;
		}
		public override void DrawGUIOverlay()
		{
			if (this.Medical)
			{
				return;
			}
			if (Find.CameraMap.CurrentZoom == CameraZoomRange.Closest)
			{
				Color textColor = new Color(1f, 1f, 1f, 0.75f);
				if (!this.owners.Any<Pawn>())
				{
					GenWorldUI.DrawThingLabel(this, "Unowned".Translate(), textColor);
				}
				else
				{
					if (this.owners.Count == 1)
					{
						if (this.owners[0].InBed() && this.owners[0].CurrentBed() == this)
						{
							return;
						}
						GenWorldUI.DrawThingLabel(this, this.owners[0].NameStringShort, textColor);
					}
					else
					{
						for (int i = 0; i < this.owners.Count; i++)
						{
							if (!this.owners[i].InBed() || this.owners[i].CurrentBed() != this || !(this.owners[i].Position == this.GetSleepingSlotPos(i)))
							{
								Vector3 multiOwnersLabelScreenPosFor = this.GetMultiOwnersLabelScreenPosFor(i);
								GenWorldUI.DrawThingLabel(multiOwnersLabelScreenPosFor, this.owners[i].NameStringShort, textColor);
							}
						}
					}
				}
			}
		}
		public Pawn GetCurOccupant(int slotIndex)
		{
			IntVec3 sleepingSlotPos = this.GetSleepingSlotPos(slotIndex);
			List<Thing> list = Find.ThingGrid.ThingsListAt(sleepingSlotPos);
			for (int i = 0; i < list.Count; i++)
			{
				Pawn pawn = list[i] as Pawn;
				if (pawn != null)
				{
					if (pawn.CurJob != null)
					{
						if (pawn.jobs.curDriver.layingDown && pawn.jobs.curDriver.layingDownBed == this)
						{
							return pawn;
						}
					}
				}
			}
			return null;
		}
		public int GetCurOccupantSlotIndex(Pawn curOccupant)
		{
			for (int i = 0; i < this.SleepingSlotsCount; i++)
			{
				if (this.GetCurOccupant(i) == curOccupant)
				{
					return i;
				}
			}
			Log.Error("Could not find pawn " + curOccupant + " on any of sleeping slots.");
			return 0;
		}
		public Pawn GetCurOccupantAt(IntVec3 pos)
		{
			for (int i = 0; i < this.SleepingSlotsCount; i++)
			{
				if (this.GetSleepingSlotPos(i) == pos)
				{
					return this.GetCurOccupant(i);
				}
			}
			return null;
		}
		public IntVec3 GetSleepingSlotPos(int index)
		{
			if (index < 0 || index >= this.SleepingSlotsCount)
			{
				Log.Error(string.Concat(new object[]
				{
					"Tried to get sleeping slot pos with index ",
					index,
					", but there are only ",
					this.SleepingSlotsCount,
					" sleeping slots available."
				}));
				return base.Position;
			}
			CellRect cellRect = this.OccupiedRect();
			if (base.Rotation == Rot4.North)
			{
				return new IntVec3(cellRect.minX + index, base.Position.y, cellRect.minZ);
			}
			if (base.Rotation == Rot4.East)
			{
				return new IntVec3(cellRect.minX, base.Position.y, cellRect.maxZ - index);
			}
			if (base.Rotation == Rot4.South)
			{
				return new IntVec3(cellRect.minX + index, base.Position.y, cellRect.maxZ);
			}
			return new IntVec3(cellRect.maxX, base.Position.y, cellRect.maxZ - index);
		}
		private void RemoveAllOwners()
		{
			for (int i = this.owners.Count - 1; i >= 0; i--)
			{
				this.owners[i].ownership.UnclaimBed();
			}
		}
		private void NotifyRoomBedTypeChanged()
		{
			Room room = this.GetRoom();
			if (room != null)
			{
				room.Notify_BedTypeChanged();
			}
		}
		private void FacilityChanged()
		{
			CompFacility compFacility = this.TryGetComp<CompFacility>();
			CompAffectedByFacilities compAffectedByFacilities = this.TryGetComp<CompAffectedByFacilities>();
			if (compFacility != null)
			{
				compFacility.Notify_ThingChanged();
			}
			if (compAffectedByFacilities != null)
			{
				compAffectedByFacilities.Notify_ThingChanged();
			}
		}
		private Vector3 GetMultiOwnersLabelScreenPosFor(int slotIndex)
		{
			IntVec3 sleepingSlotPos = this.GetSleepingSlotPos(slotIndex);
			Vector3 drawPos = this.DrawPos;
			if (base.Rotation.IsHorizontal)
			{
				drawPos.z = (float)sleepingSlotPos.z + 0.6f;
			}
			else
			{
				drawPos.x = (float)sleepingSlotPos.x + 0.5f;
				drawPos.z += -0.4f;
			}
			Vector2 v = drawPos.ToScreenPosition();
			if (!base.Rotation.IsHorizontal && this.SleepingSlotsCount == 2)
			{
				v = this.AdjustOwnerLabelPosToAvoidOverlapping(v, slotIndex);
			}
			return v;
		}
		private Vector3 AdjustOwnerLabelPosToAvoidOverlapping(Vector3 screenPos, int slotIndex)
		{
			Text.Font = GameFont.Tiny;
			float num = Text.CalcSize(this.owners[slotIndex].NameStringShort).x + 1f;
			Vector2 vector = this.DrawPos.ToScreenPosition();
			float num2 = Mathf.Abs(screenPos.x - vector.x);
			IntVec3 sleepingSlotPos = this.GetSleepingSlotPos(slotIndex);
			if (num > num2 * 2f)
			{
				float num3;
				if (slotIndex == 0)
				{
					num3 = (float)this.GetSleepingSlotPos(1).x;
				}
				else
				{
					num3 = (float)this.GetSleepingSlotPos(0).x;
				}
				if ((float)sleepingSlotPos.x < num3)
				{
					screenPos.x -= (num - num2 * 2f) / 2f;
				}
				else
				{
					screenPos.x += (num - num2 * 2f) / 2f;
				}
			}
			return screenPos;
		}
	}
}
