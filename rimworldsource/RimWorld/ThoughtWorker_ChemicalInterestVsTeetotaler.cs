using System;
using Verse;
namespace RimWorld
{
	public class ThoughtWorker_ChemicalInterestVsTeetotaler : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (!p.RaceProps.Humanlike)
			{
				return false;
			}
			if (p.story.traits.DegreeOfTrait(TraitDefOf.DrugDesire) <= 0)
			{
				return false;
			}
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
			int num = p.story.traits.DegreeOfTrait(TraitDefOf.DrugDesire);
			int num2 = other.story.traits.DegreeOfTrait(TraitDefOf.DrugDesire);
			if (num2 >= 0)
			{
				return 0f;
			}
			if (num == 1)
			{
				return -20f;
			}
			return -30f;
		}
	}
}
