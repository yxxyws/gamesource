using System;
using UnityEngine;
namespace Verse
{
	internal class MoteThrownAttached : MoteThrown
	{
		private Vector3 attachedDrawOffset;
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			this.attachedDrawOffset = this.def.mote.attachedDrawOffset;
		}
		public override void Draw()
		{
			if (this.att1.isSet)
			{
				if (!this.att1.target.ThingDestroyed)
				{
					this.att1.UpdateLastDrawPos();
				}
				this.exactPosition = this.att1.lastDrawPos + this.attachedDrawOffset;
			}
			this.exactPosition.y = Altitudes.AltitudeFor(AltitudeLayer.MoteOverhead);
			if (this.exactPosition.InBounds())
			{
				base.Draw();
			}
		}
		protected override void TimeInterval(float deltaTime)
		{
			this.attachedDrawOffset += this.velocity;
			this.exactRotation += this.exactRotationRate;
			base.UpdateAirTicks();
			if (base.Skidding)
			{
				base.UpdateSkidding();
			}
		}
	}
}
