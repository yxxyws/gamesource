using RimWorld;
using System;
namespace Verse
{
	public class HediffComp_Discoverable : HediffComp
	{
		private bool discovered;
		public override string CompDebugString
		{
			get
			{
				return "discovered: " + this.discovered;
			}
		}
		public override void CompExposeData()
		{
			Scribe_Values.LookValue<bool>(ref this.discovered, "discovered", false, false);
		}
		public override bool CompDisallowVisible()
		{
			return !this.discovered;
		}
		public override void CompPostTick()
		{
			if (Find.TickManager.TicksGame % 103 == 0)
			{
				this.CheckDiscovered();
			}
		}
		public override void CompPostPostAdd()
		{
			this.CheckDiscovered();
		}
		private void CheckDiscovered()
		{
			if (this.discovered)
			{
				return;
			}
			if (!this.parent.CurStage.everVisible)
			{
				return;
			}
			this.discovered = true;
			if (this.props.sendLetterWhenDiscovered && PawnUtility.ShouldSendNotificationAbout(base.Pawn))
			{
				string label;
				if (!this.props.discoverLetterLabel.NullOrEmpty())
				{
					label = string.Format(this.props.discoverLetterLabel, base.Pawn.LabelBaseShort).CapitalizeFirst();
				}
				else
				{
					label = "LetterLabelNewDisease".Translate() + " (" + base.Def.label + ")";
				}
				string text;
				if (!this.props.discoverLetterText.NullOrEmpty())
				{
					text = string.Format(this.props.discoverLetterText, base.Pawn.LabelDefinite()).CapitalizeFirst();
				}
				else
				{
					if (this.parent.Part == null)
					{
						text = "NewDisease".Translate(new object[]
						{
							base.Pawn.LabelIndefinite(),
							base.Def.label,
							base.Pawn.LabelDefinite()
						}).AdjustedFor(base.Pawn).CapitalizeFirst();
					}
					else
					{
						text = "NewPartDisease".Translate(new object[]
						{
							base.Pawn.LabelIndefinite(),
							this.parent.Part.def.label,
							base.Pawn.LabelDefinite(),
							base.Def.LabelCap
						}).AdjustedFor(base.Pawn).CapitalizeFirst();
					}
				}
				if (base.Pawn.RaceProps.Humanlike)
				{
					Find.LetterStack.ReceiveLetter(label, text, LetterType.BadNonUrgent, base.Pawn, null);
				}
				else
				{
					Messages.Message(text, base.Pawn, MessageSound.Standard);
				}
			}
		}
	}
}
