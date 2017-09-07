using System;
using Verse;
namespace RimWorld
{
	public abstract class Thought : IExposable
	{
		public Pawn pawn;
		public ThoughtDef def;
		public abstract int CurStageIndex
		{
			get;
		}
		public ThoughtStage CurStage
		{
			get
			{
				return this.def.stages[this.CurStageIndex];
			}
		}
		public abstract bool ShouldDiscard
		{
			get;
		}
		public virtual bool VisibleInNeedsTab
		{
			get
			{
				return !this.ShouldDiscard && this.CurStage.visible;
			}
		}
		public string LabelCap
		{
			get
			{
				string result;
				if (this.def.Worker != null)
				{
					result = this.def.Worker.GetLabel(this.pawn, this.CurStage);
				}
				else
				{
					result = this.CurStage.label.CapitalizeFirst();
				}
				return result;
			}
		}
		public string LabelCapSocial
		{
			get
			{
				if (this.CurStage.labelSocial != null)
				{
					return this.CurStage.labelSocial.CapitalizeFirst();
				}
				return this.LabelCap;
			}
		}
		public string Description
		{
			get
			{
				string description = this.CurStage.description;
				if (description != null)
				{
					return description;
				}
				return this.def.description;
			}
		}
		public virtual void ExposeData()
		{
			Scribe_Defs.LookDef<ThoughtDef>(ref this.def, "def");
		}
		public virtual float MoodOffset()
		{
			if (this.ShouldDiscard)
			{
				return 0f;
			}
			if (this.CurStage == null)
			{
				Log.Error(string.Concat(new object[]
				{
					"CurStage is null while ShouldDiscard is false on ",
					this.def.defName,
					" for ",
					this.pawn
				}));
				return 0f;
			}
			float num;
			if (this.def.Worker != null)
			{
				num = this.def.Worker.GetMoodOffset(this.pawn, this.CurStage);
			}
			else
			{
				num = this.CurStage.baseMoodEffect;
			}
			if (this.def.effectMultiplyingStat != null)
			{
				num *= this.pawn.GetStatValue(this.def.effectMultiplyingStat, true);
			}
			return num;
		}
		public virtual bool TryMergeWithExistingThought()
		{
			ThoughtHandler thoughts = this.pawn.needs.mood.thoughts;
			if (thoughts.NumThoughtsOfDef(this.def) >= this.def.stackLimit)
			{
				Thought_Memory thought_Memory = thoughts.OldestMemoryOfDef(this.def);
				if (thought_Memory != null)
				{
					thought_Memory.Renew();
					return true;
				}
			}
			return false;
		}
		public abstract void ThoughtInterval();
		public virtual void PreAdd()
		{
		}
		public virtual void Init()
		{
		}
		public override string ToString()
		{
			return "(" + this.def.defName + ")";
		}
	}
}
