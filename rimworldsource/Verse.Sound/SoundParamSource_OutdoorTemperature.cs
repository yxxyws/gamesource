using System;
namespace Verse.Sound
{
	public class SoundParamSource_OutdoorTemperature : SoundParamSource
	{
		public override string Label
		{
			get
			{
				return "Outdoor temperature";
			}
		}
		public override float ValueFor(Sample samp)
		{
			if (Game.Mode != GameMode.MapPlaying)
			{
				return 0f;
			}
			return GenTemperature.OutdoorTemp;
		}
	}
}
