using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Verse;
namespace RimWorld
{
	[StaticConstructorOnStartup]
	public class Building_WindTurbine : Building_PowerPlant
	{
		private const float MaxUsableWindIntensity = 1.5f;
		private const float SpinRateFactor = 0.05f;
		private const float PowerReductionPercentPerObstacle = 0.2f;
		private const string TranslateWindPathIsBlockedBy = "WindTurbine_WindPathIsBlockedBy";
		private const string TranslateWindPathIsBlockedByRoof = "WindTurbine_WindPathIsBlockedByRoof";
		public int updateWeatherEveryXTicks = 250;
		private int ticksSinceWeatherUpdate;
		private float cachedPowerOutput;
		private List<IntVec3> windPathCells = new List<IntVec3>();
		private List<Thing> windPathBlockedByThings = new List<Thing>();
		private List<IntVec3> windPathBlockedCells = new List<IntVec3>();
		private float spinPosition;
		private static Vector2 BarSize;
		private static readonly Material WindTurbineBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.5f, 0.475f, 0.1f));
		private static readonly Material WindTurbineBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f));
		private static readonly Material WindTurbineBladesMat = MaterialPool.MatFrom("Things/Building/Power/WindTurbine/WindTurbineBlades");
		protected override float DesiredPowerOutput
		{
			get
			{
				return this.cachedPowerOutput;
			}
		}
		private float PowerPercent
		{
			get
			{
				return this.powerComp.PowerOutput / (-this.powerComp.Props.basePowerConsumption * 1.5f);
			}
		}
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			Building_WindTurbine.BarSize = new Vector2((float)this.def.size.z - 0.95f, 0.14f);
			this.RecalculateBlockages();
			this.spinPosition = Rand.Range(0f, 15f);
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<int>(ref this.ticksSinceWeatherUpdate, "updateCounter", 0, false);
			Scribe_Values.LookValue<float>(ref this.cachedPowerOutput, "cachedPowerOutput", 0f, false);
		}
		public override void Tick()
		{
			base.Tick();
			if (this.powerComp == null || !this.powerComp.PowerOn)
			{
				this.cachedPowerOutput = 0f;
				return;
			}
			this.ticksSinceWeatherUpdate++;
			if (this.ticksSinceWeatherUpdate >= this.updateWeatherEveryXTicks)
			{
				float num = Mathf.Min(WindManager.WindSpeed, 1.5f);
				this.ticksSinceWeatherUpdate = 0;
				this.cachedPowerOutput = -(this.powerComp.Props.basePowerConsumption * num);
				this.RecalculateBlockages();
				if (this.windPathBlockedCells.Count > 0)
				{
					float num2 = 0f;
					for (int i = 0; i < this.windPathBlockedCells.Count; i++)
					{
						num2 += this.cachedPowerOutput * 0.2f;
					}
					this.cachedPowerOutput -= num2;
					if (this.cachedPowerOutput < 0f)
					{
						this.cachedPowerOutput = 0f;
					}
				}
			}
			if (this.cachedPowerOutput > 0.01f)
			{
				this.spinPosition += this.PowerPercent * 0.05f;
			}
		}
		public override void Draw()
		{
			base.Draw();
			GenDraw.FillableBarRequest r = new GenDraw.FillableBarRequest
			{
				center = this.DrawPos + Vector3.up * 0.1f,
				size = Building_WindTurbine.BarSize,
				fillPercent = this.PowerPercent,
				filledMat = Building_WindTurbine.WindTurbineBarFilledMat,
				unfilledMat = Building_WindTurbine.WindTurbineBarUnfilledMat,
				margin = 0.15f
			};
			Rot4 rotation = base.Rotation;
			rotation.Rotate(RotationDirection.Clockwise);
			r.rotation = rotation;
			GenDraw.DrawFillableBar(r);
			Vector3 vector = this.TrueCenter();
			vector += base.Rotation.FacingCell.ToVector3() * 0.7f;
			vector.y += 0.05f;
			float num = 4f * Mathf.Sin(this.spinPosition);
			if (num < 0f)
			{
				num *= -1f;
			}
			bool flag = this.spinPosition % 3.14159274f * 2f < 3.14159274f;
			Vector2 vector2 = new Vector2(num, 1f);
			Vector3 s = new Vector3(vector2.x, 1f, vector2.y);
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(vector, base.Rotation.AsQuat, s);
			Graphics.DrawMesh((!flag) ? MeshPool.plane10Flip : MeshPool.plane10, matrix, Building_WindTurbine.WindTurbineBladesMat, 0);
			vector.y -= 0.1f;
			matrix.SetTRS(vector, base.Rotation.AsQuat, s);
			Graphics.DrawMesh((!flag) ? MeshPool.plane10 : MeshPool.plane10Flip, matrix, Building_WindTurbine.WindTurbineBladesMat, 0);
		}
		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetInspectString());
			stringBuilder.AppendLine();
			if (this.windPathBlockedCells.Count > 0)
			{
				Thing thing = null;
				if (this.windPathBlockedByThings != null)
				{
					thing = this.windPathBlockedByThings[0];
				}
				if (thing != null)
				{
					stringBuilder.Append("WindTurbine_WindPathIsBlockedBy".Translate() + " " + thing.Label);
				}
				else
				{
					stringBuilder.Append("WindTurbine_WindPathIsBlockedByRoof".Translate());
				}
			}
			return stringBuilder.ToString();
		}
		private void RecalculateBlockages()
		{
			if (this.windPathCells.Count == 0)
			{
				IEnumerable<IntVec3> collection = Building_WindTurbine.CalculateWindCells(base.Position, base.Rotation, this.def.size);
				this.windPathCells.AddRange(collection);
			}
			this.windPathBlockedCells.Clear();
			this.windPathBlockedByThings.Clear();
			for (int i = 0; i < this.windPathCells.Count; i++)
			{
				IntVec3 intVec = this.windPathCells[i];
				if (Find.RoofGrid.Roofed(intVec))
				{
					this.windPathBlockedByThings.Add(null);
					this.windPathBlockedCells.Add(intVec);
				}
				else
				{
					List<Thing> list = Find.ThingGrid.ThingsListAt(intVec);
					for (int j = 0; j < list.Count; j++)
					{
						Thing thing = list[j];
						if (thing.def.altitudeLayer == AltitudeLayer.BuildingTall)
						{
							this.windPathBlockedByThings.Add(thing);
							this.windPathBlockedCells.Add(intVec);
							break;
						}
					}
				}
			}
		}
		[DebuggerHidden]
		public static IEnumerable<IntVec3> CalculateWindCells(IntVec3 center, Rot4 rot, IntVec2 size)
		{
			Building_WindTurbine.<CalculateWindCells>c__IteratorE6 <CalculateWindCells>c__IteratorE = new Building_WindTurbine.<CalculateWindCells>c__IteratorE6();
			<CalculateWindCells>c__IteratorE.rot = rot;
			<CalculateWindCells>c__IteratorE.center = center;
			<CalculateWindCells>c__IteratorE.<$>rot = rot;
			<CalculateWindCells>c__IteratorE.<$>center = center;
			Building_WindTurbine.<CalculateWindCells>c__IteratorE6 expr_23 = <CalculateWindCells>c__IteratorE;
			expr_23.$PC = -2;
			return expr_23;
		}
	}
}
