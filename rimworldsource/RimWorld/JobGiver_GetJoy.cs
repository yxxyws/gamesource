using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;
namespace RimWorld
{
	public class JobGiver_GetJoy : ThinkNode_JobGiver
	{
		private const int GameStartNoJoyTicks = 5000;
		private DefMap<JoyGiverDef, float> joyGiverChances;
		protected virtual bool JoyGiverAllowed(JoyGiverDef def)
		{
			return true;
		}
		protected virtual Job TryGiveJobFromJoyGiverDefDirect(JoyGiverDef def, Pawn pawn)
		{
			return def.Worker.TryGiveJob(pawn);
		}
		public override float GetPriority(Pawn pawn)
		{
			if (pawn.needs.joy == null)
			{
				return 0f;
			}
			if (Find.TickManager.TicksGame < 5000)
			{
				return 0f;
			}
			Lord lord = pawn.GetLord();
			if (lord != null && !lord.CurLordToil.AllowSatisfyLongNeeds)
			{
				return 0f;
			}
			float curLevel = pawn.needs.joy.CurLevel;
			TimeAssignmentDef timeAssignmentDef = (pawn.timetable != null) ? pawn.timetable.CurrentAssignment : TimeAssignmentDefOf.Anything;
			if (!timeAssignmentDef.allowJoy)
			{
				return 0f;
			}
			if (pawn.InBed() && pawn.health.PrefersMedicalRest)
			{
				return 0f;
			}
			if (timeAssignmentDef == TimeAssignmentDefOf.Anything)
			{
				if (curLevel < 0.35f)
				{
					return 6f;
				}
				return 0f;
			}
			else
			{
				if (timeAssignmentDef == TimeAssignmentDefOf.Joy)
				{
					if (curLevel < 0.95f)
					{
						return 7f;
					}
					return 0f;
				}
				else
				{
					if (timeAssignmentDef != TimeAssignmentDefOf.Sleep)
					{
						throw new NotImplementedException();
					}
					if (curLevel < 0.95f)
					{
						return 2f;
					}
					return 0f;
				}
			}
		}
		public override void ResolveReferences()
		{
			this.joyGiverChances = new DefMap<JoyGiverDef, float>();
		}
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			List<JoyGiverDef> allDefsListForReading = DefDatabase<JoyGiverDef>.AllDefsListForReading;
			JoyToleranceSet tolerances = pawn.needs.joy.tolerances;
			for (int i = 0; i < allDefsListForReading.Count; i++)
			{
				JoyGiverDef joyGiverDef = allDefsListForReading[i];
				this.joyGiverChances[joyGiverDef] = 0f;
				if (this.JoyGiverAllowed(joyGiverDef))
				{
					if (joyGiverDef.Worker.MissingRequiredCapacity(pawn) == null)
					{
						if (joyGiverDef.pctPawnsEverDo < 1f)
						{
							Rand.PushSeed();
							Rand.Seed = (pawn.thingIDNumber ^ 63216713);
							if (Rand.Value >= joyGiverDef.pctPawnsEverDo)
							{
								Rand.PopSeed();
								goto IL_DD;
							}
							Rand.PopSeed();
						}
						float num = joyGiverDef.Worker.GetChance(pawn);
						float num2 = 1f - tolerances[joyGiverDef.joyKind];
						num *= num2 * num2;
						this.joyGiverChances[joyGiverDef] = num;
					}
				}
				IL_DD:;
			}
			for (int j = 0; j < this.joyGiverChances.Count; j++)
			{
				JoyGiverDef def;
				if (!allDefsListForReading.TryRandomElementByWeight((JoyGiverDef d) => this.joyGiverChances[d], out def))
				{
					break;
				}
				Job job = this.TryGiveJobFromJoyGiverDefDirect(def, pawn);
				if (job != null)
				{
					return job;
				}
				this.joyGiverChances[def] = 0f;
			}
			return null;
		}
	}
}
