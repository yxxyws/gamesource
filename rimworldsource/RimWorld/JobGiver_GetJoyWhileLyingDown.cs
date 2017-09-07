using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobGiver_GetJoyWhileLyingDown : JobGiver_GetJoy
	{
		private const float MaxJoyLevel = 0.5f;
		protected override bool JoyGiverAllowed(JoyGiverDef def)
		{
			return def.canDoWhileInBed;
		}
		protected override Job TryGiveJobFromJoyGiverDefDirect(JoyGiverDef def, Pawn pawn)
		{
			return def.Worker.TryGiveJobWhileInBed(pawn);
		}
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			if (pawn.CurJob == null || !pawn.jobs.curDriver.layingDown || pawn.jobs.curDriver.layingDownBed == null || !pawn.Awake() || pawn.needs.joy == null)
			{
				return null;
			}
			float curLevel = pawn.needs.joy.CurLevel;
			if (curLevel > 0.5f)
			{
				return null;
			}
			return base.TryGiveTerminalJob(pawn);
		}
	}
}
