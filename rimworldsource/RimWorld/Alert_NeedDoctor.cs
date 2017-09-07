using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
namespace RimWorld
{
	public class Alert_NeedDoctor : Alert_High
	{
		private IEnumerable<Pawn> Patients
		{
			get
			{
				Alert_NeedDoctor.<>c__Iterator10B <>c__Iterator10B = new Alert_NeedDoctor.<>c__Iterator10B();
				Alert_NeedDoctor.<>c__Iterator10B expr_07 = <>c__Iterator10B;
				expr_07.$PC = -2;
				return expr_07;
			}
		}
		public override string FullExplanation
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (Pawn current in this.Patients)
				{
					stringBuilder.AppendLine("    " + current.NameStringShort);
				}
				return string.Format("NeedDoctorDesc".Translate(), stringBuilder.ToString());
			}
		}
		public override AlertReport Report
		{
			get
			{
				foreach (Pawn current in Find.MapPawns.FreeColonistsSpawned)
				{
					if (!current.Downed && current.workSettings != null && current.workSettings.WorkIsActive(WorkTypeDefOf.Doctor))
					{
						AlertReport inactive = AlertReport.Inactive;
						return inactive;
					}
				}
				Pawn pawn = this.Patients.FirstOrDefault<Pawn>();
				if (pawn == null)
				{
					return false;
				}
				return AlertReport.CulpritIs(pawn);
			}
		}
		public Alert_NeedDoctor()
		{
			this.baseLabel = "NeedDoctor".Translate();
		}
	}
}
