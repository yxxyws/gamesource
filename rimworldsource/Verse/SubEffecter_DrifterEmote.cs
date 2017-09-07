using System;
using UnityEngine;
namespace Verse
{
	public abstract class SubEffecter_DrifterEmote : SubEffecter
	{
		public SubEffecter_DrifterEmote(SubEffecterDef def) : base(def)
		{
		}
		protected void MakeMote(TargetInfo A)
		{
			Vector3 vector = (!A.HasThing) ? A.Cell.ToVector3Shifted() : A.Thing.DrawPos;
			if (vector.ShouldSpawnMotesAt())
			{
				int randomInRange = this.def.burstCount.RandomInRange;
				for (int i = 0; i < randomInRange; i++)
				{
					Mote mote = (Mote)ThingMaker.MakeThing(this.def.moteDef, null);
					mote.ScaleUniform = this.def.startScale.RandomInRange;
					mote.exactPosition = vector + Gen.RandomHorizontalVector(this.def.positionRadius);
					mote.exactRotationRate = this.def.rotationRate.RandomInRange;
					mote.exactRotation = this.def.rotation.RandomInRange;
					MoteThrown moteThrown = mote as MoteThrown;
					if (moteThrown != null)
					{
						moteThrown.airTicksLeft = this.def.airTicks.RandomInRange;
						moteThrown.SetVelocityAngleSpeed(this.def.moveDirection.RandomInRange, this.def.velocity.RandomInRange);
					}
					if (A.HasThing)
					{
						mote.AttachTo(A);
					}
					GenSpawn.Spawn(mote, vector.ToIntVec3());
				}
			}
		}
	}
}
