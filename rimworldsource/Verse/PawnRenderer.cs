using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Verse
{
	public class PawnRenderer
	{
		private const float CarriedThingDrawAngle = 16f;
		private const float AltInterval = 0.005f;
		private const int NumAltIntervals = 10;
		private const float UpHeadOffset = 0.34f;
		private Pawn pawn;
		public PawnGraphicSet graphics;
		public PawnDownedWiggler wiggler;
		private PawnHeadOverlays statusOverlays;
		private PawnStatusEffecters effecters;
		private PawnWoundDrawer woundOverlays;
		private Graphic_Shadow shadowGraphic;
		private static readonly float[] HorHeadOffsets = new float[]
		{
			0f,
			0.04f,
			0.1f,
			0.09f,
			0.1f,
			0.09f
		};
		public PawnRenderer(Pawn pawn)
		{
			this.pawn = pawn;
			this.wiggler = new PawnDownedWiggler(pawn);
			this.statusOverlays = new PawnHeadOverlays(pawn);
			this.woundOverlays = new PawnWoundDrawer(pawn);
			this.graphics = new PawnGraphicSet(pawn);
			this.effecters = new PawnStatusEffecters(pawn);
		}
		private Rot4 LayingFacing()
		{
			if (this.pawn.GetPosture() == PawnPosture.LayingFaceUp)
			{
				return Rot4.South;
			}
			if (this.pawn.RaceProps.Humanlike)
			{
				switch (this.pawn.thingIDNumber % 4)
				{
				case 0:
					return Rot4.South;
				case 1:
					return Rot4.South;
				case 2:
					return Rot4.East;
				case 3:
					return Rot4.West;
				}
			}
			else
			{
				switch (this.pawn.thingIDNumber % 4)
				{
				case 0:
					return Rot4.South;
				case 1:
					return Rot4.East;
				case 2:
					return Rot4.West;
				case 3:
					return Rot4.West;
				}
			}
			return Rot4.Random;
		}
		public void RenderPawnAt(Vector3 drawLoc, RotDrawMode bodyDrawType = RotDrawMode.Fresh)
		{
			if (!this.graphics.AllResolved)
			{
				this.graphics.ResolveAllGraphics();
			}
			if (this.pawn.GetPosture() == PawnPosture.Standing)
			{
				this.RenderPawnInternal(drawLoc, Quaternion.identity, true, bodyDrawType);
				if (this.pawn.carrier != null)
				{
					Thing carriedThing = this.pawn.carrier.CarriedThing;
					if (carriedThing != null)
					{
						Vector3 vector = drawLoc;
						vector.y += 0.0449999981f;
						Vector2 vector2;
						if (this.pawn.CurJob != null && this.pawn.jobs.curDriver.TryGetCarriedThingDrawPos(out vector2))
						{
							vector.x = vector2.x;
							vector.z = vector2.y;
						}
						else
						{
							if (carriedThing is Pawn || carriedThing is Corpse)
							{
								vector.x += 0.5f;
							}
							else
							{
								vector += new Vector3(0.18f, 0f, 0.05f);
							}
						}
						carriedThing.DrawAt(vector);
					}
				}
				if (this.pawn.def.race.specialShadowData != null)
				{
					if (this.shadowGraphic == null)
					{
						this.shadowGraphic = new Graphic_Shadow(this.pawn.def.race.specialShadowData);
					}
					this.shadowGraphic.Draw(drawLoc, Rot4.North, this.pawn);
				}
				if (this.graphics.nakedGraphic != null && this.graphics.nakedGraphic.ShadowGraphic != null)
				{
					this.graphics.nakedGraphic.ShadowGraphic.Draw(drawLoc, Rot4.North, this.pawn);
				}
			}
			else
			{
				Rot4 rot = this.LayingFacing();
				Building_Bed building_Bed = this.pawn.CurrentBed();
				bool renderBody;
				Quaternion quat;
				Vector3 loc;
				if (building_Bed != null && this.pawn.RaceProps.Humanlike)
				{
					renderBody = building_Bed.def.building.bed_showSleeperBody;
					Rot4 rotation = building_Bed.Rotation;
					rotation.AsInt += 2;
					quat = rotation.AsQuat;
					AltitudeLayer altLayer = (AltitudeLayer)Mathf.Max((int)building_Bed.def.altitudeLayer, 12);
					Vector3 a = this.pawn.Position.ToVector3ShiftedWithAltitude(altLayer);
					float d = -this.BaseHeadOffsetAt(Rot4.South).z;
					Vector3 a2 = rotation.FacingCell.ToVector3();
					loc = a + a2 * d;
					loc.y -= 0.005f;
				}
				else
				{
					renderBody = true;
					loc = drawLoc;
					if (this.pawn.Downed || this.pawn.Dead)
					{
						quat = Quaternion.AngleAxis(this.wiggler.downedAngle, Vector3.up);
					}
					else
					{
						if (this.pawn.RaceProps.Humanlike)
						{
							quat = rot.AsQuat;
						}
						else
						{
							Rot4 rot2 = Rot4.West;
							int num = this.pawn.thingIDNumber % 2;
							if (num != 0)
							{
								if (num == 1)
								{
									rot2 = Rot4.East;
								}
							}
							else
							{
								rot2 = Rot4.West;
							}
							quat = rot2.AsQuat;
						}
					}
				}
				this.RenderPawnInternal(loc, quat, renderBody, rot, rot, bodyDrawType);
			}
			if (this.pawn.Spawned && !this.pawn.Dead)
			{
				this.pawn.stances.StanceTrackerDraw();
				this.pawn.pather.PatherDraw();
			}
			this.DrawDebug();
		}
		private void RenderPawnInternal(Vector3 loc, Quaternion quat, bool renderBody, RotDrawMode draw = RotDrawMode.Fresh)
		{
			this.RenderPawnInternal(loc, quat, renderBody, this.pawn.Rotation, this.pawn.Rotation, draw);
		}
		private void RenderPawnInternal(Vector3 loc, Quaternion quat, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType = RotDrawMode.Fresh)
		{
			if (!this.graphics.AllResolved)
			{
				this.graphics.ResolveAllGraphics();
			}
			Mesh mesh = null;
			if (renderBody)
			{
				if (this.pawn.RaceProps.Humanlike)
				{
					mesh = MeshPool.humanlikeBodySet.MeshAt(bodyFacing);
				}
				else
				{
					mesh = this.graphics.nakedGraphic.MeshAt(bodyFacing);
				}
				List<Material> list = this.graphics.MatsBodyBaseAt(bodyFacing, bodyDrawType);
				for (int i = 0; i < list.Count; i++)
				{
					Material damagedMat = this.graphics.flasher.GetDamagedMat(list[i]);
					Graphics.DrawMesh(mesh, loc, quat, damagedMat, 0);
					loc.y += 0.005f;
				}
				if (bodyDrawType == RotDrawMode.Fresh)
				{
					this.woundOverlays.RenderOverBody(loc, mesh, quat);
					loc.y += 0.005f;
				}
			}
			float y;
			float y2;
			if (bodyFacing == Rot4.North)
			{
				y = loc.y;
				y2 = loc.y + 0.02f;
			}
			else
			{
				y = loc.y + 0.02f;
				y2 = loc.y;
			}
			loc.y += 0.01f;
			if (this.graphics.headGraphic != null)
			{
				loc.y = y;
				Vector3 b = quat * this.BaseHeadOffsetAt(headFacing);
				Mesh mesh2 = MeshPool.humanlikeHeadSet.MeshAt(headFacing);
				Material material = this.graphics.HeadMatAt(headFacing, bodyDrawType);
				Graphics.DrawMesh(mesh2, loc + b, quat, material, 0);
				loc.y += 0.005f;
				bool flag = false;
				Mesh mesh3 = this.graphics.HairMeshSet.MeshAt(headFacing);
				List<ApparelGraphicRecord> apparelGraphics = this.graphics.apparelGraphics;
				for (int j = 0; j < apparelGraphics.Count; j++)
				{
					if (apparelGraphics[j].sourceApparel.def.apparel.LastLayer == ApparelLayer.Overhead)
					{
						flag = true;
						Material material2 = apparelGraphics[j].graphic.MatAt(bodyFacing, null);
						material2 = this.graphics.flasher.GetDamagedMat(material2);
						Graphics.DrawMesh(mesh3, loc + b, quat, material2, 0);
						loc.y += 0.005f;
					}
				}
				if (!flag && bodyDrawType != RotDrawMode.Dessicated)
				{
					Mesh mesh4 = this.graphics.HairMeshSet.MeshAt(headFacing);
					Material material3 = this.graphics.HairMatAt(headFacing);
					Graphics.DrawMesh(mesh4, loc + b, quat, material3, 0);
					loc.y += 0.005f;
				}
			}
			if (renderBody)
			{
				loc.y = y2;
				for (int k = 0; k < this.graphics.apparelGraphics.Count; k++)
				{
					ApparelGraphicRecord apparelGraphicRecord = this.graphics.apparelGraphics[k];
					if (apparelGraphicRecord.sourceApparel.def.apparel.LastLayer == ApparelLayer.Shell)
					{
						Material material4 = apparelGraphicRecord.graphic.MatAt(bodyFacing, null);
						material4 = this.graphics.flasher.GetDamagedMat(material4);
						Graphics.DrawMesh(mesh, loc, quat, material4, 0);
						loc.y += 0.005f;
					}
				}
			}
			this.DrawEquipment(loc);
			loc.y += 0.005f;
			if (this.pawn.apparel != null)
			{
				List<Apparel> wornApparel = this.pawn.apparel.WornApparel;
				for (int l = 0; l < wornApparel.Count; l++)
				{
					wornApparel[l].DrawWornExtras();
				}
			}
			this.statusOverlays.RenderStatusOverlays(loc, quat, MeshPool.humanlikeHeadSet.MeshAt(headFacing));
		}
		private void DrawEquipment(Vector3 rootLoc)
		{
			if (this.pawn.Dead || !this.pawn.Spawned)
			{
				return;
			}
			if (this.pawn.equipment == null || this.pawn.equipment.Primary == null)
			{
				return;
			}
			if (this.pawn.CurJob != null && this.pawn.CurJob.def.neverShowWeapon)
			{
				return;
			}
			rootLoc.y += 0.0449999981f;
			Stance_Busy stance_Busy = this.pawn.stances.curStance as Stance_Busy;
			if (stance_Busy != null && !stance_Busy.neverAimWeapon && stance_Busy.focusTarg.IsValid)
			{
				Vector3 a;
				if (stance_Busy.focusTarg.HasThing)
				{
					a = stance_Busy.focusTarg.Thing.DrawPos;
				}
				else
				{
					a = stance_Busy.focusTarg.Cell.ToVector3Shifted();
				}
				float num = 0f;
				if ((a - this.pawn.DrawPos).MagnitudeHorizontalSquared() > 0.001f)
				{
					num = (a - this.pawn.DrawPos).AngleFlat();
				}
				Vector3 b = new Vector3(0f, 0f, 0.4f).RotatedBy(num);
				this.DrawEquipmentAiming(this.pawn.equipment.Primary, rootLoc + b, num);
			}
			else
			{
				if (this.CarryWeaponOpenly())
				{
					if (this.pawn.Rotation == Rot4.South)
					{
						Vector3 drawLoc = rootLoc + new Vector3(0f, 0f, -0.22f);
						this.DrawEquipmentAiming(this.pawn.equipment.Primary, drawLoc, 143f);
					}
					else
					{
						if (this.pawn.Rotation == Rot4.East)
						{
							Vector3 drawLoc2 = rootLoc + new Vector3(0.2f, 0f, -0.22f);
							this.DrawEquipmentAiming(this.pawn.equipment.Primary, drawLoc2, 143f);
						}
						else
						{
							if (this.pawn.Rotation == Rot4.West)
							{
								Vector3 drawLoc3 = rootLoc + new Vector3(-0.2f, 0f, -0.22f);
								this.DrawEquipmentAiming(this.pawn.equipment.Primary, drawLoc3, 217f);
							}
						}
					}
				}
			}
		}
		public void DrawEquipmentAiming(Thing eq, Vector3 drawLoc, float aimAngle)
		{
			float num = aimAngle - 90f;
			Mesh mesh;
			if (aimAngle > 20f && aimAngle < 160f)
			{
				mesh = MeshPool.plane10;
				num += eq.def.equippedAngleOffset;
			}
			else
			{
				if (aimAngle > 200f && aimAngle < 340f)
				{
					mesh = MeshPool.plane10Flip;
					num -= 180f;
					num -= eq.def.equippedAngleOffset;
				}
				else
				{
					mesh = MeshPool.plane10;
					num += eq.def.equippedAngleOffset;
				}
			}
			num %= 360f;
			Graphic_StackCount graphic_StackCount = eq.Graphic as Graphic_StackCount;
			Material matSingle;
			if (graphic_StackCount != null)
			{
				matSingle = graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingle;
			}
			else
			{
				matSingle = eq.Graphic.MatSingle;
			}
			Graphics.DrawMesh(mesh, drawLoc, Quaternion.AngleAxis(num, Vector3.up), matSingle, 0);
		}
		private bool CarryWeaponOpenly()
		{
			return (this.pawn.carrier == null || this.pawn.carrier.CarriedThing == null) && (this.pawn.Drafted || (this.pawn.CurJob != null && this.pawn.CurJob.def.alwaysShowWeapon) || (this.pawn.mindState.duty != null && this.pawn.mindState.duty.def.alwaysShowWeapon));
		}
		public Vector3 BaseHeadOffsetAt(Rot4 rotation)
		{
			float num = PawnRenderer.HorHeadOffsets[(int)this.pawn.story.BodyType];
			switch (rotation.AsInt)
			{
			case 0:
				return new Vector3(0f, 0f, 0.34f);
			case 1:
				return new Vector3(num, 0f, 0.34f);
			case 2:
				return new Vector3(0f, 0f, 0.34f);
			case 3:
				return new Vector3(-num, 0f, 0.34f);
			default:
				Log.Error("BaseHeadOffsetAt error in " + this.pawn);
				return Vector3.zero;
			}
		}
		public void Notify_DamageApplied(DamageInfo dam)
		{
			this.graphics.flasher.Notify_DamageApplied(dam);
			this.wiggler.Notify_DamageApplied(dam);
		}
		public void RendererTick()
		{
			this.wiggler.WigglerTick();
			this.effecters.EffectersTick();
		}
		private void DrawDebug()
		{
			if (DebugViewSettings.drawDuties && Find.Selector.IsSelected(this.pawn) && this.pawn.mindState != null && this.pawn.mindState.duty != null)
			{
				this.pawn.mindState.duty.DrawDebug(this.pawn);
			}
		}
	}
}
