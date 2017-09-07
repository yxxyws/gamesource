using RimWorld;
using System;
namespace Verse
{
	public static class CompPropertiesGenerator
	{
		public static CompProperties Forbiddable()
		{
			return new CompProperties
			{
				compClass = typeof(CompForbiddable)
			};
		}
	}
}
