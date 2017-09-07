using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	[StaticConstructorOnStartup]
	public class Building_PowerPlantSolar : Building_PowerPlant
	{
		private const float FullSunPower = 1700f;
		private const float NightPower = 0f;
		private static readonly Vector2 BarSize = new Vector2(2.3f, 0.14f);
		private static readonly Material PowerPlantSolarBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.5f, 0.475f, 0.1f));
		private static readonly Material PowerPlantSolarBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f));
		protected override float DesiredPowerOutput
		{
			get
			{
				if (Find.RoofGrid.Roofed(base.Position))
				{
					return 0f;
				}
				return Mathf.Lerp(0f, 1700f, SkyManager.CurSkyGlow);
			}
		}
		public override void Draw()
		{
			base.Draw();
			GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
			r.center = this.DrawPos + Vector3.up * 0.1f;
			r.size = Building_PowerPlantSolar.BarSize;
			r.fillPercent = this.powerComp.PowerOutput / 1700f;
			r.filledMat = Building_PowerPlantSolar.PowerPlantSolarBarFilledMat;
			r.unfilledMat = Building_PowerPlantSolar.PowerPlantSolarBarUnfilledMat;
			r.margin = 0.15f;
			Rot4 rotation = base.Rotation;
			rotation.Rotate(RotationDirection.Clockwise);
			r.rotation = rotation;
			GenDraw.DrawFillableBar(r);
		}
	}
}
