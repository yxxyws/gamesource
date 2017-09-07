using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class ITab_Growing : ITab
	{
		private const float TopAreaHeight = 35f;
		private static readonly Vector2 WinSize = new Vector2(300f, 200f);
		public ITab_Growing()
		{
			this.size = ITab_Growing.WinSize;
			this.labelKey = "TabGrowing";
		}
		protected override void FillTab()
		{
			Text.Font = GameFont.Small;
			IPlantToGrowSettable grower = (IPlantToGrowSettable)Find.Selector.SelectedObjects.First<object>();
			Rect position = new Rect(0f, 0f, ITab_Growing.WinSize.x, ITab_Growing.WinSize.y).ContractedBy(10f);
			GUI.BeginGroup(position);
			Rect rect = new Rect(0f, 0f, 150f, 29f);
			Widgets.Label(rect, "PlantVerb".Translate() + ": ");
			Rect rect2 = new Rect(100f, 0f, 130f, 29f);
			if (Widgets.TextButton(rect2, grower.GetPlantDefToGrow().LabelCap, true, false))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (ThingDef current in GenPlant.ValidPlantTypesForGrower(Find.Selector.SingleSelectedObject))
				{
					if (this.IsPlantAvailable(current))
					{
						ThingDef localPlantDef = current;
						list.Add(new FloatMenuOption(current.LabelCap, delegate
						{
							grower.SetPlantDefToGrow(localPlantDef);
							ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.SetGrowingZonePlant, KnowledgeAmount.Total);
						}, MenuOptionPriority.Medium, null, null));
					}
				}
				Find.WindowStack.Add(new FloatMenu(list, false));
			}
			Widgets.InfoCardButton(240f, 0f, grower.GetPlantDefToGrow());
			TutorUIHighlighter.HighlightOpportunity("GrowingZoneSetPlant", rect2);
			GUI.EndGroup();
		}
		private bool IsPlantAvailable(ThingDef plantDef)
		{
			List<ResearchProjectDef> sowResearchPrerequisites = plantDef.plant.sowResearchPrerequisites;
			if (sowResearchPrerequisites == null)
			{
				return true;
			}
			for (int i = 0; i < sowResearchPrerequisites.Count; i++)
			{
				if (!Find.ResearchManager.IsFinished(sowResearchPrerequisites[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}
