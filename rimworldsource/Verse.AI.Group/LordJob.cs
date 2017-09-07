using System;
namespace Verse.AI.Group
{
	public abstract class LordJob : IExposable
	{
		public Lord lord;
		public abstract StateGraph CreateGraph();
		public virtual void ExposeData()
		{
		}
		public virtual void Cleanup()
		{
		}
		public virtual void Notify_PawnAdded(Pawn p)
		{
		}
		public virtual void Notify_PawnLost(Pawn p, PawnLostCondition condition)
		{
		}
	}
}
