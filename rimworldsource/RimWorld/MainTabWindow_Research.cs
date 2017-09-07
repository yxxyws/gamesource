using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace RimWorld
{
	[StaticConstructorOnStartup]
	public class MainTabWindow_Research : MainTabWindow
	{
		private const float LeftAreaWidth = 330f;
		private const int ModeSelectButHeight = 40;
		private const float ProjectTitleHeight = 50f;
		private const float ProjectTitleLeftMargin = 20f;
		private const int ProjectIntervalY = 25;
		private const float PrereqsLineSpacing = 15f;
		protected ResearchProjectDef selectedProject;
		private bool showResearchedProjects;
		private Vector2 projectListScrollPosition = default(Vector2);
		private bool noBenchWarned;
		private Vector2 scrollPosition = Vector2.zero;
		private float scrollViewHeight;
		private static readonly Texture2D ResearchBarFillTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.8f, 0.85f));
		private static readonly Texture2D ResearchBarBGTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.1f, 0.1f, 0.1f));
		private static readonly Color FulfilledPrerequisiteColor = Color.green;
		private static readonly Color MissingPrerequisiteColor = Color.red;
		private static readonly Color ProjectWithMissingPrerequisiteLabelColor = Color.gray;
		public override float TabButtonBarPercent
		{
			get
			{
				ResearchProjectDef currentProj = Find.ResearchManager.currentProj;
				if (currentProj == null)
				{
					return 0f;
				}
				return currentProj.PercentComplete;
			}
		}
		public override void PreOpen()
		{
			base.PreOpen();
			this.selectedProject = Find.ResearchManager.currentProj;
		}
		public override void DoWindowContents(Rect inRect)
		{
			base.DoWindowContents(inRect);
			if (!this.noBenchWarned)
			{
				if (!Find.ListerBuildings.ColonistsHaveResearchBench())
				{
					Find.WindowStack.Add(new Dialog_Message("ResearchMenuWithoutBench".Translate(), null));
				}
				this.noBenchWarned = true;
			}
			Text.Font = GameFont.Medium;
			Text.Anchor = TextAnchor.UpperCenter;
			Widgets.Label(new Rect(0f, 0f, inRect.width, 300f), "Research".Translate());
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
			Rect rect = new Rect(0f, 75f, 330f, inRect.height - 75f);
			Rect rect2 = new Rect(rect.xMax + 10f, 45f, inRect.width - rect.width - 10f, inRect.height - 45f);
			Widgets.DrawMenuSection(rect, false);
			Widgets.DrawMenuSection(rect2, true);
			this.DrawLeftRect(rect);
			this.DrawRightRect(rect2);
		}
		private void DrawLeftRect(Rect leftOutRect)
		{
			Rect outRect = leftOutRect.ContractedBy(10f);
			IEnumerable<ResearchProjectDef> enumerable;
			if (this.showResearchedProjects)
			{
				enumerable = 
					from proj in DefDatabase<ResearchProjectDef>.AllDefs
					where proj.IsFinished
					select proj;
			}
			else
			{
				enumerable = 
					from proj in DefDatabase<ResearchProjectDef>.AllDefs
					where !proj.IsFinished
					select proj;
			}
			enumerable = 
				from x in enumerable
				orderby x.PlayerCanStart descending, x.totalCost
				select x;
			float height = (float)(25 * enumerable.Count<ResearchProjectDef>() + 100);
			Rect rect = new Rect(0f, 0f, outRect.width - 16f, height);
			Widgets.BeginScrollView(outRect, ref this.projectListScrollPosition, rect);
			Rect position = rect.ContractedBy(10f);
			GUI.BeginGroup(position);
			int num = 0;
			foreach (ResearchProjectDef current in enumerable)
			{
				Rect rect2 = new Rect(0f, (float)num, position.width, 25f);
				if (this.selectedProject == current)
				{
					GUI.DrawTexture(rect2, TexUI.HighlightTex);
				}
				string text = current.LabelCap + " (" + current.totalCost.ToString("F0") + ")";
				Rect rect3 = new Rect(rect2);
				rect3.x += 6f;
				rect3.width -= 6f;
				float num2 = Text.CalcHeight(text, rect3.width);
				if (rect3.height < num2)
				{
					rect3.height = num2 + 3f;
				}
				Color textColor = Widgets.NormalOptionColor;
				if (!current.IsFinished && !current.PlayerCanStart)
				{
					textColor = MainTabWindow_Research.ProjectWithMissingPrerequisiteLabelColor;
				}
				if (Widgets.TextButton(rect3, text, false, true, textColor))
				{
					SoundDefOf.Click.PlayOneShotOnCamera();
					this.selectedProject = current;
				}
				num += 25;
			}
			GUI.EndGroup();
			Widgets.EndScrollView();
			List<TabRecord> list = new List<TabRecord>();
			TabRecord item = new TabRecord("Researched".Translate(), delegate
			{
				this.showResearchedProjects = true;
			}, this.showResearchedProjects);
			list.Add(item);
			TabRecord item2 = new TabRecord("Unresearched".Translate(), delegate
			{
				this.showResearchedProjects = false;
			}, !this.showResearchedProjects);
			list.Add(item2);
			TabDrawer.DrawTabs(leftOutRect, list);
		}
		private void DrawRightRect(Rect rightOutRect)
		{
			Rect position = rightOutRect.ContractedBy(20f);
			GUI.BeginGroup(position);
			if (this.selectedProject != null)
			{
				Rect outRect = new Rect(0f, 0f, position.width, 280f);
				Rect viewRect = new Rect(0f, 0f, outRect.width - 16f, this.scrollViewHeight);
				Widgets.BeginScrollView(outRect, ref this.scrollPosition, viewRect);
				Text.Font = GameFont.Medium;
				GenUI.SetLabelAlign(TextAnchor.MiddleLeft);
				Rect rect = new Rect(20f, 0f, position.width - 20f, 50f);
				Widgets.Label(rect, this.selectedProject.LabelCap);
				GenUI.ResetLabelAlign();
				Text.Font = GameFont.Small;
				float num = Text.CalcHeight(this.selectedProject.description, position.width);
				Rect rect2 = new Rect(0f, 50f, position.width, num + 10f);
				Widgets.Label(rect2, this.selectedProject.description);
				Rect rect3 = new Rect(0f, rect2.yMax, position.width, 500f);
				float num2 = this.DrawResearchPrereqs(this.selectedProject, rect3);
				if (num2 > 0f)
				{
					num2 += 15f;
				}
				Rect rect4 = new Rect(0f, rect2.yMax + num2, position.width, 500f);
				float num3 = this.DrawResearchBenchRequirements(this.selectedProject, rect4);
				this.scrollViewHeight = rect2.yMax + num2 + num3 + 3f;
				Widgets.EndScrollView();
				Rect rect5 = new Rect(position.width / 2f - 50f, 300f, 100f, 50f);
				if (this.selectedProject.IsFinished)
				{
					Widgets.DrawMenuSection(rect5, true);
					Text.Anchor = TextAnchor.MiddleCenter;
					Widgets.Label(rect5, "Finished".Translate());
					Text.Anchor = TextAnchor.UpperLeft;
				}
				else
				{
					if (this.selectedProject == Find.ResearchManager.currentProj)
					{
						Widgets.DrawMenuSection(rect5, true);
						Text.Anchor = TextAnchor.MiddleCenter;
						Widgets.Label(rect5, "InProgress".Translate());
						Text.Anchor = TextAnchor.UpperLeft;
					}
					else
					{
						if (!this.selectedProject.PlayerCanStart)
						{
							Widgets.DrawMenuSection(rect5, true);
							Text.Anchor = TextAnchor.MiddleCenter;
							Widgets.Label(rect5, "Locked".Translate());
							Text.Anchor = TextAnchor.UpperLeft;
						}
						else
						{
							if (Widgets.TextButton(rect5, "Research".Translate(), true, false))
							{
								SoundDef.Named("ResearchStart").PlayOneShotOnCamera();
								Find.ResearchManager.currentProj = this.selectedProject;
							}
						}
						if (Prefs.DevMode && this.selectedProject.ResearchPrereqsFulfilled)
						{
							Rect rect6 = rect5;
							rect6.x += rect6.width + 4f;
							if (Widgets.TextButton(rect6, "Debug Insta-finish", true, false))
							{
								Find.ResearchManager.currentProj = this.selectedProject;
								Find.ResearchManager.InstantFinish(this.selectedProject);
							}
						}
					}
				}
				Rect rect7 = new Rect(15f, 450f, position.width - 30f, 35f);
				Widgets.FillableBar(rect7, this.selectedProject.PercentComplete, MainTabWindow_Research.ResearchBarFillTex, MainTabWindow_Research.ResearchBarBGTex, true);
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.Label(rect7, this.selectedProject.ProgressNumbersString);
				Text.Anchor = TextAnchor.UpperLeft;
			}
			GUI.EndGroup();
		}
		private float DrawResearchPrereqs(ResearchProjectDef project, Rect rect)
		{
			if (project.prerequisites.NullOrEmpty<ResearchProjectDef>())
			{
				return 0f;
			}
			float yMin = rect.yMin;
			Widgets.Label(rect, "ResearchPrerequisites".Translate() + ":");
			rect.yMin += 15f;
			for (int i = 0; i < project.prerequisites.Count; i++)
			{
				this.SetPrerequisiteStatusColor(project.prerequisites[i].IsFinished, project);
				Widgets.Label(rect, "  " + project.prerequisites[i].LabelCap);
				rect.yMin += 15f;
			}
			GUI.color = Color.white;
			return rect.yMin - yMin;
		}
		private float DrawResearchBenchRequirements(ResearchProjectDef project, Rect rect)
		{
			float yMin = rect.yMin;
			if (project.requiredResearchBuilding != null)
			{
				bool present = Find.ListerBuildings.allBuildingsColonist.Find((Building x) => x.def == project.requiredResearchBuilding) != null;
				Widgets.Label(rect, "RequiredResearchBench".Translate() + ":");
				rect.yMin += 15f;
				this.SetPrerequisiteStatusColor(present, project);
				Widgets.Label(rect, "  " + project.requiredResearchBuilding.LabelCap);
				rect.yMin += 30f;
				GUI.color = Color.white;
			}
			if (!project.requiredResearchFacilities.NullOrEmpty<ThingDef>())
			{
				Widgets.Label(rect, "RequiredResearchBenchFacilities".Translate() + ":");
				rect.yMin += 15f;
				Building_ResearchBench building_ResearchBench = this.FindBenchFulfillingMostRequirements(project.requiredResearchBuilding, project.requiredResearchFacilities);
				CompAffectedByFacilities bestMatchingBench = null;
				if (building_ResearchBench != null)
				{
					bestMatchingBench = building_ResearchBench.TryGetComp<CompAffectedByFacilities>();
				}
				for (int i = 0; i < project.requiredResearchFacilities.Count; i++)
				{
					this.DrawResearchBenchFacilityRequirement(project.requiredResearchFacilities[i], bestMatchingBench, project, rect);
					rect.yMin += 15f;
				}
			}
			GUI.color = Color.white;
			return rect.yMin - yMin;
		}
		private void SetPrerequisiteStatusColor(bool present, ResearchProjectDef project)
		{
			if (project.IsFinished)
			{
				return;
			}
			if (present)
			{
				GUI.color = MainTabWindow_Research.FulfilledPrerequisiteColor;
			}
			else
			{
				GUI.color = MainTabWindow_Research.MissingPrerequisiteColor;
			}
		}
		private void DrawResearchBenchFacilityRequirement(ThingDef requiredFacility, CompAffectedByFacilities bestMatchingBench, ResearchProjectDef project, Rect rect)
		{
			Thing thing = null;
			Thing thing2 = null;
			if (bestMatchingBench != null)
			{
				thing = bestMatchingBench.LinkedFacilitiesListForReading.Find((Thing x) => x.def == requiredFacility);
				thing2 = bestMatchingBench.LinkedFacilitiesListForReading.Find((Thing x) => x.def == requiredFacility && bestMatchingBench.IsFacilityActive(x));
			}
			this.SetPrerequisiteStatusColor(thing2 != null, project);
			string text = requiredFacility.LabelCap;
			if (thing != null && thing2 == null)
			{
				text = text + " (" + "InactiveFacility".Translate() + ")";
			}
			Widgets.Label(rect, "  " + text);
		}
		private Building_ResearchBench FindBenchFulfillingMostRequirements(ThingDef requiredResearchBench, List<ThingDef> requiredFacilities)
		{
			List<Building> allBuildingsColonist = Find.ListerBuildings.allBuildingsColonist;
			float num = 0f;
			Building_ResearchBench building_ResearchBench = null;
			for (int i = 0; i < allBuildingsColonist.Count; i++)
			{
				Building_ResearchBench building_ResearchBench2 = allBuildingsColonist[i] as Building_ResearchBench;
				if (building_ResearchBench2 != null)
				{
					if (requiredResearchBench == null || building_ResearchBench2.def == requiredResearchBench)
					{
						float researchBenchRequirementsScore = this.GetResearchBenchRequirementsScore(building_ResearchBench2, requiredFacilities);
						if (building_ResearchBench == null || researchBenchRequirementsScore > num)
						{
							num = researchBenchRequirementsScore;
							building_ResearchBench = building_ResearchBench2;
						}
					}
				}
			}
			return building_ResearchBench;
		}
		private float GetResearchBenchRequirementsScore(Building_ResearchBench bench, List<ThingDef> requiredFacilities)
		{
			float num = 0f;
			int i;
			for (i = 0; i < requiredFacilities.Count; i++)
			{
				CompAffectedByFacilities benchComp = bench.GetComp<CompAffectedByFacilities>();
				if (benchComp != null)
				{
					List<Thing> linkedFacilitiesListForReading = benchComp.LinkedFacilitiesListForReading;
					if (linkedFacilitiesListForReading.Find((Thing x) => x.def == requiredFacilities[i] && benchComp.IsFacilityActive(x)) != null)
					{
						num += 1f;
					}
					else
					{
						if (linkedFacilitiesListForReading.Find((Thing x) => x.def == requiredFacilities[i]) != null)
						{
							num += 0.6f;
						}
					}
				}
			}
			return num;
		}
	}
}
