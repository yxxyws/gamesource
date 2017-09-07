using System;
namespace RimWorld
{
	public static class JoyKindDefOf
	{
		public static JoyKindDef Work;
		public static JoyKindDef Gluttonous;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<JoyKindDef>(typeof(JoyKindDefOf));
		}
	}
}
