using System;
using UnityEngine;
namespace Verse
{
	public class TextMote : MoteThrown
	{
		public string text;
		public Color textColor = Color.white;
		public int overrideTicksBeforeStartFadeout = -1;
		protected int TicksBeforeStartFadeout
		{
			get
			{
				return (this.overrideTicksBeforeStartFadeout < 0) ? this.def.mote.ticksBeforeStartFadeout : this.overrideTicksBeforeStartFadeout;
			}
		}
		protected override int Lifespan
		{
			get
			{
				return this.TicksBeforeStartFadeout + this.def.mote.fadeoutDuration;
			}
		}
		public override void Draw()
		{
		}
		public override void DrawGUIOverlay()
		{
			float a = 1f - (base.MoteAge - (float)this.TicksBeforeStartFadeout) / (float)this.def.mote.fadeoutDuration;
			Color color = new Color(this.textColor.r, this.textColor.g, this.textColor.b, a);
			GenWorldUI.DrawText(new Vector2(this.exactPosition.x, this.exactPosition.z), this.text, color);
		}
	}
}
