using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class SkillNeed_BaseBonus : SkillNeed
	{
		private float baseFactor = 0.5f;
		private float bonusFactor = 0.05f;
		public override float FactorFor(Pawn pawn)
		{
			if (pawn.skills == null)
			{
				return 1f;
			}
			int level = pawn.skills.GetSkill(this.skill).level;
			return this.FactorAt(level);
		}
		private float FactorAt(int level)
		{
			return this.baseFactor + this.bonusFactor * (float)level;
		}
		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			SkillNeed_BaseBonus.<ConfigErrors>c__Iterator6B <ConfigErrors>c__Iterator6B = new SkillNeed_BaseBonus.<ConfigErrors>c__Iterator6B();
			<ConfigErrors>c__Iterator6B.<>f__this = this;
			SkillNeed_BaseBonus.<ConfigErrors>c__Iterator6B expr_0E = <ConfigErrors>c__Iterator6B;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
