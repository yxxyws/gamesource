using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse
{
	public class SpecialThingFilterDef : Def
	{
		public ThingCategoryDef parentCategory;
		public string saveKey;
		public bool allowedByDefault;
		public bool configurable = true;
		public Type workerClass;
		[Unsaved]
		private SpecialThingFilterWorker workerInt;
		public SpecialThingFilterWorker Worker
		{
			get
			{
				if (this.workerInt == null)
				{
					this.workerInt = (SpecialThingFilterWorker)Activator.CreateInstance(this.workerClass);
				}
				return this.workerInt;
			}
		}
		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			SpecialThingFilterDef.<ConfigErrors>c__Iterator1B1 <ConfigErrors>c__Iterator1B = new SpecialThingFilterDef.<ConfigErrors>c__Iterator1B1();
			<ConfigErrors>c__Iterator1B.<>f__this = this;
			SpecialThingFilterDef.<ConfigErrors>c__Iterator1B1 expr_0E = <ConfigErrors>c__Iterator1B;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public static SpecialThingFilterDef Named(string defName)
		{
			return DefDatabase<SpecialThingFilterDef>.GetNamed(defName, true);
		}
	}
}
