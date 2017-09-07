using System;
namespace Verse.AI
{
	public abstract class ThinkNode_ChancePerHour : ThinkNode_Priority
	{
		public override ThinkResult TryIssueJobPackage(Pawn pawn)
		{
			if (Find.TickManager.TicksGame < this.GetLastTryTick(pawn) + 2500)
			{
				return ThinkResult.NoJob;
			}
			this.SetLastTryTick(pawn, Find.TickManager.TicksGame);
			int salt = Gen.HashCombineInt(base.UniqueSaveKey, 26504059);
			if (pawn.HashRandForHour(salt) < this.ChancePerHour(pawn))
			{
				return base.TryIssueJobPackage(pawn);
			}
			return ThinkResult.NoJob;
		}
		protected abstract float ChancePerHour(Pawn pawn);
		private int GetLastTryTick(Pawn pawn)
		{
			int result;
			if (pawn.mindState.thinkData.TryGetValue(base.UniqueSaveKey, out result))
			{
				return result;
			}
			return -99999;
		}
		private void SetLastTryTick(Pawn pawn, int val)
		{
			pawn.mindState.thinkData[base.UniqueSaveKey] = val;
		}
	}
}
