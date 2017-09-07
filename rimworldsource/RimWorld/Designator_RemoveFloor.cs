using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Designator_RemoveFloor : Designator
	{
		public override int DraggableDimensions
		{
			get
			{
				return 2;
			}
		}
		public override bool DragDrawMeasurements
		{
			get
			{
				return true;
			}
		}
		public Designator_RemoveFloor()
		{
			this.defaultLabel = "DesignatorRemoveFloor".Translate();
			this.defaultDesc = "DesignatorRemoveFloorDesc".Translate();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.icon = ContentFinder<Texture2D>.Get("UI/Designators/RemoveFloor", true);
			});
			this.useMouseIcon = true;
			this.soundDragSustain = SoundDefOf.DesignateDragStandard;
			this.soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
			this.soundSucceeded = SoundDefOf.DesignateSmoothFloor;
			this.hotKey = KeyBindingDefOf.Misc1;
		}
		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			if (!c.InBounds() || c.Fogged())
			{
				return false;
			}
			if (Find.DesignationManager.DesignationAt(c, DesignationDefOf.RemoveFloor) != null)
			{
				return false;
			}
			if (c.InNoBuildEdgeArea())
			{
				return "TooCloseToMapEdge".Translate();
			}
			Building edifice = c.GetEdifice();
			if (edifice != null && edifice.def.Fillage == FillCategory.Full && edifice.def.passability == Traversability.Impassable)
			{
				return false;
			}
			if (!Find.TerrainGrid.TerrainAt(c).layerable)
			{
				return "TerrainMustBeRemovable".Translate();
			}
			return AcceptanceReport.WasAccepted;
		}
		public override void DesignateSingleCell(IntVec3 c)
		{
			if (Game.GodMode)
			{
				Find.TerrainGrid.RemoveTopLayer(c);
				return;
			}
			Find.DesignationManager.AddDesignation(new Designation(c, DesignationDefOf.RemoveFloor));
		}
		public override void SelectedUpdate()
		{
			GenUI.RenderMouseoverBracket();
		}
	}
}
