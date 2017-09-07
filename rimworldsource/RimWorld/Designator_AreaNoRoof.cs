using System;
using System.Collections.Generic;
using Verse;
namespace RimWorld
{
	public abstract class Designator_AreaNoRoof : Designator
	{
		private DesignateMode mode;
		private static List<IntVec3> justRemovedCells = new List<IntVec3>();
		private static List<Room> requestedRooms = new List<Room>();
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
		public Designator_AreaNoRoof(DesignateMode mode)
		{
			this.mode = mode;
			this.soundDragSustain = SoundDefOf.DesignateDragStandard;
			this.soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
			this.useMouseIcon = true;
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
			RoofDef roofDef = Find.RoofGrid.RoofAt(c);
			if (roofDef != null && roofDef.isThickRoof)
			{
				return "MessageNothingCanRemoveThickRoofs".Translate();
			}
			bool flag = Find.AreaNoRoof[c];
			if (this.mode == DesignateMode.Add)
			{
				return !flag;
			}
			return flag;
		}
		public override void DesignateSingleCell(IntVec3 c)
		{
			if (this.mode == DesignateMode.Add)
			{
				Find.AreaNoRoof.Set(c);
				RoofDef roofDef = Find.RoofGrid.RoofAt(c);
				if (roofDef != null && !roofDef.isThickRoof)
				{
					Find.RoofGrid.SetRoof(c, null);
				}
			}
			else
			{
				if (this.mode == DesignateMode.Remove)
				{
					Find.AreaNoRoof.Clear(c);
					Designator_AreaNoRoof.justRemovedCells.Add(c);
				}
			}
		}
		protected override void FinalizeDesignationSucceeded()
		{
			base.FinalizeDesignationSucceeded();
			if (this.mode == DesignateMode.Remove)
			{
				for (int i = 0; i < Designator_AreaNoRoof.justRemovedCells.Count; i++)
				{
					IntVec3 intVec = Designator_AreaNoRoof.justRemovedCells[i];
					Room room = intVec.GetRoom();
					if (room == null)
					{
						RoofMaker.TryGenerateRoofOnImpassable(intVec);
					}
					else
					{
						if (!Designator_AreaNoRoof.requestedRooms.Contains(room))
						{
							RoofMaker.TryGenerateRoofFor(room);
							Designator_AreaNoRoof.requestedRooms.Add(room);
						}
					}
				}
				Designator_AreaNoRoof.justRemovedCells.Clear();
				Designator_AreaNoRoof.requestedRooms.Clear();
			}
		}
		public override void SelectedUpdate()
		{
			GenUI.RenderMouseoverBracket();
			Find.AreaNoRoof.MarkForDraw();
		}
	}
}
