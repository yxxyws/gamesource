using System;
namespace Verse
{
	public abstract class IngredientValueGetter
	{
		public abstract float ValuePerUnitOf(ThingDef t);
		public abstract string BillRequirementsDescription(IngredientCount ing);
		public virtual string ExtraDescriptionLine()
		{
			return null;
		}
	}
}
