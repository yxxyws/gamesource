using System;
using System.Collections.Generic;
using Verse;
namespace RimWorld
{
	public class TraitSet : IExposable
	{
		protected Pawn pawn;
		public List<Trait> allTraits = new List<Trait>();
		public TraitSet(Pawn pawn)
		{
			this.pawn = pawn;
		}
		public void ExposeData()
		{
			Scribe_Collections.LookList<Trait>(ref this.allTraits, "allTraits", LookMode.Deep, new object[0]);
		}
		public void GainTrait(Trait trait)
		{
			if (this.HasTrait(trait.def))
			{
				Log.Warning(this.pawn + " already has trait " + trait.def);
				return;
			}
			this.allTraits.Add(trait);
			if (this.pawn.workSettings != null)
			{
				this.pawn.workSettings.Notify_GainedTrait();
			}
		}
		public bool HasTrait(TraitDef tDef)
		{
			for (int i = 0; i < this.allTraits.Count; i++)
			{
				if (this.allTraits[i].def == tDef)
				{
					return true;
				}
			}
			return false;
		}
		public int DegreeOfTrait(TraitDef tDef)
		{
			if (tDef.degreeDatas.Count == 1)
			{
				Log.ErrorOnce("DegreeOfTrait on non-spectrum trait " + tDef, 744323);
			}
			for (int i = 0; i < this.allTraits.Count; i++)
			{
				if (this.allTraits[i].def == tDef)
				{
					return this.allTraits[i].degree;
				}
			}
			return 0;
		}
	}
}
