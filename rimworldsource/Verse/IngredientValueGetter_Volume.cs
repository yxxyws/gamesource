using System;
namespace Verse
{
	public class IngredientValueGetter_Volume : IngredientValueGetter
	{
		public override float ValuePerUnitOf(ThingDef t)
		{
			if (t.IsStuff)
			{
				return t.VolumePerUnit;
			}
			return 1f;
		}
		public override string BillRequirementsDescription(IngredientCount ing)
		{
			return "BillRequires".Translate(new object[]
			{
				ing.GetBaseCount(),
				ing.filter.Summary
			});
		}
		public override string ExtraDescriptionLine()
		{
			return "BillRequiresMayVary".Translate();
		}
	}
}
