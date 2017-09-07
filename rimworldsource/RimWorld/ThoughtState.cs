using System;
namespace RimWorld
{
	public struct ThoughtState
	{
		private const int InactiveIndex = -99999;
		private int stageIndex;
		public bool Active
		{
			get
			{
				return this.stageIndex != -99999;
			}
		}
		public int StageIndex
		{
			get
			{
				return this.stageIndex;
			}
		}
		public static ThoughtState ActiveDefault
		{
			get
			{
				return ThoughtState.ActiveAtStage(0);
			}
		}
		public static ThoughtState Inactive
		{
			get
			{
				return new ThoughtState
				{
					stageIndex = -99999
				};
			}
		}
		public static ThoughtState ActiveAtStage(int stageIndex)
		{
			return new ThoughtState
			{
				stageIndex = stageIndex
			};
		}
		public static implicit operator ThoughtState(bool value)
		{
			if (value)
			{
				return ThoughtState.ActiveDefault;
			}
			return ThoughtState.Inactive;
		}
	}
}
