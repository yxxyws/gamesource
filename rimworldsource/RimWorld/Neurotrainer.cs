using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class Neurotrainer : ThingWithComps, IUsable
	{
		private const float XPGainAmount = 50000f;
		private SkillDef skill;
		public override string LabelBase
		{
			get
			{
				return this.skill.label + " " + this.def.label;
			}
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.LookDef<SkillDef>(ref this.skill, "skill");
		}
		public override void PostMake()
		{
			base.PostMake();
			this.skill = DefDatabase<SkillDef>.GetRandom();
		}
		[DebuggerHidden]
		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
		{
			Neurotrainer.<GetFloatMenuOptions>c__IteratorEA <GetFloatMenuOptions>c__IteratorEA = new Neurotrainer.<GetFloatMenuOptions>c__IteratorEA();
			<GetFloatMenuOptions>c__IteratorEA.myPawn = myPawn;
			<GetFloatMenuOptions>c__IteratorEA.<$>myPawn = myPawn;
			<GetFloatMenuOptions>c__IteratorEA.<>f__this = this;
			Neurotrainer.<GetFloatMenuOptions>c__IteratorEA expr_1C = <GetFloatMenuOptions>c__IteratorEA;
			expr_1C.$PC = -2;
			return expr_1C;
		}
		public void UsedBy(Pawn user)
		{
			int level = user.skills.GetSkill(this.skill).level;
			user.skills.Learn(this.skill, 50000f);
			int level2 = user.skills.GetSkill(this.skill).level;
			if (PawnUtility.ShouldSendNotificationAbout(user))
			{
				Messages.Message("NeurotrainerUsed".Translate(new object[]
				{
					user.LabelBaseShort,
					this.skill.label,
					level,
					level2
				}), user, MessageSound.Benefit);
			}
			this.Destroy(DestroyMode.Vanish);
		}
		public override bool CanStackWith(Thing other)
		{
			if (!base.CanStackWith(other))
			{
				return false;
			}
			Neurotrainer neurotrainer = other as Neurotrainer;
			return neurotrainer != null && neurotrainer.skill == this.skill;
		}
		public override Thing SplitOff(int count)
		{
			Neurotrainer neurotrainer = (Neurotrainer)base.SplitOff(count);
			if (neurotrainer != null)
			{
				neurotrainer.skill = this.skill;
			}
			return neurotrainer;
		}
	}
}
