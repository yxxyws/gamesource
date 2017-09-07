using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class ITab_Pawn_Gear : ITab
	{
		private const float TopPadding = 20f;
		private const float ThingIconSize = 28f;
		private const float ThingRowHeight = 28f;
		private const float ThingLeftX = 36f;
		private Vector2 scrollPosition = Vector2.zero;
		private float scrollViewHeight;
		private static readonly Color ThingLabelColor = new Color(0.9f, 0.9f, 0.9f, 1f);
		private static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);
		public override bool IsVisible
		{
			get
			{
				return this.SelPawnForGear.RaceProps.ToolUser || this.SelPawnForGear.inventory.container.Any<Thing>();
			}
		}
		private bool CanEdit
		{
			get
			{
				return this.SelPawnForGear.IsColonistPlayerControlled;
			}
		}
		private Pawn SelPawnForGear
		{
			get
			{
				if (base.SelPawn != null)
				{
					return base.SelPawn;
				}
				Corpse corpse = base.SelThing as Corpse;
				if (corpse != null)
				{
					return corpse.innerPawn;
				}
				throw new InvalidOperationException("Gear tab on non-pawn non-corpse " + base.SelThing);
			}
		}
		public ITab_Pawn_Gear()
		{
			this.size = new Vector2(440f, 450f);
			this.labelKey = "TabGear";
		}
		protected override void FillTab()
		{
			Text.Font = GameFont.Small;
			Rect rect = new Rect(0f, 20f, this.size.x, this.size.y - 20f);
			Rect rect2 = rect.ContractedBy(10f);
			Rect position = new Rect(rect2.x, rect2.y, rect2.width, rect2.height);
			GUI.BeginGroup(position);
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			Rect outRect = new Rect(0f, 0f, position.width, position.height);
			Rect viewRect = new Rect(0f, 0f, position.width - 16f, this.scrollViewHeight);
			Widgets.BeginScrollView(outRect, ref this.scrollPosition, viewRect);
			float num = 0f;
			if (this.SelPawnForGear.equipment != null)
			{
				Widgets.ListSeparator(ref num, viewRect.width, "Equipment".Translate());
				foreach (ThingWithComps current in this.SelPawnForGear.equipment.AllEquipment)
				{
					this.DrawThingRow(ref num, viewRect.width, current);
				}
			}
			if (this.SelPawnForGear.apparel != null)
			{
				Widgets.ListSeparator(ref num, viewRect.width, "Apparel".Translate());
				foreach (Apparel current2 in 
					from ap in this.SelPawnForGear.apparel.WornApparel
					orderby ap.def.apparel.bodyPartGroups[0].listOrder descending
					select ap)
				{
					this.DrawThingRow(ref num, viewRect.width, current2);
				}
			}
			if (this.SelPawnForGear.inventory != null)
			{
				Widgets.ListSeparator(ref num, viewRect.width, "Inventory".Translate());
				foreach (Thing current3 in this.SelPawnForGear.inventory.container)
				{
					this.DrawThingRow(ref num, viewRect.width, current3);
				}
			}
			if (Event.current.type == EventType.Layout)
			{
				this.scrollViewHeight = num + 30f;
			}
			Widgets.EndScrollView();
			GUI.EndGroup();
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
		}
		private void DrawThingRow(ref float y, float width, Thing thing)
		{
			Rect rect = new Rect(0f, y, width, 28f);
			if (Mouse.IsOver(rect))
			{
				GUI.color = ITab_Pawn_Gear.HighlightColor;
				GUI.DrawTexture(rect, TexUI.HighlightTex);
			}
			if (Widgets.InvisibleButton(rect) && Event.current.button == 1)
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				list.Add(new FloatMenuOption("ThingInfo".Translate(), delegate
				{
					Find.WindowStack.Add(new Dialog_InfoCard(thing));
				}, MenuOptionPriority.Medium, null, null));
				if (this.CanEdit)
				{
					Action action = null;
					ThingWithComps eq = thing as ThingWithComps;
					Apparel ap = thing as Apparel;
					if (ap != null)
					{
						action = delegate
						{
							Pawn selPawnForGear = this.SelPawnForGear;
							if (selPawnForGear.drafter.CanTakeOrderedJob())
							{
								Job job = new Job(JobDefOf.RemoveApparel, ap);
								job.playerForced = true;
								selPawnForGear.drafter.TakeOrderedJob(job);
							}
						};
					}
					else
					{
						if (eq != null && this.SelPawnForGear.equipment.AllEquipment.Contains(eq))
						{
							ThingWithComps unused;
							action = delegate
							{
								this.SelPawnForGear.equipment.TryDropEquipment(eq, out unused, this.SelPawnForGear.Position, true);
							};
						}
						else
						{
							if (!thing.def.destroyOnDrop)
							{
								Thing unused;
								action = delegate
								{
									this.SelPawnForGear.inventory.container.TryDrop(thing, this.SelPawnForGear.Position, ThingPlaceMode.Near, out unused);
								};
							}
						}
					}
					list.Add(new FloatMenuOption("DropThing".Translate(), action, MenuOptionPriority.Medium, null, null));
				}
				FloatMenu window = new FloatMenu(list, thing.LabelCap, false, false);
				Find.WindowStack.Add(window);
			}
			if (thing.def.DrawMatSingle != null && thing.def.DrawMatSingle.mainTexture != null)
			{
				Widgets.ThingIcon(new Rect(4f, y, 28f, 28f), thing);
			}
			Text.Anchor = TextAnchor.MiddleLeft;
			GUI.color = ITab_Pawn_Gear.ThingLabelColor;
			Rect rect2 = new Rect(36f, y, width - 36f, 28f);
			string text = thing.LabelCap;
			if (thing is Apparel && this.SelPawnForGear.outfits != null && this.SelPawnForGear.outfits.forcedHandler.IsForced((Apparel)thing))
			{
				text = text + ", " + "ApparelForcedLower".Translate();
			}
			Widgets.Label(rect2, text);
			y += 28f;
		}
	}
}
