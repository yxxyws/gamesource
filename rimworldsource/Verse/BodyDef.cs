using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse
{
	public class BodyDef : Def
	{
		public BodyPartRecord corePart;
		[Unsaved]
		private Dictionary<PawnCapacityDef, List<string>> capacityGroups = new Dictionary<PawnCapacityDef, List<string>>();
		[Unsaved]
		private List<BodyPartRecord> cachedAllParts = new List<BodyPartRecord>();
		[Unsaved]
		private List<BodyPartRecord> cachedPartsVulnerableToFrostbite;
		public List<BodyPartRecord> AllParts
		{
			get
			{
				return this.cachedAllParts;
			}
		}
		public List<BodyPartRecord> AllPartsVulnerableToFrostbite
		{
			get
			{
				return this.cachedPartsVulnerableToFrostbite;
			}
		}
		public List<string> GetActivityGroups(PawnCapacityDef actDef)
		{
			List<string> list;
			if (!this.capacityGroups.TryGetValue(actDef, out list))
			{
				list = new List<string>();
				for (int i = 0; i < this.AllParts.Count; i++)
				{
					for (int j = 0; j < this.AllParts[i].def.Activities.Count; j++)
					{
						Pair<PawnCapacityDef, string> pair = this.AllParts[i].def.Activities[j];
						if (pair.First == actDef && !list.Contains(pair.Second))
						{
							list.Add(pair.Second);
						}
					}
				}
				this.capacityGroups.Add(actDef, list);
			}
			return list;
		}
		[DebuggerHidden]
		public IEnumerable<BodyPartRecord> GetParts(PawnCapacityDef activity, string activityGroup)
		{
			BodyDef.<GetParts>c__Iterator130 <GetParts>c__Iterator = new BodyDef.<GetParts>c__Iterator130();
			<GetParts>c__Iterator.activity = activity;
			<GetParts>c__Iterator.activityGroup = activityGroup;
			<GetParts>c__Iterator.<$>activity = activity;
			<GetParts>c__Iterator.<$>activityGroup = activityGroup;
			<GetParts>c__Iterator.<>f__this = this;
			BodyDef.<GetParts>c__Iterator130 expr_2A = <GetParts>c__Iterator;
			expr_2A.$PC = -2;
			return expr_2A;
		}
		public BodyPartRecord GetPartAtIndex(int index)
		{
			return this.cachedAllParts[index];
		}
		public int GetIndexOfPart(BodyPartRecord rec)
		{
			for (int i = 0; i < this.cachedAllParts.Count; i++)
			{
				if (this.cachedAllParts[i] == rec)
				{
					return i;
				}
			}
			throw new ArgumentException("Cannot get index of BodyPartRecord that is not in this BodyDef.");
		}
		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			BodyDef.<ConfigErrors>c__Iterator131 <ConfigErrors>c__Iterator = new BodyDef.<ConfigErrors>c__Iterator131();
			<ConfigErrors>c__Iterator.<>f__this = this;
			BodyDef.<ConfigErrors>c__Iterator131 expr_0E = <ConfigErrors>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override void ResolveReferences()
		{
			if (this.corePart != null)
			{
				this.CacheDataRecursive(this.corePart);
			}
			this.cachedPartsVulnerableToFrostbite = new List<BodyPartRecord>();
			List<BodyPartRecord> allParts = this.AllParts;
			for (int i = 0; i < allParts.Count; i++)
			{
				if (allParts[i].def.frostbiteVulnerability > 0f)
				{
					this.cachedPartsVulnerableToFrostbite.Add(allParts[i]);
				}
			}
		}
		private void CacheDataRecursive(BodyPartRecord node)
		{
			for (int i = 0; i < node.parts.Count; i++)
			{
				node.parts[i].parent = node;
			}
			node.absoluteCoverage = 1f;
			if (node.parent != null)
			{
				node.absoluteCoverage = node.parent.absoluteCoverage * node.coverage;
			}
			node.fleshCoverage = 1f;
			for (int j = 0; j < node.parts.Count; j++)
			{
				node.fleshCoverage -= node.parts[j].coverage;
			}
			if (node.fleshCoverage < 0f)
			{
				Log.Warning(string.Concat(new string[]
				{
					"BodyPartRecord coverage value is negative (",
					node.fleshCoverage.ToString("F5"),
					") for node ",
					node.def.defName,
					" in BodyDef ",
					this.defName,
					"."
				}));
				node.fleshCoverage = 0f;
			}
			node.absoluteFleshCoverage = node.absoluteCoverage * node.fleshCoverage;
			if (node.height == BodyPartHeight.Inherit)
			{
				node.height = BodyPartHeight.Middle;
			}
			if (node.depth == BodyPartDepth.Inherit)
			{
				node.depth = BodyPartDepth.Outside;
			}
			for (int k = 0; k < node.parts.Count; k++)
			{
				if (node.parts[k].height == BodyPartHeight.Inherit)
				{
					node.parts[k].height = node.height;
				}
				if (node.parts[k].depth == BodyPartDepth.Inherit)
				{
					node.parts[k].depth = node.depth;
				}
			}
			this.cachedAllParts.Add(node);
			for (int l = 0; l < node.parts.Count; l++)
			{
				this.CacheDataRecursive(node.parts[l]);
			}
		}
	}
}
