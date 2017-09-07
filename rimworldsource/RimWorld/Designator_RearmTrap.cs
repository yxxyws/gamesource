using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Designator_RearmTrap : Designator
	{
		public override int DraggableDimensions
		{
			get
			{
				return 2;
			}
		}
		public Designator_RearmTrap()
		{
			this.defaultLabel = "DesignatorRearmTrap".Translate();
			this.defaultDesc = "DesignatorRearmTrapDesc".Translate();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.icon = ContentFinder<Texture2D>.Get("UI/Designators/RearmTrap", true);
			});
			this.soundDragSustain = SoundDefOf.DesignateDragStandard;
			this.soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
			this.useMouseIcon = true;
			this.soundSucceeded = SoundDefOf.DesignateClaim;
			this.hotKey = KeyBindingDefOf.Misc7;
		}
		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			if (!c.InBounds())
			{
				return false;
			}
			if (!this.RearmablesInCell(c).Any<Thing>())
			{
				return false;
			}
			return true;
		}
		public override void DesignateSingleCell(IntVec3 c)
		{
			foreach (Thing current in this.RearmablesInCell(c))
			{
				this.DesignateThing(current);
			}
		}
		public override AcceptanceReport CanDesignateThing(Thing t)
		{
			Building_TrapRearmable building_TrapRearmable = t as Building_TrapRearmable;
			return building_TrapRearmable != null && !building_TrapRearmable.Armed && Find.DesignationManager.DesignationOn(building_TrapRearmable, DesignationDefOf.RearmTrap) == null;
		}
		public override void DesignateThing(Thing t)
		{
			Find.DesignationManager.AddDesignation(new Designation(t, DesignationDefOf.RearmTrap));
		}
		[DebuggerHidden]
		private IEnumerable<Thing> RearmablesInCell(IntVec3 c)
		{
			Designator_RearmTrap.<RearmablesInCell>c__Iterator10E <RearmablesInCell>c__Iterator10E = new Designator_RearmTrap.<RearmablesInCell>c__Iterator10E();
			<RearmablesInCell>c__Iterator10E.c = c;
			<RearmablesInCell>c__Iterator10E.<$>c = c;
			<RearmablesInCell>c__Iterator10E.<>f__this = this;
			Designator_RearmTrap.<RearmablesInCell>c__Iterator10E expr_1C = <RearmablesInCell>c__Iterator10E;
			expr_1C.$PC = -2;
			return expr_1C;
		}
	}
}
