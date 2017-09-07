using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class TrainableDef : Def
	{
		public float difficulty = -1f;
		public float minBodySize;
		public List<TrainableDef> prerequisites;
		public List<string> tags = new List<string>();
		public bool defaultTrainable;
		public TrainableIntelligence requiredTrainableIntelligence = TrainableIntelligence.Simple;
		public int steps = 1;
		public float listPriority;
		[Unsaved]
		public int indent;
		public bool MatchesTag(string tag)
		{
			if (tag == this.defName)
			{
				return true;
			}
			for (int i = 0; i < this.tags.Count; i++)
			{
				if (this.tags[i] == tag)
				{
					return true;
				}
			}
			return false;
		}
		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			TrainableDef.<ConfigErrors>c__Iterator79 <ConfigErrors>c__Iterator = new TrainableDef.<ConfigErrors>c__Iterator79();
			<ConfigErrors>c__Iterator.<>f__this = this;
			TrainableDef.<ConfigErrors>c__Iterator79 expr_0E = <ConfigErrors>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
