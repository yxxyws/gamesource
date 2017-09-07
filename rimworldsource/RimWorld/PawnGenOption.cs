using System;
using Verse;
namespace RimWorld
{
	public class PawnGenOption
	{
		public PawnKindDef kind;
		public int selectionWeight;
		public float Cost
		{
			get
			{
				return this.kind.combatPower;
			}
		}
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"(",
				this.kind.ToString(),
				" w=",
				this.selectionWeight.ToString("F2"),
				" c=",
				this.Cost.ToString("F2"),
				")"
			});
		}
	}
}
