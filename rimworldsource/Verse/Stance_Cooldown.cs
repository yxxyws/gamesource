using System;
using UnityEngine;
namespace Verse
{
	public class Stance_Cooldown : Stance_Busy
	{
		private const float RadiusPerTick = 0.002f;
		private const float MaxRadius = 0.5f;
		public Stance_Cooldown()
		{
		}
		public Stance_Cooldown(int ticks, TargetInfo focusTarg) : base(ticks, focusTarg)
		{
		}
		public override void StanceDraw()
		{
			if (Find.Selector.IsSelected(this.stanceTracker.pawn))
			{
				float radius = Mathf.Min(0.5f, (float)this.ticksLeft * 0.002f);
				GenDraw.DrawCooldownCircle(this.stanceTracker.pawn.Drawer.DrawPos + new Vector3(0f, 0.2f, 0f), radius);
			}
		}
	}
}
