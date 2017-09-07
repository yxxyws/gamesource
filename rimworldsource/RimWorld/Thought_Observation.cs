using System;
using Verse;
namespace RimWorld
{
	public class Thought_Observation : Thought_Memory
	{
		private int targetThingID;
		public Thing Target
		{
			set
			{
				this.targetThingID = value.thingIDNumber;
			}
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<int>(ref this.targetThingID, "targetThingID", 0, false);
		}
		public override bool TryMergeWithExistingThought()
		{
			ThoughtHandler thoughts = this.pawn.needs.mood.thoughts;
			Thought_Observation thought_Observation = null;
			for (int i = 0; i < thoughts.Thoughts.Count; i++)
			{
				Thought_Observation thought_Observation2 = thoughts.Thoughts[i] as Thought_Observation;
				if (thought_Observation2 != null && thought_Observation2.def == this.def && thought_Observation2.targetThingID == this.targetThingID && (thought_Observation == null || thought_Observation2.age > thought_Observation.age))
				{
					thought_Observation = thought_Observation2;
				}
			}
			if (thought_Observation != null)
			{
				thought_Observation.Renew();
				return true;
			}
			return false;
		}
	}
}
