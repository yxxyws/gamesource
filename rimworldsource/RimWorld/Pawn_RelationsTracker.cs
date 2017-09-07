using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Pawn_RelationsTracker : IExposable
	{
		private const int CheckDevelopBondRelationIntervalTicks = 2500;
		private const float MaxBondRelationCheckDist = 12f;
		private const float BondRelationPerIntervalChance = 0.01f;
		private Pawn pawn;
		private List<DirectPawnRelation> directRelations = new List<DirectPawnRelation>();
		public bool everSeenByPlayer;
		public bool canGetRescuedThought = true;
		private HashSet<Pawn> pawnsWithDirectRelationsWithMe = new HashSet<Pawn>();
		public List<DirectPawnRelation> DirectRelations
		{
			get
			{
				return this.directRelations;
			}
		}
		public IEnumerable<Pawn> Children
		{
			get
			{
				Pawn_RelationsTracker.<>c__IteratorA4 <>c__IteratorA = new Pawn_RelationsTracker.<>c__IteratorA4();
				<>c__IteratorA.<>f__this = this;
				Pawn_RelationsTracker.<>c__IteratorA4 expr_0E = <>c__IteratorA;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public int ChildrenCount
		{
			get
			{
				return this.Children.Count<Pawn>();
			}
		}
		public bool RelatedToAnyoneOrAnyoneRelatedToMe
		{
			get
			{
				return this.directRelations.Any<DirectPawnRelation>() || this.pawnsWithDirectRelationsWithMe.Any<Pawn>();
			}
		}
		public IEnumerable<Pawn> FamilyByBlood
		{
			get
			{
				Pawn_RelationsTracker.<>c__IteratorA5 <>c__IteratorA = new Pawn_RelationsTracker.<>c__IteratorA5();
				<>c__IteratorA.<>f__this = this;
				Pawn_RelationsTracker.<>c__IteratorA5 expr_0E = <>c__IteratorA;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public IEnumerable<Pawn> PotentiallyRelatedPawns
		{
			get
			{
				Pawn_RelationsTracker.<>c__IteratorA6 <>c__IteratorA = new Pawn_RelationsTracker.<>c__IteratorA6();
				<>c__IteratorA.<>f__this = this;
				Pawn_RelationsTracker.<>c__IteratorA6 expr_0E = <>c__IteratorA;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public IEnumerable<Pawn> RelatedPawns
		{
			get
			{
				return 
					from x in this.PotentiallyRelatedPawns
					where this.pawn.GetRelations(x).Any<PawnRelationDef>()
					select x;
			}
		}
		public Pawn_RelationsTracker(Pawn pawn)
		{
			this.pawn = pawn;
		}
		public void ExposeData()
		{
			Scribe_Collections.LookList<DirectPawnRelation>(ref this.directRelations, "directRelations", LookMode.Deep, new object[0]);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				for (int i = 0; i < this.directRelations.Count; i++)
				{
					if (this.directRelations[i].otherPawn == null)
					{
						Log.Warning(string.Concat(new object[]
						{
							"Pawn ",
							this.pawn,
							" has relation \"",
							this.directRelations[i].def.defName,
							"\" with null pawn after loading. This means that we forgot to serialize pawns somewhere (e.g. pawns from passing trade ships)."
						}));
					}
				}
				this.directRelations.RemoveAll((DirectPawnRelation x) => x.otherPawn == null);
				for (int j = 0; j < this.directRelations.Count; j++)
				{
					this.directRelations[j].otherPawn.relations.pawnsWithDirectRelationsWithMe.Add(this.pawn);
				}
			}
			Scribe_Values.LookValue<bool>(ref this.everSeenByPlayer, "everSeenByPlayer", true, false);
			Scribe_Values.LookValue<bool>(ref this.canGetRescuedThought, "canGetRescuedThought", true, false);
		}
		public void SocialTrackerTick()
		{
			if (this.pawn.Dead)
			{
				return;
			}
			this.Tick_CheckStartMarriageCeremony();
			this.Tick_CheckDevelopBondRelation();
		}
		public DirectPawnRelation GetDirectRelation(PawnRelationDef def, Pawn otherPawn)
		{
			return this.directRelations.Find((DirectPawnRelation x) => x.def == def && x.otherPawn == otherPawn);
		}
		public Pawn GetFirstDirectRelationPawn(PawnRelationDef def, Predicate<Pawn> predicate = null)
		{
			for (int i = 0; i < this.directRelations.Count; i++)
			{
				DirectPawnRelation directPawnRelation = this.directRelations[i];
				if (directPawnRelation.def == def && (predicate == null || predicate(directPawnRelation.otherPawn)))
				{
					return directPawnRelation.otherPawn;
				}
			}
			return null;
		}
		public bool DirectRelationExists(PawnRelationDef def, Pawn otherPawn)
		{
			for (int i = 0; i < this.directRelations.Count; i++)
			{
				DirectPawnRelation directPawnRelation = this.directRelations[i];
				if (directPawnRelation.def == def && directPawnRelation.otherPawn == otherPawn)
				{
					return true;
				}
			}
			return false;
		}
		public void AddDirectRelation(PawnRelationDef def, Pawn otherPawn)
		{
			if (def.implied)
			{
				Log.Warning(string.Concat(new object[]
				{
					"Tried to directly add implied pawn relation ",
					def,
					", pawn=",
					this.pawn,
					", otherPawn=",
					otherPawn
				}));
				return;
			}
			if (otherPawn == this.pawn)
			{
				Log.Warning(string.Concat(new object[]
				{
					"Tried to add pawn relation ",
					def,
					" with self, pawn=",
					this.pawn
				}));
				return;
			}
			if (this.DirectRelationExists(def, otherPawn))
			{
				Log.Warning(string.Concat(new object[]
				{
					"Tried to add the same relation twice: ",
					def,
					", pawn=",
					this.pawn,
					", otherPawn=",
					otherPawn
				}));
				return;
			}
			int startTicks = (Game.Mode != GameMode.MapPlaying) ? 0 : Find.TickManager.TicksGame;
			this.directRelations.Add(new DirectPawnRelation(def, otherPawn, startTicks));
			otherPawn.relations.pawnsWithDirectRelationsWithMe.Add(this.pawn);
			if (def.reflexive)
			{
				otherPawn.relations.directRelations.Add(new DirectPawnRelation(def, this.pawn, startTicks));
				this.pawnsWithDirectRelationsWithMe.Add(otherPawn);
			}
			this.GainedOrLostDirectRelation();
			otherPawn.relations.GainedOrLostDirectRelation();
		}
		public void RemoveDirectRelation(DirectPawnRelation relation)
		{
			this.RemoveDirectRelation(relation.def, relation.otherPawn);
		}
		public void RemoveDirectRelation(PawnRelationDef def, Pawn otherPawn)
		{
			if (!this.TryRemoveDirectRelation(def, otherPawn))
			{
				Log.Warning(string.Concat(new object[]
				{
					"Could not remove relation ",
					def,
					" because it's not here. pawn=",
					this.pawn,
					", otherPawn=",
					otherPawn
				}));
			}
		}
		public bool TryRemoveDirectRelation(PawnRelationDef def, Pawn otherPawn)
		{
			if (def.implied)
			{
				Log.Warning(string.Concat(new object[]
				{
					"Tried to remove implied pawn relation ",
					def,
					", pawn=",
					this.pawn,
					", otherPawn=",
					otherPawn
				}));
				return false;
			}
			for (int i = 0; i < this.directRelations.Count; i++)
			{
				if (this.directRelations[i].def == def && this.directRelations[i].otherPawn == otherPawn)
				{
					if (def.reflexive)
					{
						List<DirectPawnRelation> list = otherPawn.relations.directRelations;
						DirectPawnRelation item = list.Find((DirectPawnRelation x) => x.def == def && x.otherPawn == this.pawn);
						list.Remove(item);
						if (list.Find((DirectPawnRelation x) => x.otherPawn == this.pawn) == null)
						{
							this.pawnsWithDirectRelationsWithMe.Remove(otherPawn);
						}
					}
					this.directRelations.RemoveAt(i);
					if (this.directRelations.Find((DirectPawnRelation x) => x.otherPawn == otherPawn) == null)
					{
						otherPawn.relations.pawnsWithDirectRelationsWithMe.Remove(this.pawn);
					}
					this.GainedOrLostDirectRelation();
					otherPawn.relations.GainedOrLostDirectRelation();
					return true;
				}
			}
			return false;
		}
		public int OpinionOf(Pawn other)
		{
			if (!other.RaceProps.Humanlike || this.pawn == other)
			{
				return 0;
			}
			if (this.pawn.Dead)
			{
				return 0;
			}
			ProfilerThreadCheck.BeginSample("OpinionOf()");
			int num = 0;
			foreach (PawnRelationDef current in this.pawn.GetRelations(other))
			{
				num += current.opinionOffset;
			}
			if (this.pawn.RaceProps.Humanlike)
			{
				ThoughtHandler thoughts = this.pawn.needs.mood.thoughts;
				foreach (ThoughtDef current2 in this.pawn.needs.mood.thoughts.DistinctSocialThoughtDefs(other))
				{
					num += thoughts.OpinionOffsetOfThoughtGroup(current2, other);
				}
			}
			if (num != 0)
			{
				float num2 = 1f;
				List<Hediff> hediffs = this.pawn.health.hediffSet.hediffs;
				for (int i = 0; i < hediffs.Count; i++)
				{
					if (hediffs[i].CurStage != null)
					{
						num2 *= hediffs[i].CurStage.opinionOfOthersFactor;
					}
				}
				num = Mathf.RoundToInt((float)num * num2);
			}
			if (num > 0 && this.pawn.HostileTo(other))
			{
				num = 0;
			}
			ProfilerThreadCheck.EndSample();
			return Mathf.Clamp(num, -100, 100);
		}
		public string OpinionExplanation(Pawn other)
		{
			if (!other.RaceProps.Humanlike || this.pawn == other)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("OpinionOf".Translate(new object[]
			{
				other.LabelBaseShort
			}) + ": " + this.OpinionOf(other).ToStringWithSign());
			string pawnSituationLabel = SocialCardUtility.GetPawnSituationLabel(other, this.pawn);
			if (!pawnSituationLabel.NullOrEmpty())
			{
				stringBuilder.AppendLine(pawnSituationLabel);
			}
			stringBuilder.AppendLine("--------------");
			bool flag = false;
			if (this.pawn.Dead)
			{
				stringBuilder.AppendLine("IAmDead".Translate());
				flag = true;
			}
			else
			{
				IEnumerable<PawnRelationDef> relations = this.pawn.GetRelations(other);
				foreach (PawnRelationDef current in relations)
				{
					stringBuilder.AppendLine(current.GetGenderSpecificLabelCap(other) + ": " + current.opinionOffset.ToStringWithSign());
					flag = true;
				}
				if (this.pawn.RaceProps.Humanlike)
				{
					ThoughtHandler thoughts = this.pawn.needs.mood.thoughts;
					foreach (ThoughtDef t in thoughts.DistinctSocialThoughtDefs(other))
					{
						int num = 1;
						if (t.IsMemory)
						{
							num = thoughts.NumSocialMemoryThoughtsOfDef(t, other.thingIDNumber);
						}
						Thought thought = thoughts.Thoughts.Find((Thought x) => x.def == t && x is ISocialThought && ((ISocialThought)x).OpinionOffset(other) != 0f);
						stringBuilder.Append(thought.LabelCapSocial);
						if (num != 1)
						{
							stringBuilder.Append(" x" + num);
						}
						stringBuilder.AppendLine(": " + thoughts.OpinionOffsetOfThoughtGroup(t, other).ToStringWithSign());
						flag = true;
					}
				}
				List<Hediff> hediffs = this.pawn.health.hediffSet.hediffs;
				for (int i = 0; i < hediffs.Count; i++)
				{
					HediffStage curStage = hediffs[i].CurStage;
					if (curStage != null && curStage.opinionOfOthersFactor != 1f)
					{
						stringBuilder.Append(hediffs[i].LabelBase.CapitalizeFirst());
						if (curStage.opinionOfOthersFactor != 0f)
						{
							stringBuilder.AppendLine(": x" + curStage.opinionOfOthersFactor.ToStringPercent());
						}
						else
						{
							stringBuilder.AppendLine();
						}
						flag = true;
					}
				}
				if (this.pawn.HostileTo(other))
				{
					stringBuilder.AppendLine("Hostile".Translate());
					flag = true;
				}
			}
			if (!flag)
			{
				stringBuilder.AppendLine("NoneBrackets".Translate());
			}
			return stringBuilder.ToString().TrimEndNewlines();
		}
		public float AttractionTo(Pawn otherPawn)
		{
			if (this.pawn.def != otherPawn.def || this.pawn == otherPawn)
			{
				return 0f;
			}
			float num = 1f;
			float num2 = 1f;
			float ageBiologicalYearsFloat = this.pawn.ageTracker.AgeBiologicalYearsFloat;
			float ageBiologicalYearsFloat2 = otherPawn.ageTracker.AgeBiologicalYearsFloat;
			if (this.pawn.gender == Gender.Male)
			{
				if (this.pawn.RaceProps.Humanlike && this.pawn.story.traits.HasTrait(TraitDefOf.Gay))
				{
					if (otherPawn.gender == Gender.Female)
					{
						return 0f;
					}
				}
				else
				{
					if (otherPawn.gender == Gender.Male)
					{
						return 0f;
					}
				}
				num2 = GenMath.FlatHill(16f, 20f, ageBiologicalYearsFloat, ageBiologicalYearsFloat + 15f, ageBiologicalYearsFloat2);
			}
			else
			{
				if (this.pawn.gender == Gender.Female)
				{
					if (this.pawn.RaceProps.Humanlike && this.pawn.story.traits.HasTrait(TraitDefOf.Gay))
					{
						if (otherPawn.gender == Gender.Male)
						{
							return 0f;
						}
					}
					else
					{
						if (otherPawn.gender == Gender.Female)
						{
							num = 0.15f;
						}
					}
					if (ageBiologicalYearsFloat2 < ageBiologicalYearsFloat - 10f)
					{
						return 0f;
					}
					if (ageBiologicalYearsFloat2 < ageBiologicalYearsFloat - 3f)
					{
						num2 = Mathf.InverseLerp(ageBiologicalYearsFloat - 10f, ageBiologicalYearsFloat - 3f, ageBiologicalYearsFloat2) * 0.2f;
					}
					else
					{
						num2 = GenMath.FlatHill(0.2f, ageBiologicalYearsFloat - 3f, ageBiologicalYearsFloat, ageBiologicalYearsFloat + 10f, ageBiologicalYearsFloat + 40f, 0.1f, ageBiologicalYearsFloat2);
					}
				}
			}
			float num3 = 1f;
			num3 *= Mathf.Lerp(0.2f, 1f, otherPawn.health.capacities.GetEfficiency(PawnCapacityDefOf.Talking));
			num3 *= Mathf.Lerp(0.2f, 1f, otherPawn.health.capacities.GetEfficiency(PawnCapacityDefOf.Manipulation));
			num3 *= Mathf.Lerp(0.2f, 1f, otherPawn.health.capacities.GetEfficiency(PawnCapacityDefOf.Moving));
			float num4 = 1f;
			foreach (PawnRelationDef current in this.pawn.GetRelations(otherPawn))
			{
				num4 *= current.attractionFactor;
			}
			int num5 = 0;
			if (otherPawn.RaceProps.Humanlike)
			{
				num5 = otherPawn.story.traits.DegreeOfTrait(TraitDefOf.Prettiness);
			}
			float num6 = 1f;
			if (num5 < 0)
			{
				num6 = 0.3f;
			}
			else
			{
				if (num5 > 0)
				{
					num6 = 2.3f;
				}
			}
			float num7 = Mathf.InverseLerp(15f, 18f, ageBiologicalYearsFloat);
			float num8 = Mathf.InverseLerp(15f, 18f, ageBiologicalYearsFloat2);
			return num * num2 * num3 * num4 * num7 * num8 * num6;
		}
		public float CompatibilityWith(Pawn otherPawn)
		{
			if (this.pawn.def != otherPawn.def || this.pawn == otherPawn)
			{
				return 0f;
			}
			float x = Mathf.Abs(this.pawn.ageTracker.AgeBiologicalYearsFloat - otherPawn.ageTracker.AgeBiologicalYearsFloat);
			float num = GenMath.LerpDouble(0f, 20f, 0.45f, -0.45f, x);
			num = Mathf.Clamp(num, -0.45f, 0.45f);
			float num2 = this.ConstantPerPawnsPairCompatibilityOffset(otherPawn.thingIDNumber);
			return num + num2;
		}
		public float ConstantPerPawnsPairCompatibilityOffset(int otherPawnID)
		{
			Rand.PushSeed();
			Rand.Seed = (this.pawn.thingIDNumber ^ otherPawnID) * 37;
			float result = Rand.GaussianAsymmetric(0.3f, 1f, 1.4f);
			Rand.PopSeed();
			return result;
		}
		public void ClearAllRelations()
		{
			List<DirectPawnRelation> list = this.directRelations.ToList<DirectPawnRelation>();
			for (int i = 0; i < list.Count; i++)
			{
				this.RemoveDirectRelation(list[i]);
			}
			List<Pawn> list2 = this.pawnsWithDirectRelationsWithMe.ToList<Pawn>();
			for (int j = 0; j < list2.Count; j++)
			{
				List<DirectPawnRelation> list3 = list2[j].relations.directRelations.ToList<DirectPawnRelation>();
				for (int k = 0; k < list3.Count; k++)
				{
					if (list3[k].otherPawn == this.pawn)
					{
						list2[j].relations.RemoveDirectRelation(list3[k]);
					}
				}
			}
		}
		public void Notify_PawnKilled(DamageInfo? dinfo)
		{
			foreach (Pawn current in this.PotentiallyRelatedPawns)
			{
				if (!current.Dead && current.needs.mood != null)
				{
					current.needs.mood.thoughts.RecalculateSituationalThoughts();
				}
			}
			if (this.everSeenByPlayer && !this.pawn.IsWorldPawn())
			{
				foreach (Pawn current2 in this.PotentiallyRelatedPawns)
				{
					if (!current2.Dead && current2.needs.mood != null)
					{
						PawnRelationDef mostImportantRelation = current2.GetMostImportantRelation(this.pawn);
						if (mostImportantRelation != null)
						{
							ThoughtDef genderSpecificKilledThought = mostImportantRelation.GetGenderSpecificKilledThought(this.pawn);
							if (genderSpecificKilledThought != null)
							{
								current2.needs.mood.thoughts.TryGainThought(genderSpecificKilledThought);
							}
						}
					}
				}
				if (dinfo.HasValue)
				{
					Pawn pawn = dinfo.Value.Instigator as Pawn;
					if (pawn != null && pawn != this.pawn)
					{
						foreach (Pawn current3 in this.FamilyByBlood)
						{
							if (pawn != current3)
							{
								if (!current3.Dead)
								{
									if (current3.needs.mood != null)
									{
										Thought_SocialMemory thought_SocialMemory = (Thought_SocialMemory)ThoughtMaker.MakeThought(ThoughtDefOf.KilledMyKin);
										thought_SocialMemory.SetOtherPawn(pawn);
										current3.needs.mood.thoughts.TryGainThought(thought_SocialMemory);
									}
								}
							}
						}
					}
				}
				if (this.pawn.RaceProps.Humanlike)
				{
					foreach (Pawn current4 in Find.MapPawns.AllPawns)
					{
						if (current4.RaceProps.IsFlesh && current4.needs.mood != null)
						{
							int num = current4.relations.OpinionOf(this.pawn);
							if (num >= 20)
							{
								Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(ThoughtDefOf.PawnWithGoodOpinionDied);
								thought_Memory.moodPowerFactor = Mathf.Lerp(0.15f, 1f, Mathf.InverseLerp(20f, 100f, (float)num));
								current4.needs.mood.thoughts.TryGainThought(thought_Memory);
							}
							else
							{
								if (num <= -20)
								{
									Thought_Memory thought_Memory2 = (Thought_Memory)ThoughtMaker.MakeThought(ThoughtDefOf.PawnWithBadOpinionDied);
									thought_Memory2.moodPowerFactor = Mathf.Lerp(0.15f, 1f, Mathf.InverseLerp(-20f, -100f, (float)num));
									current4.needs.mood.thoughts.TryGainThought(thought_Memory2);
								}
							}
						}
					}
				}
				if (this.pawn.RaceProps.Animal)
				{
					this.SendBondedAnimalDiedLetter();
				}
				else
				{
					this.AffectBondedAnimalsOnMyDeath();
				}
			}
		}
		public void Notify_PawnSold(Pawn playerNegotiator)
		{
			foreach (Pawn current in this.PotentiallyRelatedPawns)
			{
				if (!current.Dead && current.needs.mood != null)
				{
					PawnRelationDef mostImportantRelation = current.GetMostImportantRelation(this.pawn);
					if (mostImportantRelation != null && mostImportantRelation.soldThought != null)
					{
						Thought thought = ThoughtMaker.MakeThought(mostImportantRelation.soldThought);
						Thought_SocialMemory thought_SocialMemory = thought as Thought_SocialMemory;
						if (thought_SocialMemory != null)
						{
							thought_SocialMemory.SetOtherPawn(playerNegotiator);
						}
						current.needs.mood.thoughts.TryGainThought(thought);
					}
				}
			}
		}
		public void Notify_RescuedBy(Pawn rescuer)
		{
			if (rescuer.RaceProps.Humanlike && this.canGetRescuedThought)
			{
				Thought_SocialMemory thought_SocialMemory = (Thought_SocialMemory)ThoughtMaker.MakeThought(ThoughtDefOf.RescuedMe);
				thought_SocialMemory.SetOtherPawn(rescuer);
				this.pawn.needs.mood.thoughts.TryGainThought(thought_SocialMemory);
				this.canGetRescuedThought = false;
			}
		}
		private void SendBondedAnimalDiedLetter()
		{
			int num = 0;
			for (int i = 0; i < this.directRelations.Count; i++)
			{
				if (this.directRelations[i].def == PawnRelationDefOf.Bond && !this.directRelations[i].otherPawn.Dead)
				{
					num++;
				}
			}
			if (num == 1)
			{
				Pawn firstDirectRelationPawn = this.GetFirstDirectRelationPawn(PawnRelationDefOf.Bond, (Pawn x) => !x.Dead);
				string str;
				if (this.pawn.Name != null)
				{
					str = "LetterNamedBondedAnimalDied".Translate(new object[]
					{
						this.pawn.KindLabel,
						this.pawn.Name.ToStringShort,
						firstDirectRelationPawn.LabelBaseShort
					});
				}
				else
				{
					str = "LetterBondedAnimalDied".Translate(new object[]
					{
						this.pawn.KindLabel,
						firstDirectRelationPawn.LabelBaseShort
					});
				}
				Find.LetterStack.ReceiveLetter("LetterLabelBondedAnimalDied".Translate(), str.CapitalizeFirst(), LetterType.BadNonUrgent, this.pawn.Position, null);
			}
			else
			{
				if (num > 1)
				{
					StringBuilder stringBuilder = new StringBuilder();
					for (int j = 0; j < this.directRelations.Count; j++)
					{
						if (this.directRelations[j].def == PawnRelationDefOf.Bond && !this.directRelations[j].otherPawn.Dead)
						{
							stringBuilder.AppendLine("  - " + this.directRelations[j].otherPawn.LabelBaseShort);
						}
					}
					string str2;
					if (this.pawn.Name != null)
					{
						str2 = "LetterNamedBondedAnimalDiedMulti".Translate(new object[]
						{
							this.pawn.KindLabel,
							this.pawn.Name.ToStringShort,
							stringBuilder.ToString().TrimEndNewlines()
						});
					}
					else
					{
						str2 = "LetterBondedAnimalDiedMulti".Translate(new object[]
						{
							this.pawn.KindLabel,
							stringBuilder.ToString().TrimEndNewlines()
						});
					}
					Find.LetterStack.ReceiveLetter("LetterLabelBondedAnimalDied".Translate(), str2.CapitalizeFirst(), LetterType.BadNonUrgent, this.pawn.Position, null);
				}
			}
		}
		private void AffectBondedAnimalsOnMyDeath()
		{
			int num = 0;
			Pawn pawn = null;
			for (int i = 0; i < this.directRelations.Count; i++)
			{
				if (this.directRelations[i].def == PawnRelationDefOf.Bond && this.directRelations[i].otherPawn.Spawned)
				{
					pawn = this.directRelations[i].otherPawn;
					num++;
					float value = Rand.Value;
					MentalStateDef stateDef;
					if (value < 0.5f)
					{
						stateDef = MentalStateDefOf.DazedWander;
					}
					else
					{
						if (value < 0.75f)
						{
							stateDef = MentalStateDefOf.Berserk;
						}
						else
						{
							stateDef = MentalStateDefOf.Manhunter;
						}
					}
					this.directRelations[i].otherPawn.mindState.mentalStateStarter.TryStartMentalState(stateDef);
				}
			}
			if (num == 1)
			{
				string str;
				if (pawn.Name != null && !pawn.Name.Numerical)
				{
					str = "MessageNamedBondedAnimalMentalBreak".Translate(new object[]
					{
						pawn.KindLabel,
						pawn.Name.ToStringShort,
						this.pawn.LabelBaseShort
					});
				}
				else
				{
					str = "MessageBondedAnimalMentalBreak".Translate(new object[]
					{
						pawn.KindLabel,
						this.pawn.LabelBaseShort
					});
				}
				Messages.Message(str.CapitalizeFirst(), pawn, MessageSound.SeriousAlert);
			}
			else
			{
				if (num > 1)
				{
					Messages.Message("MessageBondedAnimalsMentalBreak".Translate(new object[]
					{
						num,
						this.pawn.LabelBaseShort
					}).CapitalizeFirst(), pawn, MessageSound.SeriousAlert);
				}
			}
		}
		private void Tick_CheckStartMarriageCeremony()
		{
			if (!this.pawn.Spawned || this.pawn.RaceProps.Animal)
			{
				return;
			}
			if (this.pawn.IsHashIntervalTick(1017))
			{
				int ticksGame = Find.TickManager.TicksGame;
				for (int i = 0; i < this.directRelations.Count; i++)
				{
					float num = (float)(ticksGame - this.directRelations[i].startTicks) / 60000f;
					if (this.directRelations[i].def == PawnRelationDefOf.Fiance && this.pawn.thingIDNumber < this.directRelations[i].otherPawn.thingIDNumber && num > 10f && Rand.MTBEventOccurs(2f, 60000f, 1017f) && MarriageCeremonyUtility.AcceptableMapConditionsToStartCeremony() && MarriageCeremonyUtility.FianceReadyToStartCeremony(this.pawn) && MarriageCeremonyUtility.FianceReadyToStartCeremony(this.directRelations[i].otherPawn))
					{
						Find.VoluntarilyJoinableLordsStarter.TryStartMarriageCeremony(this.pawn, this.directRelations[i].otherPawn);
					}
				}
			}
		}
		private void Tick_CheckDevelopBondRelation()
		{
			if (!this.pawn.Spawned || !this.pawn.RaceProps.Animal || this.pawn.Faction != Faction.OfColony || this.pawn.playerSettings.master == null)
			{
				return;
			}
			Pawn master = this.pawn.playerSettings.master;
			if (this.pawn.IsHashIntervalTick(2500) && this.pawn.Position.InHorDistOf(master.Position, 12f) && GenSight.LineOfSight(this.pawn.Position, master.Position, false))
			{
				RelationsUtility.TryDevelopBondRelation(master, this.pawn, 0.01f);
			}
		}
		private void GainedOrLostDirectRelation()
		{
			if (Game.Mode == GameMode.MapPlaying && !this.pawn.Dead && this.pawn.needs.mood != null)
			{
				this.pawn.needs.mood.thoughts.RecalculateSituationalThoughts();
			}
		}
	}
}
