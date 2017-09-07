using System;
namespace Verse.AI.Group
{
	public class LordJob_ExitMapNearest : LordJob
	{
		private LocomotionUrgency locomotion = LocomotionUrgency.Jog;
		public LordJob_ExitMapNearest()
		{
		}
		public LordJob_ExitMapNearest(LocomotionUrgency locomotion)
		{
			this.locomotion = locomotion;
		}
		public override StateGraph CreateGraph()
		{
			return new StateGraph
			{
				lordToils = 
				{
					new LordToil_ExitMapNearest(this.locomotion)
				}
			};
		}
		public override void ExposeData()
		{
			Scribe_Values.LookValue<LocomotionUrgency>(ref this.locomotion, "locomotion", LocomotionUrgency.Jog, false);
		}
	}
}
