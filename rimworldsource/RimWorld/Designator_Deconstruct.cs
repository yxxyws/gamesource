using System;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Designator_Deconstruct : Designator
	{
		public override int DraggableDimensions
		{
			get
			{
				return 2;
			}
		}
		public Designator_Deconstruct()
		{
			this.defaultLabel = "DesignatorDeconstruct".Translate();
			this.defaultDesc = "DesignatorDeconstructDesc".Translate();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.icon = ContentFinder<Texture2D>.Get("UI/Designators/Deconstruct", true);
			});
			this.soundDragSustain = SoundDefOf.DesignateDragStandard;
			this.soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
			this.useMouseIcon = true;
			this.soundSucceeded = SoundDefOf.DesignateDeconstruct;
			this.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
		}
		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			if (!c.InBounds())
			{
				return false;
			}
			if (!Game.GodMode && c.Fogged())
			{
				return false;
			}
			if (this.TopDeconstructibleInCell(c) == null)
			{
				return false;
			}
			return true;
		}
		public override void DesignateSingleCell(IntVec3 loc)
		{
			this.DesignateThing(this.TopDeconstructibleInCell(loc));
		}
		private Thing TopDeconstructibleInCell(IntVec3 loc)
		{
			foreach (Thing current in 
				from t in Find.ThingGrid.ThingsAt(loc)
				orderby t.def.altitudeLayer descending
				select t)
			{
				if (this.CanDesignateThing(current).Accepted)
				{
					return current;
				}
			}
			return null;
		}
		public override void DesignateThing(Thing t)
		{
			if (t.Faction != Faction.OfColony)
			{
				t.SetFaction(Faction.OfColony, null);
			}
			Thing innerIfMinified = t.GetInnerIfMinified();
			if (Game.GodMode || innerIfMinified.GetStatValue(StatDefOf.WorkToMake, true) == 0f || t.def.IsFrame)
			{
				t.Destroy(DestroyMode.Deconstruct);
			}
			else
			{
				Find.DesignationManager.AddDesignation(new Designation(t, DesignationDefOf.Deconstruct));
			}
		}
		public override AcceptanceReport CanDesignateThing(Thing t)
		{
			Building building = t.GetInnerIfMinified() as Building;
			if (building == null)
			{
				return false;
			}
			if (building.def.category != ThingCategory.Building)
			{
				return false;
			}
			if (!Game.GodMode)
			{
				if (!building.def.building.IsDeconstructible)
				{
					return false;
				}
				if (building.Faction != Faction.OfColony)
				{
					if (building.Faction != null)
					{
						return false;
					}
					if (!building.ClaimableBy(Faction.OfColony))
					{
						return false;
					}
				}
			}
			if (Find.DesignationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
			{
				return false;
			}
			if (Find.DesignationManager.DesignationOn(t, DesignationDefOf.Uninstall) != null)
			{
				return false;
			}
			return true;
		}
		public override void SelectedUpdate()
		{
			GenUI.RenderMouseoverBracket();
		}
	}
}
