using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
namespace RimWorld
{
	public class Alert_ColonistNeedsRescuing : Alert_Critical
	{
		private IEnumerable<Pawn> ColonistsNeedingRescue
		{
			get
			{
				Alert_ColonistNeedsRescuing.<>c__Iterator108 <>c__Iterator = new Alert_ColonistNeedsRescuing.<>c__Iterator108();
				Alert_ColonistNeedsRescuing.<>c__Iterator108 expr_07 = <>c__Iterator;
				expr_07.$PC = -2;
				return expr_07;
			}
		}
		public override string FullLabel
		{
			get
			{
				if (this.ColonistsNeedingRescue.Count<Pawn>() == 1)
				{
					return "ColonistNeedsRescue".Translate();
				}
				return "ColonistsNeedRescue".Translate();
			}
		}
		public override string FullExplanation
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (Pawn current in this.ColonistsNeedingRescue)
				{
					stringBuilder.AppendLine("    " + current.NameStringShort);
				}
				return string.Format("ColonistsNeedRescueDesc".Translate(), stringBuilder.ToString());
			}
		}
		public override AlertReport Report
		{
			get
			{
				return AlertReport.CulpritIs(this.ColonistsNeedingRescue.FirstOrDefault<Pawn>());
			}
		}
		public static bool NeedsRescue(Pawn p)
		{
			return p.Downed && !p.InBed() && p.holder == null && (p.jobs.jobQueue == null || p.jobs.jobQueue.Count <= 0 || !p.jobs.jobQueue.Peek().CanBeginNow(p));
		}
	}
}
