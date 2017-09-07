using System;
namespace Verse.AI
{
	public class ThinkNode_ConditionalFunc : ThinkNode_Priority
	{
		public Func<bool> condition;
		public override ThinkNode DeepCopy()
		{
			ThinkNode_ConditionalFunc thinkNode_ConditionalFunc = (ThinkNode_ConditionalFunc)base.DeepCopy();
			thinkNode_ConditionalFunc.condition = this.condition;
			return thinkNode_ConditionalFunc;
		}
		public override ThinkResult TryIssueJobPackage(Pawn pawn)
		{
			if (!this.condition())
			{
				return ThinkResult.NoJob;
			}
			return base.TryIssueJobPackage(pawn);
		}
	}
}
