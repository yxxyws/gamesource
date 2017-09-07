using System;
using System.Collections.Generic;
using Verse;
namespace RimWorld
{
	public sealed class ResearchManager : IExposable
	{
		public ResearchProjectDef currentProj;
		private Dictionary<ResearchProjectDef, float> progress = new Dictionary<ResearchProjectDef, float>();
		private float GlobalProgressFactor = 0.01f;
		public bool AnyProjectIsAvailable
		{
			get
			{
				return DefDatabase<ResearchProjectDef>.AllDefsListForReading.Find((ResearchProjectDef x) => x.PlayerCanStart) != null;
			}
		}
		public void ExposeData()
		{
			Scribe_Defs.LookDef<ResearchProjectDef>(ref this.currentProj, "currentProj");
			Scribe_Collections.LookDictionary<ResearchProjectDef, float>(ref this.progress, "progress", LookMode.DefReference, LookMode.Value);
		}
		public void DebugSetAllProjectsFinished()
		{
			this.progress.Clear();
			foreach (ResearchProjectDef current in DefDatabase<ResearchProjectDef>.AllDefs)
			{
				this.progress.Add(current, current.totalCost);
			}
		}
		public float PercentComplete(ResearchProjectDef def)
		{
			return this.ProgressOf(def) / def.totalCost;
		}
		public bool IsFinished(ResearchProjectDef def)
		{
			return this.ProgressOf(def) >= def.totalCost;
		}
		public float ProgressOf(ResearchProjectDef def)
		{
			float result;
			if (this.progress.TryGetValue(def, out result))
			{
				return result;
			}
			this.progress.Add(def, 0f);
			return 0f;
		}
		public void MakeProgress(float amount, Pawn researcher)
		{
			if (this.currentProj == null)
			{
				Log.Error("Researched without having an active project.");
				return;
			}
			amount *= this.GlobalProgressFactor;
			if (DebugSettings.fastResearch)
			{
				amount *= 500f;
			}
			if (researcher != null)
			{
				researcher.records.AddTo(RecordDefOf.ResearchPointsResearched, amount);
			}
			float num = this.ProgressOf(this.currentProj);
			num += amount;
			this.progress[this.currentProj] = num;
			if (this.currentProj.IsFinished)
			{
				string text = "ResearchFinished".Translate(new object[]
				{
					this.currentProj.LabelCap
				}) + "\n\n" + this.currentProj.DescriptionDiscovered;
				DiaNode diaNode = new DiaNode(text);
				diaNode.options.Add(DiaOption.DefaultOK);
				DiaOption diaOption = new DiaOption("ResearchScreen".Translate());
				diaOption.resolveTree = true;
				diaOption.action = delegate
				{
					Find.MainTabsRoot.SetCurrentTab(MainTabDefOf.Research, true);
				};
				diaNode.options.Add(diaOption);
				Find.WindowStack.Add(new Dialog_NodeTree(diaNode));
				if (researcher != null)
				{
					TaleRecorder.RecordTale(TaleDefOf.FinishedResearchProject, new object[]
					{
						researcher,
						this.currentProj
					});
				}
				this.currentProj = null;
				this.ReapplyAllMods();
			}
		}
		public void ReapplyAllMods()
		{
			foreach (ResearchProjectDef current in DefDatabase<ResearchProjectDef>.AllDefs)
			{
				if (this.IsFinished(current))
				{
					current.ReapplyAllMods();
				}
			}
		}
		public void InstantFinish(ResearchProjectDef proj)
		{
			this.MakeProgress((proj.totalCost - this.ProgressOf(proj)) / this.GlobalProgressFactor, null);
		}
	}
}
