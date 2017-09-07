using System;
namespace Verse
{
	public static class TemperatureTuning
	{
		public const float MinimumTemperature = -270f;
		public const float MaximumTemperature = 2000f;
		public const float DefaultTemperature = 21f;
		public const float DeepUndergroundTemperature = 15f;
		public const float DailyTempVariationAmplitude = 7f;
		public const float DailySunEffect = 14f;
		public const float FoodRefrigerationTemp = 10f;
		public const float FoodFreezingTemp = 0f;
		public const int RoomTempEqualizeInterval = 120;
		public const float AutoIgnitionChanceFactor = 0.25f;
		public static readonly SimpleCurve SeasonalTempVariationCurve = new SimpleCurve
		{
			new CurvePoint(0f, 3f),
			new CurvePoint(0.1f, 4f),
			new CurvePoint(1f, 28f)
		};
		public static readonly FloatRange AutoIgnitionTemperatureRange = new FloatRange(250f, 1200f);
	}
}
