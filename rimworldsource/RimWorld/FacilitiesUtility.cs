using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public static class FacilitiesUtility
	{
		private static int RegionsToSearch = (1 + 2 * Mathf.CeilToInt(0.6666667f)) * (1 + 2 * Mathf.CeilToInt(0.6666667f));
		public static void NotifyFacilitiesAboutChangedLOSBlockers(List<Region> affectedRegions)
		{
			for (int i = 0; i < affectedRegions.Count; i++)
			{
				RegionTraverser.BreadthFirstTraverse(affectedRegions[i], null, delegate(Region x)
				{
					List<Thing> list = x.ListerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial);
					for (int j = 0; j < list.Count; j++)
					{
						CompFacility compFacility = list[j].TryGetComp<CompFacility>();
						CompAffectedByFacilities compAffectedByFacilities = list[j].TryGetComp<CompAffectedByFacilities>();
						if (compFacility != null)
						{
							compFacility.Notify_LOSBlockerSpawnedOrDespawned();
						}
						if (compAffectedByFacilities != null)
						{
							compAffectedByFacilities.Notify_LOSBlockerSpawnedOrDespawned();
						}
					}
					return false;
				}, FacilitiesUtility.RegionsToSearch);
			}
		}
	}
}
