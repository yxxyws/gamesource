using System;
namespace Verse
{
	public class HediffComp
	{
		public HediffWithComps parent;
		public HediffCompProperties props;
		public Pawn Pawn
		{
			get
			{
				return this.parent.pawn;
			}
		}
		public HediffDef Def
		{
			get
			{
				return this.parent.def;
			}
		}
		public virtual string CompLabelInBracketsExtra
		{
			get
			{
				return null;
			}
		}
		public virtual string CompTipStringExtra
		{
			get
			{
				return null;
			}
		}
		public virtual bool CompShouldRemove
		{
			get
			{
				return false;
			}
		}
		public virtual string CompDebugString
		{
			get
			{
				return null;
			}
		}
		public virtual void CompPostTick()
		{
		}
		public virtual void CompExposeData()
		{
		}
		public virtual bool CompDisallowVisible()
		{
			return false;
		}
		public virtual void CompPostPostAdd()
		{
		}
		public virtual void CompPostDirectHeal(float amount)
		{
		}
		public virtual void CompTended(float quality, int batchPosition = 0)
		{
		}
	}
}
