using System;
namespace Verse
{
	public class SubEffecterDef
	{
		public Type subEffecterClass;
		public ThingDef moteDef;
		public FloatRange startScale = new FloatRange(1f, 1f);
		public MoteSpawnLocType spawnLocType = MoteSpawnLocType.BetweenPositions;
		public float positionLerpFactor = 0.5f;
		public float positionRadius;
		public IntRange burstCount = new IntRange(1, 1);
		public IntRange airTicks = new IntRange(0, 0);
		public FloatRange rotationRate = new FloatRange(0f, 0f);
		public FloatRange velocity = new FloatRange(0f, 0f);
		public FloatRange rotation = new FloatRange(0f, 360f);
		public FloatRange moveDirection = new FloatRange(0f, 360f);
		public FloatRange damping = new FloatRange(0f, 0f);
		public float chancePerTick = 0.1f;
		public int ticksBetweenMotes = 40;
		public SoundDef soundDef;
		public int intermittentSoundInterval = 50;
		public int ticksBeforeSustainerStart;
		public SubEffecter Spawn()
		{
			return (SubEffecter)Activator.CreateInstance(this.subEffecterClass, new object[]
			{
				this
			});
		}
	}
}
