using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Designator_Open : Designator
	{
		public override int DraggableDimensions
		{
			get
			{
				return 2;
			}
		}
		public Designator_Open()
		{
			this.defaultLabel = "DesignatorOpen".Translate();
			this.defaultDesc = "DesignatorOpenDesc".Translate();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.icon = ContentFinder<Texture2D>.Get("UI/Designators/Open", true);
			});
			this.soundDragSustain = SoundDefOf.DesignateDragStandard;
			this.soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
			this.useMouseIcon = true;
			this.soundSucceeded = SoundDefOf.DesignateClaim;
		}
		protected override void FinalizeDesignationFailed()
		{
			base.FinalizeDesignationFailed();
			Messages.Message("MessageMustDesignateOpenable".Translate(), MessageSound.RejectInput);
		}
		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			if (!c.InBounds())
			{
				return false;
			}
			if (!this.OpenablesInCell(c).Any<Thing>())
			{
				return false;
			}
			return true;
		}
		public override void DesignateSingleCell(IntVec3 c)
		{
			foreach (Thing current in this.OpenablesInCell(c))
			{
				this.DesignateThing(current);
			}
		}
		public override AcceptanceReport CanDesignateThing(Thing t)
		{
			IOpenable openable = t as IOpenable;
			if (openable == null || !openable.CanOpen || Find.DesignationManager.DesignationOn(t, DesignationDefOf.Open) != null)
			{
				return false;
			}
			return true;
		}
		public override void DesignateThing(Thing t)
		{
			Find.DesignationManager.AddDesignation(new Designation(t, DesignationDefOf.Open));
		}
		[DebuggerHidden]
		private IEnumerable<Thing> OpenablesInCell(IntVec3 c)
		{
			Designator_Open.<OpenablesInCell>c__Iterator10D <OpenablesInCell>c__Iterator10D = new Designator_Open.<OpenablesInCell>c__Iterator10D();
			<OpenablesInCell>c__Iterator10D.c = c;
			<OpenablesInCell>c__Iterator10D.<$>c = c;
			<OpenablesInCell>c__Iterator10D.<>f__this = this;
			Designator_Open.<OpenablesInCell>c__Iterator10D expr_1C = <OpenablesInCell>c__Iterator10D;
			expr_1C.$PC = -2;
			return expr_1C;
		}
	}
}
