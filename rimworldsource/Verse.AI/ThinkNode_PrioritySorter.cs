using System;
using System.Collections.Generic;
namespace Verse.AI
{
	public class ThinkNode_PrioritySorter : ThinkNode
	{
		public float minPriority;
		private static List<ThinkNode> workingNodes = new List<ThinkNode>();
		public override ThinkNode DeepCopy()
		{
			ThinkNode_PrioritySorter thinkNode_PrioritySorter = (ThinkNode_PrioritySorter)base.DeepCopy();
			thinkNode_PrioritySorter.minPriority = this.minPriority;
			return thinkNode_PrioritySorter;
		}
		public override ThinkResult TryIssueJobPackage(Pawn pawn)
		{
			ThinkNode_PrioritySorter.workingNodes.Clear();
			int count = this.subNodes.Count;
			for (int i = 0; i < count; i++)
			{
				ThinkNode_PrioritySorter.workingNodes.Insert(Rand.Range(0, ThinkNode_PrioritySorter.workingNodes.Count - 1), this.subNodes[i]);
			}
			while (ThinkNode_PrioritySorter.workingNodes.Count > 0)
			{
				float num = 0f;
				int num2 = -1;
				for (int j = 0; j < ThinkNode_PrioritySorter.workingNodes.Count; j++)
				{
					float priority = ThinkNode_PrioritySorter.workingNodes[j].GetPriority(pawn);
					if (priority > 0f && priority >= this.minPriority)
					{
						if (priority > num)
						{
							num = priority;
							num2 = j;
						}
					}
				}
				if (num2 == -1)
				{
					break;
				}
				ThinkResult result = ThinkNode_PrioritySorter.workingNodes[num2].TryIssueJobPackage(pawn);
				if (result.IsValid)
				{
					return result;
				}
				ThinkNode_PrioritySorter.workingNodes.RemoveAt(num2);
			}
			return ThinkResult.NoJob;
		}
	}
}
