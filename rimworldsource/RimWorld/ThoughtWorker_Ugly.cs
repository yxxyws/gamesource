using System;
using Verse;
namespace RimWorld
{
	public class ThoughtWorker_Ugly : ThoughtWorker
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
			if (other.story.traits.DegreeOfTrait(TraitDefOf.Prettiness) < 0)
			{
				return stage.baseOpinionOffset;
			}
			return 0f;
		}
	}
}
