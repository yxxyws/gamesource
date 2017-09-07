using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Building_Vent : Building_TempControl
	{
		private const float EqualizationPercentPerTickRare = 0.25f;
		public override void TickRare()
		{
			IntVec3 loc = base.Position + IntVec3.South.RotatedBy(base.Rotation);
			IntVec3 loc2 = base.Position + IntVec3.North.RotatedBy(base.Rotation);
			Room room = loc.GetRoom();
			if (room == null)
			{
				return;
			}
			Room room2 = loc2.GetRoom();
			if (room2 == null)
			{
				return;
			}
			if (room == room2)
			{
				return;
			}
			if (room.UsesOutdoorTemperature && room2.UsesOutdoorTemperature)
			{
				return;
			}
			float targetTemp;
			if (room.UsesOutdoorTemperature)
			{
				targetTemp = room.Temperature;
			}
			else
			{
				if (room2.UsesOutdoorTemperature)
				{
					targetTemp = room2.Temperature;
				}
				else
				{
					targetTemp = (room.Temperature * (float)room.CellCount + room2.Temperature * (float)room2.CellCount) / (float)(room.CellCount + room2.CellCount);
				}
			}
			if (!room.UsesOutdoorTemperature)
			{
				this.Equalize(room, targetTemp);
			}
			if (!room2.UsesOutdoorTemperature)
			{
				this.Equalize(room2, targetTemp);
			}
		}
		private void Equalize(Room r, float targetTemp)
		{
			float num = Mathf.Abs(r.Temperature - targetTemp);
			float num2 = num * 0.25f;
			if (targetTemp < r.Temperature)
			{
				r.Temperature = Mathf.Max(targetTemp, r.Temperature - num2);
			}
			else
			{
				if (targetTemp > r.Temperature)
				{
					r.Temperature = Mathf.Min(targetTemp, r.Temperature + num2);
				}
			}
		}
	}
}
