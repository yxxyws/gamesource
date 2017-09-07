using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Thought_SocialMemory : Thought_Memory, ISocialThought
	{
		public float opinionOffset;
		public int otherPawnID = -1;
		public override bool ShouldDiscard
		{
			get
			{
				return this.opinionOffset == 0f || base.ShouldDiscard;
			}
		}
		public override bool VisibleInNeedsTab
		{
			get
			{
				return base.VisibleInNeedsTab && this.MoodOffset() != 0f;
			}
		}
		private float AgePct
		{
			get
			{
				return (float)this.age / (float)this.def.DurationTicks;
			}
		}
		private float AgeFactor
		{
			get
			{
				return Mathf.InverseLerp(1f, this.def.lerpOpinionToZeroAfterDurationPct, this.AgePct);
			}
		}
		public void SetOtherPawn(Pawn pawn)
		{
			this.otherPawnID = pawn.thingIDNumber;
		}
		public virtual float OpinionOffset(Pawn pawn)
		{
			if (this.ShouldDiscard)
			{
				return 0f;
			}
			if (this.otherPawnID == pawn.thingIDNumber)
			{
				return this.opinionOffset * this.AgeFactor;
			}
			return 0f;
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<float>(ref this.opinionOffset, "opinionOffset", 0f, false);
			Scribe_Values.LookValue<int>(ref this.otherPawnID, "otherPawnID", 0, false);
		}
		public override void Init()
		{
			base.Init();
			this.opinionOffset = base.CurStage.baseOpinionOffset;
		}
		public override bool TryMergeWithExistingThought()
		{
			return false;
		}
	}
}
