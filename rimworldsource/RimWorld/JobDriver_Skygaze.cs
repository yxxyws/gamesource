using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_Skygaze : JobDriver
	{
		private Toil gaze;
		public override PawnPosture Posture
		{
			get
			{
				return (base.CurToil != this.gaze) ? PawnPosture.Standing : PawnPosture.LayingFaceUp;
			}
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_Skygaze.<MakeNewToils>c__Iterator23 <MakeNewToils>c__Iterator = new JobDriver_Skygaze.<MakeNewToils>c__Iterator23();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_Skygaze.<MakeNewToils>c__Iterator23 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override string GetReport()
		{
			if (Find.MapConditionManager.ConditionIsActive(MapConditionDefOf.Eclipse))
			{
				return "WatchingEclipse".Translate();
			}
			float num = GenCelestial.CurCelestialSunGlow();
			if (num < 0.1f)
			{
				return "Stargazing".Translate();
			}
			if (num >= 0.65f)
			{
				return "CloudWatching".Translate();
			}
			if (GenDate.CurrentDayPercent < 0.5f)
			{
				return "WatchingSunrise".Translate();
			}
			return "WatchingSunset".Translate();
		}
	}
}
