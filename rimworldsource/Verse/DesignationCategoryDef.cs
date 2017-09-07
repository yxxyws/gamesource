using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Verse
{
	public class DesignationCategoryDef : Def
	{
		public List<Type> specialDesignatorClasses = new List<Type>();
		public int order;
		[Unsaved]
		public List<Designator> resolvedDesignators = new List<Designator>();
		[Unsaved]
		public KeyBindingCategoryDef bindingCatDef;
		public override void ResolveReferences()
		{
			base.ResolveReferences();
			this.resolvedDesignators.Clear();
			foreach (Type current in this.specialDesignatorClasses)
			{
				Designator designator = null;
				try
				{
					designator = (Designator)Activator.CreateInstance(current);
				}
				catch (Exception ex)
				{
					Log.Error(string.Concat(new object[]
					{
						"DesignationCategoryDef",
						this.defName,
						" could not instantiate special designator from class ",
						current,
						".\n Exception: \n",
						ex.ToString()
					}));
				}
				if (designator != null)
				{
					this.resolvedDesignators.Add(designator);
				}
			}
			IEnumerable<BuildableDef> enumerable = 
				from tDef in DefDatabase<ThingDef>.AllDefs.Cast<BuildableDef>().Concat(DefDatabase<TerrainDef>.AllDefs.Cast<BuildableDef>())
				where tDef.designationCategory == this.defName
				select tDef;
			foreach (BuildableDef current2 in enumerable)
			{
				this.resolvedDesignators.Add(new Designator_Build(current2));
			}
		}
	}
}
