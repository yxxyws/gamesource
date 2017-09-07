using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Designator_AreaAllowedClear : Designator_AreaAllowed
	{
		public Designator_AreaAllowedClear() : base(DesignateMode.Remove)
		{
			this.defaultLabel = "DesignatorClearAreaAllowed".Translate();
			this.defaultDesc = "DesignatorClearAreaAllowedDesc".Translate();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.icon = ContentFinder<Texture2D>.Get("UI/Designators/AreaAllowedClear", true);
			});
			this.soundDragSustain = SoundDefOf.DesignateDragAreaDelete;
			this.soundDragChanged = SoundDefOf.DesignateDragAreaDeleteChanged;
			this.soundSucceeded = SoundDefOf.DesignateAreaDelete;
			this.hotKey = KeyBindingDefOf.Misc9;
		}
		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			return c.InBounds() && Designator_AreaAllowed.SelectedArea != null && Designator_AreaAllowed.SelectedArea[c];
		}
		public override void DesignateSingleCell(IntVec3 c)
		{
			Designator_AreaAllowed.SelectedArea.Clear(c);
		}
	}
}
