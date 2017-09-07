using System;
using System.Collections.Generic;
using System.Text;
using Verse;
namespace RimWorld
{
	public class Trait : IExposable
	{
		public TraitDef def;
		public int degree;
		public TraitDegreeData CurrentData
		{
			get
			{
				return this.def.DataAtDegree(this.degree);
			}
		}
		public string Label
		{
			get
			{
				return this.CurrentData.label;
			}
		}
		public string LabelCap
		{
			get
			{
				return this.Label.CapitalizeFirst();
			}
		}
		public Trait()
		{
		}
		public Trait(TraitDef def)
		{
			this.def = def;
		}
		public string TipString(Pawn pawn)
		{
			StringBuilder stringBuilder = new StringBuilder();
			TraitDegreeData currentData = this.CurrentData;
			stringBuilder.Append(currentData.description.AdjustedFor(pawn));
			int count = this.CurrentData.skillGains.Count;
			if (count > 0)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
			}
			int num = 0;
			foreach (KeyValuePair<SkillDef, int> current in this.CurrentData.skillGains)
			{
				if (current.Value != 0)
				{
					string value = "    " + current.Key.skillLabel + ":   " + current.Value.ToString("+##;-##");
					if (num < count - 1)
					{
						stringBuilder.AppendLine(value);
					}
					else
					{
						stringBuilder.Append(value);
					}
					num++;
				}
			}
			if (currentData.permaThought != null)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				Thought thought = ThoughtMaker.MakeThought(currentData.permaThought);
				stringBuilder.Append("    " + "PermanentMoodEffect".Translate() + " " + thought.def.stages[0].baseMoodEffect.ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Offset));
			}
			if (currentData.statOffsets != null)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				for (int i = 0; i < currentData.statOffsets.Count; i++)
				{
					StatModifier statModifier = currentData.statOffsets[i];
					string toStringAsOffset = statModifier.ToStringAsOffset;
					string value2 = "    " + statModifier.stat.LabelCap + " " + toStringAsOffset;
					if (i < currentData.statOffsets.Count - 1)
					{
						stringBuilder.AppendLine(value2);
					}
					else
					{
						stringBuilder.Append(value2);
					}
				}
			}
			if (currentData.statFactors != null)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				for (int j = 0; j < currentData.statFactors.Count; j++)
				{
					StatModifier statModifier2 = currentData.statFactors[j];
					string toStringAsFactor = statModifier2.ToStringAsFactor;
					string value3 = "    " + statModifier2.stat.LabelCap + " " + toStringAsFactor;
					if (j < currentData.statFactors.Count - 1)
					{
						stringBuilder.AppendLine(value3);
					}
					else
					{
						stringBuilder.Append(value3);
					}
				}
			}
			return stringBuilder.ToString();
		}
		public void ExposeData()
		{
			Scribe_Defs.LookDef<TraitDef>(ref this.def, "def");
			Scribe_Values.LookValue<int>(ref this.degree, "degree", 0, false);
			if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs && this.def == null)
			{
				this.def = DefDatabase<TraitDef>.GetRandom();
				this.degree = PawnGenerator.RandomTraitDegree(this.def);
			}
		}
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Trait(",
				this.def.ToString(),
				"-",
				this.degree,
				")"
			});
		}
		public float OffsetOfStat(StatDef stat)
		{
			float num = 0f;
			TraitDegreeData currentData = this.CurrentData;
			if (currentData.statOffsets != null)
			{
				for (int i = 0; i < currentData.statOffsets.Count; i++)
				{
					if (currentData.statOffsets[i].stat == stat)
					{
						num += currentData.statOffsets[i].value;
					}
				}
			}
			if (currentData.statFactors != null)
			{
				for (int j = 0; j < currentData.statFactors.Count; j++)
				{
					if (currentData.statFactors[j].stat == stat)
					{
						num *= currentData.statFactors[j].value;
					}
				}
			}
			return num;
		}
	}
}
