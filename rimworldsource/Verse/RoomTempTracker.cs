using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Verse
{
	public sealed class RoomTempTracker
	{
		private Room room;
		private float temperatureInt;
		private List<IntVec3> equalizeCells = new List<IntVec3>();
		private float thickRoofCoverage;
		private int cycleIndex;
		private static int debugGetFrame = -999;
		private static float debugWallEq;
		public float Temperature
		{
			get
			{
				return this.temperatureInt;
			}
			set
			{
				this.temperatureInt = Mathf.Clamp(value, -270f, 2000f);
			}
		}
		public RoomTempTracker(Room room)
		{
			this.room = room;
			this.Temperature = GenTemperature.OutdoorTemp;
		}
		public void RoomChanged()
		{
			this.thickRoofCoverage = 0f;
			this.equalizeCells.Clear();
			RoofMaker.ResolveQueuedGenerateRoofs();
			if (!this.room.UsesOutdoorTemperature)
			{
				int num = 0;
				int num2 = 0;
				foreach (IntVec3 current in this.room.Cells)
				{
					RoofDef roof = current.GetRoof();
					if (roof != null && roof.isThickRoof)
					{
						num2++;
					}
					num++;
				}
				this.thickRoofCoverage = (float)num2 / (float)num;
				foreach (IntVec3 current2 in this.room.Cells)
				{
					for (int i = 0; i < 4; i++)
					{
						IntVec3 loc = current2 + GenAdj.CardinalDirections[i];
						IntVec3 intVec = current2 + GenAdj.CardinalDirections[i] * 2;
						if (loc.GetRoom() == null)
						{
							Room room = intVec.GetRoom();
							if (room != this.room)
							{
								bool flag = false;
								for (int j = 0; j < 4; j++)
								{
									IntVec3 loc2 = intVec + GenAdj.CardinalDirections[j];
									if (loc2.GetRoom() == this.room)
									{
										flag = true;
										break;
									}
								}
								if (!flag)
								{
									this.equalizeCells.Add(intVec);
								}
							}
						}
					}
				}
				this.equalizeCells.Shuffle<IntVec3>();
			}
		}
		public void EqualizeTemperature()
		{
			if (this.room.UsesOutdoorTemperature)
			{
				this.Temperature = GenTemperature.OutdoorTemp;
			}
			else
			{
				if (this.room.Regions.Count == 1 && this.room.Regions[0].portal != null)
				{
					return;
				}
				float num = this.RoofEqualizationTempChangePerInterval();
				float num2 = this.WallEqualizationTempChangePerInterval();
				float num3 = this.DeepEqualizationTempChangePerInterval();
				this.Temperature += num + num2 + num3;
			}
		}
		private float WallEqualizationTempChangePerInterval()
		{
			if (this.equalizeCells.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			int num2 = Mathf.CeilToInt((float)this.equalizeCells.Count * 0.2f);
			for (int i = 0; i < num2; i++)
			{
				this.cycleIndex++;
				int index = this.cycleIndex % this.equalizeCells.Count;
				float num3;
				if (GenTemperature.TryGetDirectAirTemperatureForCell(this.equalizeCells[index], out num3))
				{
					num += num3 - this.Temperature;
				}
				else
				{
					num += Mathf.Lerp(this.Temperature, GenTemperature.OutdoorTemp, 0.5f) - this.Temperature;
				}
			}
			float num4 = num / (float)num2;
			float num5 = num4 * (float)this.equalizeCells.Count;
			return num5 * 120f * 0.00017f / (float)this.room.CellCount;
		}
		private float RoofEqualizationTempChangePerInterval()
		{
			if (this.thickRoofCoverage > 0.99f)
			{
				return 0f;
			}
			float num = GenTemperature.OutdoorTemp - this.temperatureInt;
			float num2;
			if (Mathf.Abs(num) < 100f)
			{
				num2 = num;
			}
			else
			{
				num2 = Mathf.Sign(num) * 100f + 5f * (num - Mathf.Sign(num) * 100f);
			}
			num2 *= (1f - this.thickRoofCoverage) * 5E-05f;
			if ((num < 0f && num2 < num) || (num > 0f && num2 > num))
			{
				num2 = num;
			}
			return num2 * 120f;
		}
		private float DeepEqualizationTempChangePerInterval()
		{
			if (this.thickRoofCoverage <= 0.01f)
			{
				return 0f;
			}
			float num = 15f - this.temperatureInt;
			if (num > 0f)
			{
				return 0f;
			}
			float num2 = num * this.thickRoofCoverage * 5E-05f;
			if ((num < 0f && num2 < num) || (num > 0f && num2 > num))
			{
				num2 = num;
			}
			return num2 * 120f;
		}
		public void DebugDraw()
		{
			foreach (IntVec3 current in this.equalizeCells)
			{
				DebugRender.RenderCell(current, 0.5f);
			}
		}
		internal string DebugString()
		{
			if (Time.frameCount > RoomTempTracker.debugGetFrame + 60)
			{
				RoomTempTracker.debugWallEq = 0f;
				for (int i = 0; i < 40; i++)
				{
					RoomTempTracker.debugWallEq += this.WallEqualizationTempChangePerInterval();
				}
				RoomTempTracker.debugWallEq /= 40f;
				RoomTempTracker.debugGetFrame = Time.frameCount;
			}
			return string.Concat(new object[]
			{
				"TemperatureTuning.RoomTempEqualizeInterval: ",
				120,
				"\n  equalizeCells:",
				this.equalizeCells.Count,
				"\n  thickRoofCoverage: ",
				this.thickRoofCoverage.ToStringPercent("F0"),
				"\n  wall equalization: ",
				(RoomTempTracker.debugWallEq * 60f).ToStringTemperature("F3"),
				"\n  roof equalization: ",
				(this.RoofEqualizationTempChangePerInterval() * 60f).ToStringTemperature("F3"),
				"\n  deep equalization: ",
				(this.DeepEqualizationTempChangePerInterval() * 60f).ToStringTemperature("F3"),
				"\n  tempChange e=20 targ= 200C: ",
				GenTemperature.ControlTemperatureTempChange(this.room.Cells.First<IntVec3>(), 20f, 200f),
				"\n  tempChange e=20 targ=-200C: ",
				GenTemperature.ControlTemperatureTempChange(this.room.Cells.First<IntVec3>(), 20f, -200f)
			});
		}
	}
}
