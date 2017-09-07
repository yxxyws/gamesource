using System;
using UnityEngine;
namespace RimWorld
{
	public class Thought_Situational : Thought
	{
		private int curStageIndex = -1;
		private bool shouldDiscard;
		public override int CurStageIndex
		{
			get
			{
				if (this.curStageIndex < 0)
				{
					this.RecalculateState();
				}
				return this.curStageIndex;
			}
		}
		public override bool ShouldDiscard
		{
			get
			{
				if (this.curStageIndex < 0)
				{
					this.RecalculateState();
				}
				return this.shouldDiscard;
			}
		}
		public void RecalculateState()
		{
			ThoughtState thoughtState = this.def.Worker.CurrentState(this.pawn);
			if (thoughtState.Active)
			{
				this.curStageIndex = Mathf.Min(thoughtState.StageIndex, this.def.stages.Count - 1);
			}
			else
			{
				this.shouldDiscard = true;
			}
		}
		public override void ThoughtInterval()
		{
			this.RecalculateState();
		}
		public override bool TryMergeWithExistingThought()
		{
			return this.pawn.needs.mood.thoughts.Thoughts.Find((Thought x) => x.def == this.def) != null;
		}
	}
}
