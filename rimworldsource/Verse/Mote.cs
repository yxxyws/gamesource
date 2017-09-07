using System;
using UnityEngine;
namespace Verse
{
	public abstract class Mote : Thing
	{
		protected const float MinimumVelocity = 0.02f;
		public Vector3 exactPosition;
		public float exactRotation;
		public Vector3 exactScale = new Vector3(1f, 1f, 1f);
		public float exactRotationRate;
		public float skidSpeedMultiplierPerTick = Rand.Range(0.3f, 0.95f);
		public Color color = Color.white;
		protected int spawnTick;
		protected float spawnRealTime;
		private int lastMaintainTick;
		protected MoteAttachee att1 = new MoteAttachee();
		public float ScaleUniform
		{
			set
			{
				this.exactScale = new Vector3(value, 1f, value);
			}
		}
		public float MoteAge
		{
			get
			{
				if (this.def.mote.realTime)
				{
					return (Time.realtimeSinceStartup - this.spawnRealTime) * 60f;
				}
				return (float)(Find.TickManager.TicksGame - this.spawnTick);
			}
		}
		public override Vector3 DrawPos
		{
			get
			{
				return this.exactPosition;
			}
		}
		protected virtual int Lifespan
		{
			get
			{
				return this.def.mote.ticksBeforeStartFadeout + this.def.mote.fadeoutDuration;
			}
		}
		public override void Draw()
		{
			this.exactPosition.y = Altitudes.AltitudeFor(this.def.altitudeLayer);
			base.Draw();
		}
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			this.spawnTick = Find.TickManager.TicksGame;
			this.spawnRealTime = Time.realtimeSinceStartup;
			Find.RealTime.moteList.MoteSpawned(this);
			MoteCounter.Notify_MoteSpawned();
			this.exactPosition.y = Altitudes.AltitudeFor(this.def.altitudeLayer);
		}
		public override void DeSpawn()
		{
			base.DeSpawn();
			Find.RealTime.moteList.MoteDespawned(this);
			MoteCounter.Notify_MoteDespawned();
		}
		public virtual void RealtimeUpdate()
		{
			if (this.MoteAge >= (float)this.Lifespan)
			{
				this.Destroy(DestroyMode.Vanish);
			}
		}
		public override void Tick()
		{
			if (this.def.mote.growthRate > 0f)
			{
				this.exactScale = new Vector3(this.exactScale.x + this.def.mote.growthRate, this.exactScale.y, this.exactScale.z + this.def.mote.growthRate);
			}
			if (this.def.mote.needsMaintenance && this.lastMaintainTick < Find.TickManager.TicksGame - 1)
			{
				this.Destroy(DestroyMode.Vanish);
			}
		}
		public void Maintain()
		{
			this.lastMaintainTick = Find.TickManager.TicksGame;
		}
		public void AttachTo(TargetInfo A)
		{
			this.att1.Attachee = A;
		}
	}
}
