using System;
using System.Collections.Generic;
using Verse;
namespace RimWorld
{
	public class ThoughtWorker_Affair : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			Pawn spouse = p.GetSpouse();
			if (spouse == null)
			{
				return false;
			}
			List<DirectPawnRelation> directRelations = spouse.relations.DirectRelations;
			for (int i = 0; i < directRelations.Count; i++)
			{
				if (directRelations[i].otherPawn != p)
				{
					if (!directRelations[i].otherPawn.Dead)
					{
						if (directRelations[i].def == PawnRelationDefOf.Lover || directRelations[i].def == PawnRelationDefOf.Fiance)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		public override float GetOpinionOffset(Pawn p, Pawn other, ThoughtStage stage)
		{
			if (p.GetSpouse() == other)
			{
				return stage.baseOpinionOffset;
			}
			return 0f;
		}
	}
}
