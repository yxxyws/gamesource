using System;
using System.Collections.Generic;
namespace Verse
{
	public class ThingOverlays
	{
		public void ThingOverlaysOnGUI()
		{
			CellRect currentViewRect = Find.CameraMap.CurrentViewRect;
			List<Thing> list = Find.Map.listerThings.ThingsInGroup(ThingRequestGroup.HasGUIOverlay);
			for (int i = 0; i < list.Count; i++)
			{
				Thing thing = list[i];
				if (currentViewRect.Contains(thing.Position))
				{
					if (!Find.FogGrid.IsFogged(thing.Position))
					{
						try
						{
							thing.DrawGUIOverlay();
						}
						catch (Exception ex)
						{
							Log.Error(string.Concat(new object[]
							{
								"Exception drawing ThingOverlay for ",
								thing,
								": ",
								ex
							}));
						}
					}
				}
			}
		}
	}
}
