using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class ThoughtWorker_SharedBed : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (!p.RaceProps.Humanlike)
			{
				return false;
			}
			return this.GetMostDislikedNonPartnerBedOwner(p) != null;
		}
		public override float GetMoodOffset(Pawn p, ThoughtStage stage)
		{
			Pawn mostDislikedNonPartnerBedOwner = this.GetMostDislikedNonPartnerBedOwner(p);
			if (mostDislikedNonPartnerBedOwner == null)
			{
				return 0f;
			}
			return Mathf.Min(0.05f * (float)p.relations.OpinionOf(mostDislikedNonPartnerBedOwner) - 5f, 0f);
		}
		private Pawn GetMostDislikedNonPartnerBedOwner(Pawn p)
		{
			Building_Bed ownedBed = p.ownership.OwnedBed;
			if (ownedBed == null)
			{
				return null;
			}
			Pawn pawn = null;
			int num = 0;
			for (int i = 0; i < ownedBed.owners.Count; i++)
			{
				if (ownedBed.owners[i] != p)
				{
					if (!LovePartnerRelationUtility.LovePartnerRelationExists(p, ownedBed.owners[i]))
					{
						int num2 = p.relations.OpinionOf(ownedBed.owners[i]);
						if (pawn == null || num2 < num)
						{
							pawn = ownedBed.owners[i];
							num = num2;
						}
					}
				}
			}
			return pawn;
		}
	}
}
