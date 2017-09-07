using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class ITab_Storage : ITab
	{
		private const float TopAreaHeight = 35f;
		private Vector2 scrollPosition = default(Vector2);
		private static readonly Vector2 WinSize = new Vector2(300f, 480f);
		private IStoreSettingsParent SelStoreSettingsParent
		{
			get
			{
				return (IStoreSettingsParent)base.SelObject;
			}
		}
		public override bool IsVisible
		{
			get
			{
				return this.SelStoreSettingsParent.StorageTabVisible;
			}
		}
		public ITab_Storage()
		{
			this.size = ITab_Storage.WinSize;
			this.labelKey = "TabStorage";
			this.tutorHighlightTag = "TabStorage";
		}
		protected override void FillTab()
		{
			ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.StorageTab, KnowledgeAmount.GuiFrame);
			ConceptDecider.TeachOpportunity(ConceptDefOf.StorageTabCategories, OpportunityType.GuiFrame);
			ConceptDecider.TeachOpportunity(ConceptDefOf.StoragePriority, OpportunityType.GuiFrame);
			IStoreSettingsParent selStoreSettingsParent = this.SelStoreSettingsParent;
			StorageSettings settings = selStoreSettingsParent.GetStoreSettings();
			Rect position = new Rect(0f, 0f, ITab_Storage.WinSize.x, ITab_Storage.WinSize.y).ContractedBy(10f);
			GUI.BeginGroup(position);
			Text.Font = GameFont.Small;
			Rect rect = new Rect(0f, 0f, 160f, 29f);
			if (Widgets.TextButton(rect, "Priority".Translate() + ": " + settings.Priority.Label(), true, false))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				IEnumerator enumerator = Enum.GetValues(typeof(StoragePriority)).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						StoragePriority storagePriority = (StoragePriority)((byte)enumerator.Current);
						if (storagePriority != StoragePriority.Unstored)
						{
							StoragePriority localPr = storagePriority;
							list.Add(new FloatMenuOption(localPr.Label(), delegate
							{
								settings.Priority = localPr;
								ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.StoragePriority, KnowledgeAmount.Total);
							}, MenuOptionPriority.Medium, null, null));
						}
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				Find.WindowStack.Add(new FloatMenu(list, false));
			}
			TutorUIHighlighter.HighlightOpportunity("StoragePriority", rect);
			ThingFilter parentFilter = null;
			if (selStoreSettingsParent.GetParentStoreSettings() != null)
			{
				parentFilter = selStoreSettingsParent.GetParentStoreSettings().filter;
			}
			Rect rect2 = new Rect(0f, 35f, position.width, position.height - 35f);
			ThingFilterUI.DoThingFilterConfigWindow(rect2, ref this.scrollPosition, settings.filter, parentFilter, 8);
			GUI.EndGroup();
		}
	}
}
