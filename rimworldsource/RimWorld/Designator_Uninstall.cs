using System;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Designator_Uninstall : Designator
	{
		public override int DraggableDimensions
		{
			get
			{
				return 2;
			}
		}
		public Designator_Uninstall()
		{
			this.defaultLabel = "DesignatorUninstall".Translate();
			this.defaultDesc = "DesignatorUninstallDesc".Translate();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.icon = ContentFinder<Texture2D>.Get("UI/Designators/Uninstall", true);
			});
			this.soundDragSustain = SoundDefOf.DesignateDragStandard;
			this.soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
			this.useMouseIcon = true;
			this.soundSucceeded = SoundDefOf.DesignateDeconstruct;
			this.hotKey = KeyBindingDefOf.Misc12;
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
			if (this.TopUninstallableInCell(c) == null)
			{
				return false;
			}
			return true;
		}
		public override void DesignateSingleCell(IntVec3 loc)
		{
			this.DesignateThing(this.TopUninstallableInCell(loc));
		}
		private Thing TopUninstallableInCell(IntVec3 loc)
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
			if (Game.GodMode || t.GetStatValue(StatDefOf.WorkToMake, true) == 0f || t.def.IsFrame)
			{
				t.Uninstall();
			}
			else
			{
				Find.DesignationManager.AddDesignation(new Designation(t, DesignationDefOf.Uninstall));
			}
		}
		public override AcceptanceReport CanDesignateThing(Thing t)
		{
			Building building = t as Building;
			if (building == null)
			{
				return false;
			}
			if (building.def.category != ThingCategory.Building)
			{
				return false;
			}
			if (!building.def.Minifiable)
			{
				return false;
			}
			if (!Game.GodMode && building.Faction != Faction.OfColony)
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
			if (Find.DesignationManager.DesignationOn(t, DesignationDefOf.Uninstall) != null)
			{
				return false;
			}
			if (Find.DesignationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
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
