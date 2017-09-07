using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Verse;
namespace RimWorld
{
	public class Backstory
	{
		public string uniqueSaveKey;
		public string title;
		public string titleShort;
		public string baseDesc;
		public BackstorySlot slot;
		public Dictionary<string, int> skillGains = new Dictionary<string, int>();
		public Dictionary<SkillDef, int> skillGainsResolved = new Dictionary<SkillDef, int>();
		public WorkTags workDisables;
		public List<string> spawnCategories = new List<string>();
		[LoadAlias("bodyNameGlobal")]
		public BodyType bodyTypeGlobal;
		[LoadAlias("bodyNameFemale")]
		public BodyType bodyTypeFemale;
		[LoadAlias("bodyNameMale")]
		public BodyType bodyTypeMale;
		public bool shuffleable = true;
		public IEnumerable<WorkTypeDef> DisabledWorkTypes
		{
			get
			{
				Backstory.<>c__Iterator98 <>c__Iterator = new Backstory.<>c__Iterator98();
				<>c__Iterator.<>f__this = this;
				Backstory.<>c__Iterator98 expr_0E = <>c__Iterator;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public BodyType BodyTypeFor(Gender g)
		{
			if (this.bodyTypeGlobal != BodyType.Undefined || g == Gender.None)
			{
				return this.bodyTypeGlobal;
			}
			if (g == Gender.Female)
			{
				return this.bodyTypeFemale;
			}
			return this.bodyTypeMale;
		}
		public string FullDescriptionFor(Pawn p)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.baseDesc.AdjustedFor(p));
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
			for (int i = 0; i < allDefsListForReading.Count; i++)
			{
				SkillDef skillDef = allDefsListForReading[i];
				if (this.skillGainsResolved.ContainsKey(skillDef))
				{
					stringBuilder.AppendLine(skillDef.skillLabel + ":   " + this.skillGainsResolved[skillDef].ToString("+##;-##"));
				}
			}
			foreach (WorkTypeDef current in this.DisabledWorkTypes)
			{
				stringBuilder.AppendLine(current.gerundLabel + " " + "DisabledLower".Translate());
			}
			return stringBuilder.ToString();
		}
		public bool AllowsWorkType(WorkTypeDef workDef)
		{
			return (this.workDisables & workDef.workTags) == WorkTags.None;
		}
		public void PostLoad()
		{
			this.uniqueSaveKey = GenText.CapitalizedNoSpaces(this.title + GenText.StableStringHash(this.baseDesc).ToString());
			if (!this.title.Equals(GenText.ToNewsCase(this.title)))
			{
				Log.Warning("Bad capitalization on backstory title: " + this.title);
				this.title = GenText.ToNewsCase(this.title);
			}
			if (this.slot == BackstorySlot.Adulthood && this.bodyTypeGlobal == BodyType.Undefined)
			{
				if (this.bodyTypeMale == BodyType.Undefined)
				{
					Log.Error("Adulthood backstory " + this.title + " is missing male body type. Defaulting...");
					this.bodyTypeMale = BodyType.Male;
				}
				if (this.bodyTypeFemale == BodyType.Undefined)
				{
					Log.Error("Adulthood backstory " + this.title + " is missing female body type. Defaulting...");
					this.bodyTypeFemale = BodyType.Female;
				}
			}
			this.baseDesc = this.baseDesc.TrimEnd(new char[0]);
		}
		public void ResolveReferences()
		{
			foreach (KeyValuePair<string, int> current in this.skillGains)
			{
				this.skillGainsResolved.Add(DefDatabase<SkillDef>.GetNamed(current.Key, true), current.Value);
			}
			this.skillGains = null;
		}
		[DebuggerHidden]
		public IEnumerable<string> ConfigErrors(bool ignoreNoSpawnCategories)
		{
			Backstory.<ConfigErrors>c__Iterator99 <ConfigErrors>c__Iterator = new Backstory.<ConfigErrors>c__Iterator99();
			<ConfigErrors>c__Iterator.ignoreNoSpawnCategories = ignoreNoSpawnCategories;
			<ConfigErrors>c__Iterator.<$>ignoreNoSpawnCategories = ignoreNoSpawnCategories;
			<ConfigErrors>c__Iterator.<>f__this = this;
			Backstory.<ConfigErrors>c__Iterator99 expr_1C = <ConfigErrors>c__Iterator;
			expr_1C.$PC = -2;
			return expr_1C;
		}
		public override int GetHashCode()
		{
			return this.uniqueSaveKey.GetHashCode();
		}
	}
}
