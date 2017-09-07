using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Designator_Claim : Designator
	{
		public override int DraggableDimensions
		{
			get
			{
				return 2;
			}
		}
		public Designator_Claim()
		{
			this.defaultLabel = "DesignatorClaim".Translate();
			this.defaultDesc = "DesignatorClaimDesc".Translate();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.icon = ContentFinder<Texture2D>.Get("UI/Designators/Claim", true);
			});
			this.soundDragSustain = SoundDefOf.DesignateDragStandard;
			this.soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
			this.useMouseIcon = true;
			this.soundSucceeded = SoundDefOf.DesignateClaim;
			this.hotKey = KeyBindingDefOf.Misc4;
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
			if (!(
				from t in c.GetThingList()
				where this.CanDesignateThing(t).Accepted
				select t).Any<Thing>())
			{
				return "MessageMustDesignateClaimable".Translate();
			}
			return true;
		}
		public override void DesignateSingleCell(IntVec3 c)
		{
			List<Thing> thingList = c.GetThingList();
			for (int i = 0; i < thingList.Count; i++)
			{
				if (this.CanDesignateThing(thingList[i]).Accepted)
				{
					this.DesignateThing(thingList[i]);
				}
			}
		}
		public override AcceptanceReport CanDesignateThing(Thing t)
		{
			Building building = t as Building;
			if (building != null && building.Faction == null && building.ClaimableBy(Faction.OfColony))
			{
				return true;
			}
			return false;
		}
		public override void DesignateThing(Thing t)
		{
			t.SetFaction(Faction.OfColony, null);
			CellRect.CellRectIterator iterator = t.OccupiedRect().GetIterator();
			while (!iterator.Done())
			{
				MoteThrower.ThrowMetaPuffs(iterator.Current);
				iterator.MoveNext();
			}
		}
	}
}
