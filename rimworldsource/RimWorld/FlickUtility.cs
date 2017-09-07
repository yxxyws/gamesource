using System;
using Verse;
namespace RimWorld
{
	public static class FlickUtility
	{
		public static void UpdateFlickDesignation(Thing t)
		{
			bool flag = false;
			ThingWithComps thingWithComps = t as ThingWithComps;
			if (thingWithComps != null)
			{
				for (int i = 0; i < thingWithComps.AllComps.Count; i++)
				{
					CompFlickable compFlickable = thingWithComps.AllComps[i] as CompFlickable;
					if (compFlickable != null && compFlickable.WantsFlick())
					{
						flag = true;
						break;
					}
				}
			}
			Designation designation = Find.DesignationManager.DesignationOn(t, DesignationDefOf.Flick);
			if (flag && designation == null)
			{
				Find.DesignationManager.AddDesignation(new Designation(t, DesignationDefOf.Flick));
			}
			else
			{
				if (!flag && designation != null)
				{
					designation.Delete();
				}
			}
			TutorUtility.DoModalDialogIfNotKnown(ConceptDefOf.SwitchFlickingDesignation);
		}
	}
}
