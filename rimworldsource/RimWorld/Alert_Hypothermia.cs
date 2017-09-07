using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
namespace RimWorld
{
	public class Alert_Hypothermia : Alert_Critical
	{
		private IEnumerable<Pawn> HypothermiaDangerColonists
		{
			get
			{
				Alert_Hypothermia.<>c__Iterator107 <>c__Iterator = new Alert_Hypothermia.<>c__Iterator107();
				Alert_Hypothermia.<>c__Iterator107 expr_07 = <>c__Iterator;
				expr_07.$PC = -2;
				return expr_07;
			}
		}
		public override string FullExplanation
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (Pawn current in this.HypothermiaDangerColonists)
				{
					stringBuilder.AppendLine("    " + current.NameStringShort);
				}
				return "AlertHypothermiaDesc".Translate(new object[]
				{
					stringBuilder.ToString()
				});
			}
		}
		public override AlertReport Report
		{
			get
			{
				Pawn pawn = this.HypothermiaDangerColonists.FirstOrDefault<Pawn>();
				if (pawn == null)
				{
					return false;
				}
				return AlertReport.CulpritIs(pawn);
			}
		}
		public Alert_Hypothermia()
		{
			this.baseLabel = "AlertHypothermia".Translate();
		}
	}
}
