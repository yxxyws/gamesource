using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Verse
{
	public class MentalStateStarter : IExposable
	{
		private const int CheckInterval = 150;
		private const float StartHardMentalStateMTBDays = 0.7f;
		private const float StartSoftMentalStateMTBDays = 3.5f;
		private const int MinTicksBelowToStartMentalState = 2000;
		private const float SoftMentalStateMoodSpan = 0.15f;
		private Pawn pawn;
		private int ticksBelowHard;
		private int ticksBelowSoft;
		public float StartHardMentalStateThreshold
		{
			get
			{
				return this.pawn.GetStatValue(StatDefOf.MentalBreakThreshold, true);
			}
		}
		public float StartSoftMentalStateThreshold
		{
			get
			{
				return this.pawn.GetStatValue(StatDefOf.MentalBreakThreshold, true) + 0.15f;
			}
		}
		public bool HardMentalStateImminent
		{
			get
			{
				return this.pawn.MentalStateDef == null && this.CurMood < this.StartHardMentalStateThreshold;
			}
		}
		public bool MentalStateApproaching
		{
			get
			{
				return this.pawn.MentalStateDef == null && !this.HardMentalStateImminent && this.CurMood < this.StartHardMentalStateThreshold + 0.1f;
			}
		}
		private float CurMood
		{
			get
			{
				if (this.pawn.needs.mood == null)
				{
					return 0.5f;
				}
				return this.pawn.needs.mood.CurLevel;
			}
		}
		public MentalStateStarter()
		{
		}
		public MentalStateStarter(Pawn pawn)
		{
			this.pawn = pawn;
		}
		public void ExposeData()
		{
			Scribe_Values.LookValue<int>(ref this.ticksBelowHard, "ticksBelowHard", 0, false);
			Scribe_Values.LookValue<int>(ref this.ticksBelowSoft, "ticksBelowSoft", 0, false);
		}
		public void MentalStateStarterTick()
		{
			if (this.pawn.IsHashIntervalTick(150) && this.pawn.MentalStateDef == null)
			{
				if (!DebugSettings.enableRandomMentalStates || !this.pawn.RaceProps.Humanlike)
				{
					return;
				}
				if (this.CurMood < this.StartHardMentalStateThreshold)
				{
					this.ticksBelowHard += 150;
				}
				else
				{
					this.ticksBelowHard = 0;
				}
				if (this.CurMood < this.StartSoftMentalStateThreshold)
				{
					this.ticksBelowSoft += 150;
				}
				else
				{
					this.ticksBelowSoft = 0;
				}
				if (this.pawn.mindState.mentalStateHandler.CurState == null)
				{
					if (this.IntervalStartMentalStateTest())
					{
						this.TryStartMentalState(null);
					}
					if (this.pawn.story != null)
					{
						List<Trait> allTraits = this.pawn.story.traits.allTraits;
						for (int i = 0; i < allTraits.Count; i++)
						{
							TraitDegreeData currentData = allTraits[i].CurrentData;
							if (currentData.randomMentalState != null)
							{
								float mtb = currentData.randomMentalStateMtbDaysMoodCurve.Evaluate(this.CurMood);
								if (Rand.MTBEventOccurs(mtb, 60000f, 150f) && currentData.randomMentalState.Worker.StateCanOccur(this.pawn))
								{
									this.pawn.mindState.mentalStateHandler.StartMentalState(currentData.randomMentalState);
								}
							}
						}
					}
				}
			}
		}
		public bool TryStartMentalState(MentalStateDef stateDef = null)
		{
			if (this.pawn.Downed || !this.pawn.Awake())
			{
				return false;
			}
			if (stateDef == null && !DefDatabase<MentalStateDef>.AllDefsListForReading.TryRandomElementByWeight((MentalStateDef d) => this.MentalStateDefSelectionProbability(d), out stateDef))
			{
				return false;
			}
			this.pawn.mindState.mentalStateHandler.StartMentalState(stateDef);
			return true;
		}
		internal void Reset()
		{
			this.ticksBelowHard = 0;
			this.ticksBelowSoft = 0;
		}
		private bool IntervalStartMentalStateTest()
		{
			if (this.ticksBelowSoft <= 2000)
			{
				return false;
			}
			if (this.ticksBelowHard > 2000)
			{
				return Rand.MTBEventOccurs(0.7f, 60000f, 150f);
			}
			return Rand.MTBEventOccurs(3.5f, 60000f, 150f);
		}
		private float MentalStateDefSelectionProbability(MentalStateDef def)
		{
			if (def.stateType == MentalStateType.Special)
			{
				return 0f;
			}
			if (def.stateType == MentalStateType.Hard && this.ticksBelowHard < 2000)
			{
				return 0f;
			}
			if (def.stateType == MentalStateType.Soft && this.ticksBelowSoft < 2000)
			{
				return 0f;
			}
			if (!def.prisonersCanDo && this.pawn.HostFaction != null)
			{
				return 0f;
			}
			if (!def.Worker.StateCanOccur(this.pawn))
			{
				return 0f;
			}
			if (this.pawn.story.traits.allTraits.Any((Trait tr) => tr.CurrentData.disallowedMentalStates != null && tr.CurrentData.disallowedMentalStates.Contains(def)))
			{
				return 0f;
			}
			float num = this.StartMentalStateThresholdFor(def.stateType);
			float a;
			if (this.CurMood > num)
			{
				a = def.commonality * (0.06f - Mathf.Abs(num - this.CurMood));
			}
			else
			{
				a = def.commonality * (0.5f - Mathf.Abs(num - this.CurMood));
			}
			return Mathf.Max(a, 0.001f);
		}
		public float StartMentalStateThresholdFor(MentalStateType thresh)
		{
			if (thresh == MentalStateType.Hard)
			{
				return this.StartHardMentalStateThreshold;
			}
			return this.StartSoftMentalStateThreshold;
		}
		internal string DebugString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Concat(new object[]
			{
				this.pawn,
				" ticksBelowSoft=",
				this.ticksBelowSoft,
				"/",
				2000,
				", ticksBelowHard=",
				this.ticksBelowHard,
				"/",
				2000
			}));
			float num = (
				from d in DefDatabase<MentalStateDef>.AllDefsListForReading
				select this.MentalStateDefSelectionProbability(d)).Sum();
			foreach (MentalStateDef current in DefDatabase<MentalStateDef>.AllDefsListForReading)
			{
				float num2 = this.MentalStateDefSelectionProbability(current);
				if (num2 > 0f)
				{
					stringBuilder.AppendLine(string.Concat(new object[]
					{
						"   ",
						current,
						" ",
						(num2 / num).ToStringPercent("F4")
					}));
				}
			}
			return stringBuilder.ToString();
		}
	}
}
