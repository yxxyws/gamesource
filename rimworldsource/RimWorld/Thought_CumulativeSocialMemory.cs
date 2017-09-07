using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Thought_CumulativeSocialMemory : Thought_SocialMemory
	{
		private const float OpinionOffsetChangePerDay = 1f;
		public override bool ShouldDiscard
		{
			get
			{
				return this.opinionOffset == 0f;
			}
		}
		public override float OpinionOffset(Pawn pawn)
		{
			if (this.ShouldDiscard)
			{
				return 0f;
			}
			if (this.otherPawnID == pawn.thingIDNumber)
			{
				return Mathf.Min(this.opinionOffset, this.def.maxCumulatedOpinionOffset);
			}
			return 0f;
		}
		public override void ThoughtInterval()
		{
			base.ThoughtInterval();
			if (this.age >= 60000)
			{
				if (this.opinionOffset < 0f)
				{
					this.opinionOffset += 1f;
					if (this.opinionOffset > 0f)
					{
						this.opinionOffset = 0f;
					}
				}
				else
				{
					if (this.opinionOffset > 0f)
					{
						this.opinionOffset -= 1f;
						if (this.opinionOffset < 0f)
						{
							this.opinionOffset = 0f;
						}
					}
				}
				this.age = 0;
			}
		}
		public override bool TryMergeWithExistingThought()
		{
			ThoughtHandler thoughts = this.pawn.needs.mood.thoughts;
			List<Thought> thoughts2 = thoughts.Thoughts;
			for (int i = 0; i < thoughts2.Count; i++)
			{
				if (thoughts2[i].def == this.def)
				{
					Thought_CumulativeSocialMemory thought_CumulativeSocialMemory = (Thought_CumulativeSocialMemory)thoughts2[i];
					if (this.otherPawnID == thought_CumulativeSocialMemory.otherPawnID)
					{
						thought_CumulativeSocialMemory.opinionOffset += this.opinionOffset;
						return true;
					}
				}
			}
			return false;
		}
	}
}
