using System;
using Verse;
namespace RimWorld
{
	public class ThoughtWorker_Man : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			return p.RaceProps.Humanlike && p.story.traits.HasTrait(TraitDefOf.DislikesMen);
		}
		public override float GetOpinionOffset(Pawn p, Pawn other, ThoughtStage stage)
		{
			if (!RelationsUtility.PawnsKnowEachOther(p, other))
			{
				return 0f;
			}
			if (other.def != p.def)
			{
				return 0f;
			}
			if (other.gender == Gender.Male)
			{
				return stage.baseOpinionOffset;
			}
			return 0f;
		}
	}
}
