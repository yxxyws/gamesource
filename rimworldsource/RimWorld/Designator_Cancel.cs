using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Designator_Cancel : Designator
	{
		public override int DraggableDimensions
		{
			get
			{
				return 2;
			}
		}
		public Designator_Cancel()
		{
			this.defaultLabel = "DesignatorCancel".Translate();
			this.defaultDesc = "DesignatorCancelDesc".Translate();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.icon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel", true);
			});
			this.useMouseIcon = true;
			this.soundDragSustain = SoundDefOf.DesignateDragStandard;
			this.soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
			this.soundSucceeded = SoundDefOf.DesignateCancel;
			this.hotKey = KeyBindingDefOf.DesignatorCancel;
		}
		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			if (!c.InBounds())
			{
				return false;
			}
			if (Find.DesignationManager.AllDesignationsAt(c).Count<Designation>() > 0)
			{
				return true;
			}
			List<Thing> thingList = c.GetThingList();
			for (int i = 0; i < thingList.Count; i++)
			{
				if (this.CanDesignateThing(thingList[i]).Accepted)
				{
					return true;
				}
			}
			return false;
		}
		public override void DesignateSingleCell(IntVec3 c)
		{
			foreach (Designation current in Find.DesignationManager.AllDesignationsAt(c).ToList<Designation>())
			{
				if (current.def.designateCancelable)
				{
					Find.DesignationManager.RemoveDesignation(current);
				}
			}
			List<Thing> thingList = c.GetThingList();
			for (int i = thingList.Count - 1; i >= 0; i--)
			{
				this.DesignateThing(thingList[i]);
			}
		}
		public override AcceptanceReport CanDesignateThing(Thing t)
		{
			if (Find.DesignationManager.DesignationOn(t) != null)
			{
				foreach (Designation current in Find.DesignationManager.AllDesignationsOn(t))
				{
					if (current.def.designateCancelable)
					{
						return true;
					}
				}
			}
			if (t.def.mineable && Find.DesignationManager.DesignationAt(t.Position, DesignationDefOf.Mine) != null)
			{
				return true;
			}
			return t.Faction == Faction.OfColony && (t is Frame || t is Blueprint);
		}
		public override void DesignateThing(Thing t)
		{
			if (t is Frame || t is Blueprint)
			{
				t.Destroy(DestroyMode.Cancel);
			}
			else
			{
				Find.DesignationManager.RemoveAllDesignationsOn(t, true);
				if (t.def.mineable)
				{
					Designation designation = Find.DesignationManager.DesignationAt(t.Position, DesignationDefOf.Mine);
					if (designation != null)
					{
						Find.DesignationManager.RemoveDesignation(designation);
					}
				}
			}
		}
		public override void SelectedUpdate()
		{
			GenUI.RenderMouseoverBracket();
		}
	}
}
