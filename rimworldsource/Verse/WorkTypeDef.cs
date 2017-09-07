using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace Verse
{
	public class WorkTypeDef : Def
	{
		public WorkTags workTags;
		public string labelShort;
		public string pawnLabel;
		public string gerundLabel;
		public string verb;
		public bool visible = true;
		public int naturalPriority;
		public bool alwaysStartActive;
		public bool requireCapableColonist;
		public List<SkillDef> relevantSkills = new List<SkillDef>();
		[Unsaved]
		public List<WorkGiverDef> workGiversByPriority = new List<WorkGiverDef>();
		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			WorkTypeDef.<ConfigErrors>c__Iterator155 <ConfigErrors>c__Iterator = new WorkTypeDef.<ConfigErrors>c__Iterator155();
			<ConfigErrors>c__Iterator.<>f__this = this;
			WorkTypeDef.<ConfigErrors>c__Iterator155 expr_0E = <ConfigErrors>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override void ResolveReferences()
		{
			foreach (WorkGiverDef current in 
				from d in DefDatabase<WorkGiverDef>.AllDefs
				where d.workType == this
				orderby d.priorityInType descending
				select d)
			{
				this.workGiversByPriority.Add(current);
			}
		}
		public override int GetHashCode()
		{
			return Gen.HashCombine<string>(this.defName.GetHashCode(), this.gerundLabel);
		}
	}
}
