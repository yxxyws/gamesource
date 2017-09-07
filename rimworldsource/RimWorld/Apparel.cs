using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class Apparel : ThingWithComps
	{
		public Pawn wearer;
		public virtual void DrawWornExtras()
		{
		}
		public virtual bool CheckPreAbsorbDamage(DamageInfo dinfo)
		{
			return false;
		}
		public virtual bool AllowVerbCast(IntVec3 root, TargetInfo targ)
		{
			return true;
		}
		[DebuggerHidden]
		public virtual IEnumerable<Gizmo> GetWornGizmos()
		{
			Apparel.<GetWornGizmos>c__IteratorE8 <GetWornGizmos>c__IteratorE = new Apparel.<GetWornGizmos>c__IteratorE8();
			Apparel.<GetWornGizmos>c__IteratorE8 expr_07 = <GetWornGizmos>c__IteratorE;
			expr_07.$PC = -2;
			return expr_07;
		}
		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			base.Destroy(mode);
			if (base.Destroyed && this.wearer != null)
			{
				this.wearer.apparel.Notify_WornApparelDestroyed(this);
			}
		}
	}
}
