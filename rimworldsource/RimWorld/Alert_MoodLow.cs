using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
namespace RimWorld
{
	public class Alert_MoodLow : Alert_Critical
	{
		private IEnumerable<Pawn> PoorMooders
		{
			get
			{
				Alert_MoodLow.<>c__Iterator105 <>c__Iterator = new Alert_MoodLow.<>c__Iterator105();
				Alert_MoodLow.<>c__Iterator105 expr_07 = <>c__Iterator;
				expr_07.$PC = -2;
				return expr_07;
			}
		}
		private IEnumerable<Pawn> TerribleMooders
		{
			get
			{
				Alert_MoodLow.<>c__Iterator106 <>c__Iterator = new Alert_MoodLow.<>c__Iterator106();
				Alert_MoodLow.<>c__Iterator106 expr_07 = <>c__Iterator;
				expr_07.$PC = -2;
				return expr_07;
			}
		}
		public override string FullLabel
		{
			get
			{
				int num = this.TerribleMooders.Count<Pawn>();
				int num2 = this.PoorMooders.Count<Pawn>();
				string text;
				if (num > 0)
				{
					text = "TerribleMood".Translate();
				}
				else
				{
					text = "PoorMood".Translate();
				}
				if (num + num2 > 1)
				{
					text = text + " x" + (num + num2).ToStringCached();
				}
				return text;
			}
		}
		public override string FullExplanation
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (this.TerribleMooders.Any<Pawn>())
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					foreach (Pawn current in this.TerribleMooders)
					{
						stringBuilder2.AppendLine("    " + current.NameStringShort);
					}
					stringBuilder.Append("TerribleMoodDesc".Translate(new object[]
					{
						stringBuilder2.ToString()
					}));
				}
				if (this.PoorMooders.Any<Pawn>())
				{
					if (this.TerribleMooders.Any<Pawn>())
					{
						stringBuilder.AppendLine();
					}
					StringBuilder stringBuilder3 = new StringBuilder();
					foreach (Pawn current2 in this.PoorMooders)
					{
						stringBuilder3.AppendLine("    " + current2.NameStringShort);
					}
					stringBuilder.Append("PoorMoodDesc".Translate(new object[]
					{
						stringBuilder3.ToString()
					}));
				}
				stringBuilder.AppendLine();
				stringBuilder.Append("LowMoodDescEnding".Translate());
				return stringBuilder.ToString();
			}
		}
		public override AlertReport Report
		{
			get
			{
				Pawn pawn = this.TerribleMooders.FirstOrDefault<Pawn>();
				if (pawn != null)
				{
					return AlertReport.CulpritIs(pawn);
				}
				pawn = this.PoorMooders.FirstOrDefault<Pawn>();
				if (pawn != null)
				{
					return AlertReport.CulpritIs(pawn);
				}
				return AlertReport.Inactive;
			}
		}
	}
}
