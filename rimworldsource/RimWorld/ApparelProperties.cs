using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Verse;
namespace RimWorld
{
	public class ApparelProperties
	{
		public List<BodyPartGroupDef> bodyPartGroups = new List<BodyPartGroupDef>();
		public List<ApparelLayer> layers = new List<ApparelLayer>();
		public string wornGraphicPath = string.Empty;
		public float commonality = 100f;
		public List<string> tags = new List<string>();
		public List<string> defaultOutfitTags;
		public float wearPerDay = 0.4f;
		public ApparelLayer LastLayer
		{
			get
			{
				return this.layers[this.layers.Count - 1];
			}
		}
		[DebuggerHidden]
		public IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			ApparelProperties.<ConfigErrors>c__Iterator62 <ConfigErrors>c__Iterator = new ApparelProperties.<ConfigErrors>c__Iterator62();
			<ConfigErrors>c__Iterator.parentDef = parentDef;
			<ConfigErrors>c__Iterator.<$>parentDef = parentDef;
			<ConfigErrors>c__Iterator.<>f__this = this;
			ApparelProperties.<ConfigErrors>c__Iterator62 expr_1C = <ConfigErrors>c__Iterator;
			expr_1C.$PC = -2;
			return expr_1C;
		}
		public bool CoversBodyPart(BodyPartRecord partRec)
		{
			for (int i = 0; i < partRec.groups.Count; i++)
			{
				for (int j = 0; j < this.bodyPartGroups.Count; j++)
				{
					if (partRec.groups[i] == this.bodyPartGroups[j])
					{
						return true;
					}
				}
			}
			return false;
		}
		public string GetCoveredOuterPartsString(BodyDef body)
		{
			IEnumerable<BodyPartRecord> source = 
				from x in body.AllParts
				where x.depth == BodyPartDepth.Outside && x.groups.Any((BodyPartGroupDef y) => this.bodyPartGroups.Contains(y))
				select x;
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (BodyPartRecord current in source.Distinct<BodyPartRecord>())
			{
				if (!flag)
				{
					stringBuilder.Append(", ");
				}
				flag = false;
				stringBuilder.Append(current.def.label);
			}
			return stringBuilder.ToString().CapitalizeFirst();
		}
	}
}
