using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class IncidentMaker_ShipChunkDrop : IncidentMaker
	{
		private const float BaseShipChunkDropMTBDays = 20f;
		private float ShipChunkDropMTBDays
		{
			get
			{
				float num = (float)Find.TickManager.TicksGame / 3600000f;
				if (num > 10f)
				{
					num = 10f;
				}
				return 20f * Mathf.Pow(2f, num);
			}
		}
		[DebuggerHidden]
		public override IEnumerable<QueuedIncident> NewIncidentSet()
		{
			IncidentMaker_ShipChunkDrop.<NewIncidentSet>c__IteratorAC <NewIncidentSet>c__IteratorAC = new IncidentMaker_ShipChunkDrop.<NewIncidentSet>c__IteratorAC();
			<NewIncidentSet>c__IteratorAC.<>f__this = this;
			IncidentMaker_ShipChunkDrop.<NewIncidentSet>c__IteratorAC expr_0E = <NewIncidentSet>c__IteratorAC;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
