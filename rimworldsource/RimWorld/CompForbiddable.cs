using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class CompForbiddable : ThingComp
	{
		private bool forbiddenInt;
		public bool Forbidden
		{
			get
			{
				return this.forbiddenInt;
			}
			set
			{
				if (value == this.forbiddenInt)
				{
					return;
				}
				this.forbiddenInt = value;
				if (this.forbiddenInt)
				{
					ListerHaulables.Notify_Forbidden(this.parent);
				}
				else
				{
					ListerHaulables.Notify_Unforbidden(this.parent);
				}
				if (this.parent is Building_Door)
				{
					Reachability.ClearCache();
				}
			}
		}
		public override void PostExposeData()
		{
			Scribe_Values.LookValue<bool>(ref this.forbiddenInt, "forbidden", false, false);
		}
		public override void PostDraw()
		{
			if (this.forbiddenInt)
			{
				if (this.parent is Blueprint || this.parent is Frame)
				{
					if (this.parent.def.size.x > 1 || this.parent.def.size.z > 1)
					{
						OverlayDrawer.DrawOverlay(this.parent, OverlayTypes.ForbiddenBig);
					}
					else
					{
						OverlayDrawer.DrawOverlay(this.parent, OverlayTypes.Forbidden);
					}
				}
				else
				{
					if (this.parent.def.category == ThingCategory.Building)
					{
						OverlayDrawer.DrawOverlay(this.parent, OverlayTypes.ForbiddenBig);
					}
					else
					{
						OverlayDrawer.DrawOverlay(this.parent, OverlayTypes.Forbidden);
					}
				}
			}
		}
		public override void PostSplitOff(Thing piece)
		{
			piece.SetForbidden(this.forbiddenInt, true);
		}
		[DebuggerHidden]
		public override IEnumerable<Command> CompGetGizmosExtra()
		{
			CompForbiddable.<CompGetGizmosExtra>c__IteratorEE <CompGetGizmosExtra>c__IteratorEE = new CompForbiddable.<CompGetGizmosExtra>c__IteratorEE();
			<CompGetGizmosExtra>c__IteratorEE.<>f__this = this;
			CompForbiddable.<CompGetGizmosExtra>c__IteratorEE expr_0E = <CompGetGizmosExtra>c__IteratorEE;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
