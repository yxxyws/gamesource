using System;
using Verse;
namespace RimWorld
{
	public static class KeyBindingCategoryDefOf
	{
		public static KeyBindingCategoryDef MainTabs;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<KeyBindingCategoryDef>(typeof(KeyBindingCategoryDefOf));
		}
	}
}
