using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public abstract class JoyGiver
	{
		public JoyGiverDef def;
		private static List<Thing> tmpCandidates = new List<Thing>();
		protected List<Thing> ThingsOfRequiredThingDefs
		{
			get
			{
				if (this.def.thingDefs == null)
				{
					JoyGiver.tmpCandidates.Clear();
					return JoyGiver.tmpCandidates;
				}
				if (this.def.thingDefs.Count == 1)
				{
					return Find.ListerThings.ThingsOfDef(this.def.thingDefs[0]);
				}
				JoyGiver.tmpCandidates.Clear();
				for (int i = 0; i < this.def.thingDefs.Count; i++)
				{
					JoyGiver.tmpCandidates.AddRange(Find.ListerThings.ThingsOfDef(this.def.thingDefs[i]));
				}
				return JoyGiver.tmpCandidates;
			}
		}
		public virtual float GetChance(Pawn pawn)
		{
			return this.def.baseChance;
		}
		public abstract Job TryGiveJob(Pawn pawn);
		public virtual Job TryGiveJobWhileInBed(Pawn pawn)
		{
			return null;
		}
		public virtual Job TryGiveJobInPartyArea(Pawn pawn, IntVec3 partySpot)
		{
			return null;
		}
		public PawnCapacityDef MissingRequiredCapacity(Pawn pawn)
		{
			for (int i = 0; i < this.def.requiredCapacities.Count; i++)
			{
				if (!pawn.health.capacities.CapableOf(this.def.requiredCapacities[i]))
				{
					return this.def.requiredCapacities[i];
				}
			}
			return null;
		}
	}
}
