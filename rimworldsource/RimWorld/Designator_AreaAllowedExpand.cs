using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Designator_AreaAllowedExpand : Designator_AreaAllowed
	{
		public Designator_AreaAllowedExpand() : base(DesignateMode.Add)
		{
			this.defaultLabel = "DesignatorExpandAreaAllowed".Translate();
			this.defaultDesc = "DesignatorExpandAreaAllowedDesc".Translate();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.icon = ContentFinder<Texture2D>.Get("UI/Designators/AreaAllowedExpand", true);
			});
			this.soundDragSustain = SoundDefOf.DesignateDragAreaAdd;
			this.soundDragChanged = SoundDefOf.DesignateDragAreaAddChanged;
			this.soundSucceeded = SoundDefOf.DesignateAreaAdd;
			this.hotKey = KeyBindingDefOf.Misc8;
		}
		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			return c.InBounds() && Designator_AreaAllowed.SelectedArea != null && !Designator_AreaAllowed.SelectedArea[c];
		}
		public override void DesignateSingleCell(IntVec3 c)
		{
			Designator_AreaAllowed.SelectedArea.Set(c);
		}
	}
}
