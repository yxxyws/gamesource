using System;
using UnityEngine;
using Verse.Sound;
namespace Verse
{
	public class MoteThrown : Mote
	{
		public int airTicksLeft = 9999;
		protected Vector3 velocity;
		public float damping;
		protected bool Flying
		{
			get
			{
				return this.airTicksLeft > 0;
			}
		}
		protected bool Skidding
		{
			get
			{
				return !this.Flying && this.ExactSpeed > 0.01f;
			}
		}
		public Vector3 Velocity
		{
			get
			{
				return this.velocity;
			}
			set
			{
				this.velocity = value;
			}
		}
		public float ExactMoveDirection
		{
			get
			{
				return this.velocity.AngleFlat();
			}
			set
			{
				this.SetVelocityAngleSpeed(value, this.ExactSpeed);
			}
		}
		public float ExactSpeed
		{
			get
			{
				return this.velocity.MagnitudeHorizontal();
			}
			set
			{
				if (value == 0f)
				{
					this.velocity = Vector3.zero;
					return;
				}
				if (this.velocity == Vector3.zero)
				{
					this.velocity = new Vector3(value, 0f, 0f);
					return;
				}
				this.velocity = this.velocity.normalized * value;
			}
		}
		public void SetVelocityAngleSpeed(float angle, float speed)
		{
			this.velocity = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward * speed;
		}
		public override void Tick()
		{
			base.Tick();
			if (!this.def.mote.realTime)
			{
				this.TimeInterval(0.0166666675f);
			}
		}
		public override void RealtimeUpdate()
		{
			if (this.def.mote.realTime)
			{
				this.TimeInterval(Time.deltaTime);
			}
			base.RealtimeUpdate();
		}
		protected virtual void TimeInterval(float deltaTime)
		{
			if (this.Flying || this.Skidding)
			{
				IntVec3 intVec = new IntVec3(this.exactPosition + this.velocity);
				if (intVec != base.Position)
				{
					if (!intVec.InBounds())
					{
						this.Destroy(DestroyMode.Vanish);
						return;
					}
					if (this.def.mote.useCollision && !intVec.Standable())
					{
						this.WallHit();
						return;
					}
				}
				base.Position = intVec;
				this.exactPosition += this.velocity;
				this.exactRotation += this.exactRotationRate;
				if (this.damping != 0f)
				{
					if (this.ExactSpeed < this.damping)
					{
						this.ExactSpeed = 0f;
					}
					else
					{
						this.ExactSpeed -= this.damping;
					}
				}
				this.UpdateAirTicks();
				if (this.Skidding)
				{
					this.UpdateSkidding();
				}
			}
		}
		protected void UpdateAirTicks()
		{
			if (this.airTicksLeft > 0)
			{
				this.airTicksLeft--;
				if (this.airTicksLeft == 0 && !this.def.mote.landSound.NullOrUndefined())
				{
					this.def.mote.landSound.PlayOneShot(base.Position);
				}
			}
		}
		protected void UpdateSkidding()
		{
			this.ExactSpeed *= this.skidSpeedMultiplierPerTick;
			this.exactRotationRate *= this.skidSpeedMultiplierPerTick;
			if (this.ExactSpeed < 0.02f)
			{
				this.ExactSpeed = 0f;
			}
		}
		protected virtual void WallHit()
		{
			this.airTicksLeft = 0;
			this.ExactSpeed = 0f;
			this.exactRotationRate = 0f;
		}
	}
}
