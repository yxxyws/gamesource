using System;
namespace Verse.AI
{
	public class ThinkNode_ChancePerHour_Constant : ThinkNode_ChancePerHour
	{
		private float chancePerHour = 1f;
		public override ThinkNode DeepCopy()
		{
			ThinkNode_ChancePerHour_Constant thinkNode_ChancePerHour_Constant = (ThinkNode_ChancePerHour_Constant)base.DeepCopy();
			thinkNode_ChancePerHour_Constant.chancePerHour = this.chancePerHour;
			return thinkNode_ChancePerHour_Constant;
		}
		protected override float ChancePerHour(Pawn Pawn)
		{
			return this.chancePerHour;
		}
	}
}
