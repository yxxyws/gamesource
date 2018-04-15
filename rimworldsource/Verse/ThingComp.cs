using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse
{
	public abstract class ThingComp
	{
		public ThingWithComps parent;
		public CompProperties props;
		public virtual void Initialize(CompProperties props)
		{
			this.props = props;
		}
		public virtual void ReceiveCompSignal(string signal)
		{
		}
		public virtual void PostExposeData()
		{
		}
		public virtual void PostSpawnSetup()
		{
		}
		public virtual void PostDeSpawn()
		{
		}
		public virtual void PostDestroy(DestroyMode mode, bool wasSpawned)
		{
		}
		public virtual void CompTick()
		{
		}
		public virtual void CompTickRare()
		{
		}
		public virtual void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
		{
			absorbed = false;
		}
		public virtual void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
		{
		}
		public virtual void PostDraw()
		{
		}
		public virtual void PostDrawExtraSelectionOverlays()
		{
		}
		public virtual void PostPrintOnto(SectionLayer layer)
		{
		}
		public virtual void CompPrintForPowerGrid(SectionLayer layer)
		{
		}
		public virtual void PreAbsorbStack(Thing otherStack, int count)
		{
		}
		public virtual void PostSplitOff(Thing piece)
		{
		}
		[DebuggerHidden]
		public virtual IEnumerable<Command> CompGetGizmosExtra()
		{
			ThingComp.<CompGetGizmosExtra>c__Iterator7D <CompGetGizmosExtra>c__Iterator7D = new ThingComp.<CompGetGizmosExtra>c__Iterator7D();
			ThingComp.<CompGetGizmosExtra>c__Iterator7D expr_07 = <CompGetGizmosExtra>c__Iterator7D;
			expr_07.$PC = -2;
			return expr_07;
		}
		public virtual bool AllowStackWith(Thing other)
		{
			return true;
		}
		public virtual string CompInspectStringExtra()
		{
			return null;
		}
		public virtual string GetDescriptionPart()
		{
			return null;
		}
		[DebuggerHidden]
		public virtual IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			ThingComp.<CompFloatMenuOptions>c__Iterator7E <CompFloatMenuOptions>c__Iterator7E = new ThingComp.<CompFloatMenuOptions>c__Iterator7E();
			ThingComp.<CompFloatMenuOptions>c__Iterator7E expr_07 = <CompFloatMenuOptions>c__Iterator7E;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}
