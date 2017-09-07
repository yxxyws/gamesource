using System;
using Verse;
namespace RimWorld
{
	public abstract class ThoughtWorker
	{
		public ThoughtDef def;
		public ThoughtState CurrentState(Pawn p)
		{
			ThoughtState result = this.CurrentStateInternal(p);
			if (this.def.invert)
			{
				if (result.Active)
				{
					result = ThoughtState.Inactive;
				}
				else
				{
					result = ThoughtState.ActiveAtStage(0);
				}
			}
			return result;
		}
		protected abstract ThoughtState CurrentStateInternal(Pawn p);
		public virtual string GetLabel(Pawn p, ThoughtStage stage)
		{
			return stage.label.CapitalizeFirst();
		}
		public virtual float GetMoodOffset(Pawn p, ThoughtStage stage)
		{
			return stage.baseMoodEffect;
		}
		public virtual float GetOpinionOffset(Pawn p, Pawn other, ThoughtStage stage)
		{
			return 0f;
		}
	}
}
