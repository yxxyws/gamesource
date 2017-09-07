using System;
using Verse;
namespace RimWorld
{
	public class IncidentWorker
	{
		public IncidentDef def;
		public virtual float AdjustedChance
		{
			get
			{
				return this.def.baseChance;
			}
		}
		public bool StorytellerCanUseNow()
		{
			return GenDate.DaysPassed >= this.def.earliestDay && (this.def.allowedBiomes == null || this.def.allowedBiomes.Contains(Find.Map.Biome)) && this.StorytellerCanUseNowSub();
		}
		protected virtual bool StorytellerCanUseNowSub()
		{
			return true;
		}
		public virtual bool TryExecute(IncidentParms parms)
		{
			Log.Error("Unimplemented incident " + this);
			return false;
		}
		protected void SendStandardLetter()
		{
			if (this.def.letterLabel.NullOrEmpty() || this.def.letterText.NullOrEmpty())
			{
				Log.Error("Sending standard incident letter with no label or text.");
			}
			Find.LetterStack.ReceiveLetter(this.def.letterLabel, this.def.letterText, this.def.letterType, null);
		}
		protected void SendStandardLetter(TargetInfo target)
		{
			if (this.def.letterLabel.NullOrEmpty() || this.def.letterText.NullOrEmpty())
			{
				Log.Error("Sending standard incident letter with no label or text.");
			}
			Find.LetterStack.ReceiveLetter(this.def.letterLabel, this.def.letterText, this.def.letterType, target, null);
		}
	}
}
