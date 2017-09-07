using System;
using Verse;
namespace RimWorld
{
	public abstract class Designator_Plan : Designator
	{
		private DesignateMode mode;
		private DesignationDef desDef = DefDatabase<DesignationDef>.GetNamed("Plan", true);
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
		public Designator_Plan(DesignateMode mode)
		{
			this.mode = mode;
			this.soundDragSustain = SoundDefOf.DesignateDragStandard;
			this.soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
			this.useMouseIcon = true;
			this.hotKey = KeyBindingDefOf.Misc9;
			this.desDef = DefDatabase<DesignationDef>.GetNamed("Plan", true);
		}
		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			if (!c.InBounds())
			{
				return false;
			}
			if (c.InNoBuildEdgeArea())
			{
				return "TooCloseToMapEdge".Translate();
			}
			if (this.mode == DesignateMode.Add)
			{
				if (Find.DesignationManager.DesignationAt(c, this.desDef) != null)
				{
					return false;
				}
			}
			else
			{
				if (this.mode == DesignateMode.Remove && Find.DesignationManager.DesignationAt(c, this.desDef) == null)
				{
					return false;
				}
			}
			return true;
		}
		public override void DesignateSingleCell(IntVec3 c)
		{
			if (this.mode == DesignateMode.Add)
			{
				Find.DesignationManager.AddDesignation(new Designation(c, this.desDef));
			}
			else
			{
				if (this.mode == DesignateMode.Remove)
				{
					Find.DesignationManager.DesignationAt(c, this.desDef).Delete();
				}
			}
		}
		public override void SelectedUpdate()
		{
			GenUI.RenderMouseoverBracket();
			GenDraw.DrawNoBuildEdgeLines();
		}
	}
}
