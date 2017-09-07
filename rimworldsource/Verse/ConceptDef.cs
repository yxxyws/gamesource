using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse
{
	public class ConceptDef : Def
	{
		public int priority = 9999;
		public bool alertDisplay;
		public bool alertStartActive;
		public bool noteTeaches;
		public bool needsOpportunity;
		public bool opportunityDecays = true;
		public GameMode gameMode = GameMode.MapPlaying;
		public float minSecondsSinceLesson;
		private List<string> helpTextKeys;
		private List<string> helpTexts;
		public string highlightTag;
		public int HelpTextsCount
		{
			get
			{
				int num = 0;
				if (this.helpTextKeys != null)
				{
					num += this.helpTextKeys.Count;
				}
				if (this.helpTexts != null)
				{
					num += this.helpTexts.Count;
				}
				return num;
			}
		}
		public string GetHelpText(int ind)
		{
			if (this.helpTextKeys != null)
			{
				return this.helpTextKeys[ind].Translate();
			}
			if (this.helpTexts != null)
			{
				return this.helpTexts[ind];
			}
			return "No help texts for this lesson.";
		}
		public override void PostLoad()
		{
			base.PostLoad();
			if (this.defName == "UnnamedDef")
			{
				this.defName = this.defName.ToString();
			}
		}
		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			ConceptDef.<ConfigErrors>c__Iterator134 <ConfigErrors>c__Iterator = new ConceptDef.<ConfigErrors>c__Iterator134();
			<ConfigErrors>c__Iterator.<>f__this = this;
			ConceptDef.<ConfigErrors>c__Iterator134 expr_0E = <ConfigErrors>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public static ConceptDef Named(string defName)
		{
			return DefDatabase<ConceptDef>.GetNamed(defName, true);
		}
	}
}
