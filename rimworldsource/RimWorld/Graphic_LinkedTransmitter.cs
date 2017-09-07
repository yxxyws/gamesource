using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Graphic_LinkedTransmitter : Graphic_Linked
	{
		public Graphic_LinkedTransmitter(Graphic subGraphic) : base(subGraphic)
		{
		}
		public override bool ShouldLinkWith(IntVec3 c, Thing parent)
		{
			return c.InBounds() && (base.ShouldLinkWith(c, parent) || PowerNetGrid.TransmittedPowerNetAt(c) != null);
		}
		public override void Print(SectionLayer layer, Thing thing)
		{
			base.Print(layer, thing);
			for (int i = 0; i < 4; i++)
			{
				IntVec3 intVec = thing.Position + GenAdj.CardinalDirections[i];
				if (intVec.InBounds())
				{
					Building transmitter = intVec.GetTransmitter();
					if (transmitter != null && !transmitter.def.graphicData.Linked)
					{
						Material mat = base.LinkedDrawMatFrom(thing, intVec);
						Printer_Plane.PrintPlane(layer, intVec.ToVector3ShiftedWithAltitude(thing.def.Altitude), Vector2.one, mat, 0f, false, null, null, 0.01f);
					}
				}
			}
		}
	}
}