using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	[StaticConstructorOnStartup]
	public class Building_Campfire : Building_WorkTable
	{
		private const float FuelConsumptionPerTickInRain = 0.045f;
		private static Graphic FireGraphic = GraphicDatabase.Get<Graphic_Flicker>("Things/Special/Fire", ShaderDatabase.TransparentPostLight, Vector2.one, Color.white);
		public override void DrawAt(Vector3 drawLoc)
		{
			this.Graphic.Draw(drawLoc, Rot4.North, this);
			Building_Campfire.FireGraphic.Draw(drawLoc, Rot4.North, this);
		}
		public override void Tick()
		{
			base.Tick();
			if (base.Spawned && Find.WeatherManager.RainRate > 0.01f && !Find.RoofGrid.Roofed(base.Position))
			{
				this.TryGetComp<CompRefuelable>().ConsumeFuel(0.045f);
			}
		}
	}
}
