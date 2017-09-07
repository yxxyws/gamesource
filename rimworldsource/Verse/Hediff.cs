using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse.AI;
namespace Verse
{
	public class Hediff : IExposable
	{
		public HediffDef def;
		private int partIndex = -1;
		public int ticksSinceCreation;
		public ThingDef source;
		public BodyPartGroupDef sourceBodyPartGroup;
		public HediffDef sourceHediffDef;
		protected float severityInt;
		private bool recordedTale;
		protected bool causesNoPain;
		[Unsaved]
		public Pawn pawn;
		[Unsaved]
		private BodyPartRecord cachedPart;
		public virtual string LabelBase
		{
			get
			{
				return this.def.label;
			}
		}
		public string Label
		{
			get
			{
				return this.LabelBase + ((!this.LabelInBrackets.NullOrEmpty()) ? (" (" + this.LabelInBrackets + ")") : string.Empty);
			}
		}
		public string LabelCap
		{
			get
			{
				return this.Label.CapitalizeFirst();
			}
		}
		public virtual Color LabelColor
		{
			get
			{
				return this.def.defaultLabelColor;
			}
		}
		public virtual string LabelInBrackets
		{
			get
			{
				return (this.CurStage != null && !this.CurStage.label.NullOrEmpty()) ? this.CurStage.label : null;
			}
		}
		public virtual string DamageLabel
		{
			get
			{
				return (this.def.lethalSeverity >= 0f) ? (this.Severity / this.def.lethalSeverity).ToStringPercent() : null;
			}
		}
		public virtual int UIGroupKey
		{
			get
			{
				return this.Label.GetHashCode();
			}
		}
		public virtual string DebugString
		{
			get
			{
				return null;
			}
		}
		public virtual string TipStringExtra
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (!this.CapMods.NullOrEmpty<PawnCapacityModifier>() || this.AffectsMemory || this.AffectsSocialInteractions)
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					if (this.CapMods != null)
					{
						for (int i = 0; i < this.CapMods.Count; i++)
						{
							if (i != 0)
							{
								stringBuilder2.Append(", ");
							}
							stringBuilder2.Append(this.CapMods[i].capacity.GetLabelFor(true, true));
						}
					}
					if (this.AffectsMemory)
					{
						if (stringBuilder2.Length != 0)
						{
							stringBuilder2.Append(", ");
						}
						stringBuilder2.Append("MemoryLower".Translate());
					}
					if (this.AffectsSocialInteractions)
					{
						if (stringBuilder2.Length != 0)
						{
							stringBuilder2.Append(", ");
						}
						stringBuilder2.Append("SocialInteractionsLower".Translate());
					}
					stringBuilder.AppendLine("AffectsLower".Translate().CapitalizeFirst() + ": " + stringBuilder2.ToString());
				}
				if (this.BleedRate > 0.001f)
				{
					stringBuilder.AppendLine("BleedingRate".Translate() + ": " + (this.BleedRate / this.MaxBleeding).ToStringPercent());
				}
				if (this.CurStage != null)
				{
					if (this.CurStage.hungerRateFactor != 1f)
					{
						stringBuilder.AppendLine("HungerRate".Translate() + ": x" + this.CurStage.hungerRateFactor);
					}
					if (this.CurStage.restFallFactor != 1f)
					{
						stringBuilder.AppendLine("Tiredness".Translate() + ": x" + this.CurStage.restFallFactor);
					}
				}
				return stringBuilder.ToString();
			}
		}
		public virtual HediffStage CurStage
		{
			get
			{
				return (!this.def.stages.NullOrEmpty<HediffStage>()) ? this.def.stages[this.CurStageIndex] : null;
			}
		}
		public virtual bool ShouldRemove
		{
			get
			{
				return this.Severity <= 0f;
			}
		}
		public virtual bool Visible
		{
			get
			{
				return this.CurStage == null || this.CurStage.everVisible;
			}
		}
		public virtual float BleedRate
		{
			get
			{
				return 0f;
			}
		}
		public virtual float MaxBleeding
		{
			get
			{
				return 0f;
			}
		}
		public virtual float PainOffset
		{
			get
			{
				return (this.CurStage != null && !this.causesNoPain) ? this.CurStage.painOffset : 0f;
			}
		}
		public virtual float PainFactor
		{
			get
			{
				return (this.CurStage != null) ? this.CurStage.painFactor : 1f;
			}
		}
		public List<PawnCapacityModifier> CapMods
		{
			get
			{
				return (this.CurStage != null) ? this.CurStage.capMods : null;
			}
		}
		public virtual float SummaryHealthPercentImpact
		{
			get
			{
				return 0f;
			}
		}
		public virtual int CurStageIndex
		{
			get
			{
				if (this.def.stages == null)
				{
					return 0;
				}
				List<HediffStage> stages = this.def.stages;
				for (int i = stages.Count - 1; i >= 0; i--)
				{
					if (this.Severity >= stages[i].minSeverity)
					{
						return i;
					}
				}
				return 0;
			}
		}
		public virtual float Severity
		{
			get
			{
				return this.severityInt;
			}
			set
			{
				if (this.def.lethalSeverity > 0f && value > this.def.lethalSeverity)
				{
					value = this.def.lethalSeverity;
				}
				int curStageIndex = this.CurStageIndex;
				this.severityInt = value;
				if (this.CurStageIndex != curStageIndex)
				{
					this.pawn.health.Notify_HediffChanged(this);
					if (!this.pawn.Dead && this.pawn.needs.mood != null)
					{
						this.pawn.needs.mood.thoughts.Notify_HediffChanged();
					}
				}
			}
		}
		public BodyPartRecord Part
		{
			get
			{
				if (this.cachedPart == null && this.partIndex >= 0)
				{
					this.cachedPart = this.pawn.RaceProps.body.GetPartAtIndex(this.partIndex);
				}
				return this.cachedPart;
			}
			set
			{
				if (this.pawn == null)
				{
					Log.Error("Hediff: Cannot set Part without setting pawn first.");
					return;
				}
				if (value != null)
				{
					this.partIndex = this.pawn.RaceProps.body.GetIndexOfPart(value);
				}
				else
				{
					this.partIndex = -1;
				}
				this.cachedPart = value;
			}
		}
		public virtual bool TendableNow
		{
			get
			{
				return this.def.tendable && this.Severity > 0f && this.Visible && !this.FullyImmune() && !this.IsTended() && !this.IsOld();
			}
		}
		private bool AffectsMemory
		{
			get
			{
				return this.CurStage != null && (this.CurStage.forgetMemoryThoughtMtbDays > 0f || this.CurStage.pctConditionalThoughtsNullified > 0f);
			}
		}
		private bool AffectsSocialInteractions
		{
			get
			{
				return this.CurStage != null && this.CurStage.opinionOfOthersFactor != 1f;
			}
		}
		public virtual void Tick()
		{
			this.ticksSinceCreation++;
			if (this.CurStage != null)
			{
				if (this.CurStage.hediffGivers != null && this.pawn.IsHashIntervalTick(60))
				{
					for (int i = 0; i < this.CurStage.hediffGivers.Count; i++)
					{
						HediffGiver hediffGiver = this.CurStage.hediffGivers[i];
						if (hediffGiver.CheckGiveEverySecond(this.pawn) && (this.pawn.IsColonist || this.pawn.IsPrisonerOfColony))
						{
							Find.LetterStack.ReceiveLetter(new Letter("LetterHealthComplicationsLabel".Translate(new object[]
							{
								this.pawn.LabelBaseShort,
								hediffGiver.hediff.label
							}), "LetterHealthComplications".Translate(new object[]
							{
								this.pawn.LabelBaseShort,
								hediffGiver.hediff.label,
								this.def.label
							}), LetterType.BadNonUrgent, this.pawn), null);
						}
					}
				}
				if (this.CurStage.mentalStateGivers != null && !this.pawn.InMentalState && this.pawn.IsHashIntervalTick(60))
				{
					for (int j = 0; j < this.CurStage.mentalStateGivers.Count; j++)
					{
						MentalStateGiver mentalStateGiver = this.CurStage.mentalStateGivers[j];
						if (Rand.MTBEventOccurs(mentalStateGiver.mtbDays, 60000f, 60f))
						{
							this.pawn.mindState.mentalStateHandler.StartMentalState(mentalStateGiver.mentalState);
						}
					}
				}
				if (this.CurStage.vomitMtbDays > 0f && this.pawn.IsHashIntervalTick(1000) && Rand.MTBEventOccurs(this.CurStage.vomitMtbDays, 60000f, 1000f) && this.pawn.Spawned && this.pawn.Awake())
				{
					this.pawn.jobs.StartJob(new Job(JobDefOf.Vomit), JobCondition.InterruptForced, null, true, true, null);
				}
				if (this.CurStage.forgetMemoryThoughtMtbDays > 0f && this.pawn.needs.mood != null && this.pawn.IsHashIntervalTick(400) && Rand.MTBEventOccurs(this.CurStage.forgetMemoryThoughtMtbDays, 60000f, 400f))
				{
					Thought th;
					if ((
						from x in this.pawn.needs.mood.thoughts.Thoughts
						where x is Thought_Memory
						select x).TryRandomElement(out th))
					{
						this.pawn.needs.mood.thoughts.RemoveThought(th);
					}
				}
				if (!this.recordedTale && this.CurStage.tale != null)
				{
					TaleRecorder.RecordTale(this.CurStage.tale, new object[]
					{
						this.pawn
					});
					this.recordedTale = true;
				}
				if (this.CurStage.destroyPart && this.Part != null && this.Part != this.pawn.RaceProps.body.corePart)
				{
					this.pawn.health.AddHediff(HediffDefOf.MissingBodyPart, this.Part, null);
				}
			}
		}
		public virtual void ExposeData()
		{
			Scribe_Defs.LookDef<HediffDef>(ref this.def, "def");
			Scribe_Values.LookValue<int>(ref this.ticksSinceCreation, "ticksSinceCreation", 0, false);
			Scribe_Defs.LookDef<ThingDef>(ref this.source, "source");
			Scribe_Defs.LookDef<BodyPartGroupDef>(ref this.sourceBodyPartGroup, "sourceBodyPartGroup");
			Scribe_Defs.LookDef<HediffDef>(ref this.sourceHediffDef, "sourceHediffDef");
			Scribe_Values.LookValue<int>(ref this.partIndex, "partIndex", -1, false);
			Scribe_Values.LookValue<float>(ref this.severityInt, "severity", 0f, false);
			Scribe_Values.LookValue<bool>(ref this.recordedTale, "recordedTale", false, false);
			Scribe_Values.LookValue<bool>(ref this.causesNoPain, "causesNoPain", false, false);
			if (Scribe.mode == LoadSaveMode.PostLoadInit && this.partIndex >= 0)
			{
				this.cachedPart = this.pawn.RaceProps.body.GetPartAtIndex(this.partIndex);
			}
		}
		public virtual void PostMake()
		{
			this.Severity = Mathf.Max(this.Severity, this.def.initialSeverity);
			this.causesNoPain = (Rand.Value < this.def.chanceToCauseNoPain);
		}
		public virtual void PostAdd()
		{
		}
		public virtual void PostTick()
		{
		}
		public virtual void Tended(float quality, int batchPosition = 0)
		{
		}
		public virtual bool TryMergeWith(Hediff other)
		{
			if (other == null || other.def != this.def || other.Part != this.Part)
			{
				return false;
			}
			this.Severity += other.Severity;
			this.ticksSinceCreation = 0;
			return true;
		}
		public virtual void DirectHeal(float amount)
		{
			if (amount <= 0f)
			{
				return;
			}
			this.Severity -= amount;
		}
		public virtual bool CauseDeathNow()
		{
			return this.def.lethalSeverity >= 0f && this.Severity >= this.def.lethalSeverity;
		}
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"(",
				this.def.defName,
				(this.cachedPart == null) ? string.Empty : (" " + this.cachedPart.def.label),
				" ticksSinceCreation=",
				this.ticksSinceCreation,
				")"
			});
		}
		public override int GetHashCode()
		{
			return this.def.GetHashCode();
		}
	}
}
