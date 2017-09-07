using System;
namespace Verse.AI.Group
{
	public class LordToilData_ExitMap : LordToilData
	{
		public LocomotionUrgency locomotion;
		public override void ExposeData()
		{
			Scribe_Values.LookValue<LocomotionUrgency>(ref this.locomotion, "locomotion", LocomotionUrgency.None, false);
		}
	}
}
