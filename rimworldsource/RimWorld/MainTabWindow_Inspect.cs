using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace RimWorld
{
	[StaticConstructorOnStartup]
	public class MainTabWindow_Inspect : MainTabWindow
	{
		private const float PaneWidth = 432f;
		private const float PaneInnerMargin = 12f;
		protected const float TabWidth = 72f;
		public const float TabHeight = 30f;
		private Type openTabType;
		private float recentHeight;
		public static readonly Vector2 PaneSize = new Vector2(432f, 165f);
		public static readonly Vector2 PaneInnerSize = new Vector2(MainTabWindow_Inspect.PaneSize.x - 24f, MainTabWindow_Inspect.PaneSize.y - 24f);
		private static readonly Texture2D InspectTabButtonFillTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.07450981f, 0.08627451f, 0.105882354f, 1f));
		private static IntVec3 lastSelectCell;
		private IEnumerable<object> Selected
		{
			get
			{
				return Find.Selector.SelectedObjects;
			}
		}
		private Thing SelThing
		{
			get
			{
				return Find.Selector.SingleSelectedThing;
			}
		}
		private Zone SelZone
		{
			get
			{
				return Find.Selector.SelectedZone;
			}
		}
		private int NumSelected
		{
			get
			{
				return Find.Selector.NumSelected;
			}
		}
		public float PaneTopY
		{
			get
			{
				return (float)Screen.height - MainTabWindow_Inspect.PaneSize.y - 35f;
			}
		}
		public float CurDrawHeight
		{
			get
			{
				return this.recentHeight;
			}
		}
		public IEnumerable<ITab> CurTabs
		{
			get
			{
				if (this.NumSelected == 1)
				{
					if (this.SelThing != null && this.SelThing.def.inspectorTabsResolved != null)
					{
						return this.SelThing.def.inspectorTabsResolved;
					}
					if (this.SelZone != null)
					{
						return this.SelZone.GetInspectionTabs();
					}
				}
				return Enumerable.Empty<ITab>();
			}
		}
		public override Vector2 RequestedTabSize
		{
			get
			{
				return MainTabWindow_Inspect.PaneSize;
			}
		}
		protected override float WindowPadding
		{
			get
			{
				return 0f;
			}
		}
		public MainTabWindow_Inspect()
		{
			this.closeOnEscapeKey = false;
		}
		public override void PreOpen()
		{
			base.PreOpen();
			this.Reset();
		}
		public override void WindowUpdate()
		{
			base.WindowUpdate();
			foreach (ITab current in this.CurTabs)
			{
				if (current.IsVisible)
				{
					if (current.GetType() == this.openTabType)
					{
						current.TabUpdate();
					}
				}
			}
		}
		public override void ExtraOnGUI()
		{
			base.ExtraOnGUI();
			if (this.NumSelected > 0)
			{
				if (KeyBindingDefOf.SelectNextInCell.KeyDownEvent)
				{
					this.SelectNextInCell();
				}
				if (DesignatorManager.SelectedDesignator != null)
				{
					DesignatorManager.SelectedDesignator.DoExtraGuiControls(0f, this.PaneTopY);
				}
				InspectGizmoGrid.DrawInspectGizmoGridFor(this.Selected);
				this.DoTabs(this.CurTabs);
			}
		}
		public override void DoWindowContents(Rect inRect)
		{
			base.DoWindowContents(inRect);
			this.recentHeight = MainTabWindow_Inspect.PaneSize.y;
			if (this.NumSelected > 0)
			{
				try
				{
					Rect rect = inRect.ContractedBy(12f);
					rect.yMin -= 4f;
					GUI.BeginGroup(rect);
					bool flag = true;
					if (this.NumSelected > 1)
					{
						flag = !(
							from t in this.Selected
							where !InspectPaneUtility.CanInspectTogether(this.Selected.First<object>(), t)
							select t).Any<object>();
					}
					else
					{
						Rect rect2 = new Rect(rect.width - 30f, 0f, 30f, 30f);
						if (Find.Selector.SelectedZone == null || Find.Selector.SelectedZone.ContainsCell(MainTabWindow_Inspect.lastSelectCell))
						{
							if (Widgets.ImageButton(rect2, TexButton.SelectOverlappingNext))
							{
								this.SelectNextInCell();
							}
							TooltipHandler.TipRegion(rect2, "SelectNextInSquareTip".Translate(new object[]
							{
								KeyBindingDefOf.SelectNextInCell.MainKeyLabel
							}));
						}
						Thing singleSelectedThing = Find.Selector.SingleSelectedThing;
						if (singleSelectedThing != null)
						{
							Widgets.InfoCardButton(rect.width - 60f, 0f, Find.Selector.SingleSelectedThing);
							Pawn pawn = singleSelectedThing as Pawn;
							if (pawn != null && pawn.IsColonist)
							{
								HostilityResponseModeUtility.DrawResponseButton(new Vector2(rect.width - 90f, 0f), pawn);
							}
						}
					}
					Rect rect3 = new Rect(0f, 0f, rect.width - 88f, 50f);
					string label = InspectPaneUtility.AdjustedLabelFor(this.Selected, rect3);
					rect3.width += 300f;
					Text.Font = GameFont.Medium;
					Text.Anchor = TextAnchor.UpperLeft;
					Widgets.Label(rect3, label);
					if (flag && this.NumSelected == 1)
					{
						Rect rect4 = rect.AtZero();
						rect4.yMin += 26f;
						InspectPaneFiller.DoPaneContentsFor((ISelectable)Find.Selector.FirstSelectedObject, rect4);
					}
				}
				catch (Exception ex)
				{
					Log.Error("Exception doing inspect pane: " + ex.ToString());
				}
				finally
				{
					GUI.EndGroup();
				}
			}
		}
		private void SelectNextInCell()
		{
			if (this.NumSelected == 0)
			{
				return;
			}
			if (Find.Selector.SelectedZone == null || Find.Selector.SelectedZone.ContainsCell(MainTabWindow_Inspect.lastSelectCell))
			{
				if (Find.Selector.SelectedZone == null)
				{
					MainTabWindow_Inspect.lastSelectCell = Find.Selector.SingleSelectedThing.Position;
				}
				Find.Selector.SelectNextAt(MainTabWindow_Inspect.lastSelectCell);
			}
		}
		private void DoTabs(IEnumerable<ITab> tabs)
		{
			try
			{
				float top = this.PaneTopY - 30f;
				float num = 360f;
				float width = 0f;
				bool flag = false;
				foreach (ITab current in tabs)
				{
					if (current.IsVisible)
					{
						Rect rect = new Rect(num, top, 72f, 30f);
						width = num;
						Text.Font = GameFont.Small;
						if (Widgets.TextButton(rect, current.labelKey.Translate(), true, false))
						{
							this.ToggleTab(current);
						}
						if (!current.tutorHighlightTag.NullOrEmpty())
						{
							TutorUIHighlighter.HighlightOpportunity(current.tutorHighlightTag, rect);
						}
						if (current.GetType() == this.openTabType)
						{
							current.DoTabGui();
							this.recentHeight = 700f;
							flag = true;
						}
						num -= 72f;
					}
				}
				if (flag)
				{
					GUI.DrawTexture(new Rect(0f, top, width, 30f), MainTabWindow_Inspect.InspectTabButtonFillTex);
				}
			}
			catch (Exception ex)
			{
				Log.ErrorOnce(ex.ToString(), 742783);
			}
		}
		public void CloseOpenTab()
		{
			this.openTabType = null;
		}
		private void ToggleTab(ITab tab)
		{
			if ((tab == null && this.openTabType == null) || tab.GetType() == this.openTabType)
			{
				this.openTabType = null;
				SoundDefOf.TabClose.PlayOneShotOnCamera();
			}
			else
			{
				tab.OnOpen();
				this.openTabType = tab.GetType();
				SoundDefOf.TabOpen.PlayOneShotOnCamera();
			}
		}
		public void Reset()
		{
			this.openTabType = null;
		}
	}
}
