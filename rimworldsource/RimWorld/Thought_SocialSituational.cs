using System;
using Verse;
namespace RimWorld
{
	public class Thought_SocialSituational : Thought_Situational, ISocialThought
	{
		public override bool VisibleInNeedsTab
		{
			get
			{
				return base.VisibleInNeedsTab && this.MoodOffset() != 0f;
			}
		}
		public float OpinionOffset(Pawn other)
		{
			if (this.ShouldDiscard)
			{
				return 0f;
			}
			return this.def.Worker.GetOpinionOffset(this.pawn, other, base.CurStage);
		}
	}
}
