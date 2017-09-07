using System;
using System.Collections.Generic;
using Verse;
namespace RimWorld
{
	public class ThoughtWorker_Incestuous : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			return true;
		}
		public override float GetOpinionOffset(Pawn p, Pawn other, ThoughtStage stage)
		{
			if (!other.RaceProps.Humanlike)
			{
				return 0f;
			}
			if (!RelationsUtility.PawnsKnowEachOther(p, other))
			{
				return 0f;
			}
			float num = 0f;
			List<DirectPawnRelation> directRelations = other.relations.DirectRelations;
			for (int i = 0; i < directRelations.Count; i++)
			{
				if (LovePartnerRelationUtility.IsLovePartnerRelation(directRelations[i].def))
				{
					if (directRelations[i].otherPawn != p)
					{
						if (!directRelations[i].otherPawn.Dead)
						{
							foreach (PawnRelationDef current in other.GetRelations(directRelations[i].otherPawn))
							{
								float incestOpinionOffset = current.incestOpinionOffset;
								if (incestOpinionOffset < num)
								{
									num = incestOpinionOffset;
								}
							}
						}
					}
				}
			}
			return num;
		}
	}
}
