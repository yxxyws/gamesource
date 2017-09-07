using System;
using UnityEngine;
namespace Verse
{
	public class CameraShaker
	{
		private const float ShakeDecayRate = 0.5f;
		private const float ShakeFrequency = 24f;
		private const float MaxShakeMag = 0.1f;
		private float curShakeMag;
		public Vector3 ShakeOffset
		{
			get
			{
				float x = (float)Math.Sin((double)(Find.RealTime.timeUnpaused * 24f)) * this.curShakeMag;
				float y = (float)Math.Sin((double)(Find.RealTime.timeUnpaused * 24f) * 1.05) * this.curShakeMag;
				float z = (float)Math.Sin((double)(Find.RealTime.timeUnpaused * 24f) * 1.1) * this.curShakeMag;
				return new Vector3(x, y, z);
			}
		}
		public void DoShake(float mag)
		{
			this.curShakeMag += mag;
			if (this.curShakeMag > 0.1f)
			{
				this.curShakeMag = 0.1f;
			}
		}
		public void Update()
		{
			this.curShakeMag -= 0.5f * Time.deltaTime;
			if (this.curShakeMag < 0f)
			{
				this.curShakeMag = 0f;
			}
		}
	}
}
