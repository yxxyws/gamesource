using System;
using System.Collections.Generic;
using UnityEngine;
namespace Verse
{
	public class ThingCategoryDef : Def
	{
		public ThingCategoryDef parent;
		public string iconPath;
		public bool resourceReadoutRoot;
		[Unsaved]
		public TreeNode_ThingCategory treeNode;
		[Unsaved]
		public List<ThingCategoryDef> childCategories = new List<ThingCategoryDef>();
		[Unsaved]
		public List<ThingDef> childThingDefs = new List<ThingDef>();
		[Unsaved]
		public List<SpecialThingFilterDef> childSpecialFilters = new List<SpecialThingFilterDef>();
		[Unsaved]
		public Texture2D icon = BaseContent.BadTex;
		public IEnumerable<ThingCategoryDef> Parents
		{
			get
			{
				ThingCategoryDef.<>c__Iterator14B <>c__Iterator14B = new ThingCategoryDef.<>c__Iterator14B();
				<>c__Iterator14B.<>f__this = this;
				ThingCategoryDef.<>c__Iterator14B expr_0E = <>c__Iterator14B;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public IEnumerable<ThingCategoryDef> ThisAndChildCategoryDefs
		{
			get
			{
				ThingCategoryDef.<>c__Iterator14C <>c__Iterator14C = new ThingCategoryDef.<>c__Iterator14C();
				<>c__Iterator14C.<>f__this = this;
				ThingCategoryDef.<>c__Iterator14C expr_0E = <>c__Iterator14C;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public IEnumerable<ThingDef> DescendantThingDefs
		{
			get
			{
				ThingCategoryDef.<>c__Iterator14D <>c__Iterator14D = new ThingCategoryDef.<>c__Iterator14D();
				<>c__Iterator14D.<>f__this = this;
				ThingCategoryDef.<>c__Iterator14D expr_0E = <>c__Iterator14D;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public IEnumerable<SpecialThingFilterDef> DescendantSpecialThingFilterDefs
		{
			get
			{
				ThingCategoryDef.<>c__Iterator14E <>c__Iterator14E = new ThingCategoryDef.<>c__Iterator14E();
				<>c__Iterator14E.<>f__this = this;
				ThingCategoryDef.<>c__Iterator14E expr_0E = <>c__Iterator14E;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public IEnumerable<SpecialThingFilterDef> ParentsSpecialThingFilterDefs
		{
			get
			{
				ThingCategoryDef.<>c__Iterator14F <>c__Iterator14F = new ThingCategoryDef.<>c__Iterator14F();
				<>c__Iterator14F.<>f__this = this;
				ThingCategoryDef.<>c__Iterator14F expr_0E = <>c__Iterator14F;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public override void PostLoad()
		{
			this.treeNode = new TreeNode_ThingCategory(this);
			if (!this.iconPath.NullOrEmpty())
			{
				LongEventHandler.ExecuteWhenFinished(delegate
				{
					this.icon = ContentFinder<Texture2D>.Get(this.iconPath, true);
				});
			}
		}
		public static ThingCategoryDef Named(string defName)
		{
			return DefDatabase<ThingCategoryDef>.GetNamed(defName, true);
		}
		public override int GetHashCode()
		{
			return this.defName.GetHashCode();
		}
	}
}
