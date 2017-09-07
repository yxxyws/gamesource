using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
namespace Verse
{
	public class HediffSet : IExposable
	{
		public Pawn pawn;
		public List<Hediff> hediffs = new List<Hediff>();
		private List<Hediff_MissingPart> cachedMissingPartsCommonAncestors;
		private float cachedPain = -1f;
		private float cachedBleedRate = -1f;
		private Stack<BodyPartRecord> coveragePartsStack = new Stack<BodyPartRecord>();
		private HashSet<BodyPartRecord> coverageRejectedPartsSet = new HashSet<BodyPartRecord>();
		private Queue<BodyPartRecord> missingPartsCommonAncestorsQueue = new Queue<BodyPartRecord>();
		public float Pain
		{
			get
			{
				if (this.cachedPain < 0f)
				{
					this.cachedPain = this.CalculatePain();
				}
				return this.cachedPain;
			}
		}
		public float BleedingRate
		{
			get
			{
				if (this.cachedBleedRate < 0f)
				{
					this.cachedBleedRate = this.CalculateBleedingRate();
				}
				return this.cachedBleedRate;
			}
		}
		public bool HasTendableInjury
		{
			get
			{
				for (int i = 0; i < this.hediffs.Count; i++)
				{
					Hediff_Injury hediff_Injury = this.hediffs[i] as Hediff_Injury;
					if (hediff_Injury != null && hediff_Injury.TendableNow)
					{
						return true;
					}
				}
				return false;
			}
		}
		public bool HasNaturallyHealingInjuries
		{
			get
			{
				for (int i = 0; i < this.hediffs.Count; i++)
				{
					Hediff_Injury hediff_Injury = this.hediffs[i] as Hediff_Injury;
					if (hediff_Injury != null && hediff_Injury.IsNaturallyHealing())
					{
						return true;
					}
				}
				return false;
			}
		}
		public bool HasTendedAndHealingInjury
		{
			get
			{
				for (int i = 0; i < this.hediffs.Count; i++)
				{
					Hediff_Injury hediff_Injury = this.hediffs[i] as Hediff_Injury;
					if (hediff_Injury != null && hediff_Injury.IsTendedAndHealing() && hediff_Injury.Severity > 0f)
					{
						return true;
					}
				}
				return false;
			}
		}
		public bool HasTendedNonInjuryNonMissingPartHediff
		{
			get
			{
				for (int i = 0; i < this.hediffs.Count; i++)
				{
					if (!(this.hediffs[i] is Hediff_Injury))
					{
						if (!(this.hediffs[i] is Hediff_MissingPart))
						{
							if (this.hediffs[i].IsTended())
							{
								if (this.hediffs[i].Visible)
								{
									return true;
								}
							}
						}
					}
				}
				return false;
			}
		}
		public bool HasTendedImmunizableNonInjuryNonMissingPartHediff
		{
			get
			{
				for (int i = 0; i < this.hediffs.Count; i++)
				{
					if (!(this.hediffs[i] is Hediff_Injury))
					{
						if (!(this.hediffs[i] is Hediff_MissingPart))
						{
							if (this.hediffs[i].IsTended())
							{
								if (this.hediffs[i].Visible)
								{
									if (this.hediffs[i].def.PossibleToDevelopImmunity())
									{
										return true;
									}
								}
							}
						}
					}
				}
				return false;
			}
		}
		public bool AnyHediffMakesSickThought
		{
			get
			{
				for (int i = 0; i < this.hediffs.Count; i++)
				{
					if (this.hediffs[i].def.makesSickThought)
					{
						if (this.hediffs[i].Visible)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
		public HediffSet(Pawn newPawn)
		{
			this.pawn = newPawn;
		}
		public void ExposeData()
		{
			Scribe_Collections.LookList<Hediff>(ref this.hediffs, "hediffs", LookMode.Deep, new object[0]);
			if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
			{
				for (int i = 0; i < this.hediffs.Count; i++)
				{
					this.hediffs[i].pawn = this.pawn;
				}
				this.DirtyCache();
			}
		}
		public void AddHediffDirect(Hediff hediff, DamageInfo? dinfo = null)
		{
			if (hediff.def == null)
			{
				Log.Error("Tried to add health diff with null def. Canceling.");
				return;
			}
			if (hediff.Part != null && !this.GetNotMissingParts(null, null).Contains(hediff.Part))
			{
				Log.Error("Tried to add health diff to missing part.");
				return;
			}
			ProfilerThreadCheck.BeginSample("HediffSet.AddHediff()");
			hediff.ticksSinceCreation = 0;
			hediff.pawn = this.pawn;
			ProfilerThreadCheck.BeginSample("Attempt merge or add new hediff");
			bool flag = false;
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i].TryMergeWith(hediff))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				this.hediffs.Add(hediff);
				hediff.PostAdd();
				if (this.pawn.needs != null && this.pawn.needs.mood != null)
				{
					this.pawn.needs.mood.thoughts.Notify_HediffChanged();
				}
			}
			ProfilerThreadCheck.EndSample();
			bool flag2 = hediff is Hediff_MissingPart;
			if (!(hediff is Hediff_MissingPart) && hediff.Part != null && hediff.Part != this.pawn.RaceProps.body.corePart && this.GetPartHealth(hediff.Part) == 0f)
			{
				ProfilerThreadCheck.BeginSample("Handle missing body part");
				if (hediff.Part != this.pawn.RaceProps.body.corePart)
				{
					bool flag3 = this.HasDirectlyAddedPartFor(hediff.Part);
					Hediff_MissingPart hediff_MissingPart = (Hediff_MissingPart)HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, this.pawn, null);
					hediff_MissingPart.IsFresh = !flag3;
					hediff_MissingPart.lastInjury = hediff.def;
					this.pawn.health.AddHediff(hediff_MissingPart, hediff.Part, dinfo);
					if (flag3)
					{
						if (dinfo.HasValue)
						{
							hediff_MissingPart.lastInjury = HealthUtility.GetHediffDefFromDamage(dinfo.Value.Def, this.pawn, hediff.Part);
						}
						else
						{
							hediff_MissingPart.lastInjury = null;
						}
					}
					flag2 = true;
				}
				ProfilerThreadCheck.EndSample();
			}
			ProfilerThreadCheck.BeginSample("Dirty cache");
			this.DirtyCache();
			ProfilerThreadCheck.EndSample();
			if (flag2 && this.pawn.apparel != null)
			{
				this.pawn.apparel.Notify_LostBodyPart();
			}
			if (this.pawn.meleeVerbs != null)
			{
				this.pawn.meleeVerbs.Notify_HediffAddedOrRemoved();
			}
			ProfilerThreadCheck.EndSample();
		}
		public void HealHediff(Hediff hediff, int amount)
		{
			hediff.DirectHeal((float)amount);
			this.DirtyCache();
		}
		public void DirtyCache()
		{
			this.CacheMissingPartsCommonAncestors();
			this.cachedPain = -1f;
			this.cachedBleedRate = -1f;
			this.pawn.health.capacities.Notify_ActivitiesEfficiencyDirty();
		}
		[DebuggerHidden]
		public IEnumerable<T> GetHediffs<T>() where T : Hediff
		{
			HediffSet.<GetHediffs>c__Iterator17A<T> <GetHediffs>c__Iterator17A = new HediffSet.<GetHediffs>c__Iterator17A<T>();
			<GetHediffs>c__Iterator17A.<>f__this = this;
			HediffSet.<GetHediffs>c__Iterator17A<T> expr_0E = <GetHediffs>c__Iterator17A;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public Hediff GetFirstHediffOfDef(HediffDef def)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i].def == def)
				{
					return this.hediffs[i];
				}
			}
			return null;
		}
		public bool PartIsMissing(BodyPartRecord part)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i].Part == part && this.hediffs[i] is Hediff_MissingPart)
				{
					return true;
				}
			}
			return false;
		}
		public float GetPartHealth(BodyPartRecord part)
		{
			if (part == null)
			{
				return 0f;
			}
			float num = part.def.GetMaxHealth(this.pawn);
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i] is Hediff_MissingPart && this.hediffs[i].Part == part)
				{
					return 0f;
				}
				if (this.hediffs[i].Part == part)
				{
					Hediff_Injury hediff_Injury = this.hediffs[i] as Hediff_Injury;
					if (hediff_Injury != null)
					{
						num -= (float)Mathf.RoundToInt(hediff_Injury.Severity);
					}
				}
			}
			if (num < 0f)
			{
				num = 0f;
			}
			return num;
		}
		public BodyPartRecord GetBrain()
		{
			foreach (BodyPartRecord current in this.GetNotMissingParts(null, null))
			{
				for (int i = 0; i < current.def.Activities.Count; i++)
				{
					if (current.def.Activities[i].First == PawnCapacityDefOf.Consciousness)
					{
						return current;
					}
				}
			}
			return null;
		}
		public bool HasHediff(HediffDef def)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i].def == def)
				{
					return true;
				}
			}
			return false;
		}
		public bool HasHediff(HediffDef def, BodyPartRecord bodyPart)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i].def == def && this.hediffs[i].Part == bodyPart)
				{
					return true;
				}
			}
			return false;
		}
		[DebuggerHidden]
		public IEnumerable<Verb> GetHediffsVerbs()
		{
			HediffSet.<GetHediffsVerbs>c__Iterator17B <GetHediffsVerbs>c__Iterator17B = new HediffSet.<GetHediffsVerbs>c__Iterator17B();
			<GetHediffsVerbs>c__Iterator17B.<>f__this = this;
			HediffSet.<GetHediffsVerbs>c__Iterator17B expr_0E = <GetHediffsVerbs>c__Iterator17B;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		[DebuggerHidden]
		public IEnumerable<Hediff_Injury> GetInjuriesTendable()
		{
			HediffSet.<GetInjuriesTendable>c__Iterator17C <GetInjuriesTendable>c__Iterator17C = new HediffSet.<GetInjuriesTendable>c__Iterator17C();
			<GetInjuriesTendable>c__Iterator17C.<>f__this = this;
			HediffSet.<GetInjuriesTendable>c__Iterator17C expr_0E = <GetInjuriesTendable>c__Iterator17C;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public bool HasTemperatureInjury(TemperatureInjuryStage minStage)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if ((this.hediffs[i].def == HediffDefOf.Hypothermia || this.hediffs[i].def == HediffDefOf.Heatstroke) && this.hediffs[i].CurStageIndex >= (int)minStage)
				{
					return true;
				}
			}
			return false;
		}
		public IEnumerable<BodyPartRecord> GetInjuredParts()
		{
			return (
				from x in this.hediffs
				where x is Hediff_Injury
				select x.Part).Distinct<BodyPartRecord>();
		}
		[DebuggerHidden]
		public IEnumerable<BodyPartRecord> GetNaturallyHealingInjuredParts()
		{
			HediffSet.<GetNaturallyHealingInjuredParts>c__Iterator17D <GetNaturallyHealingInjuredParts>c__Iterator17D = new HediffSet.<GetNaturallyHealingInjuredParts>c__Iterator17D();
			<GetNaturallyHealingInjuredParts>c__Iterator17D.<>f__this = this;
			HediffSet.<GetNaturallyHealingInjuredParts>c__Iterator17D expr_0E = <GetNaturallyHealingInjuredParts>c__Iterator17D;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public List<Hediff_MissingPart> GetMissingPartsCommonAncestors()
		{
			if (this.cachedMissingPartsCommonAncestors == null)
			{
				this.CacheMissingPartsCommonAncestors();
			}
			return this.cachedMissingPartsCommonAncestors;
		}
		[DebuggerHidden]
		public IEnumerable<BodyPartRecord> GetNotMissingParts(BodyPartHeight? height = null, BodyPartDepth? depth = null)
		{
			HediffSet.<GetNotMissingParts>c__Iterator17E <GetNotMissingParts>c__Iterator17E = new HediffSet.<GetNotMissingParts>c__Iterator17E();
			<GetNotMissingParts>c__Iterator17E.height = height;
			<GetNotMissingParts>c__Iterator17E.depth = depth;
			<GetNotMissingParts>c__Iterator17E.<$>height = height;
			<GetNotMissingParts>c__Iterator17E.<$>depth = depth;
			<GetNotMissingParts>c__Iterator17E.<>f__this = this;
			HediffSet.<GetNotMissingParts>c__Iterator17E expr_2A = <GetNotMissingParts>c__Iterator17E;
			expr_2A.$PC = -2;
			return expr_2A;
		}
		public BodyPartRecord GetRandomNotMissingPart(BodyPartHeight? height = null, BodyPartDepth? depth = null)
		{
			IEnumerable<BodyPartRecord> source = Enumerable.Empty<BodyPartRecord>();
			if (this.GetNotMissingParts(height, depth).Any<BodyPartRecord>())
			{
				source = this.GetNotMissingParts(height, depth);
			}
			else
			{
				if (this.GetNotMissingParts(null, depth).Any<BodyPartRecord>())
				{
					source = this.GetNotMissingParts(null, depth);
				}
			}
			if (!source.Any<BodyPartRecord>())
			{
				return null;
			}
			return source.RandomElementByWeight((BodyPartRecord x) => x.absoluteFleshCoverage);
		}
		public bool HasFreshMissingPartsCommonAncestor()
		{
			if (this.cachedMissingPartsCommonAncestors == null)
			{
				this.CacheMissingPartsCommonAncestors();
			}
			for (int i = 0; i < this.cachedMissingPartsCommonAncestors.Count; i++)
			{
				if (this.cachedMissingPartsCommonAncestors[i].IsFresh)
				{
					return true;
				}
			}
			return false;
		}
		public float GetCoverageOfNotMissingNaturalParts(BodyPartRecord part)
		{
			if (this.PartIsMissing(part))
			{
				return 0f;
			}
			if (this.PartOrAnyAncestorHasDirectlyAddedParts(part))
			{
				return 0f;
			}
			this.coverageRejectedPartsSet.Clear();
			List<Hediff_MissingPart> missingPartsCommonAncestors = this.GetMissingPartsCommonAncestors();
			for (int i = 0; i < missingPartsCommonAncestors.Count; i++)
			{
				this.coverageRejectedPartsSet.Add(missingPartsCommonAncestors[i].Part);
			}
			for (int j = 0; j < this.hediffs.Count; j++)
			{
				if (this.hediffs[j] is Hediff_AddedPart)
				{
					this.coverageRejectedPartsSet.Add(this.hediffs[j].Part);
				}
			}
			float num = 0f;
			this.coveragePartsStack.Clear();
			this.coveragePartsStack.Push(part);
			while (this.coveragePartsStack.Any<BodyPartRecord>())
			{
				BodyPartRecord bodyPartRecord = this.coveragePartsStack.Pop();
				num += bodyPartRecord.absoluteFleshCoverage;
				for (int k = 0; k < bodyPartRecord.parts.Count; k++)
				{
					if (!this.coverageRejectedPartsSet.Contains(bodyPartRecord.parts[k]))
					{
						this.coveragePartsStack.Push(bodyPartRecord.parts[k]);
					}
				}
			}
			this.coveragePartsStack.Clear();
			this.coverageRejectedPartsSet.Clear();
			return num;
		}
		private void CacheMissingPartsCommonAncestors()
		{
			if (this.cachedMissingPartsCommonAncestors == null)
			{
				this.cachedMissingPartsCommonAncestors = new List<Hediff_MissingPart>();
			}
			else
			{
				this.cachedMissingPartsCommonAncestors.Clear();
			}
			this.missingPartsCommonAncestorsQueue.Clear();
			this.missingPartsCommonAncestorsQueue.Enqueue(this.pawn.def.race.body.corePart);
			while (this.missingPartsCommonAncestorsQueue.Count != 0)
			{
				BodyPartRecord node = this.missingPartsCommonAncestorsQueue.Dequeue();
				if (!this.PartOrAnyAncestorHasDirectlyAddedParts(node))
				{
					Hediff_MissingPart hediff_MissingPart = (
						from x in this.GetHediffs<Hediff_MissingPart>()
						where x.Part == node
						select x).FirstOrDefault<Hediff_MissingPart>();
					if (hediff_MissingPart != null)
					{
						this.cachedMissingPartsCommonAncestors.Add(hediff_MissingPart);
					}
					else
					{
						for (int i = 0; i < node.parts.Count; i++)
						{
							this.missingPartsCommonAncestorsQueue.Enqueue(node.parts[i]);
						}
					}
				}
			}
		}
		public bool HasDirectlyAddedPartFor(BodyPartRecord part)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i].Part == part && this.hediffs[i] is Hediff_AddedPart)
				{
					return true;
				}
			}
			return false;
		}
		public bool PartOrAnyAncestorHasDirectlyAddedParts(BodyPartRecord part)
		{
			return this.HasDirectlyAddedPartFor(part) || (part.parent != null && this.PartOrAnyAncestorHasDirectlyAddedParts(part.parent));
		}
		[DebuggerHidden]
		public IEnumerable<Hediff> GetTendableNonInjuryNonMissingPartHediffs()
		{
			HediffSet.<GetTendableNonInjuryNonMissingPartHediffs>c__Iterator17F <GetTendableNonInjuryNonMissingPartHediffs>c__Iterator17F = new HediffSet.<GetTendableNonInjuryNonMissingPartHediffs>c__Iterator17F();
			<GetTendableNonInjuryNonMissingPartHediffs>c__Iterator17F.<>f__this = this;
			HediffSet.<GetTendableNonInjuryNonMissingPartHediffs>c__Iterator17F expr_0E = <GetTendableNonInjuryNonMissingPartHediffs>c__Iterator17F;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public bool HasTendableNonInjuryNonMissingPartHediff(bool forAlert = false)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (!forAlert || this.hediffs[i].def.makesAlert)
				{
					if (!(this.hediffs[i] is Hediff_Injury))
					{
						if (!(this.hediffs[i] is Hediff_MissingPart))
						{
							if (this.hediffs[i].TendableNow)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		private float CalculateBleedingRate()
		{
			if (!this.pawn.RaceProps.IsFlesh || this.pawn.health.Dead)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				float num2 = this.hediffs[i].BleedRate;
				if (this.hediffs[i].Part != null)
				{
					num2 *= this.hediffs[i].Part.def.bleedingRateMultiplier;
				}
				num += num2;
			}
			return 0.0142857144f * num / this.pawn.HealthScale;
		}
		private float CalculatePain()
		{
			if (!this.pawn.RaceProps.IsFlesh || this.pawn.Dead)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				num += this.hediffs[i].PainOffset;
			}
			float num2 = num / this.pawn.HealthScale;
			for (int j = 0; j < this.hediffs.Count; j++)
			{
				num2 *= this.hediffs[j].PainFactor;
			}
			return Mathf.Clamp(num2, 0f, 1f);
		}
		public void Clear()
		{
			this.hediffs.Clear();
			this.DirtyCache();
		}
	}
}
