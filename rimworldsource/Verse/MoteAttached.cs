using System;
namespace Verse
{
	public class MoteAttached : Mote
	{
		protected MoteAttachee att2 = new MoteAttachee();
		public void AttachTo(TargetInfo A, TargetInfo B)
		{
			this.att1.Attachee = A;
			this.att2.Attachee = B;
		}
		public override void Draw()
		{
			this.UpdatePosition();
			base.Draw();
		}
		protected void UpdatePosition()
		{
			if (this.att1.isSet && this.att2.isSet)
			{
				if (!this.att1.target.ThingDestroyed)
				{
					this.att1.UpdateLastDrawPos();
				}
				if (!this.att2.target.ThingDestroyed)
				{
					this.att2.UpdateLastDrawPos();
				}
				this.exactPosition = (this.att1.lastDrawPos + this.att2.lastDrawPos) * 0.5f;
			}
			else
			{
				if (this.att1.isSet)
				{
					if (!this.att1.target.ThingDestroyed)
					{
						this.att1.UpdateLastDrawPos();
					}
					this.exactPosition = this.att1.lastDrawPos + this.def.mote.attachedDrawOffset;
				}
			}
			this.exactPosition.y = Altitudes.AltitudeFor(this.def.altitudeLayer);
		}
	}
}
