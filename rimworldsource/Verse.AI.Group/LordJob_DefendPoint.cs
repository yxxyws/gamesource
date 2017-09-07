using System;
namespace Verse.AI.Group
{
	public class LordJob_DefendPoint : LordJob
	{
		private IntVec3 point;
		public LordJob_DefendPoint()
		{
		}
		public LordJob_DefendPoint(IntVec3 point)
		{
			this.point = point;
		}
		public override StateGraph CreateGraph()
		{
			return new StateGraph
			{
				lordToils = 
				{
					new LordToil_DefendPoint(this.point, 28f)
				}
			};
		}
		public override void ExposeData()
		{
			Scribe_Values.LookValue<IntVec3>(ref this.point, "point", default(IntVec3), false);
		}
	}
}
