using System;
using UnityEngine;
namespace Verse.Sound
{
	public class SoundFilterHighPass : SoundFilter
	{
		[Description("This filter will attenuate frequencies below this cutoff frequency."), EditSliderRange(50f, 20000f)]
		private float cutoffFrequency = 10000f;
		[Description("The resonance Q value."), EditSliderRange(1f, 10f)]
		private float highpassResonaceQ = 1f;
		public override void SetupOn(AudioSource source)
		{
			AudioHighPassFilter orMakeFilterOn = SoundFilter.GetOrMakeFilterOn<AudioHighPassFilter>(source);
			orMakeFilterOn.cutoffFrequency = this.cutoffFrequency;
			orMakeFilterOn.highpassResonaceQ = this.highpassResonaceQ;
		}
	}
}
