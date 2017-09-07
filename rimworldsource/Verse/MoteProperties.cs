using System;
using UnityEngine;
namespace Verse
{
	public class MoteProperties
	{
		public int fadeinDuration;
		public int ticksBeforeStartFadeout = 50;
		public int fadeoutDuration = 20;
		public float growthRate;
		public bool useCollision;
		public SoundDef landSound;
		public bool realTime;
		public Vector3 attachedDrawOffset;
		public bool needsMaintenance;
		public Color explosionColorStart = Color.white;
		public Color explosionColorEnd = Color.white;
	}
}
