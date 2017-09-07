using System;
using Verse;
namespace RimWorld
{
	public class Thought_Memory : Thought
	{
		public float moodPowerFactor = 1f;
		public int age;
		private int forcedStage;
		public override int CurStageIndex
		{
			get
			{
				return this.forcedStage;
			}
		}
		public override bool ShouldDiscard
		{
			get
			{
				return this.age > this.def.DurationTicks;
			}
		}
		public void SetForcedStage(int stageIndex)
		{
			this.forcedStage = stageIndex;
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<float>(ref this.moodPowerFactor, "moodPowerFactor", 1f, false);
			Scribe_Values.LookValue<int>(ref this.age, "age", 0, false);
			Scribe_Values.LookValue<int>(ref this.forcedStage, "stageIndex", 0, false);
		}
		public override void ThoughtInterval()
		{
			this.age += 150;
		}
		public void Renew()
		{
			this.age = 0;
		}
		public override float MoodOffset()
		{
			float num = base.MoodOffset();
			return num * this.moodPowerFactor;
		}
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"(",
				this.def.defName,
				", moodPowerFactor=",
				this.moodPowerFactor,
				", age=",
				this.age,
				")"
			});
		}
	}
}
