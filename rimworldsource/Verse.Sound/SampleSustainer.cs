using System;
using UnityEngine;
namespace Verse.Sound
{
	public class SampleSustainer : Sample
	{
		public SubSustainer subSustainer;
		public float scheduledEndTime;
		public bool resolvedSkipAttack;
		public override float ParentStartRealTime
		{
			get
			{
				return this.subSustainer.creationRealTime;
			}
		}
		public override float ParentStartTick
		{
			get
			{
				return (float)this.subSustainer.creationTick;
			}
		}
		public override float ParentHashCode
		{
			get
			{
				return (float)this.subSustainer.GetHashCode();
			}
		}
		public override SoundParams ExternalParams
		{
			get
			{
				return this.subSustainer.ExternalParams;
			}
		}
		protected override bool TestPlaying
		{
			get
			{
				return this.subSustainer.Info.testPlay;
			}
		}
		private SampleSustainer(SubSoundDef def) : base(def)
		{
		}
		public static SampleSustainer TryMakeAndPlay(SubSustainer subSus, AudioClip clip, float scheduledEndTime)
		{
			SampleSustainer sampleSustainer = new SampleSustainer(subSus.subDef);
			sampleSustainer.subSustainer = subSus;
			sampleSustainer.scheduledEndTime = scheduledEndTime;
			GameObject gameObject = new GameObject(string.Concat(new object[]
			{
				"SampleSource_",
				sampleSustainer.subDef.name,
				"_",
				sampleSustainer.startRealTime
			}));
			GameObject gameObject2 = (!subSus.subDef.onCamera) ? subSus.parent.worldRootObject : Find.AudioListenerDummy;
			gameObject.transform.parent = gameObject2.transform;
			gameObject.transform.localPosition = Vector3.zero;
			sampleSustainer.source = AudioSourceMaker.NewAudioSourceOn(gameObject);
			if (sampleSustainer.source == null)
			{
				if (gameObject != null)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
				return null;
			}
			sampleSustainer.source.clip = clip;
			sampleSustainer.source.volume = sampleSustainer.resolvedVolume;
			sampleSustainer.source.pitch = sampleSustainer.resolvedPitch;
			sampleSustainer.source.minDistance = sampleSustainer.subDef.distRange.TrueMin;
			sampleSustainer.source.maxDistance = sampleSustainer.subDef.distRange.TrueMax;
			foreach (SoundFilter current in sampleSustainer.subDef.filters)
			{
				current.SetupOn(sampleSustainer.source);
			}
			if (sampleSustainer.subDef.sustainLoop)
			{
				sampleSustainer.source.loop = true;
			}
			sampleSustainer.UpdateSourceVolume();
			sampleSustainer.ApplyMappedParameters();
			sampleSustainer.source.Play();
			sampleSustainer.source.Play();
			return sampleSustainer;
		}
		public void UpdateSourceVolume()
		{
			float num = this.resolvedVolume * this.subSustainer.parent.scopeFader.inScopePercent * base.MappedVolumeMultiplier;
			if (base.AgeRealTime < this.subDef.sustainAttack)
			{
				if (this.resolvedSkipAttack || this.subDef.sustainAttack < 0.01f)
				{
					this.source.volume = num;
				}
				else
				{
					float num2 = base.AgeRealTime / this.subDef.sustainAttack;
					num2 = Mathf.Sqrt(num2);
					this.source.volume = Mathf.Lerp(0f, num, num2);
				}
			}
			else
			{
				if (Time.realtimeSinceStartup > this.scheduledEndTime - this.subDef.sustainRelease)
				{
					float num3 = (Time.realtimeSinceStartup - (this.scheduledEndTime - this.subDef.sustainRelease)) / this.subDef.sustainRelease;
					num3 = 1f - num3;
					num3 = Mathf.Sqrt(num3);
					num3 = 1f - num3;
					this.source.volume = Mathf.Lerp(num, 0f, num3);
				}
				else
				{
					this.source.volume = num;
				}
			}
			if (this.subSustainer.parent.Ended)
			{
				float num4 = 1f - this.subSustainer.parent.TimeSinceEnd / this.subDef.parentDef.sustainFadeoutTime;
				this.source.volume *= num4;
			}
			if (this.source.volume < 0.001f)
			{
				this.source.mute = true;
			}
			else
			{
				this.source.mute = false;
			}
		}
		public override void SampleCleanup()
		{
			base.SampleCleanup();
			UnityEngine.Object.Destroy(this.source.gameObject);
		}
	}
}
