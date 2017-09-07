using System;
using Verse;
namespace RimWorld
{
	public class ThoughtWorker_Drunk : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (!p.RaceProps.Humanlike)
			{
				return false;
			}
			if (p.story.traits.DegreeOfTrait(TraitDefOf.DrugDesire) >= 0)
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
			Hediff firstHediffOfDef = other.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Alcohol);
			if (firstHediffOfDef != null && firstHediffOfDef.Visible)
			{
				return stage.baseOpinionOffset;
			}
			return 0f;
		}
	}
}
