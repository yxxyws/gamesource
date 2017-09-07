using System;
using Verse;
namespace RimWorld
{
	public class ThoughtWorker_Disfigured : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			return true;
		}
		public override float GetOpinionOffset(Pawn p, Pawn other, ThoughtStage stage)
		{
			if (!other.RaceProps.Humanlike || other.Dead)
			{
				return 0f;
			}
			if (!RelationsUtility.PawnsKnowEachOther(p, other))
			{
				return 0f;
			}
			if (RelationsUtility.IsDisfigured(other))
			{
				return stage.baseOpinionOffset;
			}
			return 0f;
		}
	}
}
