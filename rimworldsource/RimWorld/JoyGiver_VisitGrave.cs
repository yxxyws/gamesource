using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JoyGiver_VisitGrave : JoyGiver
	{
		public override Job TryGiveJob(Pawn pawn)
		{
			bool allowedOutside = JoyUtility.EnjoyableOutsideNow(pawn);
			IEnumerable<Thing> source = Find.ListerThings.ThingsInGroup(ThingRequestGroup.Grave).Where(delegate(Thing x)
			{
				Building_Grave building_Grave = (Building_Grave)x;
				return x.Faction == Faction.OfColony && building_Grave.HasCorpse && !building_Grave.IsForbidden(pawn) && building_Grave.Corpse.innerPawn.Faction == Faction.OfColony && (allowedOutside || building_Grave.Position.Roofed()) && pawn.CanReserveAndReach(x, PathEndMode.Touch, Danger.None, 1);
			});
			Thing t;
			if (!source.TryRandomElementByWeight(delegate(Thing x)
			{
				float lengthHorizontal = (x.Position - pawn.Position).LengthHorizontal;
				return Mathf.Max(150f - lengthHorizontal, 5f);
			}, out t))
			{
				return null;
			}
			return new Job(this.def.jobDef, t);
		}
	}
}
