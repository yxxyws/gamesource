using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
namespace RimWorld
{
	public class Alert_ColonistNeedsTend : Alert_Critical
	{
		private IEnumerable<Pawn> NeedingColonists
		{
			get
			{
				Alert_ColonistNeedsTend.<>c__Iterator109 <>c__Iterator = new Alert_ColonistNeedsTend.<>c__Iterator109();
				Alert_ColonistNeedsTend.<>c__Iterator109 expr_07 = <>c__Iterator;
				expr_07.$PC = -2;
				return expr_07;
			}
		}
		public override string FullExplanation
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (Pawn current in this.NeedingColonists)
				{
					stringBuilder.AppendLine("    " + current.NameStringShort);
				}
				return string.Format("ColonistNeedsTreatmentDesc".Translate(), stringBuilder.ToString());
			}
		}
		public override AlertReport Report
		{
			get
			{
				Pawn pawn = this.NeedingColonists.FirstOrDefault<Pawn>();
				if (pawn == null)
				{
					return false;
				}
				return AlertReport.CulpritIs(pawn);
			}
		}
		public Alert_ColonistNeedsTend()
		{
			this.baseLabel = "ColonistNeedsTreatment".Translate();
		}
	}
}
