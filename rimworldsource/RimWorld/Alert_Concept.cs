using System;
using Verse;
namespace RimWorld
{
	public class Alert_Concept : Alert_Tutorial
	{
		public ConceptDef concept;
		public override string FullLabel
		{
			get
			{
				return this.concept.label;
			}
		}
		public override string FullExplanation
		{
			get
			{
				return this.concept.GetHelpText(0);
			}
		}
		public override AlertReport Report
		{
			get
			{
				return Prefs.AdaptiveTrainingEnabled && ConceptDecider.AlertShouldDisplay(this.concept);
			}
		}
		public Alert_Concept(ConceptDef concept)
		{
			this.concept = concept;
		}
		public override void AlertActiveUpdate()
		{
			base.AlertActiveUpdate();
			TutorUIHighlighter.HighlightTag(this.concept.highlightTag);
		}
	}
}
