using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Designator_SmoothFloor : Designator
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
		public Designator_SmoothFloor()
		{
			this.defaultLabel = "DesignatorSmoothFloor".Translate();
			this.defaultDesc = "DesignatorSmoothFloorDesc".Translate();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.icon = ContentFinder<Texture2D>.Get("UI/Designators/SmoothFloor", true);
			});
			this.useMouseIcon = true;
			this.soundDragSustain = SoundDefOf.DesignateDragStandard;
			this.soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
			this.soundSucceeded = SoundDefOf.DesignateSmoothFloor;
			this.hotKey = KeyBindingDefOf.Misc1;
		}
		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			if (!c.InBounds())
			{
				return false;
			}
			if (c.Fogged())
			{
				return false;
			}
			if (Find.DesignationManager.DesignationAt(c, DesignationDefOf.SmoothFloor) != null)
			{
				return "TerrainBeingSmoothed".Translate();
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
			TerrainDef terrain = c.GetTerrain();
			if (!terrain.affordances.Contains(TerrainAffordance.SmoothableStone))
			{
				return "MessageMustDesignateSmoothableFloor".Translate();
			}
			return AcceptanceReport.WasAccepted;
		}
		public override void DesignateSingleCell(IntVec3 c)
		{
			Find.DesignationManager.AddDesignation(new Designation(c, DesignationDefOf.SmoothFloor));
		}
		public override void SelectedUpdate()
		{
			GenUI.RenderMouseoverBracket();
		}
	}
}
