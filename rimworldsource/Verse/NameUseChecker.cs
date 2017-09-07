using System;
using System.Collections.Generic;
namespace Verse
{
	public static class NameUseChecker
	{
		public static IEnumerable<NameTriple> AllPawnsNamesEverUsed
		{
			get
			{
				NameUseChecker.<>c__Iterator163 <>c__Iterator = new NameUseChecker.<>c__Iterator163();
				NameUseChecker.<>c__Iterator163 expr_07 = <>c__Iterator;
				expr_07.$PC = -2;
				return expr_07;
			}
		}
		public static bool ElementIsUsed(string singleName)
		{
			foreach (NameTriple current in NameUseChecker.AllPawnsNamesEverUsed)
			{
				if (singleName == current.First || singleName == current.Nick || singleName == current.Last)
				{
					return true;
				}
			}
			return false;
		}
		public static bool NameSingleIsUsedOnMap(string candidate)
		{
			foreach (Pawn current in Find.MapPawns.AllPawns)
			{
				NameSingle nameSingle = current.Name as NameSingle;
				if (nameSingle != null && nameSingle.Name == candidate)
				{
					return true;
				}
			}
			return false;
		}
	}
}
