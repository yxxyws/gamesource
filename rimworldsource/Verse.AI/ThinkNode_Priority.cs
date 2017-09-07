using System;
namespace Verse.AI
{
	public class ThinkNode_Priority : ThinkNode
	{
		public override ThinkResult TryIssueJobPackage(Pawn pawn)
		{
			int count = this.subNodes.Count;
			for (int i = 0; i < count; i++)
			{
				ThinkResult result = this.subNodes[i].TryIssueJobPackage(pawn);
				if (result.IsValid)
				{
					return result;
				}
			}
			return ThinkResult.NoJob;
		}
	}
}
